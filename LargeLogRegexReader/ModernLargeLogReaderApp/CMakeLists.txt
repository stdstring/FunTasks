cmake_minimum_required(VERSION 3.13 FATAL_ERROR)

project(ModernLargeLogReaderApp CXX)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

include_directories(include)

set(SOURCES source/CLogReader.cpp source/Handlers.cpp source/main.cpp source/Match.cpp source/Source.cpp source/Utils.cpp)

set(HEADERS include/CLogReader.h include/CommonDefs.h include/Handlers.h include/Match.h include/Result.h include/Source.h include/State.h include/Utils.h)

add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})