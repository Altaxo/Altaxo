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
    private System.Windows.Forms.ContextMenu _treeViewContextMenu;
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
      this._treeViewContextMenu = new System.Windows.Forms.ContextMenu();
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
      this._twFitFunctions.ContextMenu = this._treeViewContextMenu;
      this._twFitFunctions.ImageIndex = -1;
      this._twFitFunctions.Location = new System.Drawing.Point(8, 16);
      this._twFitFunctions.Name = "_twFitFunctions";
      this._twFitFunctions.SelectedImageIndex = -1;
      this._twFitFunctions.Size = new System.Drawing.Size(136, 272);
      this._twFitFunctions.TabIndex = 1;
      this._twFitFunctions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._twFitFunctions_AfterSelect);
      // 
      // _treeViewContextMenu
      // 
      this._treeViewContextMenu.Popup += new System.EventHandler(this._treeViewContextMenu_Popup);
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


    #region Node classes

    enum RootNodeType { Builtin, Document, User };

    class RootNode : TreeNode
    {
      public RootNodeType RootNodeType;

      public RootNode(string text, RootNodeType type)
      :
      base(text)
      {
        RootNodeType = type;
        this.Tag = type;
      }
    }

    class CategoryNode : TreeNode
    {
      public CategoryNode(string text) : base(text) { }
    }

    class LeafNode : TreeNode
    {
      public LeafNode(string text) : base(text) { }  
    }

    class BuiltinLeafNode : LeafNode
    {
      public System.Type FunctionType;

      public BuiltinLeafNode(string text, System.Type functionType)
        : base(text)
      {
        FunctionType = functionType;
        this.Tag = functionType;
      }
    }

    class DocumentLeafNode : LeafNode
    {
      public IFitFunction FunctionInstance;
      public DocumentLeafNode(string text, IFitFunction func)
        : base(text)
      {
        FunctionInstance = func;
        this.Tag = func;
      }
    }


    #endregion

    public void InitializeFitFunctionList(DictionaryEntry[] entries, Type currentSelection)
    {
      // The key of the entries is the FitFunctionAttribute, the value is the type of the fitting function

      this._twFitFunctions.BeginUpdate();
      this._twFitFunctions.Nodes.Clear();

      RootNode rnode = new RootNode("Builtin", RootNodeType.Builtin);
      this._twFitFunctions.Nodes.Add(rnode);
      TreeNodeCollection root = rnode.Nodes; 


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
            node = new CategoryNode(path[j]);
            where.Add(node);
          }
          where = node.Nodes;
        }

        BuiltinLeafNode leaf = new BuiltinLeafNode(attr.Name,fitfunctype);
        where.Add(leaf);
      }
      this._twFitFunctions.EndUpdate();
    }


    public void InitializeDocumentFitFunctionList(DictionaryEntry[] entries, object currentSelection)
    {
      // The key of the entries is the name string, the value is the object

      this._twFitFunctions.BeginUpdate();

         RootNode rnode = new RootNode("Document", RootNodeType.Document);
         this._twFitFunctions.Nodes.Add(rnode);
      TreeNodeCollection root = rnode.Nodes;

      foreach (DictionaryEntry entry in entries)
      {
        string fullname = (string)entry.Key;
        IFitFunction fitfunc = (IFitFunction)entry.Value;

#if NET_2_0 
        string[] path = fullname.Split(new char[] { '\\', '/' },true);
#else
          string[] path = fullname.Split(new char[] { '\\', '/' });
#endif
        
        if(path.Length==0)
          continue;

        TreeNodeCollection where = root;
        for (int j = 0; j < path.Length-1; j++)
        {
          TreeNode node = GetPathNode(where, path[j]);
          if (node == null)
          {
            node = new CategoryNode(path[j]);
            where.Add(node);
          }
          where = node.Nodes;
        }

        TreeNode leaf = new DocumentLeafNode(path[path.Length-1],fitfunc);
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

    private void EhEditItem(object sender, EventArgs e)
    {
    }

    private void _treeViewContextMenu_Popup(object sender, System.EventArgs e)
    {
      if(this._twFitFunctions.SelectedNode!=null)
      {
        if(_twFitFunctions.SelectedNode is DocumentLeafNode)
        {
          this._treeViewContextMenu.MenuItems.Add(new MenuItem("Edit",new EventHandler(this.EhEditItem)));
        }
      }
    }

  }
}
