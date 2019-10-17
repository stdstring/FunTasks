#include <cstdlib>
#include <fstream>
#include <sstream>
#include <streambuf>
#include <string>
#include <vector>
#include "gtest/gtest.h"

namespace LargeLogReader
{
    namespace
    {
        // TODO (std_string) : use C++17 filesystem lib instead of these
        // TODO (std_string) : these valid only for ctest & Debug config & VS Generator. Think about universal solution
        const std::string RootPath("..\\..\\");
        const std::string InputPath(RootPath + "input\\");
        //const std::string OutputPath(RootPath + "output\\");
        const std::string ModernReaderAppPath(RootPath + "bin\\ModernLargeLogReaderApp\\Debug\\");
        const std::string ModernReaderApp(ModernReaderAppPath + "ModernLargeLogReaderApp.exe");
        //const std::string ModernReaderAppActualOutput(ModernReaderAppPath + "ModernLargeLogReaderApp.out");
        const std::string ModernReaderAppActualOutput(".\\Debug\\ModernLargeLogReaderApp.out");

        std::string CreateCommand(std::string const &appPath, std::string const &inputFile, std::string const &pattern, std::string const &outputFile)
        {
            std::stringstream stream;
            stream << appPath << " " << inputFile << " " << pattern << " > " << outputFile << " 2>&1";
            return stream.str();
        }

        std::string PrepareExpectedOutputString(std::vector<std::string> expectedOutput)
        {
            std::stringstream stream;
            for (std::string const &item: expectedOutput)
            {
                stream << item << std::endl;
            }
            return stream.str();
        }

        struct ReaderAppIntegrationData
        {
        public:
            ReaderAppIntegrationData(std::string const &inputFilename, std::string const &pattern, std::vector<std::string> const &output, int returnCode) :
                InputFilename(inputFilename), Pattern(pattern), Output(output), ReturnCode(returnCode)
            {
            }

            std::string InputFilename;
            std::string Pattern;
            std::vector<std::string> Output;
            int ReturnCode;
        };
    }

    class ReaderAppIntegrationTests : public testing::TestWithParam<ReaderAppIntegrationData>
    {
    };

    TEST_P(ReaderAppIntegrationTests, Execute)
    {
        ReaderAppIntegrationData data(GetParam());
        std::string command(CreateCommand(ModernReaderApp, InputPath + data.InputFilename, data.Pattern, ModernReaderAppActualOutput));
        int returnCode = system(command.c_str());
        EXPECT_EQ(data.ReturnCode, returnCode);
        std::string expectedOutput(PrepareExpectedOutputString(data.Output));
        std::ifstream actualOutputFile(ModernReaderAppActualOutput);
        std::string actualOutput((std::istreambuf_iterator<char>(actualOutputFile)), std::istreambuf_iterator<char>());
        EXPECT_EQ(expectedOutput, actualOutput);
    }

    std::vector<std::string> FullOutput = {"abc", "aabc", "aaaabc", "abbc", "abbbbc", "abcc", "abcccc", "aabbc", "aabcc", "abbcc", "aabbcc", "aabbbcccc", "a*bc", "ab*c", "a*b*c", "a**b*c", "a*b**c", "a?bc", "ab?c", "a?b?c", "a*?b*c", "a*b*?c", "a*?b*?c", "axbc", "abyc", "axbyc"};
    INSTANTIATE_TEST_CASE_P(LargeLogReaderApp,
                            ReaderAppIntegrationTests,
                            testing::Values(ReaderAppIntegrationData("data.txt", "a", {}, 0),
                                            ReaderAppIntegrationData("data.txt", "abc", {"abc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a*bc", {"abc", "aabc", "aaaabc", "abbc", "abbbbc", "aabbc", "a*bc", "a?bc", "axbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab*c", {"abc", "abbc", "abbbbc", "abcc", "abcccc", "abbcc", "ab*c", "ab?c", "abyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a*b*c", FullOutput, 0),
                                            ReaderAppIntegrationData("data.txt", "a**b*c", FullOutput, 0),
                                            ReaderAppIntegrationData("data.txt", "a*b**c", FullOutput, 0),
                                            ReaderAppIntegrationData("data.txt", "a****b******c", FullOutput, 0),
                                            ReaderAppIntegrationData("data.txt", "a?bc", {"aabc", "abbc", "a*bc", "a?bc", "axbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab?c", {"abbc", "abcc", "ab*c", "ab?c", "abyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a?b?c", {"aabbc", "aabcc", "abbcc", "a*b*c", "a?b?c", "axbyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a?*bc", {"aabc", "aaaabc", "abbc", "abbbbc", "aabbc", "a*bc", "a?bc", "axbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a*?bc", {"aabc", "aaaabc", "abbc", "abbbbc", "aabbc", "a*bc", "a?bc", "axbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a*?*bc", {"aabc", "aaaabc", "abbc", "abbbbc", "aabbc", "a*bc", "a?bc", "axbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab?*c", {"abbc", "abbbbc", "abcc", "abcccc", "abbcc", "ab*c", "ab?c", "abyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab*?c", {"abbc", "abbbbc", "abcc", "abcccc", "abbcc", "ab*c", "ab?c", "abyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab*?*c", {"abbc", "abbbbc", "abcc", "abcccc", "abbcc", "ab*c", "ab?c", "abyc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a??bc", {"aabbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a*??bc", {"aaaabc", "abbbbc", "aabbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a?*?bc", {"aaaabc", "abbbbc", "aabbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a??*bc", {"aaaabc", "abbbbc", "aabbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a**?***?****bc", {"aaaabc", "abbbbc", "aabbc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab??c", {"abbcc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab*??c", {"abbbbc", "abcccc", "abbcc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab?*?c", {"abbbbc", "abcccc", "abbcc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab??*c", {"abbbbc", "abcccc", "abbcc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab**?***?****c", {"abbbbc", "abcccc", "abbcc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*bc", {"a*bc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\**bc", {"a*bc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab\\*c", {"ab*c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab\\**c", {"ab*c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*b\\*c", {"a*b*c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\**b\\*c", {"a*b*c", "a**b*c", "a*?b*c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*b\\**c", {"a*b*c", "a*b**c", "a*b*?c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\**b\\**c", {"a*b*c", "a**b*c", "a*b**c", "a*?b*c", "a*b*?c", "a*?b*?c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*?b\\*c", {"a**b*c", "a*?b*c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*b\\*?c", {"a*b**c","a*b*?c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\*?b\\*?c", {"a*?b*?c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\?bc", {"a?bc"}, 0),
                                            ReaderAppIntegrationData("data.txt", "ab\\?c", {"ab?c"}, 0),
                                            ReaderAppIntegrationData("data.txt", "a\\?b\\?c", {"a?b?c"}, 0)));

}