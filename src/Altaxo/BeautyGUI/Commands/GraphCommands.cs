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
				= App.Current.Workbench.ActiveWorkbenchWindow.ActiveViewContent 
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
				App.Current.PageSetupDialog.ShowDialog(ctrl.View.Form);
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
				if(DialogResult.OK==App.Current.PrintDialog.ShowDialog(ctrl.View.Form))
				{
					App.Current.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
					App.Current.PrintDocument.Print();
				}
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
			}
			finally
			{
				App.Current.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
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
				App.Current.PrintDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
				dlg.Document = App.Current.PrintDocument;
				dlg.ShowDialog(ctrl.View.Form);
				dlg.Dispose();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ctrl.View.Form,ex.ToString());
			}
			finally
			{
				App.Current.PrintDocument.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(ctrl.EhPrintPage);
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
			App.Current.CreateNewGraph(newDoc);
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
			ctrl.Doc.Layers[ctrl.CurrentLayerNumber].PlotItems.Add(new XYFunctionPlotItem(new XYFunctionPlotData(),new XYLineScatterPlotStyle()));
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
				if(null!=App.Current && null!=App.Current.Workbench && null!=App.Current.Workbench.ActiveWorkbenchWindow)
					return App.Current.Workbench.ActiveWorkbenchWindow.ActiveViewContent as Altaxo.Graph.GUI.GraphController;
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
	/// Test class for a selected item
	/// </summary>
	public class SelectPointerTool : AbstractCheckableGraphControllerCommand
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
	public class SelectTextTool : AbstractCheckableGraphControllerCommand
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
