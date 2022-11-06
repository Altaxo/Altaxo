# Workspace diagnostics

Consists of two files:

- IDiagnosticsEventSink is an interface that must be implemented by all classes that want to receive
diagnostic messages.
- AnalyzerAssemblyLoader is a minimal stub implementation of IAnalyzerAssemblyLoader


Additional actions neccessary:

- When creating the workspace, it is neccessary to add analyzer references to the solution. This can be done using the following code fragment:

```` 
      // next two lines are neccessary to have diagnostics in the solution
      var solution = CurrentSolution.AddAnalyzerReferences(roslynHost.GetSolutionAnalyzerReferences());
      SetCurrentSolution(solution);
```