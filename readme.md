# Description

This is a test bench to play with cross framework serializations.

the core logic allows passing some switches to control the application:

## Usage
    -m mode [ser|des] - required  
    -f file  
    -t [all|auto|none] - required  
    -a [simple|full] - required  
    -x [netcore|default]

## Quick start

Firstly you have to build both projects `net4serde` and `net4serde`.

then running each to write a json and read it.

The purpose is to ensure we can safely ser/des jsons between multiple frameworks, namely net472 and net50