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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

using Altaxo.Graph.GUI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// GraphCommands contain operations concerning the graph document itself, such as rename.
  /// </summary>
  public class GraphCommands
  {
    public static void Rename(GraphController ctrl)
    {
      TextValueInputController tvctrl = new TextValueInputController(ctrl.Doc.Name,"Enter a name for the graph:");
      tvctrl.Validator = new GraphRenameValidator(ctrl.Doc, ctrl);

      if(Current.Gui.ShowDialog(tvctrl, "Rename graph", false))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class GraphRenameValidator : TextValueInputController.NonEmptyStringValidator
    {
      GraphDocument _doc;
      GraphController _controller;

      public GraphRenameValidator(GraphDocument graphdoc, GraphController ctrl)
        : base("The graph's name must not be empty! Please enter a valid name.")
      {
        _doc = graphdoc;
        _controller = ctrl;
      }

      public override string Validate(string graphname)
      {
        string err = base.Validate(graphname);
        if (null != err)
          return err;

        if (_doc.Name == graphname)
          return null;
        else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc) == null)
          return null; // if there is no parent data set we can enter anything
        else if (GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc).Contains(graphname))
          return "This graph name already exists, please choose another name!";
        else
          return null;
      }
    }

    public static void Refresh(GraphController ctrl)
    {
      ctrl.RefreshGraph();
    }


    /// <summary>
    /// Handler for the menu item "Edit" - "CopyPage".
    /// </summary>
    class CopyPageCommand
    {

      /*
      [DllImport("user32.dll")]
      static extern bool OpenClipboard(IntPtr hWndNewOwner);
      [DllImport("user32.dll")]
      static extern bool EmptyClipboard();
      [DllImport("user32.dll")]
      static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
      [DllImport("user32.dll")]
      static extern bool CloseClipboard();
      [DllImport("gdi32.dll")]
      static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
      [DllImport("gdi32.dll")]
      static extern bool DeleteEnhMetaFile(IntPtr hemf);

      /// <summary>
      /// Microsoft Knowledge Base Article - 323530 PRB: Metafiles on Clipboard Are Not Visible to All Applications
      /// </summary>
      /// <param name="hWnd"></param>
      /// <param name="mf"></param>
      /// <returns></returns>
      static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile mf)
      {
        bool bResult = false;
        IntPtr hEMF, hEMF2;
        hEMF = mf.GetHenhmetafile(); // invalidates mf
        if (!hEMF.Equals(new IntPtr(0)))
        {
          hEMF2 = CopyEnhMetaFile(hEMF, new IntPtr(0));
          if (!hEMF2.Equals(new IntPtr(0)))
          {
            if (OpenClipboard(hWnd))
            {
              if (EmptyClipboard())
              {
                IntPtr hRes = SetClipboardData(14 , hEMF2); // 14==CF_ENHMETAFILE
                bResult = hRes.Equals(hEMF2);
                CloseClipboard();
             }
            }
          }
          DeleteEnhMetaFile(hEMF);
        }
        return bResult;
      }
      */


      static public void Run(GraphController ctrl)
      {
       // System.Drawing.Imaging.Metafile mf = Altaxo.Graph.Procedures.Export.GetMetafile(ctrl.Doc);
       // PutEnhMetafileOnClipboard(ctrl.View.Form.Handle, mf);

        System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
        string filepath = System.IO.Path.GetTempPath();
        string filename = filepath + "AltaxoClipboardMetafile.emf";
        if (System.IO.File.Exists(filename))
          System.IO.File.Delete(filename);
        Metafile mf = Altaxo.Graph.Procedures.Export.SaveAsMetafile(ctrl.Doc, filename, 300);
        System.Collections.Specialized.StringCollection coll = new System.Collections.Specialized.StringCollection();
        coll.Add(filename);
        dao.SetFileDropList(coll);
        dao.SetData(typeof(Metafile), mf);
        System.Windows.Forms.Clipboard.SetDataObject(dao);

        
      }
    }

    public static void CopyPageToClipboard(GraphController ctrl)
    {
      CopyPageCommand.Run(ctrl);
    }

    /// <summary>
    /// Copies the current graph as an bitmap image to the clipboard.
    /// </summary>
    /// <param name="ctrl">Controller controlling the current graph.</param>
    /// <param name="dpiResolution">Resolution of the bitmap in dpi. Determines the pixel size of the bitmap.</param>
    /// <param name="backbrush">Brush used to fill the background of the image. Can be <c>null</c>.</param>
    /// <param name="pixelformat">Specify the pixelformat here.</param>
    public static void CopyPageToClipboardAsBitmap(GraphController ctrl, int dpiResolution, Brush backbrush, PixelFormat pixelformat)
    {
      System.Windows.Forms.DataObject dao = new System.Windows.Forms.DataObject();
      System.Drawing.Bitmap bitmap = Altaxo.Graph.Procedures.Export.SaveAsBitmap(ctrl.Doc, dpiResolution, backbrush, pixelformat);
      dao.SetImage(bitmap);
      System.Windows.Forms.Clipboard.SetDataObject(dao);
    }


  }
}
