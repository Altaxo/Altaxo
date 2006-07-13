using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class PositionSizeRotationScaleControl : UserControl
  {
    public PositionSizeRotationScaleControl()
    {
      InitializeComponent();
    }

    public ObjectPositionAndSizeGlue PositionSizeGlue
    {
      get
      {
        return _positionSizeGlue;
      }
    }
  }
}
