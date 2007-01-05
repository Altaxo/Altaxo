#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Altaxo.Gui;
using Altaxo.Gui.Scripting;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Summary description for PureScriptControl.
  /// </summary>
  [UserControlForController(typeof(IPureScriptViewEventSink),110)]
  public class SDPureScriptControl : System.Windows.Forms.UserControl, IPureScriptView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;


    private ICSharpCode.TextEditor.TextEditorControl edFormula;
    private ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBindingWrapper edFormulaWrapper;
    //ICSharpCode.SharpDevelop.Services.DefaultParserService _parserService = (ICSharpCode.SharpDevelop.Services.DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.DefaultParserService));


    public SDPureScriptControl()
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

      this.edFormula.VisibleChanged += new EventHandler(edFormula_VisibleChanged);
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
    void Register()
    {
        if(!_registered)
        {
          _registered = true;
          ParserService.RegisterModalContent(EditableContent);
        }
      
    }
    void Unregister()
    {

      ParserService.UnregisterModalContent();
      _registered = false;
     
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

        if(_registered)
          Unregister();

        if(edFormulaWrapper!=null)
          edFormulaWrapper.Dispose();
        if(edFormula!=null)
          edFormula.Dispose();

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
        if(this.edFormula.Text != value)
        {
          this.edFormula.Text = value;


          // The following line is a trick to re-get the complete folding of the text
          // otherwise, when you change the text here, the folding will be disabled

          // TODO 2006-11-06test if this works without the following line
        //  this.edFormula.Document.FoldingManager.FoldMarker.Clear(); 
        }
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

    public int InitialScriptCursorLocation
    {
      set
      {
        // do nothing here, because folding is active
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

  

    Form _parentForm;
    private void edFormula_VisibleChanged(object sender, EventArgs e)
    {
      if(edFormula.Visible)
      {
        if(null==_parentForm)
        {
          _parentForm = this.FindForm();
          _parentForm.Closing += new CancelEventHandler(_parentForm_Closing);

          Register();
          
         
        }
      }
    }

    private void _parentForm_Closing(object sender, CancelEventArgs e)
    {
      Unregister();
     
    }
  }
}
