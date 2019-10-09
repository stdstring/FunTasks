#pragma once

#include <cstdio>
#include <memory>
#include <string>

namespace LargeLogReader
{
    /*class IStringSource
    {
    public:
        virtual bool IsEof() = 0;
        virtual char ReadCurrent() = 0;
        virtual bool MoveTo(size_t position) = 0;
    };

    typedef std::shared_ptr<IStringSource> TIStringSourcePtr;

    class ITextSource
    {
    public:
        virtual TIStringSourcePtr GetNext() = 0;
    };

    typedef std::shared_ptr<ITextSource> TITextSourcePtr;*/

    enum SourceReadResultType { OK, END, ERROR };

    struct SourceReadResult
    {
    public:
        SourceReadResult(SourceReadResultType type, std::string data);

        SourceReadResultType Type;
        std::string Data;
    };

    class ITextSource
    {
    public:
        virtual SourceReadResult ReadNext() = 0;
    };

    typedef std::shared_ptr<ITextSource> TITextSourcePtr;

    class FileTextSource : public ITextSource
    {
    public:
        FileTextSource(std::shared_ptr<FILE> source);
        virtual SourceReadResult ReadNext() override;

    private:
        std::shared_ptr<FILE> _source;
    };
}