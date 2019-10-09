#pragma once

#include <memory>
#include <string>
#include "Result.h"
#include "State.h"

namespace LargeLogReader
{
    constexpr size_t CharHandlerId = 1;
    constexpr size_t DefaultHandlerId = CharHandlerId;
    constexpr size_t AsteriskHandlerId = 2;

    class IHandler
    {
    public:
        virtual size_t GetId() = 0;
        virtual Result Process(State const & state, bool isRestore) = 0;
    };

    typedef std::shared_ptr<IHandler> TIHandlerPtr;

    class CharHandler : public IHandler
    {
    public:
        CharHandler(std::string const &source, std::string const &pattern);

        virtual size_t GetId() override { return CharHandlerId; }
        virtual Result Process(State const & state, bool isRestore) override;

    private:
        std::string _source;
        std::string _pattern;
    };

    class AsteriskHandler : public IHandler
    {
    public:
        AsteriskHandler(std::string const &source, std::string const &pattern);

        virtual size_t GetId() override { return AsteriskHandlerId; }
        virtual Result Process(State const & state, bool isRestore) override;

    private:
        std::string _source;
        std::string _pattern;
    };
}