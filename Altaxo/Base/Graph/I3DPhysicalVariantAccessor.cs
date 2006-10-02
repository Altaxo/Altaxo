using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Graph
{
  public interface I3DPhysicalVariantAccessor
  {
    AltaxoVariant GetXPhysical(int originalRowIndex);
    AltaxoVariant GetYPhysical(int originalRowIndex);
    AltaxoVariant GetZPhysical(int originalRowIndex);
  }
}
