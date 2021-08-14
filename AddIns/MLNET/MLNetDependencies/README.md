# Purpose of this project

The purpuse of this project 
is to get the dependencies
of the MLNet nuget packages,
and, especially, the **native** libraries.

For this, it is neccessary (for targeting .NET framework) to fix to one CPU architecture.

Here, I fixed to x64, because is is more probable to use ML.Net with 64 bit.

For .Net core projects, this problem does not arise because of separate runtime
folders for x86 and x64.