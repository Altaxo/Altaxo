using Altaxo.Calc.Providers.LinearAlgebra;
using Altaxo.Main.Properties;

namespace Altaxo.Calc.LinearAlgebra
{
  public enum LinearAlgebraProvider
  {
    Managed = 0,
    MathClassLibrary = 1,
  }

  public class LinearAlgebraProviderSettings
  {
    public static PropertyKey<LinearAlgebraProvider> PropertyKeyLinearAlgebraProvider { get; } = new PropertyKey<LinearAlgebraProvider>(
        "1F697914-8701-40B6-B1F9-81661A498BE0",
        "Math\\LinearAlgebraProvider",
        PropertyLevel.Application,
        null, () => LinearAlgebraProvider.Managed)
    {
      ApplicationAction = ApplyLinearAlgebraProvider
    };

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

    public static void ApplyAlgebraProviderSetting()
    {
      LinearAlgebraProvider provider = Current.PropertyService.GetValue<LinearAlgebraProvider>(PropertyKeyLinearAlgebraProvider, Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);
      ApplyLinearAlgebraProvider(provider);
    }
  }
}

