#include <cstdio>
#include <memory>
#include "CLogReader.h"
#include "Match.h"
#include "Source.h"
#include "Utils.h"

namespace LargeLogReader
{
    CLogReader::CLogReader()
    {
        // do nothing
    }

    CLogReader::~CLogReader()
    {
        // do nothing
    }

    bool CLogReader::Open(const char *filename)
    {
        FILE * file = fopen(filename, "r");
        if (file == nullptr)
            return false;
        _source = std::make_shared<FileTextSource>(std::shared_ptr<FILE>(file, [](FILE *file) { fclose(file); }));
        return true;
    }

    void CLogReader::Close()
    {
        _source.reset();
    }

    bool CLogReader::SetFilter(const char *filter)
    {
        _filter = PreparePattern(std::string(filter));
        return true;
    }

    bool CLogReader::GetNextLine(char *buf, const int bufsize)
    {
        while (true)
        {
            SourceReadResult readResult = _source->ReadNext();
            if (readResult.Type == SourceReadResultType::END || readResult.Type == SourceReadResultType::ERROR)
                return false;
            if (IsMatch(readResult.Data, _filter))
            {
                readResult.Data.copy(buf, bufsize);
                return true;
            }
        }
    }
}