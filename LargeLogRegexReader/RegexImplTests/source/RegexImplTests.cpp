#include <algorithm>
#include <memory>
#include <stdexcept>
#include <string>
#include <vector>
#include "gtest/gtest.h"

namespace RegexImpl
{

namespace
{

constexpr char Asterisk = '*';
constexpr char Backslash = '\\';
constexpr char QuestionMark = '?';
constexpr char Eof = 0;

class IStringSource
{
public:
    virtual bool IsEof() = 0;
    virtual char ReadCurrent() = 0;
    virtual bool MoveTo(size_t position) = 0;
};

typedef std::shared_ptr<IStringSource> TIStringSourcePtr;

class SimpleStringSource : public IStringSource
{
public:
    SimpleStringSource(std::string const &source);
    virtual bool IsEof() override;
    virtual char ReadCurrent() override;
    virtual bool MoveTo(size_t position) override;

private:
    std::string _source;
    size_t _position;
};

SimpleStringSource::SimpleStringSource(std::string const &source) : _source(source), _position(0)
{
}

bool SimpleStringSource::IsEof()
{
    return _position >= _source.size();
}

char SimpleStringSource::ReadCurrent()
{
    return IsEof() ? Eof : _source.at(_position);
}

bool SimpleStringSource::MoveTo(size_t position)
{
    if (position > _source.size())
        return false;
    _position = position;
    return true;
}

bool IsEof(std::string const &str, size_t pos)
{
    return pos >= str.size();
}

enum CharType { USUAL_CHAR, BACKSLASH, ASTERISK };

std::string PreparePattern(std::string const &source)
{
    std::string dest;
    CharType lastCharType = CharType::USUAL_CHAR;
    for (std::string::const_iterator iterator = source.cbegin(); iterator != source.cend(); ++iterator)
    {
        char current = *iterator;
        switch (lastCharType)
        {
        case CharType::BACKSLASH:
            dest.push_back(current);
            lastCharType = CharType::USUAL_CHAR;
            break;
        case CharType::ASTERISK:
            if (current != Asterisk)
            {
                dest.push_back(current);
                lastCharType = current == Backslash ? CharType::BACKSLASH : CharType::USUAL_CHAR;
            }
            break;
        case CharType::USUAL_CHAR:
            dest.push_back(current);
            lastCharType = CharType::USUAL_CHAR;
            if (current == Backslash)
                lastCharType = CharType::BACKSLASH;
            if (current == Asterisk)
                lastCharType = CharType::ASTERISK;
            break;
        }
    }
    // process of single last backslash
    if (lastCharType == CharType::BACKSLASH)
        dest.push_back(Backslash);
    return dest;
}

struct State
{
public:
    State();
    State(size_t sourcePos, size_t patternPos, size_t handlerId);

    size_t SourcePos;
    size_t PatternPos;
    size_t HandlerId;
    bool Empty;
};

State::State() : SourcePos(0), PatternPos(0), HandlerId(0), Empty(true)
{
}

State::State(size_t sourcePos, size_t patternPos, size_t handlerId) : SourcePos(sourcePos), PatternPos(patternPos), HandlerId(handlerId), Empty(false)
{
}

enum ResultType { NEXT_CHAR, CHANGE_HANDLER, SUCCESS, FAIL };

struct Result
{
public:
    Result(ResultType type, State nextState);
    Result(ResultType type, State nextState, State restoreState);

    ResultType Type;
    State NextState;
    State RestoreState;
};

Result::Result(ResultType type, State nextState) : Type(type), NextState(nextState), RestoreState(State())
{
}

Result::Result(ResultType type, State nextState, State restoreState) : Type(type), NextState(nextState), RestoreState(restoreState)
{
}

class IHandler
{
public:
    virtual size_t GetId() = 0;
    virtual Result Process(State const & state, bool isRestore) = 0;
};

typedef std::shared_ptr<IHandler> TIHandlerPtr;

constexpr size_t CharHandlerId = 1;
constexpr size_t DefaultHandlerId = CharHandlerId;
constexpr size_t AsteriskHandlerId = 2;

class CharHandler : public IHandler
{
public:
    CharHandler(TIStringSourcePtr source, std::string const &pattern);

    virtual size_t GetId() override { return CharHandlerId; }
    virtual Result Process(State const & state, bool isRestore) override;

private:
    TIStringSourcePtr _source;
    std::string _pattern;
};

CharHandler::CharHandler(TIStringSourcePtr source, std::string const &pattern) : _source(source), _pattern(pattern)
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
    //char sourceCh = _source.at(state.SourcePos);
    _source->MoveTo(state.SourcePos);
    char sourceCh = _source->ReadCurrent();
    if (allowWildcards && patternCh == Asterisk)
        return Result(ResultType::CHANGE_HANDLER, State(state.SourcePos, patternPos, AsteriskHandlerId));
    if (allowWildcards && patternCh == QuestionMark)
        return Result(ResultType::NEXT_CHAR, State(state.SourcePos + 1, patternPos + 1, CharHandlerId));
    if (sourceCh == patternCh)
        return Result(ResultType::NEXT_CHAR, State(state.SourcePos + 1, patternPos + 1, CharHandlerId));
    return Result(ResultType::FAIL, State(state.SourcePos, patternPos, CharHandlerId));
}

class AsteriskHandler : public IHandler
{
public:
    AsteriskHandler(TIStringSourcePtr source, std::string const &pattern);

    virtual size_t GetId() override { return AsteriskHandlerId; }
    virtual Result Process(State const & state, bool isRestore) override;

private:
    TIStringSourcePtr _source;
    std::string _pattern;
};

AsteriskHandler::AsteriskHandler(TIStringSourcePtr source, std::string const &pattern) : _source(source), _pattern(pattern)
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

bool ProcessFinish(TIStringSourcePtr source, std::string const &pattern, State const &state)
{
    if (source->IsEof() && IsEof(pattern, state.PatternPos))
        return true;
    if (!source->IsEof() && IsEof(pattern, state.PatternPos))
        return false;
    if (source->IsEof() && !IsEof(pattern, state.PatternPos))
        return pattern.at(state.PatternPos) == Asterisk && IsEof(pattern, state.PatternPos + 1);
    return false;
}

TIHandlerPtr GetHandler(std::vector<TIHandlerPtr> const &handlers, size_t id)
{
    typedef std::vector<TIHandlerPtr>::const_iterator TIterator;
    TIterator result = std::find_if(handlers.cbegin(), handlers.cend(), [id](TIHandlerPtr handler) { return handler->GetId() == id; });
    if (result == handlers.cend())
        throw std::out_of_range("Bad handler's id");
    return *result;
}

bool IsMatchImpl(TIStringSourcePtr source, std::string const &pattern)
{
    std::vector<TIHandlerPtr> handlers = {std::make_shared<CharHandler>(source, pattern), std::make_shared<AsteriskHandler>(source, pattern)};
    State currentState(0, 0, DefaultHandlerId);
    State restoreState;
    bool restore = false;
    while (!source->IsEof())
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

bool IsMatch(TIStringSourcePtr source, std::string const &pattern)
{
    return IsMatchImpl(source, PreparePattern(pattern));
}

bool IsMatch(std::string const &source, std::string const &pattern)
{
    return IsMatch(std::make_shared<SimpleStringSource>(source), pattern);
}

}

TEST(ModernRegexImplTests, PreparePattern)
{
    ASSERT_EQ("", PreparePattern(""));
    ASSERT_EQ("aaa", PreparePattern("aaa"));
    ASSERT_EQ("abc", PreparePattern("abc"));
    ASSERT_EQ("a?b?c?", PreparePattern("a?b?c?"));
    ASSERT_EQ("a*b", PreparePattern("a*b"));
    ASSERT_EQ("ab*", PreparePattern("ab*"));
    ASSERT_EQ("*ab", PreparePattern("*ab"));
    ASSERT_EQ("*a*b*", PreparePattern("*a*b*"));
    ASSERT_EQ("?*a*b*?", PreparePattern("?*a*b*?"));
    ASSERT_EQ("*ab", PreparePattern("*****ab"));
    ASSERT_EQ("a*b", PreparePattern("a*****b"));
    ASSERT_EQ("ab*", PreparePattern("ab*****"));
    ASSERT_EQ("*a*b*", PreparePattern("****a*****b*****"));
    ASSERT_EQ("?*a*b*?", PreparePattern("?***a****b******?"));
    ASSERT_EQ("*?*a*b*?*", PreparePattern("**?***a****b*****?******"));
    ASSERT_EQ("\\\\", PreparePattern("\\"));
    ASSERT_EQ("a\\\\", PreparePattern("a\\"));
    ASSERT_EQ("a\\a\\ba\\\\", PreparePattern("a\\a\\ba\\"));
    ASSERT_EQ("a\\*b", PreparePattern("a\\*b"));
    ASSERT_EQ("a\\**b", PreparePattern("a\\**b"));
    ASSERT_EQ("a\\*?b", PreparePattern("a\\*?b"));
    ASSERT_EQ("a\\?b", PreparePattern("a\\?b"));
    ASSERT_EQ("a\\??b", PreparePattern("a\\??b"));
    ASSERT_EQ("a\\?*b", PreparePattern("a\\?*b"));
    ASSERT_EQ("a*\\**b", PreparePattern("a***\\*****b"));
}

TEST(ModernRegexImplTests, IsMatch)
{
    ASSERT_TRUE(IsMatch("", ""));
    ASSERT_FALSE(IsMatch("abc", ""));
    ASSERT_FALSE(IsMatch("", "abc"));
    ASSERT_TRUE(IsMatch("abc", "abc"));
    ASSERT_TRUE(IsMatch("abc", "a?c"));
    ASSERT_FALSE(IsMatch("abc", "adc"));
    ASSERT_TRUE(IsMatch("abc", "*"));
    ASSERT_TRUE(IsMatch("ac", "a*c"));
    ASSERT_TRUE(IsMatch("ac", "a**c"));
    ASSERT_TRUE(IsMatch("ac", "a***c"));
    ASSERT_TRUE(IsMatch("abc", "a*c"));
    ASSERT_TRUE(IsMatch("abc", "a**c"));
    ASSERT_TRUE(IsMatch("abc", "a**c"));
    ASSERT_TRUE(IsMatch("abbbbbbbbbbbbbbc", "a*c"));
    ASSERT_TRUE(IsMatch("abbbbbbbbbbbbbbc", "a**c"));
    ASSERT_TRUE(IsMatch("abbbbbbbbbbbbbbc", "a***c"));
    ASSERT_TRUE(IsMatch("abc", "a*b*c"));
    ASSERT_TRUE(IsMatch("abc", "a**b*c"));
    ASSERT_TRUE(IsMatch("abc", "a*b**c"));
    ASSERT_TRUE(IsMatch("aabbcc", "a*b*c"));
    ASSERT_TRUE(IsMatch("aabbcc", "a**b*c"));
    ASSERT_TRUE(IsMatch("aabbcc", "a*b**c"));
    ASSERT_FALSE(IsMatch("aacc", "a*b*c"));
    ASSERT_FALSE(IsMatch("aacc", "a**b*c"));
    ASSERT_FALSE(IsMatch("aacc", "a*b**c"));
    ASSERT_TRUE(IsMatch("abcabcdddabc", "*abc"));
    ASSERT_TRUE(IsMatch("abcabcdddabc", "**abc"));
    ASSERT_TRUE(IsMatch("abcabcdddabc", "***abc"));
    ASSERT_FALSE(IsMatch("abcacb", "*abc"));
    ASSERT_FALSE(IsMatch("abcacb", "**abc"));
    ASSERT_FALSE(IsMatch("abcacb", "***abc"));
    ASSERT_TRUE(IsMatch("abcabcabcddd", "abc*"));
    ASSERT_TRUE(IsMatch("abcabcabcddd", "abc**"));
    ASSERT_TRUE(IsMatch("abcabcabcddd", "abc***"));
    ASSERT_FALSE(IsMatch("acbdddabcabc", "abc*"));
    ASSERT_FALSE(IsMatch("acbdddabcabc", "abc**"));
    ASSERT_FALSE(IsMatch("acbdddabcabc", "abc***"));
    ASSERT_TRUE(IsMatch("aabbaaabcdd", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabbaaabcdd", "*aabc**"));
    ASSERT_TRUE(IsMatch("aabbaaabcdd", "*aabc***"));
    ASSERT_TRUE(IsMatch("aabbaaabc", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabbaaabc", "**aabc*"));
    ASSERT_TRUE(IsMatch("aabbaaabc", "*aabc**"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "**aabc*"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "*aabc**"));
    ASSERT_FALSE(IsMatch("aabbcabbccabc", "*aabc*"));
    ASSERT_FALSE(IsMatch("aabbcabbccabc", "**aabc*"));
    ASSERT_FALSE(IsMatch("aabbcabbccabc", "*aabc**"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "*a?bc*"));
    ASSERT_TRUE(IsMatch("axbcaaabdd", "*a?bc*"));
    ASSERT_FALSE(IsMatch("aacaaabdd", "*a?bc*"));
    ASSERT_TRUE(IsMatch("abc", "a*?*c"));
    ASSERT_TRUE(IsMatch("abc", "a**?*c"));
    ASSERT_TRUE(IsMatch("abc", "a*?**c"));
    ASSERT_FALSE(IsMatch("ac", "a?c"));
    ASSERT_FALSE(IsMatch("ac", "a*?*c"));
    ASSERT_FALSE(IsMatch("ac", "a**?*c"));
    ASSERT_FALSE(IsMatch("ac", "a*?**c"));
    ASSERT_TRUE(IsMatch("abbbc", "a*?*c"));
    ASSERT_TRUE(IsMatch("abbbc", "a**?*c"));
    ASSERT_TRUE(IsMatch("abbbc", "a*?**c"));
    ASSERT_TRUE(IsMatch("a*c", "a\\*c"));
    ASSERT_FALSE(IsMatch("abc", "a\\*c"));
    ASSERT_FALSE(IsMatch("a**c", "a\\*c"));
    ASSERT_TRUE(IsMatch("a?c", "a\\?c"));
    ASSERT_FALSE(IsMatch("abc", "a\\?c"));
    ASSERT_TRUE(IsMatch("ac\\", "ac\\"));
    ASSERT_TRUE(IsMatch("a*c", "a\\**c"));
    ASSERT_TRUE(IsMatch("a**c", "a\\**c"));
    ASSERT_TRUE(IsMatch("a***c", "a\\**c"));
    ASSERT_TRUE(IsMatch("a*ccccc", "a\\**c"));
}

}
