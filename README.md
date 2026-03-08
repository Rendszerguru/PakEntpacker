## **Version 1.0.5**
* **Added interactive File Dialog**: If no `.pak` files are found in the directory or passed as arguments, the program now opens a Windows file picker instead of closing immediately.
* **Improved Threading for UI**: Implemented STA (Single-Threaded Apartment) mode to ensure the file dialog opens reliably on all Windows systems.
* [cite_start]**Upgraded to .NET 8.0**: Migrated the project from .NET 6.0 to .NET 8.0 (LTS) for better performance, long-term support, and improved security[cite: 1].
* **Refactored Logic Priority**: Established a clear processing order: Command-line arguments first (Drag & Drop/Association), then local directory files, and finally the manual File Dialog.

## **Version 1.0.2**

The following features would yield better results when extracting .pak files containing errors:

- **Checks for safe reading conditions.**
- **Reads a UTF-8 string with error handling.**
- **Handles reading errors and invalid characters.**

## **Version 1.0.0**

Modified version of the Enfusion pak unpacker created by FlipperPlz.

Drag a .pak file onto the executable or assign it to the tool; pressing Enter or double-clicking will unpack the selected .pak file along with all other .pak files in the same directory.

### **Fixed:**
The original Enfusion pak unpacker was truncating the end of files when decompressing `localization.conf` files.
