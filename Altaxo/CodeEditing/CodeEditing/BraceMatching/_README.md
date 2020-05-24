# Brace matching

The reason that some of the code from Roslyn must be reproduced here is because of the fact that
all of the code is located in EditorFeatures.dll, which contains references to VisualStudio.

The following files where extracted from EditorFeatures.dll without modification (except namespaces and accessibility):

- AbstractBraceMatcher.cs
- AbstractDirectiveTriviaBraceMatcher.cs
- BraceCharacterAndKind.cs
- BraceMatchingService.cs
- ExportBraceMatcherAttribute.cs
- IBraceMatcher.cs
- IBraceMatchingService.cs

The following files where extracted from CSharp.EditorFeatures.dll without modification (except namespaces and accessibility):

- AbstractCSharpBraceMatcher
- CSharpDirectiveTriviaBraceMatcher
- LessThanGreaterThanBraceMatcher
- OpenCloseBraceBraceMatcher
- OpenCloseBracketBraceMatcher
- OpenCloseParenBraceMatcher
- StringLiteralBraceMatcher
 
The following files have been added additionally:

- No files at all.