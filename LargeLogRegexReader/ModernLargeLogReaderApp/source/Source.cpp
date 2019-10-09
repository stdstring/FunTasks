#include <algorithm>
#include <array>
#include <cstdio>
#include <memory>
#include "Source.h"

namespace LargeLogReader
{
    //constexpr char RCChar = '\r';
    constexpr char LFChar = '\n';
    constexpr size_t PortionSize = 1024;
    constexpr size_t InitSize = 4096;

    bool ReadPortion(std::string &dest, std::array<char, PortionSize> &buffer, FILE* source)
    {
        __int64 base = _ftelli64(source);
        size_t count = fread(buffer.data(), sizeof(char), PortionSize, source);
        if (ferror(source))
            return false;
        typedef std::array<char, PortionSize>::const_iterator TArrayIterator;
        TArrayIterator end = buffer.cbegin() + count;
        TArrayIterator findResult = std::find(buffer.cbegin(), end, LFChar);
        dest.append(buffer.cbegin(), findResult);
        if (findResult != end)
        {
            ptrdiff_t distance = std::distance(buffer.cbegin(), findResult);
            _fseeki64(source, base + distance + 1, SEEK_SET);
            return false;
        }
        return !(feof(source) || ferror(source));
    }

    void TrimRight(std::string &dest)
    {
        std::size_t position = dest.find_last_not_of("\r\n");
        if (position != std::string::npos)
            dest.erase(position + 1);
        else
            dest.clear();
    }

    SourceReadResult::SourceReadResult(SourceReadResultType type, std::string data) : Type(type), Data(data)
    {
    }

    FileTextSource::FileTextSource(std::shared_ptr<FILE> source) : _source(source)
    {
    }

    SourceReadResult FileTextSource::ReadNext()
    {
        if (feof(_source.get()))
            return SourceReadResult(SourceReadResultType::END, std::string());
        if (ferror(_source.get()))
            return SourceReadResult(SourceReadResultType::ERROR, std::string());
        std::array<char, PortionSize> buffer;
        std::string dest;
        while (ReadPortion(dest, buffer, _source.get()))
        {
        }
        if (ferror(_source.get()))
            return SourceReadResult(SourceReadResultType::ERROR, std::string());
        TrimRight(dest);
        return SourceReadResult(SourceReadResultType::OK, dest);
    }
}