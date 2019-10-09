#pragma once

#include "Source.h"

namespace LargeLogReader
{
    class CLogReader
    {
    public:
        CLogReader();
        ~CLogReader();
        bool Open(const char *filename);
        void Close();
        bool SetFilter(const char *filter);
        bool GetNextLine(char *buf, const int bufsize);

    private:
        TITextSourcePtr _source;
        std::string _filter;
    };

}