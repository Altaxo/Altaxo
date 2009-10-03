using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
  public partial class PropertyControl : UserControl, IPropertyView
  {
    public PropertyControl()
    {
      InitializeComponent();
    }

    #region IPropertyView Members

    public object[] SelectedObjectsToView
    {
      get
      {
        return _propertyGrid.SelectedObjects;
      }
      set
      {
        _propertyGrid.SelectedObjects = value;
      }
    }

    #endregion
  }
}
