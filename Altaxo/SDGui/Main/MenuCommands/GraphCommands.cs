#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using Altaxo.Graph;
using Altaxo.Graph.GUI;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Graph.Commands
{

  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractGraphControllerCommand : AbstractMenuCommand
  {
    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Run()
    {
      Altaxo.Graph.GUI.GraphController ctrl 
        = Current.Workbench.ActiveViewContent 
        as Altaxo.Graph.GUI.GraphController;
      
      if(null!=ctrl)
        Run(ctrl);
    }
  
    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Altaxo.Graph.GUI.GraphController ctrl);
  }


  /// <summary>
  /// Handler for the menu item "File" - "Setup Page".
  /// </summary>
  public class PageSetup : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        Current.PrintingService.PageSetupDialog.ShowDialog(ctrl.View.Form);
      }
      catch(Exception exc)
      {
        MessageBox.Show(ctrl.View.Form, exc.ToString(),"Exception occured!");
      }   
    }
  }


  /// <summary>
  /// Handler for the menu item "File" - "Print".
  /// </summary>
  public class Print : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        if(DialogResult.OK==Current.PrintingService.PrintDialog.ShowDialog(ctrl.View.Form))
        {
          Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
          Current.PrintingService.PrintDocument.Print();
        }
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Print Preview".
  /// </summary>
  public class PrintPreview : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      try
      {
        System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
        Current.PrintingService.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
        dlg.Document = Current.PrintingService.PrintDocument;
        dlg.ShowDialog(ctrl.View.Form);
        dlg.Dispose();
      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
      }
      finally
      {
        Current.PrintingService.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
      }
    }
  }


  public class SaveGraphAsTemplate : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          info.BeginWriting(myStream);
          info.AddValue("Graph",ctrl.Doc);
          info.EndWriting();
          myStream.Close();
        }
      }
    }
  }

  /// <summary>
  /// Handler for the menu item "File" - "Export Metafile".
  /// </summary>
  public class FileExportMetafile : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Windows Metafiles (*.emf)|*.emf|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 2 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          ctrl.SaveAsMetafile(myStream);
          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok
    }
  }


  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Normal: Bottom X Left Y".
  /// </summary>
  public class NewLayerNormalBottomXLeftY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerNormalBottomXLeftY();
  
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y".
  /// </summary>
  public class NewLayerLinkedTopXRightY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY(ctrl.CurrentLayerNumber);

    }
  }
  
  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X".
  /// </summary>
  public class NewLayerLinkedTopX : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopX(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Right Y".
  /// </summary>
  public class NewLayerLinkedRightY : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedRightY(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Handler for the menu item "Edit" - "New layer(axes)" - "Linked: Top X Right Y, X axis straight ".
  /// </summary>
  public class NewLayerLinkedTopXRightY_XAxisStraight : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.CreateNewLayerLinkedTopXRightY_XAxisStraight(ctrl.CurrentLayerNumber);
    }
  }

  /// <summary>
  /// Duplicates the Graph and the Graph view to a new one.
  /// </summary>
  public class DuplicateGraph : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      GraphDocument newDoc = new GraphDocument(ctrl.Doc);
      Current.Project.GraphDocumentCollection.Add(newDoc);
      Current.ProjectService.CreateNewGraph(newDoc);
    }
  }

  public class LayerControl : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      if(null!=ctrl.ActiveLayer)
        LayerController.ShowDialog(ctrl.View.Form,ctrl.ActiveLayer);
    }
  }

  public class AddCurvePlot : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.EnsureValidityOfCurrentLayerNumber();
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(new Graph.PolynomialFunction(new double[]{0,0,1})),new XYLineScatterPlotStyle(LineScatterPlotStyleKind.Line)));
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class NewLayerLegend : AbstractGraphControllerCommand
  {
    public override void Run(Altaxo.Graph.GUI.GraphController ctrl)
    {
      ctrl.Doc.Layers[ctrl.CurrentLayerNumber].CreateNewLayerLegend();
    }
  }


  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractCheckableGraphControllerCommand : AbstractCheckableMenuCommand
  {
    public Altaxo.Graph.GUI.GraphController Controller
    {
      get 
      {
        if(null!=Current.Workbench && null!=Current.Workbench.ActiveViewContent)
          return Current.Workbench.ActiveViewContent as Altaxo.Graph.GUI.GraphController;
        else
          return null;
      }
    }

    /// <summary>
    /// This function is never be called, since this is a CheckableMenuCommand.
    /// </summary>
    public override void Run()
    {
    }
  }

  /// <summary>
  /// This class is intented to be used for commands into the graph tools toolbar. Commands derived
  /// from it will update the toolbar whenever its state changed.
  /// </summary>
  public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
  {
    Altaxo.Graph.GUI.GraphController myCurrentGraphController;

    protected AbstractGraphToolsCommand()
    {
      if(null!=Current.Workbench)
      {
        Current.Workbench.ActiveViewContentChanged += new EventHandler(this.EhWorkbenchContentChanged);
        this.EhWorkbenchContentChanged(this,EventArgs.Empty);
      }
    }

    protected void EhWorkbenchContentChanged(object o, System.EventArgs e)
    {
      if(!object.ReferenceEquals(Controller,myCurrentGraphController))
      {
        if(null!=myCurrentGraphController)
        {
          lock(this)
          {
            this.myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(this.EhGraphToolChanged);
            this.myCurrentGraphController = null;
          }
        }
        if(Controller!=null)
        {
          lock(this)
          {
            this.myCurrentGraphController = this.Controller;
            this.myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(this.EhGraphToolChanged);
          }
        }
      }
    }

    protected void EhGraphToolChanged(object o, EventArgs e)
    {
      bool bBaseChecked = base.IsChecked;
      bool bThisChecked = this.IsChecked; // only to retrieve the checked state
      if(bBaseChecked!=bThisChecked) // to prevent that every button "updates" the toolbar
      {
        ICSharpCode.SharpDevelop.Gui.Workbench1 wb = Current.Workbench as ICSharpCode.SharpDevelop.Gui.Workbench1;
        wb.UpdateToolbars();      
      }
    }

  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectPointerTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphTool==GraphTools.ObjectPointer);
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphTool=GraphTools.ObjectPointer;
        }

        ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectTextTool : AbstractGraphToolsCommand
  {
    public override bool IsChecked 
    {
      get 
      {
        if(null!=Controller)
        {
          base.IsChecked = (Controller.CurrentGraphTool==GraphTools.Text);
        }

        return base.IsChecked;
      }
      set 
      {
        base.IsChecked = value;
        if(true==value && null!=Controller)
        {
          Controller.CurrentGraphTool=GraphTools.Text;
        }

        ((ICSharpCode.SharpDevelop.Gui.DefaultWorkbench)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).UpdateToolbars();

      }
    }
  }
}
