#include <array>
#include <iostream>
#include "CLogReader.h"

constexpr int BufferSize = 16 * 1024;

int main (int argc, char *argv[])
{
    if (argc != 3)
        return 1;
    char *filename = argv[1];
    char *filter = argv[2];
    LargeLogReader::CLogReader reader;
    if (!reader.Open(filename))
        return 1;
    reader.SetFilter(filter);
    std::array<char, BufferSize> buffer;
    buffer.fill(0);
    while (reader.GetNextLine(buffer.data(), BufferSize))
    {
        std::cout << buffer.data() << std::endl;
        buffer.fill(0);
    }
    reader.Close();
    return 0;
}