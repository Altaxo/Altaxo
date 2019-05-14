using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public DirectoryName ConfigDirectory => new DirectoryName("C:\\");

    public DirectoryName DataDirectory => new DirectoryName("C:\\");

    public IPropertyBag UserSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    public IPropertyBag ApplicationSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    public IPropertyBag BuiltinSettings { get; } = new PropertyBag() { ParentObject = SuspendableDocumentNode.StaticInstance };

    public event PropertyChangedEventHandler PropertyChanged;

    public PropertyServiceFallbackImplementation()
    {

    }

    public T GetValue<T>(string property, T defaultValue)
    {
      throw new NotImplementedException();
    }

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

    public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind, Func<T> ValueCreationIfNotFound)
    {
      if (kind == RuntimePropertyKind.UserAndApplicationAndBuiltin && UserSettings.TryGetValue<T>(p, out var result))
        return result;
      else if (kind == RuntimePropertyKind.ApplicationAndBuiltin && ApplicationSettings.TryGetValue<T>(p, out result))
        return result;
      else if (BuiltinSettings.TryGetValue<T>(p, out result))
        return result;
      else if (null != ValueCreationIfNotFound)
        return ValueCreationIfNotFound();
      else
        throw new ArgumentOutOfRangeException(nameof(p), string.Format("No entry found for property key {0}", p));

    }

    public void Save()
    {
      // Do nothing
    }

    public void SetValue<T>(string p, T value)
    {
      UserSettings.SetValue<T>(p, value);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }

    public void SetValue<T>(PropertyKey<T> p, T value)
    {
      UserSettings.SetValue(p, value);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p.GuidString));
    }
  }
}
