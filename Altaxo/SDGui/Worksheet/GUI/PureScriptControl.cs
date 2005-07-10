using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Main.GUI;

namespace Altaxo.Worksheet.GUI
{
	/// <summary>
	/// Summary description for PureScriptControl.
	/// </summary>
	[UserControlForController(typeof(IPureScriptViewEventSink))]
	public class PureScriptControl : System.Windows.Forms.UserControl, IPureScriptView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


    private ICSharpCode.TextEditor.TextEditorControl edFormula;
    private ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper edFormulaWrapper;


		public PureScriptControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

      InitializeEditor();

		}

    void InitializeEditor()
    {
      this.SuspendLayout();

      this.edFormulaWrapper = new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper();
      this.edFormula = edFormulaWrapper.TextEditorControl;

       
      this.edFormula.Location = new System.Drawing.Point(0, 0);
      this.edFormula.Dock = DockStyle.Fill;
      //this.edFormula.Multiline = true;
      this.edFormula.Name = "edFormula";
      //this.edFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.edFormula.Size = new System.Drawing.Size(528, 336);
      this.edFormula.Text = "";

      this.Controls.Add(this.edFormula);
      
      this.ScriptName = System.Guid.NewGuid().ToString() + ".cs";
      this.edFormula.Document.TextEditorProperties.TabIndent=2;
      this.edFormulaWrapper.textAreaControl.InitializeFormatter();
      this.edFormulaWrapper.textAreaControl.TextEditorProperties.MouseWheelScrollDown=true;

      this.ResumeLayout();
    }

    bool _registered;
    protected override void OnEnter(EventArgs e)
    {
      if(false==_registered)
      {
        if(_controller!=null)
        {
          _registered=true;
          _controller.EhView_RegisterEditableContent(this.EditableContent);
        }
      }
      base.OnEnter (e);
    }

    protected override void OnLeave(EventArgs e)
    {
      if(true==_registered)
      {
        if(_controller!=null)
        {
          _registered = false;
          _controller.EhView_UnregisterEditableContent(this.EditableContent);
        }
      }
      base.OnLeave (e);
    }

    private object EditableContent
    {
      get
      { 
        return this.edFormulaWrapper; 
      }
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

    #region IPureScriptView Members

    IPureScriptViewEventSink _controller;
    public IPureScriptViewEventSink Controller
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

    public string ScriptName
    {
      set
      {
        edFormulaWrapper.TextEditorControl.FileName = value;
        edFormulaWrapper.TitleName = value;
        edFormulaWrapper.FileName = value;
      }
    }

    public string ScriptText
    {
      get 
      {
        return this.edFormula.Text; 
      }
      set 
      {
        this.edFormula.Text = value;
      }
    }

    public int ScriptCursorLocation
    {
      set
      {
        System.Drawing.Point point = edFormulaWrapper.textAreaControl.Document.OffsetToPosition(value);
        this.edFormulaWrapper.JumpTo(point.Y,point.X);
      }

    }


    public void SetScriptCursorLocation(int line, int column)
    {
      this.edFormulaWrapper.JumpTo(line,column);
    }


    public void MarkText(int pos1, int pos2)
    {

    }


  

    #endregion

    class ParentFormMonitor
    {
      public event EventHandler ParentFormOpen;
      public event EventHandler ParentFormClose;
      Control _control;
      Form _parentForm;
      Control _parentControl;

      public ParentFormMonitor(UserControl control)
      {
        _control = control;
      }
      public void Start()
      {
        _parentControl = _control;
       for(;;)
        {
          if(_parentControl.Parent is Form)
          {
            _parentForm = (Form)_parentControl.Parent;
            break;
          }
          else if(_parentControl.Parent!=null)
          {
            _parentControl = _parentControl.Parent;
          }
          else
          {
            break;
          }
        }

        if(_parentControl!=null)
        {
          _parentControl.ParentChanged += new EventHandler(_parentControl_ParentChanged);
        }
       
        if(_parentForm!=null)
        {
          if(this.ParentFormOpen!=null)
            ParentFormOpen(this,EventArgs.Empty);

          _parentForm.Closed += new EventHandler(ParentForm_Closed);
        }

      }

      private void ParentForm_Closed(object sender, EventArgs e)
      {
        if(this.ParentFormClose!=null)
          ParentFormClose(this,EventArgs.Empty);

      }

      private void _parentControl_ParentChanged(object sender, EventArgs e)
      {
      }
    }
   
  }
}
