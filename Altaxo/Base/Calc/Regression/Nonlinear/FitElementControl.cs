using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Summary description for FitElementControl.
	/// </summary>
	public class FitElementControl : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FitElementControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

    FitElement _fitElement;
    int _numberOfX;
    int _numberOfY;
    int _numberOfParameter;
    int _totalSlots;
    int _slotHeight;
    Pen _pen;

    public int SlotHeight { get { return _slotHeight; }}

    public void Initialize(FitElement fitElement)
    {
      _fitElement = fitElement;

      _numberOfX = _fitElement.NumberOfIndependentVariables;
      _numberOfY = _fitElement.NumberOfDependentVariables;
      _numberOfParameter = _fitElement.NumberOfParameters;

      _totalSlots = Math.Max(_numberOfParameter,_numberOfX + _numberOfY +1);
      _slotHeight = System.Windows.Forms.SystemInformation.MenuButtonSize.Height;
      _pen = System.Drawing.Pens.Blue;

      this.ClientSize = new Size(this.ClientSize.Width,_totalSlots*_slotHeight);
    }

    protected override void OnPaint(PaintEventArgs e)
    {

     
        for(int i=0;i<_numberOfX;i++)
        {
          e.Graphics.DrawLine(_pen,0,i*_slotHeight+(1*_slotHeight)/4,_slotHeight/2,(3*_slotHeight)/4);
          e.Graphics.DrawLine(_pen,0,i*_slotHeight+(3*_slotHeight)/4,_slotHeight/2,(3*_slotHeight)/4);
          if(_fitElement.FitFunction!=null)
            e.Graphics.DrawString(
              _fitElement.FitFunction.IndependentVariableName(i),
              System.Windows.Forms.SystemInformation.MenuFont,
              System.Drawing.Brushes.Black,
              new Point(_slotHeight/2+1,i*_slotHeight + _slotHeight/2)
              );

        }


      if(_fitElement.FitFunction==null)
      {
        e.Graphics.DrawString("?", 
          System.Windows.Forms.SystemInformation.MenuFont,
              System.Drawing.Brushes.Black,
          new Point(this.ClientSize.Width/2,this.ClientSize.Height/2));
      }

      base.OnPaint (e);
    }

	}
}
