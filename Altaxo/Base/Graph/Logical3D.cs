using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Holds a triple of logical values to designate a location into a 3D coordinate system. Can
  /// also be used for 2D (with RZ=0).
  /// </summary>
  public struct Logical3D
  {
    public double RX;
    public double RY;
    public double RZ;

    public Logical3D(double rx, double ry, double rz)
    {
      RX = rx;
      RY = ry;
      RZ = rz;
    }
    public Logical3D(double rx, double ry)
    {
      RX = rx;
      RY = ry;
      RZ = 0;
    }
  }
}
