using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{

  [UserControlForController(typeof(IBackgroundStyleViewEventSink))]
  public partial class BackgroundStyleControl : UserControl, IBackgroundStyleView
  {
    public BackgroundStyleControl()
    {
      InitializeComponent();
    }

    IBackgroundStyleViewEventSink _controller;
    public IBackgroundStyleViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    
    private void EhBackgroundColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if (null != Controller)
      {
          Controller.EhView_BackgroundColorChanged(_cbColors.Color);
      }
    }

    private void _cbBackgroundStyle_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if (null != Controller)
        Controller.EhView_BackgroundStyleChanged(this._cbStyles.SelectedIndex);
    }



    public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
      box.Text = name;
    }


  

    public void BackgroundColor_Initialize(System.Drawing.Color color)
    {
      this._cbColors.Color = color;
    }

    /// <summary>
    /// Initializes the enable state of the background color combo box.
    /// </summary>
    public void BackgroundColorEnable_Initialize(bool enable)
    {
      this._cbColors.Enabled = enable;
    }

    /// <summary>
    /// Initializes the background styles.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="selection"></param>
    public void BackgroundStyle_Initialize(string[] names, int selection)
    {
      this._cbStyles.Items.AddRange(names);
      this._cbStyles.SelectedIndex = selection;
    }

  }
}
