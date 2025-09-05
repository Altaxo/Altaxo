# Reference handling

Reference handling is the ability to process #r directives, which is usually not allowed in regular DLL compiling.

The steps are the following:

- when the code has changed, and CodeEditorViewAdapterCSharp.OnSyntaxTreeChanged is called,
  the syntax tree is given to Workspace.UpdateLibrariesAsync (i.e. can be cancelled the next time the syntax tree is changing)

- Workspace.UpdateLibrariesAsync is called with the syntax tree. 
  The tree is parsed and references to libraries are put in LibRef instances.
  For each document in the workspace, a set of LibRef instances is kept in a dictionary.

- If any change to the set of LibRef instances is noticed, further processing is required:
  OnLibrariesUpdatedAsync is called. The assemblies referenced by the LibRef instances are loaded or downloaded (in case of NuGet).
  Then the difference of the assemblies with that that are already in the project is computed, 
  and the new set of assemblies is passed to the project options. This is only for diagnostics, the compilation itself uses its own calls.


 
