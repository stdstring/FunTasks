#include <string>
#include "CommonDefs.h"
#include "Handlers.h"
#include "Utils.h"

namespace LargeLogReader
{
    CharHandler::CharHandler(std::string const &source, std::string const &pattern) : _source(source), _pattern(pattern)
    {
    }

    Result CharHandler::Process(State const & state, bool isRestore)
    {
        bool allowWildcards = true;
        size_t patternPos = state.PatternPos;
        char patternCh = IsEof(_pattern, patternPos) ? Eof : _pattern.at(patternPos);
        if (patternCh == Backslash)
        {
            ++patternPos;
            patternCh = _pattern.at(patternPos);
            allowWildcards = false;
        }
        char sourceCh = _source.at(state.SourcePos);
        if (allowWildcards && patternCh == Asterisk)
            return Result(ResultType::CHANGE_HANDLER, State(state.SourcePos, patternPos, AsteriskHandlerId));
        if (allowWildcards && patternCh == QuestionMark)
            return Result(ResultType::NEXT_CHAR, State(state.SourcePos + 1, patternPos + 1, CharHandlerId));
        if (sourceCh == patternCh)
            return Result(ResultType::NEXT_CHAR, State(state.SourcePos + 1, patternPos + 1, CharHandlerId));
        return Result(ResultType::FAIL, State(state.SourcePos, patternPos, CharHandlerId));
    }

    AsteriskHandler::AsteriskHandler(std::string const &source, std::string const &pattern) : _source(source), _pattern(pattern)
    {
    }

    Result AsteriskHandler::Process(State const & state, bool isRestore)
    {
        if (isRestore)
        {
            return Result(ResultType::CHANGE_HANDLER, State(state.SourcePos + 1, state.PatternPos + 1, CharHandlerId), State(state.SourcePos + 1, state.PatternPos, AsteriskHandlerId));
        }
        else
        {
            if (IsEof(_pattern, state.PatternPos + 1))
                return Result(ResultType::SUCCESS, State(0, 0, 0));
            else
                return Result(ResultType::CHANGE_HANDLER, State(state.SourcePos, state.PatternPos + 1, CharHandlerId), State(state.SourcePos, state.PatternPos, AsteriskHandlerId));
        }
    }
}