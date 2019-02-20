#include <cstdlib>
#include <string>
#include <sstream>
#include <fstream>
#include <streambuf>
#include "gtest/gtest.h"

namespace T9Spelling
{

namespace
{

// TODO (std_string) : use C++17 filesystem lib instead of these
// TODO (std_string) : these valid only for ctest & Debug config & VS Generator. Think about universal solution
const std::string RootPath("..\\..\\");
const std::string T9SpellingPath(RootPath + "bin\\T9SpellingApp\\Debug\\");
const std::string InputPath(RootPath + "input\\");
const std::string OutputPath(RootPath + "output\\");
const std::string T9SpellingApp(T9SpellingPath + "T9SpellingApp.exe");
const std::string ActualOutputFilename(T9SpellingPath + "T9SpellingAppResult.out");

std::string CreateCommand(std::string const &input)
{
    std::stringstream stream;
    stream << T9SpellingApp << " < " << input << " > " << ActualOutputFilename << " 2>&1";
    return stream.str();
}

struct T9SpellingIntegrationData
{
public:
    std::string InputFilename;
    std::string OutputFilename;
    int ReturnCode;
    std::string Name;
};

}

class T9SpellingIntegrationTests : public testing::TestWithParam<T9SpellingIntegrationData>
{
};

TEST_P(T9SpellingIntegrationTests, Execute)
{
    T9SpellingIntegrationData data(GetParam());
    std::string command(CreateCommand(InputPath + data.InputFilename));
    int returnCode = system(command.c_str());
    EXPECT_EQ(data.ReturnCode, returnCode);
    std::ifstream expectedOutputFile(OutputPath + data.OutputFilename);
    std::string expectedOutput((std::istreambuf_iterator<char>(expectedOutputFile)), std::istreambuf_iterator<char>());
    std::ifstream actualOutputFile(ActualOutputFilename);
    std::string actualOutput((std::istreambuf_iterator<char>(actualOutputFile)), std::istreambuf_iterator<char>());
    EXPECT_EQ(expectedOutput, actualOutput);
}

INSTANTIATE_TEST_CASE_P(T9Spelling, T9SpellingIntegrationTests, testing::Values(T9SpellingIntegrationData{"C-small-practice.in", "C-small-practice.out", 0, "Processing_C_small_practice"},
                                                                                T9SpellingIntegrationData{"C-large-practice.in", "C-large-practice.out", 0, "Processing_C_large_practice"},
                                                                                T9SpellingIntegrationData{"Bad-data.in", "Bad-data.out", -1, "Processing_Bad_data"}),
                                                                [](testing::TestParamInfo<T9SpellingIntegrationData> const &data) { return data.param.Name; });

}