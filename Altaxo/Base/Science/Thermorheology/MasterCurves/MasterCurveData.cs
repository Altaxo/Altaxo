using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Science.Thermorheology.MasterCurves
{
  public class MasterCurveData
  {

    /// <summary>
    /// The data of the curves to master. The data belonging to the first master curve are _curves[0][0], _curves[1][0], _curves[2][0], .. _curves[n-1][0].
    /// The data belonging to a second master curve with the same shift factors (if there is any, e.g. the imaginary part), are _curves[0][1], _curves[1][1], .. _curves[n-1][1].
    /// </summary>
    public List<XAndYColumn?[]> CurveData { get; } = new List<XAndYColumn?[]>();
  }
}
