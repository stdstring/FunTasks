#include <string>
#include "gtest/gtest.h"

#include "BadDataException.h"
#include "T9Processor.h"

namespace T9Spelling
{

namespace
{

struct T9ProcessorData
{
public:
    std::string Source;
    std::string Result;
    std::string Name;
};

struct T9ProcessorFailsData
{
public:
    std::string Source;
    std::string Name;
};

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

}

class T9ProcessorTests : public testing::TestWithParam<T9ProcessorData>
{
};

class T9ProcessorFailsTests : public testing::TestWithParam<T9ProcessorFailsData>
{
};

TEST_P(T9ProcessorTests, ProcessString)
{
    T9ProcessorData data = GetParam();
    T9Processor processor(SpellingDef, PauseValue);
    EXPECT_EQ(data.Result, processor.ProcessString(data.Source));
}

TEST_P(T9ProcessorFailsTests, ProcessString)
{
    T9ProcessorFailsData data = GetParam();
    T9Processor processor(SpellingDef, PauseValue);
    EXPECT_THROW(processor.ProcessString(data.Source), BadDataException);
}

INSTANTIATE_TEST_CASE_P(T9Processor, T9ProcessorTests, testing::Values(T9ProcessorData {"a", "2", "Processing_a"},
                                                                       T9ProcessorData {"b", "22", "Processing_b"},
                                                                       T9ProcessorData {"c", "222", "Processing_c"},
                                                                       T9ProcessorData {"d", "3", "Processing_d"},
                                                                       T9ProcessorData {"e", "33", "Processing_e"},
                                                                       T9ProcessorData {"f", "333", "Processing_f"},
                                                                       T9ProcessorData {"g", "4", "Processing_g"},
                                                                       T9ProcessorData {"h", "44", "Processing_h"},
                                                                       T9ProcessorData {"i", "444", "Processing_i"},
                                                                       T9ProcessorData {"j", "5", "Processing_j"},
                                                                       T9ProcessorData {"k", "55", "Processing_k"},
                                                                       T9ProcessorData {"l", "555", "Processing_l"},
                                                                       T9ProcessorData {"m", "6", "Processing_m"},
                                                                       T9ProcessorData {"n", "66", "Processing_n"},
                                                                       T9ProcessorData {"o", "666", "Processing_o"},
                                                                       T9ProcessorData {"p", "7", "Processing_p"},
                                                                       T9ProcessorData {"q", "77", "Processing_q"},
                                                                       T9ProcessorData {"r", "777", "Processing_r"},
                                                                       T9ProcessorData {"s", "7777", "Processing_s"},
                                                                       T9ProcessorData {"t", "8", "Processing_t"},
                                                                       T9ProcessorData {"u", "88", "Processing_u"},
                                                                       T9ProcessorData {"v", "888", "Processing_v"},
                                                                       T9ProcessorData {"w", "9", "Processing_w"},
                                                                       T9ProcessorData {"x", "99", "Processing_x"},
                                                                       T9ProcessorData {"y", "999", "Processing_y"},
                                                                       T9ProcessorData {"z", "9999", "Processing_z"},
                                                                       T9ProcessorData {" ", "0", "Processing_space"},
                                                                       T9ProcessorData {"aa", "2 2", "Processing_aa"},
                                                                       T9ProcessorData {"ab", "2 22", "Processing_ab"},
                                                                       T9ProcessorData {"ac", "2 222", "Processing_ac"},
                                                                       T9ProcessorData {"ad", "23", "Processing_ad"},
                                                                       T9ProcessorData {"aaa", "2 2 2", "Processing_aaa"},
                                                                       T9ProcessorData {"hi", "44 444", "Processing_hi"},
                                                                       T9ProcessorData {"yes", "999337777", "Processing_yes"},
                                                                       T9ProcessorData {"foo  bar", "333666 6660 022 2777", "Processing_foo_2space_bar"},
                                                                       T9ProcessorData{ "hello world", "4433555 555666096667775553", "Processing_hello_space_world"}),
                                                       [](testing::TestParamInfo<T9ProcessorData> const &data) { return data.param.Name; });

INSTANTIATE_TEST_CASE_P(T9ProcessorFails, T9ProcessorFailsTests, testing::Values(T9ProcessorFailsData {"A", "Processing_A"},
                                                                                 T9ProcessorFailsData {"1", "Processing_1"},
                                                                                 T9ProcessorFailsData {"aA", "Processing_aA"},
                                                                                 T9ProcessorFailsData {"a A", "Processing_a_space_A"}),
                                                                 [](testing::TestParamInfo<T9ProcessorFailsData> const &data) { return data.param.Name; });

}