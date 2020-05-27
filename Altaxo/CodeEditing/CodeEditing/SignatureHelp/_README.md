# Signature help

Signature help is distributes over many ISignatureHelpProvider

The code for aggregation of these providers is located in the EditorFeature Dll,
 and thus not usable by AltaxoCodeEditing.

That's why AggregateSignatureHelpProvider does the aggregation of the ISignatureHelpProviders.

There is a new service IAxoSignatureHelpProvider that can be used as a service. 
AggregateAggregateSignatureHelpProvider is implementing this interface.

RoslynOverloadProvider is the adaptation of signature help to the needs of AvalonEdit.