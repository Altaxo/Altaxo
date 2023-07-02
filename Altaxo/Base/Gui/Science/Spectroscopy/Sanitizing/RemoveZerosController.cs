using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.Sanitizing;

namespace Altaxo.Gui.Science.Spectroscopy.Sanitizing
{
  public interface IRemoveZerosView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IRemoveZerosView))]
  [UserControllerForObject(typeof(RemoveZeros))]
  public class RemoveZerosController : MVCANControllerEditImmutableDocBase<RemoveZeros, IRemoveZerosView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _thresholdValue;

    public double ThresholdValue
    {
      get => _thresholdValue;
      set
      {
        if (!(_thresholdValue == value))
        {
          _thresholdValue = value;
          OnPropertyChanged(nameof(ThresholdValue));
        }
      }
    }

    private bool _removeZerosAtStartOfSpectrum;

    public bool RemoveZerosAtStartOfSpectrum
    {
      get => _removeZerosAtStartOfSpectrum;
      set
      {
        if (!(_removeZerosAtStartOfSpectrum == value))
        {
          _removeZerosAtStartOfSpectrum = value;
          OnPropertyChanged(nameof(RemoveZerosAtStartOfSpectrum));
        }
      }
    }

    private bool _removeZerosAtEndOfSpectrum;

    public bool RemoveZerosAtEndOfSpectrum
    {
      get => _removeZerosAtEndOfSpectrum;
      set
      {
        if (!(_removeZerosAtEndOfSpectrum == value))
        {
          _removeZerosAtEndOfSpectrum = value;
          OnPropertyChanged(nameof(RemoveZerosAtEndOfSpectrum));
        }
      }
    }

    private bool _removeZerosInMiddleOfSpectrum;

    public bool RemoveZerosInMiddleOfSpectrum
    {
      get => _removeZerosInMiddleOfSpectrum;
      set
      {
        if (!(_removeZerosInMiddleOfSpectrum == value))
        {
          _removeZerosInMiddleOfSpectrum = value;
          OnPropertyChanged(nameof(RemoveZerosInMiddleOfSpectrum));
        }
      }
    }

    private bool _splitIntoSeparateRegions;

    public bool SplitIntoSeparateRegions
    {
      get => _splitIntoSeparateRegions;
      set
      {
        if (!(_splitIntoSeparateRegions == value))
        {
          _splitIntoSeparateRegions = value;
          OnPropertyChanged(nameof(SplitIntoSeparateRegions));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ThresholdValue = _doc.ThresholdValue;
        RemoveZerosAtStartOfSpectrum = _doc.RemoveZerosAtStartOfSpectrum;
        RemoveZerosAtEndOfSpectrum = _doc.RemoveZerosAtEndOfSpectrum;
        RemoveZerosInMiddleOfSpectrum = _doc.RemoveZerosInMiddleOfSpectrum;
        SplitIntoSeparateRegions = _doc.SplitIntoSeparateRegions;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        ThresholdValue = ThresholdValue,
        RemoveZerosAtStartOfSpectrum = RemoveZerosAtStartOfSpectrum,
        RemoveZerosAtEndOfSpectrum = RemoveZerosAtEndOfSpectrum,
        RemoveZerosInMiddleOfSpectrum = RemoveZerosInMiddleOfSpectrum,
        SplitIntoSeparateRegions = SplitIntoSeparateRegions,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}
