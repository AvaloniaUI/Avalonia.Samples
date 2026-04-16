#!/bin/bash

# This script changes all .NET projects in the specified directory to target .NET 9.0.

# Set the search directory to the first argument, or default to '../src' if not provided
SEARCH_DIR="${1:-../src}"

# Find all .csproj files in the search directory
find "$SEARCH_DIR" -name "*.csproj" | while read -r csproj; do
    # Update the TargetFramework element to net9.0
    sed -i 's|<TargetFramework>net[0-9]\.[0-9]\(.*\)</TargetFramework>|<TargetFramework>net9.0\1</TargetFramework>|' "$csproj"
done
echo "All .NET projects in $SEARCH_DIR have been updated to target .NET 9.0."