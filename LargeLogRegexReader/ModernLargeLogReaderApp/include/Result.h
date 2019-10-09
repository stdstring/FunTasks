#pragma once

#include "State.h"

namespace LargeLogReader
{
    enum ResultType { NEXT_CHAR, CHANGE_HANDLER, SUCCESS, FAIL };

    struct Result
    {
    public:
        Result(ResultType type, State nextState) : Type(type), NextState(nextState), RestoreState(State()) {}
        Result(ResultType type, State nextState, State restoreState) : Type(type), NextState(nextState), RestoreState(restoreState) {}

        ResultType Type;
        State NextState;
        State RestoreState;
    };
}