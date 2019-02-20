#pragma once

#include <map>
#include <sstream>
#include <string>
#include "BadDataException.h"

namespace T9Spelling
{

typedef std::map<char, std::string> SpellingDataType;

class T9Processor
{
public:
    T9Processor(SpellingDataType const &data, std::string const &pauseValue) : _spellingData(data), _pauseValue(pauseValue) {}

    std::string ProcessString(std::string const& source) const
    {
        std::stringstream buffer;
        char prevKey = '\0';
        for (unsigned int index = 0; index < source.size(); ++index)
            prevKey = ProcessChar(source[index], prevKey, buffer);
        return buffer.str();
    }

    ~T9Processor() = default;
    // we could add these members in future if we will need in them
    T9Processor() = delete;
    T9Processor(T9Processor const&) = delete;
    T9Processor& operator=(T9Processor const&) = delete;
    T9Processor(T9Processor&&) = delete;
    T9Processor& operator=(T9Processor&&) = delete;

private:
    char ProcessChar(char ch, char prevKey, std::stringstream& buffer) const
    {
        SpellingDataType::const_iterator findResult = _spellingData.find(ch);
        if (findResult == _spellingData.end())
            throw BadDataException();
        std::string spellingValue = findResult->second;
        char currentKey = spellingValue.at(0);
        if (currentKey == prevKey)
            buffer << _pauseValue;
        buffer << spellingValue;
        return currentKey;
    }

    const SpellingDataType _spellingData;
    const std::string _pauseValue;
};

}