using PakReaderExe;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

// Display the application's version
string version = Assembly.GetExecutingAssembly()
    .GetName()
    .Version?
    .ToString(3) ?? "1.0.5";

Console.WriteLine($"Application Version: {version}");

// 1. PRIORITY: Command line argument (e.g. file drag & drop onto the EXE)
if (args.Length > 0 && File.Exists(args[0]))
{
    ProcessFile(args[0]);
}
else
{
    // 2. PRIORITY: Find all .pak files in the current directory
    string[] pakFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pak");

    if (pakFiles.Length > 0)
    {
        // Iterate over each .pak file found locally
        foreach (string pakPath in pakFiles)
        {
            ProcessFile(pakPath);
        }
    }
    else
    {
        // 3. PRIORITY: Manual selection if no files found (Dialog)
        Console.WriteLine("No '.pak' files found in the directory. Please select one manually!");

        string? selectedFile = ShowDialog();

        if (!string.IsNullOrEmpty(selectedFile))
        {
            ProcessFile(selectedFile);
        }
        else
        {
            Console.WriteLine("Operation cancelled. No file selected.");
        }
    }
}

Console.WriteLine("\nDone! Press any key to exit...");
Console.ReadKey();

// --- Internal Functions ---

static void ProcessFile(string path)
{
    try
    {
        // Check if the file extension is .pak
        if (Path.GetExtension(path).ToLower() != ".pak")
        {
            Console.WriteLine($"Skipping: {Path.GetFileName(path)} (not a .pak file)");
            return;
        }

        Console.WriteLine($"Unpacking: {path}...");

        // Create a new Pak object for the current file
        Pak pak = new Pak(path);

        // Extract the data block to a directory with the same name as the .pak file
        string outputDir = Path.ChangeExtension(path, null);
        pak.ExtractDataBlock(outputDir);

        Console.WriteLine($"Successfully extracted to: {outputDir}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR while processing {Path.GetFileName(path)}: {ex.Message}");
    }
}

static string? ShowDialog()
{
    string? selectedPath = null;

    // We use a separate Thread with STA state to ensure the dialog opens correctly
    Thread thread = new Thread(() =>
    {
        using OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Pak files (*.pak)|*.pak";
        openFileDialog.Title = "Select a .pak file to unpack";
        openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            selectedPath = openFileDialog.FileName;
        }
    });

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();

    return selectedPath;
}
