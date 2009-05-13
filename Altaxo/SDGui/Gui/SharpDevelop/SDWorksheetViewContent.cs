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
using System.Windows.Forms;
using System.Collections.Generic;
using Altaxo;
using Altaxo.Main;
using Altaxo.Worksheet;
using Altaxo.Worksheet.GUI;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace Altaxo.Gui.SharpDevelop
{
  public class SDWorksheetViewContent : AbstractViewContent, Altaxo.Gui.IMVCControllerWrapper, IEditable, IClipboardHandler
  {
    Altaxo.Worksheet.GUI.WorksheetController _controller;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Worksheet.GUI.SDWorksheetController", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Serialization of old versions is not supported");
        //        info.AddBaseValueEmbedded(obj,typeof(SDGraphController).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        WorksheetController s = new WorksheetController(null, true);
        info.GetBaseValueEmbedded(s, typeof(WorksheetController), parent);

        return new SDWorksheetViewContent(s);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SDWorksheetViewContent), 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SDWorksheetViewContent s = (SDWorksheetViewContent)obj;
        info.AddValue("Controller", s._controller);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
       WorksheetController wc = (WorksheetController)info.GetValue("Controller",parent);
       SDWorksheetViewContent s = null != o ? (SDWorksheetViewContent)o : new SDWorksheetViewContent(wc);
       s._controller = wc;
       return s;
      }
    }


#endregion

    #region Constructors
    /// <summary>
    /// Creates a GraphController which shows the <see cref="Altaxo.Graph.Gdi.GraphDocument"/> in the <c>layout</c>.    
    /// </summary>
    /// <param name="layout">The graph layout which holds the graph document.</param>
    public SDWorksheetViewContent(Altaxo.Worksheet.WorksheetLayout layout)
      : this(layout, false)
    {
    }

    /// <summary>
    /// Creates a WorksheetController which shows the table data into the 
    /// View <paramref name="view"/>.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    /// <param name="bDeserializationConstructor">If true, no layout has to be provided, since this is used as deserialization constructor.</param>
    protected SDWorksheetViewContent(Altaxo.Worksheet.WorksheetLayout layout, bool bDeserializationConstructor)
    :
      this(new WorksheetController(layout))
    {
    }

    protected SDWorksheetViewContent(WorksheetController ctrl)
    {
      _controller = ctrl;
      _controller.DataColumnHeaderRightClicked += EhDataColumnHeaderRightClicked;
      _controller.DataRowHeaderRightClicked += EhDataRowHeaderRightClicked;
      _controller.PropertyColumnHeaderRightClicked += EhPropertyColumnHeaderRightClicked;
      _controller.TableHeaderRightClicked += EhTableHeaderRightClicked;
      _controller.OutsideAllRightClicked += EhOutsideAllRightClicked;

      _controller.TitleNameChanged += EhTitleNameChanged;
			SetTitle();
		}

		void EhTitleNameChanged(object sender, EventArgs e)
		{
			SetTitle();
		}

		void SetTitle()
		{
			if(_controller!=null && _controller.Doc!=null)
				this.TitleName = _controller.Doc.Name;
		}
    

    #endregion

    public static implicit operator Altaxo.Worksheet.GUI.WorksheetController(SDWorksheetViewContent ctrl)
    {
      return ctrl._controller;
    }

    public Altaxo.Worksheet.GUI.WorksheetController Controller
    {
      get { return _controller; }
    }

    public Altaxo.Gui.IMVCController MVCController
    {
      get { return _controller; }
    }

    #region Context menu handlers
    protected void EhDataColumnHeaderRightClicked(object sender, ClickedCellInfo clickedCell)
    {
      if (!(_controller.SelectedDataColumns.Contains(clickedCell.Column)) &&
          !(Controller.SelectedPropertyRows.Contains(clickedCell.Column)))
      {
        _controller.ClearAllSelections();
        _controller.SelectedDataColumns.Add(clickedCell.Column);
        _controller.View.TableAreaInvalidate();
      }
      ContextMenuStrip mnu = MenuService.CreateContextMenu(this, "/Altaxo/Views/Worksheet/DataColumnHeader/ContextMenu");
      mnu.Show((Control)_controller.ViewObject, clickedCell.MousePositionFirstDown);
    }
    protected void EhDataRowHeaderRightClicked(object sender, ClickedCellInfo clickedCell)
    {
      if (!(_controller.SelectedDataRows.Contains(clickedCell.Row)))
      {
        _controller.ClearAllSelections();
        _controller.SelectedDataRows.Add(clickedCell.Row);
        _controller.View.TableAreaInvalidate();
      }
      ContextMenuStrip mnu = MenuService.CreateContextMenu(this, "/Altaxo/Views/Worksheet/DataRowHeader/ContextMenu");
      mnu.Show((Control)_controller.ViewObject, clickedCell.MousePositionFirstDown);
    }

    protected void EhPropertyColumnHeaderRightClicked(object sender, ClickedCellInfo clickedCell)
    {
      if (!(_controller.SelectedPropertyColumns.Contains(clickedCell.Column)))
      {
        _controller.ClearAllSelections();
        _controller.SelectedPropertyColumns.Add(clickedCell.Column);
        _controller.View.TableAreaInvalidate();
      }
      ContextMenuStrip mnu = MenuService.CreateContextMenu(this, "/Altaxo/Views/Worksheet/PropertyColumnHeader/ContextMenu");
      mnu.Show((Control)_controller.ViewObject, clickedCell.MousePositionFirstDown);
    }

    protected void EhTableHeaderRightClicked(object sender, ClickedCellInfo clickedCell)
    {
      ContextMenuStrip mnu = MenuService.CreateContextMenu(this, "/Altaxo/Views/Worksheet/DataTableHeader/ContextMenu");
      mnu.Show((Control)_controller.ViewObject, clickedCell.MousePositionFirstDown);
    }

    protected void EhOutsideAllRightClicked(object sender, ClickedCellInfo clickedCell)
    {
      ContextMenuStrip mnu = MenuService.CreateContextMenu(this, "/Altaxo/Views/Worksheet/OutsideAll/ContextMenu");
      mnu.Show((Control)_controller.ViewObject, clickedCell.MousePositionFirstDown);
    }


    #endregion

    #region Abstract View Content overrides
    #region Required
    public override Control Control
    {
      get { return (Control)_controller.ViewObject; }
    }
    #endregion

    #endregion

    #region IEditable Members

    public string Text
    {
      get
      {
        return null;
      }
      set
      {
      }
    }

    #endregion

    #region IClipboardHandler Members

    public bool EnableCut
    {
      get { return _controller.EnableCut; }
    }

    public bool EnableCopy
    {
      get { return _controller.EnableCopy; }
    }

    public bool EnablePaste
    {
      get { return _controller.EnablePaste; }
    }

    public bool EnableDelete
    {
      get { return _controller.EnableDelete; }
    }

    public bool EnableSelectAll
    {
      get { return _controller.EnableSelectAll; }
    }

    public void Cut()
    {
      _controller.Cut();
    }

    public void Copy()
    {
      _controller.Copy();
    }

    public void Paste()
    {
      _controller.Paste();
    }

    public void Delete()
    {
      _controller.Delete();
    }

    public void SelectAll()
    {
      _controller.SelectAll();
    }

    #endregion
  }
}
