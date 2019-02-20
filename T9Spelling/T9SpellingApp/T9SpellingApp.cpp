#include <iostream>
#include <map>
#include <sstream>
#include <string>
#include "BadDataException.h"
#include "T9Processor.h"

namespace T9Spelling
{

// TODO (std_string) : probably, we may read this data from external source
const SpellingDataType SpellingDef =
{
    {'a', "2"},
    {'b', "22"},
    {'c', "222"},
    {'d', "3"},
    {'e', "33"},
    {'f', "333"},
    {'g', "4"},
    {'h', "44"},
    {'i', "444"},
    {'j', "5"},
    {'k', "55"},
    {'l', "555"},
    {'m', "6"},
    {'n', "66"},
    {'o', "666"},
    {'p', "7"},
    {'q', "77"},
    {'r', "777"},
    {'s', "7777"},
    {'t', "8"},
    {'u', "88"},
    {'v', "888"},
    {'w', "9"},
    {'x', "99"},
    {'y', "999"},
    {'z', "9999"},
    {' ', "0"},
};

// TODO (std_string) : probably, we may read this data from external source
std::string PauseValue(" ");

void Process()
{
    // read case count
    unsigned int caseCount = 0;
    std::string firstLine;
    std::getline(std::cin, firstLine);
    std::stringstream firstLineBuffer(firstLine);
    firstLineBuffer >> caseCount;
    if (!firstLineBuffer.eof())
        throw BadDataException();
    // create processor
    T9Processor processor(SpellingDef, PauseValue);
    // read input
    for (unsigned int case_index = 1; case_index <= caseCount; ++case_index)
    {
        std::string source;
        std::getline(std::cin, source);
        std::string dest = processor.ProcessString(source);
        std::cout << "Case #" << case_index << ": " << dest << std::endl;
    }
}

}

int main()
{
    try
    {
        T9Spelling::Process();
    }
    catch (T9Spelling::BadDataException&)
    {
        std::cerr << "Bad input data" << std::endl;
        return -1;
    }
    return 0;
}
