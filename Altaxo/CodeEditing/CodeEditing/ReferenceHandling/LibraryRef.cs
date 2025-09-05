using System;

namespace Altaxo.CodeEditing.ReferenceHandling
{
  public record LibraryRef(LibraryRef.RefKind Kind, string Value, string Version) : IComparable<LibraryRef>
  {
    public static LibraryRef Reference(string path) => new(RefKind.Reference, path, string.Empty);
    public static LibraryRef FrameworkReference(string id) => new(RefKind.FrameworkReference, id.ToLowerInvariant(), string.Empty);
    public static LibraryRef PackageReference(string id, string versionRange) => new(RefKind.PackageReference, id.ToLowerInvariant(), versionRange);

    public int CompareTo(LibraryRef? other)
    {
      if (other is null)
        return 1;

      if (Kind.CompareTo(other.Kind) is var kindCompare and not 0)
      {
        return kindCompare;
      }

      if (StringComparer.Ordinal.Compare(Value, other.Value) is var valueCompare and not 0)
      {
        return valueCompare;
      }

      return StringComparer.Ordinal.Compare(Version, other.Version);
    }

    public enum RefKind
    {
      Reference,
      FrameworkReference,
      PackageReference
    }

    public override string ToString() => Kind switch
    {
      RefKind.Reference => Value,
      RefKind.FrameworkReference => $"framework:{Value}",
      RefKind.PackageReference => $"nuget:{Value},{Version}",
      _ => throw new NotSupportedException($"Unsupported kind: {Kind}")
    };
  }
}
