using System;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpDevelop.Gui;

namespace Altaxo.Main.Commands
{
	public class CreateNewWorksheet : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.CreateNewWorksheet();
		}
	}
	
	public class CreateNewGraph : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.CreateNewGraph();
		}
	}

	public class CreateNewWorksheetOrGraphFromFile : AbstractMenuCommand
	{
		public override void Run()
		{
			System.IO.Stream myStream ;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
 
			openFileDialog1.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*"  ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;
 
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile()) != null)
				{
					Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
					info.BeginReading(myStream);
					object deserObject = info.GetValue("Table",null);
					info.EndReading();
					myStream.Close();

					// if it is a table, add it to the DataTableCollection
					if(deserObject is Altaxo.Data.DataTable)
					{
						Altaxo.Data.DataTable table = deserObject as Altaxo.Data.DataTable;
						if(table.Name==null || table.Name==string.Empty)
							table.Name = Current.Project.DataTableCollection.FindNewTableName();
						else if( Current.Project.DataTableCollection.ContainsTable(table.Name))
							table.Name = Current.Project.DataTableCollection.FindNewTableName(table.Name);

						Current.Project.DataTableCollection.Add(table);
						info.AnnounceDeserializationEnd(Current.Project); // fire the event to resolve path references
						
						Current.ProjectService.CreateNewWorksheet(table);
					}
					else if(deserObject is Altaxo.Graph.GraphDocument)
					{
						Altaxo.Graph.GraphDocument graph = deserObject as Altaxo.Graph.GraphDocument;
						if(graph.Name==null || graph.Name==string.Empty)
							graph.Name = Current.Project.GraphDocumentCollection.FindNewName();
						else if( Current.Project.GraphDocumentCollection.Contains(graph.Name))
							graph.Name = Current.Project.GraphDocumentCollection.FindNewName(graph.Name);

						Current.Project.GraphDocumentCollection.Add(graph);
						info.AnnounceDeserializationEnd(Current.Project); // fire the event to resolve path references in the graph

						Current.ProjectService.CreateNewGraph(graph);
					}
				}
			}
		}
	}


	public class FileOpen : AbstractMenuCommand
	{
		public override void Run()
		{
			
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\temp\\" ;
			openFileDialog1.Filter = "Altaxo project files (*.axoprj)|*.axoprj|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Current.ProjectService.OpenProject(openFileDialog1.FileName);
				
				/*
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
		
					ZipFile zipFile = new ZipFile(myStream);
					Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
					AltaxoDocument newdocument = new AltaxoDocument();
					newdocument.RestoreFromZippedFile(zipFile,info);

					Current.ProjectService.SetDocumentFromFile(zipFile,info,newdocument);
						
					myStream.Close();
				}
				*/
			}
		}
	}


	public class FileSaveAs : AbstractMenuCommand
	{
		public override void Run()
		{
			Current.ProjectService.SaveProjectAs();
		}

		public  void RunOld1()
		{
			SaveFileDialog dlg = this.GetSaveAsDialog();
			if(dlg.ShowDialog(Current.MainWindow) == DialogResult.OK)
			{
				System.IO.Stream myStream;
				if((myStream = dlg.OpenFile()) != null)
				{
					try
					{
						Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
						ZipOutputStream zippedStream = new ZipOutputStream(myStream);
						Current.Project.SaveToZippedFile(zippedStream, info);
						Current.ProjectService.SaveWindowStateToZippedFile(zippedStream, info);
						zippedStream.Close();
						Current.Project.IsDirty=false;

					}
					catch(Exception exc)
					{
						System.Windows.Forms.MessageBox.Show(Current.MainWindow,"An error occured saving the document, details see below:\n" + exc.ToString(),"Error",System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
					}
					finally
					{
						myStream.Close();
					}
				}
			}
		}

		protected SaveFileDialog GetSaveAsDialog()
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
			saveFileDialog1.Filter = "Altaxo project files (*.axoprj)|*.axoprj|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 1 ;
			saveFileDialog1.RestoreDirectory = true ;
 	
			return saveFileDialog1;
		}
	}

	public class FileSave : AbstractMenuCommand
	{
		public override void Run()
		{
			if(Current.ProjectService.CurrentProjectFileName != null)
				Current.ProjectService.SaveProject();
			else
				Current.ProjectService.SaveProjectAs();
		}
	}


	public class CloseProject : AbstractMenuCommand
	{
		public override void Run()
		{
			if(Current.Project.IsDirty)
				Current.ProjectService.SaveProjectAs();
		
			Current.ProjectService.CloseProject(false);
		}
	}

	public class FileExit : AbstractMenuCommand
	{
		public override void Run()
		{
		((Form)WorkbenchSingleton.Workbench).Close();		}
	}

}
