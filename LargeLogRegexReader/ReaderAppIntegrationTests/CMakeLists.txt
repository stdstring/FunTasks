cmake_minimum_required(VERSION 3.13 FATAL_ERROR)

project(ReaderAppIntegrationTests CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories("../external/googletest/include")
link_directories(../external/googletest/bin)

set(SOURCES source/ReaderAppIntegrationTests.cpp)

set(HEADERS)

add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})

target_link_libraries(${PROJECT_NAME} PRIVATE gtest gtest_main)

add_test(NAME ${PROJECT_NAME} COMMAND ${PROJECT_NAME})