using Altaxo.Calc.Providers.LinearAlgebra;
using Altaxo.Main.Properties;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// Identifies the linear algebra provider to use.
  /// </summary>
  public enum LinearAlgebraProvider
  {
    /// <summary>
    /// Uses the managed implementation.
    /// </summary>
    Managed = 0,

    /// <summary>
    /// Uses the Math Kernel Library based implementation when available.
    /// </summary>
    MathClassLibrary = 1,
  }

  /// <summary>
  /// Provides access to linear algebra provider settings.
  /// </summary>
  public class LinearAlgebraProviderSettings
  {
    /// <summary>
    /// Gets the property key for the configured linear algebra provider.
    /// </summary>
    public static PropertyKey<LinearAlgebraProvider> PropertyKeyLinearAlgebraProvider { get; } = new PropertyKey<LinearAlgebraProvider>(
        "1F697914-8701-40B6-B1F9-81661A498BE0",
        "Math\\LinearAlgebraProvider",
        PropertyLevel.Application,
        null, () => LinearAlgebraProvider.Managed)
    {
      ApplicationAction = ApplyLinearAlgebraProvider
    };

    /// <summary>
    /// Applies the specified linear algebra provider.
    /// </summary>
    /// <param name="provider">The provider to apply.</param>
    private static void ApplyLinearAlgebraProvider(LinearAlgebraProvider provider)
    {
      switch (provider)
      {
        case LinearAlgebraProvider.Managed:
          LinearAlgebraControl.UseManaged();
          break;
        case LinearAlgebraProvider.MathClassLibrary:
          if (!LinearAlgebraControl.TryUseNativeMKL())
          {
            LinearAlgebraControl.UseManaged();
          }
          break;
        default:
          LinearAlgebraControl.UseManaged();
          break;
      }
    }

    /// <summary>
    /// Applies the currently configured linear algebra provider setting.
    /// </summary>
    public static void ApplyAlgebraProviderSetting()
    {
      LinearAlgebraProvider provider = Current.PropertyService.GetValue<LinearAlgebraProvider>(PropertyKeyLinearAlgebraProvider, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);
      ApplyLinearAlgebraProvider(provider);
    }
  }
}

