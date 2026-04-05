using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.Sanitizing;

namespace Altaxo.Gui.Science.Spectroscopy.Sanitizing
{
  /// <summary>
  /// View interface for editing <see cref="RemoveZeros"/> options.
  /// </summary>
  public interface IRemoveZerosView : IDataContextAwareView { }

  /// <summary>
  /// Controller for editing a <see cref="RemoveZeros"/> sanitizer.
  /// </summary>
  [ExpectedTypeOfView(typeof(IRemoveZerosView))]
  [UserControllerForObject(typeof(RemoveZeros))]
  public class RemoveZerosController : MVCANControllerEditImmutableDocBase<RemoveZeros, IRemoveZerosView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _thresholdValue;

    /// <summary>
    /// Gets or sets the threshold value used to detect zeros.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether zeros at the start of the spectrum should be removed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether zeros at the end of the spectrum should be removed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether zeros in the middle of the spectrum should be removed.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the spectrum should be split into separate regions.
    /// </summary>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
