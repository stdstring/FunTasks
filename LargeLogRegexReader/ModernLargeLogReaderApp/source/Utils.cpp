#include <algorithm>
#include <stdexcept>
#include <string>
#include <vector>
#include "CommonDefs.h"
#include "Handlers.h"
#include "Utils.h"

namespace LargeLogReader
{
    std::string PreparePattern(std::string const &source)
    {
        std::string dest;
        bool isAsterisk = false;
        for (std::string::const_iterator iterator = source.cbegin(); iterator != source.cend(); ++iterator)
        {
            char current = *iterator;
            if (current == Asterisk)
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
        // process of single last backslash
        if (dest.size() == 1 && dest.at(dest.size() - 1) == Backslash)
            dest.push_back(Backslash);
        if (dest.size() > 1 && dest.at(dest.size() - 1) == Backslash && dest.at(dest.size() - 2) != Backslash)
            dest.push_back(Backslash);
        return dest;
    }

    bool IsEof(std::string const &str, size_t pos)
    {
        return pos >= str.size();
    }

    TIHandlerPtr GetHandler(std::vector<TIHandlerPtr> const &handlers, size_t id)
    {
        typedef std::vector<TIHandlerPtr>::const_iterator TIterator;
        TIterator result = std::find_if(handlers.cbegin(), handlers.cend(), [id](TIHandlerPtr handler) { return handler->GetId() == id; });
        if (result == handlers.cend())
            throw std::out_of_range("Bad handler's id");
        return *result;
    }
}