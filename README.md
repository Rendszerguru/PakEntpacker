## **Version 1.0.2**

The following features would yield better results when extracting .pak files containing errors:

- **Checks for safe reading conditions.**
- **Reads a UTF-8 string with error handling.**
- **Handles reading errors and invalid characters.**

## **Version 1.0.0**

Modified version of the Enfusion pak unpacker created by FlipperPlz.

When assigned to a pak file, pressing Enter or double-clicking will unpack both the specified pak file and all other pak files within the same directory.

### **Fixed:**
The original Enfusion pak unpacker was truncating the end of files when decompressing `localization.conf` files.
