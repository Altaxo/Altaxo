using System.Collections.Generic;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy.Raman;

namespace Altaxo.Gui.Analysis.Spectroscopy.Raman
{

  public interface INeonCalibrationOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(INeonCalibrationOptionsView))]
  [UserControllerForObject(typeof(NeonCalibrationOptions))]
  public class NeonCalibrationOptionsController : MVCANControllerEditImmutableDocBase<NeonCalibrationOptions, INeonCalibrationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<XAxisUnit> _XAxisUnit;

    public ItemsController<XAxisUnit> XAxisUnit
    {
      get => _XAxisUnit;
      set
      {
        if (!(_XAxisUnit == value))
        {
          _XAxisUnit = value;
          OnPropertyChanged(nameof(XAxisUnit));
        }
      }
    }

    private double _laserWavelength_Nanometer;

    public double LaserWavelength_Nanometer
    {
      get => _laserWavelength_Nanometer;
      set
      {
        if (!(_laserWavelength_Nanometer == value))
        {
          _laserWavelength_Nanometer = value;
          OnPropertyChanged(nameof(LaserWavelength_Nanometer));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        XAxisUnit = new ItemsController<XAxisUnit>(new Collections.SelectableListNodeList(_doc.XAxisUnit));

        if (_doc.LaserWavelength_Nanometer == 0)
          LaserWavelength_Nanometer = 532;
        else
          LaserWavelength_Nanometer = _doc.LaserWavelength_Nanometer;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        LaserWavelength_Nanometer = LaserWavelength_Nanometer,
        XAxisUnit = XAxisUnit.SelectedValue,
      };

      return ApplyEnd(true, disposeController);
    }

    
  }
}
