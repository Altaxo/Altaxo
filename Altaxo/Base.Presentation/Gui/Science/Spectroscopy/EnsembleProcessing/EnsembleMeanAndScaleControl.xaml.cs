using System.Windows.Controls;

namespace Altaxo.Gui.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Interaction logic for EnsembleMeanAndScaleControl.xaml
  /// </summary>
  public partial class EnsembleMeanAndScaleControl : UserControl, IEnsembleMeanAndScaleView, IMultiplicativeScatterCorrectionView
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EnsembleMeanAndScaleControl"/> class.
    /// </summary>
    public EnsembleMeanAndScaleControl()
    {
      InitializeComponent();
    }
  }
}
