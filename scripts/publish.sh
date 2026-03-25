#!/bin/bash

set -e

# Check if the .NET SDK is installed
if ! command -v dotnet &> /dev/null
then
    echo "The .NET SDK is not installed. Please install it and try again."
    exit 1
fi

# Navigate to the project directory
pushd "$(dirname "$0")/../openapi-ts-client-generator"

# Build the project and publish a self-contained executable for all platforms
dotnet publish -c Release -r win-x64 --property:PublishDir=../publish/win-x64
dotnet publish -c Release -r win-arm64 --property:PublishDir=../publish/win-arm64
dotnet publish -c Release -r linux-x64 --property:PublishDir=../publish/linux-x64
dotnet publish -c Release -r linux-arm64 --property:PublishDir=../publish/linux-arm64
dotnet publish -c Release -r osx-x64 --property:PublishDir=../publish/osx-x64
dotnet publish -c Release -r osx-arm64 --property:PublishDir=../publish/osx-arm64

echo "Publish completed successfully."
popd
