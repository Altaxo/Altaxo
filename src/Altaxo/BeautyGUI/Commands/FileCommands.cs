using System;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;
using Altaxo;
using Altaxo.Main;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Main.Commands
{
	public class CreateNewWorksheet : AbstractMenuCommand
	{
		public override void Run()
		{
			App.Current.CreateNewWorksheet();
		}
	}
	
	public class CreateNewGraph : AbstractMenuCommand
	{
		public override void Run()
		{
			App.Current.CreateNewGraph();
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
							table.Name = App.Current.Doc.DataTableCollection.FindNewTableName();
						else if( App.Current.Doc.DataTableCollection.ContainsTable(table.Name))
							table.Name = App.Current.Doc.DataTableCollection.FindNewTableName(table.Name);

						App.Current.Doc.DataTableCollection.Add(table);
						info.AnnounceDeserializationEnd(App.Current.Doc); // fire the event to resolve path references
						
						App.Current.CreateNewWorksheet(table);
					}
					else if(deserObject is Altaxo.Graph.GraphDocument)
					{
						Altaxo.Graph.GraphDocument graph = deserObject as Altaxo.Graph.GraphDocument;
						if(graph.Name==null || graph.Name==string.Empty)
							graph.Name = App.Current.Doc.GraphDocumentCollection.FindNewName();
						else if( App.Current.Doc.GraphDocumentCollection.Contains(graph.Name))
							graph.Name = App.Current.Doc.GraphDocumentCollection.FindNewName(graph.Name);

						App.Current.Doc.GraphDocumentCollection.Add(graph);
						info.AnnounceDeserializationEnd(App.Current.Doc); // fire the event to resolve path references in the graph

						App.Current.CreateNewGraph(graph);
					}
				}
			}
		}
	}


	public class FileOpen : AbstractMenuCommand
	{
		public override void Run()
		{
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\temp\\" ;
			openFileDialog1.Filter = "Altaxo project files (*.axoprj)|*.axoprj|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 1 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
		
					ZipFile zipFile = new ZipFile(myStream);
					Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
					AltaxoDocument newdocument = new AltaxoDocument();
					newdocument.RestoreFromZippedFile(zipFile,info);

					App.Current.SetDocumentFromFile(zipFile,info,newdocument);
						
					myStream.Close();
				}
			}
		}
	}


	public class FileSaveAs : AbstractMenuCommand
	{
		public override void Run()
		{
			bool bRet = true;
			SaveFileDialog dlg = this.GetSaveAsDialog();
			if(dlg.ShowDialog(App.Current.View.Form) == DialogResult.OK)
			{
				System.IO.Stream myStream;
				if((myStream = dlg.OpenFile()) != null)
				{
					try
					{
							Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
							ZipOutputStream zippedStream = new ZipOutputStream(myStream);
							App.Current.Doc.SaveToZippedFile(zippedStream, info);
							App.Current.SaveWindowStateToZippedFile(zippedStream, info);
							zippedStream.Close();
						bRet = false;; // now saving was successfull, we can close the form
					}
					catch(Exception exc)
					{
						System.Windows.Forms.MessageBox.Show(App.Current.View.Form,"An error occured saving the document, details see below:\n" + exc.ToString(),"Error",System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
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

	public class FileExit : AbstractMenuCommand
	{
		public override void Run()
		{
			System.Windows.Forms.Application.Exit();
		}
	}

}
