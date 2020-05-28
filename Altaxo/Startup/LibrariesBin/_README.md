# LibrariesBin

This folder is for placing some dependency DLLs that should override
the natural build order.

## Example:

At the time of writing, AltaxoCodeEditing needed 

System.Runtime.CompilerServices.Unsafe (Nuget-Version 4.7.1, DLL-Version 4.0.6.0)

All other components that referenced Microsoft.CSharp included DLL-Version 4.0.1.0

In order to fix that, we have to copy the newer DLLs in the very last build step, which should
be AltaxoStartup.