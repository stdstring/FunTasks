#include <string>
#include "gtest/gtest.h"

namespace RegexImpl
{

namespace
{

std::string SimplifyPattern(std::string const &source)
{
    std::string dest;
    bool isAsterisk = false;
    for (std::string::const_iterator iterator = source.cbegin(); iterator != source.cend(); ++iterator)
    {
        char current = *iterator;
        if (current == '*')
        {
            if (!isAsterisk)
            {
                dest.push_back(current);
                isAsterisk = true;
            }
        }
        else
        {
            dest.push_back(current);
            isAsterisk = false;
        }
    }
    return dest;
}

bool ProcessLastAsteriskMatch(std::string const &source, std::string const &pattern, size_t sourcePos, size_t patternPos)
{
    if (patternPos == pattern.size() - 1)
        return true;
    patternPos += 1;
    size_t startSourcePos = source.size() - (pattern.size() - patternPos);
    if (startSourcePos < sourcePos)
        return false;
    for (size_t currentSourcePos = startSourcePos, currentPatternPos = patternPos; currentSourcePos < source.size() && currentPatternPos < pattern.size(); ++currentSourcePos, ++currentPatternPos)
    {
        if (pattern[currentPatternPos] != '?' && pattern[currentPatternPos] != source[currentSourcePos])
            return false;
    }
    return true;
}

bool IsMatch(std::string const &source, std::string const &pattern)
{
    if (pattern.empty())
        return source.empty();
    size_t lastAsteriskPos = pattern.find_last_of('*');
    size_t sourcePos = 0;
    size_t patternPos = 0;
    size_t sourceStablePos = std::string::npos;
    size_t patternStablePos = std::string::npos;
    while (sourcePos < source.size())
    {
        if (pattern[patternPos] == '*')
        {
            if (patternPos == lastAsteriskPos)
                return ProcessLastAsteriskMatch(source, pattern, sourcePos, patternPos);
            sourceStablePos = sourcePos;
            patternStablePos = patternPos;
            ++patternPos;
        }
        else if (pattern[patternPos] == '?')
        {
            ++sourcePos;
            ++patternPos;
        }
        else if (source[sourcePos] == pattern[patternPos])
        {
            ++sourcePos;
            ++patternPos;
        }
        else if (source[sourcePos] != pattern[patternPos])
        {
            if (sourceStablePos == std::string::npos)
                return false;
            ++sourceStablePos;
            sourcePos = sourceStablePos;
            patternPos = patternStablePos + 1;
        }
    }
    return (patternPos == pattern.size()) || (patternPos == pattern.size() - 1 && patternPos == lastAsteriskPos);
}

}

TEST(ModernRegexImplTests, SimplifyPattern)
{
    ASSERT_EQ("", SimplifyPattern(""));
    ASSERT_EQ("aaa", SimplifyPattern("aaa"));
    ASSERT_EQ("abc", SimplifyPattern("abc"));
    ASSERT_EQ("a?b?c?", SimplifyPattern("a?b?c?"));
    ASSERT_EQ("a*b", SimplifyPattern("a*b"));
    ASSERT_EQ("ab*", SimplifyPattern("ab*"));
    ASSERT_EQ("*ab", SimplifyPattern("*ab"));
    ASSERT_EQ("*a*b*", SimplifyPattern("*a*b*"));
    ASSERT_EQ("?*a*b*?", SimplifyPattern("?*a*b*?"));
    ASSERT_EQ("*ab", SimplifyPattern("*****ab"));
    ASSERT_EQ("a*b", SimplifyPattern("a*****b"));
    ASSERT_EQ("ab*", SimplifyPattern("ab*****"));
    ASSERT_EQ("*a*b*", SimplifyPattern("****a*****b*****"));
    ASSERT_EQ("?*a*b*?", SimplifyPattern("?***a****b******?"));
    ASSERT_EQ("*?*a*b*?*", SimplifyPattern("**?***a****b*****?******"));
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
    ASSERT_TRUE(IsMatch("abc", "a*c"));
    ASSERT_TRUE(IsMatch("abbbbbbbbbbbbbbc", "a*c"));
    ASSERT_TRUE(IsMatch("abc", "a*b*c"));
    ASSERT_TRUE(IsMatch("aabbcc", "a*b*c"));
    ASSERT_FALSE(IsMatch("aacc", "a*b*c"));
    ASSERT_TRUE(IsMatch("abcabcdddabc", "*abc"));
    ASSERT_FALSE(IsMatch("abcacb", "*abc"));
    ASSERT_TRUE(IsMatch("abcabcabcddd", "abc*"));
    ASSERT_FALSE(IsMatch("acbdddabcabc", "abc*"));
    ASSERT_TRUE(IsMatch("aabbaaabcdd", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabbaaabc", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "*aabc*"));
    ASSERT_FALSE(IsMatch("aabbcabbccabc", "*aabc*"));
    ASSERT_TRUE(IsMatch("aabcaaabdd", "*a?bc*"));
    ASSERT_TRUE(IsMatch("axbcaaabdd", "*a?bc*"));
    ASSERT_FALSE(IsMatch("aacaaabdd", "*a?bc*"));
}

}
