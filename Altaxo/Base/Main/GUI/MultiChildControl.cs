using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// Summary description for MultiChildControl.
  /// </summary>
  [UserControlForController(typeof(IMultiChildViewEventSink))]
  public class MultiChildControl : System.Windows.Forms.UserControl, IMultiChildView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public MultiChildControl()
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
      this.m_Label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // m_Label1
      // 
      this.m_Label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
      this.m_Label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.m_Label1.Location = new System.Drawing.Point(8, 8);
      this.m_Label1.Name = "m_Label1";
      this.m_Label1.Size = new System.Drawing.Size(240, 16);
      this.m_Label1.TabIndex = 3;
      this.m_Label1.Text = "Please enter :";
      this.m_Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      // 
      // MultiChildControl
      // 
      this.Controls.Add(this.m_Label1);
      this.Name = "MultiChildControl";
      this.Size = new System.Drawing.Size(264, 56);
      this.ResumeLayout(false);

    }
    #endregion

    #region ISingleValueView Members

    IMultiChildViewEventSink _controller;
    private System.Windows.Forms.Label m_Label1;
  
    public IMultiChildViewEventSink Controller
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

    string _description;
    public void InitializeDescription(string value)
    {
      _description = value;
      LocateAndResize();  
    }

    Control[] _childs = new Control[0];
    public void InitializeChilds(object[] childs, int initialfocused)
    {
      for(int i=0;i<_childs.Length;i++)
      {
        _childs[i].SizeChanged -= new EventHandler(EhChilds_SizeChanged);
      }

      this.Controls.Clear();
      this.Controls.Add(m_Label1);


      _childs = new UserControl[childs.Length];
      for(int i=0;i<_childs.Length;i++)
      {
        _childs[i] = (Control)childs[i];
        _childs[i].SizeChanged += new EventHandler(EhChilds_SizeChanged);
      }
    
      this.Controls.AddRange(_childs);
      LocateAndResize();
    }

    private void EhChilds_SizeChanged(object sender, EventArgs e)
    {
      if(0==_SuspendLayout)
        LocateAndResize();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      ++_SuspendLayout;
      base.OnSizeChanged (e);
      --_SuspendLayout;
    }


    int _SuspendLayout=0;
    void LocateAndResize()
    {
      ++_SuspendLayout;

      if(_description!=null && _description!=string.Empty)
      {
        SizeF size1, size2;
        using(System.Drawing.Graphics grfx = this.CreateGraphics())
        {
          size1 = grfx.MeasureString(_description,m_Label1.Font);
          size2 = grfx.MeasureString(_description,m_Label1.Font,m_Label1.ClientSize.Width);
        }
        m_Label1.Size = new Size(this.ClientSize.Width-2*m_Label1.Location.X,(int)(m_Label1.PreferredHeight*Math.Ceiling(size2.Height/size1.Height)));
        this.m_Label1.Text = _description;
      }
      else
      {
        this.m_Label1.Text = null;
        this.m_Label1.Size = new Size(0,0);
      }


      if(_childs.Length>0)
      {
        _childs[0].Location = new Point(m_Label1.Location.X,m_Label1.Bounds.Bottom);
        for(int i=1;i<_childs.Length;i++)
        {
          _childs[i].Location = new Point(m_Label1.Location.X, _childs[i-1].Bounds.Bottom);
        }
        this.ClientSize = new Size(this.ClientSize.Width, _childs[_childs.Length-1].Bounds.Bottom);
      }
      else
      {
        this.ClientSize = new Size(this.ClientSize.Width,m_Label1.Bounds.Bottom);
      }
      --_SuspendLayout;
    }

    #endregion

  
  }
}
