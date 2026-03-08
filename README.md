# PakEntpacker v1.0.6

A high-performance tool for extracting Enfusion engine `.pak` files, optimized for .NET 8.0.

## Key Features
* **Multi-threaded Unpacking**: Simultaneously processes multiple files using all CPU cores.
* **Memory-Efficient**: Streaming decompression ensures stable extraction of massive archives.
* **Intelligent Workflow**: 
    1. Supports **Drag & Drop** directly onto the EXE.
    2. Auto-scans the local folder for `.pak` files.
    3. Opens an **Interactive File Dialog** if no files are found.
* **Robust Error Handling**: Automatically saves corrupted data blocks for debugging instead of crashing.

## Usage
* **Automatic**: Place `PakEntpacker.exe` in your game folder and run it.
* **Manual**: Run the EXE in an empty folder to open the file picker.
* **Quick**: Drag any `.pak` file and drop it onto the icon.

## Requirements
* Windows OS
* [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
