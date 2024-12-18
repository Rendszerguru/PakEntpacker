using PakReaderExe;
using System;
using System.IO;
using System.Reflection;

// Display the application's version
string version = System.Reflection.Assembly.GetExecutingAssembly()
            .GetName()
            .Version
            .ToString(3);

Console.WriteLine($"Application Version: {version}");

// Find all .pak files in the current directory
string[] pakFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pak");

// If no .pak files are found, display a message and exit
if (pakFiles.Length == 0)
{
    Console.WriteLine("No '.pak' files found in the directory.");
    return;
}

// Iterate over each .pak file
foreach (string pakPath in pakFiles)
{
    Console.WriteLine($"Unpacking: {pakPath}");

    // Create a new Pak object for the current file
    Pak pak = new(pakPath);

    // Extract the data block to a directory with the same name as the .pak file
    pak.ExtractDataBlock(Path.ChangeExtension(pakPath, null));
}

Console.WriteLine("Done!");
