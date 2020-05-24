# Workspace diagnostics

Consists of three files:

- IDiagnosticsEventSink is an interface that must be implemented by all classes that want to receive
diagnostic messages.
- AnalyzerAssemblyLoader is a minimal stub implementation of IAnalyzerAssemblyLoader
- WorkspaceDiagnostiAnalyzerProviderServer.cs provide the minimum number of DLLs neccessary to analyze CSharp source code.