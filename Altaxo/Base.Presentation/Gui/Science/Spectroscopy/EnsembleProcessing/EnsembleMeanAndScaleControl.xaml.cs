using System.Windows.Controls;

namespace Altaxo.Gui.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Interaction logic for EnsembleMeanAndScaleControl.xaml
  /// </summary>
  public partial class EnsembleMeanAndScaleControl : UserControl, IEnsembleMeanAndScaleView, IMultiplicativeScatterCorrectionView
  {
    public EnsembleMeanAndScaleControl()
    {
      InitializeComponent();
    }
  }
}
