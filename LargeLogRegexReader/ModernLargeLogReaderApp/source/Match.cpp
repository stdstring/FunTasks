#include <string>
#include <vector>
#include "CommonDefs.h"
#include "Handlers.h"
#include "Match.h"
#include "Utils.h"

namespace LargeLogReader
{
    namespace
    {
        bool ProcessFinish(std::string const & source, std::string const &pattern, State const &state)
        {
            if (IsEof(source, state.SourcePos) && IsEof(pattern, state.PatternPos))
                return true;
            if (!IsEof(source, state.SourcePos) && IsEof(pattern, state.PatternPos))
                return false;
            // TODO (std_string) : think about this case
            if (IsEof(source, state.SourcePos) && !IsEof(pattern, state.PatternPos))
                return pattern.at(state.PatternPos) == Asterisk && IsEof(pattern, state.PatternPos + 1);
            return false;
        }
    }

    bool IsMatch(std::string const &source, std::string const &pattern)
    {
        std::vector<TIHandlerPtr> handlers = { std::make_shared<CharHandler>(source, pattern), std::make_shared<AsteriskHandler>(source, pattern) };
        State currentState(0, 0, DefaultHandlerId);
        State restoreState;
        bool restore = false;
        while (!IsEof(source, currentState.SourcePos))
        {
            TIHandlerPtr handler = GetHandler(handlers, currentState.HandlerId);
            Result result = handler->Process(currentState, restore);
            restore = false;
            switch (result.Type)
            {
            case ResultType::NEXT_CHAR:
            case ResultType::CHANGE_HANDLER:
                currentState = result.NextState;
                if (!result.RestoreState.Empty)
                    restoreState = result.RestoreState;
                break;
            case ResultType::SUCCESS:
                return true;
            case ResultType::FAIL:
                if (restoreState.Empty)
                    return false;
                currentState = restoreState;
                restore = true;
                break;
            default:
                return false;
            }
        }
        return ProcessFinish(source, pattern, currentState);
    }
}