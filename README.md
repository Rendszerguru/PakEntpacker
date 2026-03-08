# PakEntpacker v1.0.6

A high-performance tool for extracting Enfusion engine `.pak` files, optimized for .NET 8.0.

## Key Features
* **Multi-threaded Unpacking**: Simultaneously processes multiple files using all CPU cores.
* **Memory-Efficient**: Streaming decompression ensures stable extraction of massive archives.
* **Intelligent Workflow**: 
    1. **Direct Drag & Drop**: Drop `.pak` files onto `PakEntpacker.exe` for instant extraction.
    2. **Auto-Scan**: Scans the local directory and processes all found `.pak` files.
    3. **Interactive Mode**: Opens a file picker if no files are provided or found.
* **File Association Support**: Can be set as the default Windows application for `.pak` files.

## Usage
* **Automatic**: Place `PakEntpacker.exe` in your game folder and run it to extract everything.
* **Drag & Drop**: Drag a `.pak` file and drop it directly onto the `PakEntpacker.exe` file.
* **File Association**: Right-click a `.pak` file -> *Open with...* -> *Choose another app* -> Select `PakEntpacker.exe` and check "Always use this app". Now you can extract any `.pak` just by double-clicking it.
* **Manual**: Run the EXE in an empty folder to manually browse for a file.

## Requirements
* Windows OS
* [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
