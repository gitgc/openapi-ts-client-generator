openapi-ts-client-generator
===========================

A .NET CLI tool that generates TypeScript code from OpenAPI schemas. It supports both local file inputs and remote URLs, making it versatile for various development workflows.

## Features
- **File and URL Support**: Process OpenAPI schemas from local files or remote URLs.
- **TypeScript Generation**: Converts OpenAPI schemas into TypeScript code for easy integration into frontend projects.
- **Command-Line Interface**: Simple and intuitive CLI for seamless integration into build processes.

## Requirements
- .NET 10.0 SDK or later
- `dotnet` CLI tool installed and available in the system PATH.
- Docker (for running tests that require a containerized environment)

## Usage
1. **Build the Project**: Navigate to the project directory and run:
```bash
    dotnet build
```
## Publishing
To publish the tool as a self-contained executable, use the included build script:
```bash
    ./scripts/publish.sh
```
This will generate executables for Windows, Linux, and macOS in the top level `publish` directory.

## Building
To build the project, navigate to the project directory and run:
```bash
    dotnet build
```
To lint and autofix code style issues, run:
```bash
    dotnet format
```
