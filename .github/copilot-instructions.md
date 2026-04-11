# Copilot Instructions

## Project Guidelines
- This workspace uses `.slnx` solution files rather than a `.sln` file.
- Prefer real XML documentation comments over using `#pragma warning disable CS1591` or script-generated comments to handle CS1591 warnings. When fixing CS1591 issues in this repository, add or fix XML comments for missing or incomplete public and protected members, use `<inheritdoc/>` for interface/base implementations, keep detailed remarks while correcting spelling/grammar, do not change any code.
- Please note that XML documentation must be placed above attributes