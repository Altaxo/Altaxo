using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Camera
{
  /// <summary>
  /// Event args that indicate that only camera settings have been changed.
  /// </summary>
  public class CameraChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    private static CameraChangedEventArgs _instance = new CameraChangedEventArgs();

    public static new CameraChangedEventArgs Empty { get { return _instance; } }

    public override void Add(SelfAccumulateableEventArgs e)
    {
    }
  }
}
