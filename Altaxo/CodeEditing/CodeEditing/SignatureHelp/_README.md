# Signature help

Signature help is distributed over many ISignatureHelpProvider

The code for aggregation of these providers is located in the EditorFeature Dll,
 and thus not usable by AltaxoCodeEditing.

That's why AggregateSignatureHelpProvider does the aggregation of the ISignatureHelpProviders.

There is a new service IAxoSignatureHelpProvider that can be used as a service. 
AggregateAggregateSignatureHelpProvider is implementing this interface.

RoslynOverloadProvider is the adaptation of signature help to the needs of AvalonEdit.

## Roslyn 3.6.0:

There is a class `Microsoft.CodeAnalysis.ExternalAccess.Pythia.PythiaSignatureHelpProvider`
which exports the signature help provider interface and thus would be aggregated by
`AggregateSignatureHelpProvider'. The problem with this class is that it has an import constructor
which needs a  Lazy<IPythiaSignatureHelpProviderImplementation> type. But this type is 
not exported anywhere in Roslyn 3.6.0, and thus the construction of `AggregateSignatureHelpProvider'
failed. The solution was to implement a dummy class `LazyPhythiaSignatureHelpProviderImplementationDummy` which exports the required interface.
