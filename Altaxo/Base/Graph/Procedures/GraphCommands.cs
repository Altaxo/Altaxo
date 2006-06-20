#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Procedures
{
  /// <summary>
  /// GraphCommands contain operations concerning the graph document itself, such as rename.
  /// </summary>
  public class GraphCommands
  {
    public static void Rename(GraphController ctrl)
    {
      Main.GUI.TextValueInputController tvctrl = new Main.GUI.TextValueInputController(
        ctrl.Doc.Name,
        new Main.GUI.SingleValueDialog("Rename graph", "Enter a name for the graph:")
        );

      tvctrl.Validator = new GraphRenameValidator(ctrl.Doc, ctrl);
      if (tvctrl.ShowDialog(Current.MainWindow))
        ctrl.Doc.Name = tvctrl.InputText.Trim();
    }

    protected class GraphRenameValidator : Main.GUI.TextValueInputController.NonEmptyStringValidator
    {
      Altaxo.Graph.GraphDocument _doc;
      GraphController _controller;

      public GraphRenameValidator(Altaxo.Graph.GraphDocument graphdoc, GraphController ctrl)
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
        else if (Graph.GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc) == null)
          return null; // if there is no parent data set we can enter anything
        else if (Graph.GraphDocumentCollection.GetParentGraphDocumentCollectionOf(_controller.Doc).Contains(graphname))
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


      static CopyPageCommand()
      {
      }

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
                IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
                bResult = hRes.Equals(hEMF2);
                CloseClipboard();
              }
            }
          }
          DeleteEnhMetaFile(hEMF);
        }
        return bResult;
      }

      static public void Run(GraphController ctrl)
      {
        // Create a bitmap just to have a graphics context
        System.Drawing.Bitmap helperbitmap = new System.Drawing.Bitmap(4, 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        helperbitmap.SetResolution(300, 300);
        Graphics grfx = Graphics.FromImage(helperbitmap);
        // Code to write the stream goes here.
        IntPtr ipHdc = grfx.GetHdc();
        System.IO.Stream stream = new System.IO.MemoryStream();
        //System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream,ipHdc);
        System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile(stream, ipHdc, ctrl.Doc.PageBounds, MetafileFrameUnit.Point);

        //      System.Drawing.Imaging.Metafile mf = new System.Drawing.Imaging.Metafile("CreateMetaFile.emf",ipHdc);
        grfx.ReleaseHdc(ipHdc);
        grfx.Dispose();
        grfx = Graphics.FromImage(mf);
        grfx.PageUnit = GraphicsUnit.Point;
        grfx.PageScale = 1;
        grfx.TranslateTransform(ctrl.Doc.PrintableBounds.X, ctrl.Doc.PrintableBounds.Y);

        ctrl.Doc.DoPaint(grfx, true);


        grfx.Dispose();
        helperbitmap.Dispose();


        stream.Flush();
        stream.Close();

        PutEnhMetafileOnClipboard(ctrl.View.Form.Handle, mf);
        //System.Windows.Forms.Clipboard.SetData(DataFormats.GetFormat(DataFormats.EnhancedMetafile).Name, mf);
      }
    }

    public static void CopyPageToClipboard(GraphController ctrl)
    {
      CopyPageCommand.Run(ctrl);
    }
  }
}
