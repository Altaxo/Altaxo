using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Main.GUI;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Summary description for FitEnsembleControl.
	/// </summary>
	[UserControlForController(typeof(IFitEnsembleViewEventSink))]
	public class FitEnsembleControl : System.Windows.Forms.UserControl, IFitEnsembleView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FitEnsembleControl()
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

    #region IFitEnsembleView Members

    IFitEnsembleViewEventSink _controller;
    public IFitEnsembleViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller=value;
      }
    }

    public void Initialize(FitEnsemble ensemble)
    {
      // remove all child controls first.
      this.Controls.Clear();

      // foreach element in the ensemble, create a new control, and 
      // position the elements

      int currentYPosition = 0;
      for(int i=0;i<ensemble.Count;i++)
      {
        FitElementControl fectrl = new FitElementControl();
        fectrl.Initialize(ensemble[i]);
        fectrl.Location = new Point(0,currentYPosition);
        this.Controls.Add(fectrl);

        currentYPosition += fectrl.Size.Height;
        currentYPosition += fectrl.SlotHeight;
      }
    }


    #endregion
  }
}
