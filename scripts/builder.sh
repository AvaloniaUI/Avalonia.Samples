#!/bin/bash

# Check if dotnet 9. is installed
if ! dotnet --list-sdks | grep -q "9."; then
    echo "Please install .NET SDK 9.0 or higher."
    exit 1
fi

# Set the solution file location
SOLUTION_FILE="${1:-../src/Avalonia.Samples/Avalonia.Samples.sln}"

# Check if the solution file exists
if [ ! -f "$SOLUTION_FILE" ]; then
    echo "Solution file not found: $SOLUTION_FILE"
    exit 1
fi

# Set the build configuration
CONFIGURATION="${2:-Release}"
dotnet build "$SOLUTION_FILE" -c "$CONFIGURATION"
if [ $? -ne 0 ]; then
    echo "Build failed."
    exit 1
fi
echo "Build succeeded."

