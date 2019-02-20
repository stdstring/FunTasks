cmake_minimum_required(VERSION 3.7 FATAL_ERROR)

project(T9SpellingTests CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories("../external/googletest/include")
include_directories("../T9SpellingCommon")
link_directories(../external/googletest/bin)

set(SOURCES T9ProcessorTests.cpp T9SpellingIntegrationTests.cpp)

set(HEADERS)

add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})

target_link_libraries(${PROJECT_NAME} PRIVATE gtest gtest_main)

add_test(NAME T9SpellingTests COMMAND ${PROJECT_NAME})