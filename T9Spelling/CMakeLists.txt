cmake_minimum_required(VERSION 3.7 FATAL_ERROR)

project(T9Spelling CXX)

add_subdirectory(T9SpellingApp bin/T9SpellingApp)

enable_testing()
add_subdirectory(T9SpellingTests bin/T9SpellingTests)

file(COPY input DESTINATION .)
file(COPY output DESTINATION .)