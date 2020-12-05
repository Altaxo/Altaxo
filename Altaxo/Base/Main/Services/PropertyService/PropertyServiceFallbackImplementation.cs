#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Main.Properties;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Fall back implementation of <see cref="IPropertyService"/> for use during debugging.
  /// </summary>
  /// <seealso cref="Altaxo.Main.Services.IPropertyService" />
  public class PropertyServiceFallbackImplementation : IPropertyService
  {
    /// <inheritdoc/>
    public DirectoryName ConfigDirectory => new DirectoryName("C:\\");

    /// <inheritdoc/>
    public DirectoryName DataDirectory => new DirectoryName("C:\\");

    /// <inheritdoc/>
    public IPropertyBag UserSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    /// <inheritdoc/>
    public IPropertyBag ApplicationSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    /// <inheritdoc/>
    public IPropertyBag BuiltinSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyServiceFallbackImplementation"/> class.
    /// </summary>
    public PropertyServiceFallbackImplementation()
    {

    }

    /// <inheritdoc/>
    public T GetValue<T>(string property, T defaultValue)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind)
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else
        throw new ArgumentOutOfRangeException(nameof(p), string.Format("No entry found for property key {0}", p));

    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public T GetValueOrNull<T>(PropertyKey<T> p, RuntimePropertyKind kind)
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else
        return default;

    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull("ValueCreationIfNotFound")]
    [return: MaybeNull]
    public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind, Func<T>? ValueCreationIfNotFound) where T: notnull
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else if (ValueCreationIfNotFound is not null)
        return ValueCreationIfNotFound();
      else
        return default;

    }

    /// <summary>
    /// Since this is the fallback implementation, this call has no effect.
    /// </summary>
    public void Save()
    {
      // Do nothing
    }

    /// <inheritdoc/>
    public void SetValue<T>(string p, T value)
    {
      UserSettings.SetValue<T>(p, value);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }

    /// <inheritdoc/>
    public void SetValue<T>(PropertyKey<T> p, T value)
    {
      UserSettings.SetValue(p, value);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p.GuidString));
    }
  }
}
