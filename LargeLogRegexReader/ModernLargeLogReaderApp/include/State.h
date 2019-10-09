#pragma once

namespace LargeLogReader
{
    struct State
    {
    public:
        State() : SourcePos(0), PatternPos(0), HandlerId(0), Empty(true) {}
        State(size_t sourcePos, size_t patternPos, size_t handlerId) : SourcePos(sourcePos), PatternPos(patternPos), HandlerId(handlerId), Empty(false) {}

        size_t SourcePos;
        size_t PatternPos;
        size_t HandlerId;
        bool Empty;
    };
}