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
	/// Summary description for FitFunctionSelectionControl.
	/// </summary>
	[UserControlForController(typeof(IFitFunctionSelectionViewEventSink))] 
	public class FitFunctionSelectionControl : System.Windows.Forms.UserControl, IFitFunctionSelectionView
	{
    IFitFunctionSelectionViewEventSink _controller;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.TreeView _twFitFunctions;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FitFunctionSelectionControl()
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
      this.splitter1 = new System.Windows.Forms.Splitter();
      this._twFitFunctions = new System.Windows.Forms.TreeView();
      this.SuspendLayout();
      // 
      // splitter1
      // 
      this.splitter1.Location = new System.Drawing.Point(0, 0);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(152, 344);
      this.splitter1.TabIndex = 0;
      this.splitter1.TabStop = false;
      // 
      // _twFitFunctions
      // 
      this._twFitFunctions.ImageIndex = -1;
      this._twFitFunctions.Location = new System.Drawing.Point(8, 16);
      this._twFitFunctions.Name = "_twFitFunctions";
      this._twFitFunctions.SelectedImageIndex = -1;
      this._twFitFunctions.Size = new System.Drawing.Size(136, 272);
      this._twFitFunctions.TabIndex = 1;
      this._twFitFunctions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._twFitFunctions_AfterSelect);
      // 
      // FitFunctionSelectionControl
      // 
      this.Controls.Add(this._twFitFunctions);
      this.Controls.Add(this.splitter1);
      this.Name = "FitFunctionSelectionControl";
      this.Size = new System.Drawing.Size(336, 344);
      this.ResumeLayout(false);

    }
		#endregion

    #region IFitFunctionSelectionView Members

    public IFitFunctionSelectionViewEventSink Controller 
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

    TreeNode GetPathNode(TreeNodeCollection coll, string path)
    {
      foreach(TreeNode node in coll)
      {
        if(node.Text==path && node.Tag==null)
          return node;
      }
      return null;
    }

    public void InitializeFitFunctionList(DictionaryEntry[] entries, Type currentSelection)
    {
      // The key of the entries is the FitFunctionAttribute, the value is the type of the fitting function

      this._twFitFunctions.BeginUpdate();
      this._twFitFunctions.Nodes.Clear();

      TreeNodeCollection root = this._twFitFunctions.Nodes; 


      foreach(DictionaryEntry entry in entries)
      {
        FitFunctionAttribute attr = (FitFunctionAttribute)entry.Key;
        System.Type fitfunctype = (System.Type)entry.Value;

        string[] path = attr.Category.Split(new char[]{'\\','/'});

        TreeNodeCollection where = root;
        for(int j=0;j<path.Length;j++)
        {
          TreeNode node = GetPathNode(where,path[j]);
          if(node==null)
          {
            node = new TreeNode(path[j]);
            where.Add(node);
          }
          where = node.Nodes;
        }

        TreeNode leaf = new TreeNode(attr.Name);
        leaf.Tag = fitfunctype;
        where.Add(leaf);
      }
      this._twFitFunctions.EndUpdate();
    }

    #endregion

    private void _twFitFunctions_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
    {
      if(_controller!=null)
        _controller.EhView_SelectionChanged(e.Node.Tag);
    }

  }
}
