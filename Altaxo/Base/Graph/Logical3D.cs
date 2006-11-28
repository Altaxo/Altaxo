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


    public Logical3D InterpolateTo(Logical3D to, double t)
    {
      return new Logical3D
        (
        this.RX+t*(to.RX-this.RX),
        this.RY+t*(to.RY-this.RY),
        this.RZ+t*(to.RZ-this.RZ)
        );
    }

    public static Logical3D Interpolate(Logical3D from, Logical3D to, double t)
    {
      return new Logical3D
        (
        from.RX + t * (to.RX - from.RX),
        from.RY + t * (to.RY - from.RY),
        from.RZ + t * (to.RZ - from.RZ)
        );
    }

    public static Logical3D operator +(Logical3D r, Logical3D s)
    {
      return new Logical3D(r.RX + s.RX, r.RY + s.RY, r.RZ + s.RZ);
    }

  }
}
