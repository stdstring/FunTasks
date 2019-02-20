cmake_minimum_required(VERSION 3.7 FATAL_ERROR)

project(T9SpellingApp CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories("../T9SpellingCommon")

set(SOURCES T9SpellingApp.cpp)

set(HEADERS)

add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})

set_directory_properties(PROPERTIES VS_STARTUP_PROJECT T9SpellingApp)
