#pragma once

#include <string>
#include <vector>
#include "Handlers.h"

namespace LargeLogReader
{
    std::string PreparePattern(std::string const &source);

    bool IsEof(std::string const &str, size_t pos);

    TIHandlerPtr GetHandler(std::vector<TIHandlerPtr> const &handlers, size_t id);
}