#include <algorithm>
#include <stdexcept>
#include <string>
#include <vector>
#include "CommonDefs.h"
#include "Handlers.h"
#include "Utils.h"

namespace LargeLogReader
{
    namespace
    {
        enum CharType { USUAL_CHAR, BACKSLASH, ASTERISK };
    }

    std::string PreparePattern(std::string const &source)
    {
        std::string dest;
        CharType lastCharType = CharType::USUAL_CHAR;
        for (std::string::const_iterator iterator = source.cbegin(); iterator != source.cend(); ++iterator)
        {
            char current = *iterator;
            switch (lastCharType)
            {
            case CharType::BACKSLASH:
                dest.push_back(current);
                lastCharType = CharType::USUAL_CHAR;
                break;
            case CharType::ASTERISK:
                if (current != Asterisk)
                {
                    dest.push_back(current);
                    lastCharType = current == Backslash ? CharType::BACKSLASH : CharType::USUAL_CHAR;
                }
                break;
            case CharType::USUAL_CHAR:
                dest.push_back(current);
                lastCharType = CharType::USUAL_CHAR;
                if (current == Backslash)
                    lastCharType = CharType::BACKSLASH;
                if (current == Asterisk)
                    lastCharType = CharType::ASTERISK;
                break;
            }
        }
        // process of single last backslash
        if (lastCharType == CharType::BACKSLASH)
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