#!/bin/bash

# Set the search directory to the first argument, or default to '../src' if not provided
SEARCH_DIR="${1:-../src}"

# Find and remove all 'bin' and 'obj' directories under the search directory
find "$SEARCH_DIR" -type d \( -name bin -o -name obj \) -exec rm -rf {} +