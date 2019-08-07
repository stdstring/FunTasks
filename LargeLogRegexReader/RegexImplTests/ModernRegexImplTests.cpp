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

}
