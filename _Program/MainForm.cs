using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using Altaxo.Serialization;

namespace Altaxo
{

	public class AltaxoAdditionalContext
	{
		public System.Runtime.Serialization.SurrogateSelector m_SurrogateSelector;
		public System.Type m_FormatterType;
	}

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class App : System.Windows.Forms.Form
	{
		private static App sm_theApplication=null;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.MenuItem menuNewWorksheet;
		private System.Windows.Forms.MenuItem menuFileOpen;
		private System.Windows.Forms.MenuItem menuFileSaveAs;

		/// <summary>
		/// Das Wichtigste - das eigentliche Dokument
		/// </summary>
		public static AltaxoDocument document=null; // das HauptDocument


		private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;
		public  System.Windows.Forms.PageSetupDialog PageSetupDialog
		{
			get { return m_PageSetupDialog; }
		}

		private System.Drawing.Printing.PrintDocument m_PrintDocument;
		public  System.Drawing.Printing.PrintDocument PrintDocument
		{
			get { return m_PrintDocument; }
		}


		private System.Windows.Forms.PrintDialog m_PrintDialog;
		public System.Windows.Forms.PrintDialog PrintDialog
		{
			get { return m_PrintDialog; }
		}

		public static System.Runtime.Serialization.SurrogateSelector m_SurrogateSelector;



		public App()
			{
				sm_theApplication = this;
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// we construct the main document
			
			if(null==document)
				document = new AltaxoDocument();
			

			// we initialize the printer variables
			m_PrintDocument = new System.Drawing.Printing.PrintDocument();
			// we set the print document default orientation to landscape
			m_PrintDocument.DefaultPageSettings.Landscape=true;
			m_PageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			m_PageSetupDialog.Document = m_PrintDocument;
			m_PrintDialog = new System.Windows.Forms.PrintDialog();
			m_PrintDialog.Document = m_PrintDocument;


			// wir konstruieren zu jeder Tabelle im Dokument ein GrafTabView
			document.CreateNewWorksheet(this);

			new Altaxo.Graph.GraphForm(this,document);
			}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuNewWorksheet = new System.Windows.Forms.MenuItem();
			this.menuFileOpen = new System.Windows.Forms.MenuItem();
			this.menuFileSaveAs = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuItem2,
																																							this.menuFileOpen,
																																							this.menuFileSaveAs});
			this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem1.Text = "File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																							this.menuNewWorksheet});
			this.menuItem2.Text = "New";
			// 
			// menuNewWorksheet
			// 
			this.menuNewWorksheet.Index = 0;
			this.menuNewWorksheet.Text = "Worksheet";
			this.menuNewWorksheet.Click += new System.EventHandler(this.menuNewWorksheet_Click);
			// 
			// menuFileOpen
			// 
			this.menuFileOpen.Index = 1;
			this.menuFileOpen.Text = "Open";
			this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
			// 
			// menuFileSaveAs
			// 
			this.menuFileSaveAs.Index = 2;
			this.menuFileSaveAs.Text = "Save As";
			this.menuFileSaveAs.Click += new System.EventHandler(this.menuFileSaveAs_Click);
			// 
			// App
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 342);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "App";
			this.Text = "Altaxo";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			sm_theApplication = new App();

			try
			{
				Application.Run(sm_theApplication);
			}
			catch(Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.ToString());
			}
		}

		public static AltaxoDocument doc
		{
			get	{	return document; }
		}
			public static App CurrentApplication
		{
			get { return sm_theApplication; }
		}

		private void menuNewWorksheet_Click(object sender, System.EventArgs e)
		{
			document.CreateNewWorksheet(this);
		}

		private void menuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream ;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
			saveFileDialog1.Filter = "Altaxo files (*.axo)|*.axo|All files (*.*)|*.*"  ;
			saveFileDialog1.FilterIndex = 2 ;
			saveFileDialog1.RestoreDirectory = true ;
 
			if(saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = saveFileDialog1.OpenFile()) != null)
				{
					System.Collections.Hashtable versionList = new System.Collections.Hashtable();
//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();
					System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
					foreach(Assembly assembly in assemblies)
					{
						// test if the assembly supports Serialization
						Attribute suppVersioning = Attribute.GetCustomAttribute(assembly,typeof(Altaxo.Serialization.SupportsSerializationVersioningAttribute));
						if(null==suppVersioning)
							continue; // this assembly don't support this, so skip it
				
						Type[] definedtypes = assembly.GetTypes();
						foreach(Type definedtype in definedtypes)
						{
							SerializationVersionAttribute versionattribute = (SerializationVersionAttribute)Attribute.GetCustomAttribute(definedtype,typeof(SerializationVersionAttribute));
							
							if(null!=versionattribute)
								versionList.Add(definedtype.FullName,versionattribute.Version);
					
							Attribute[] surrogateattributes = Attribute.GetCustomAttributes(definedtype,typeof(SerializationSurrogateAttribute));
							// compare with assembly version and search for the serialization
							// surrogate with the highest version where the version is lower than the
							// file version
							SerializationSurrogateAttribute bestattribute=null;
							int bestversion=-1;
							int objversion = null==versionattribute ? 0 : versionattribute.Version;
							foreach(SerializationSurrogateAttribute att in surrogateattributes)
							{
								if(att.Version<=objversion && att.Version>bestversion)
								{
									bestattribute = att;
									bestversion = att.Version;
								}
							}
							if(null!=bestattribute)
							{
								ss.AddSurrogate(definedtype,formatter.Context, bestattribute.Surrogate);
							}
						} // end foreach type
					} // end foreach assembly 
					
							
							formatter.SurrogateSelector=ss;
							formatter.Serialize(myStream,versionList);
							m_SurrogateSelector = ss;
							formatter.Serialize(myStream, document);
							m_SurrogateSelector=null;
							// Code to write the stream goes here.
							myStream.Close();
						} // end openfile ok
					} // end dlgresult ok
				} // end method

		private void menuFileOpen_Click(object sender, System.EventArgs e)
		{
			System.IO.Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\" ;
			openFileDialog1.Filter = "txt files (*.axo)|*.axo|All files (*.*)|*.*" ;
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.RestoreDirectory = true ;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if((myStream = openFileDialog1.OpenFile())!= null)
				{
//					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
					System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					System.Collections.Hashtable versionList = (System.Collections.Hashtable)(formatter.Deserialize(myStream));
					System.Runtime.Serialization.SurrogateSelector ss = new System.Runtime.Serialization.SurrogateSelector();

					System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
					foreach(Assembly assembly in assemblies)
					{
						// test if the assembly supports Serialization
						Attribute suppVersioning = Attribute.GetCustomAttribute(assembly,typeof(Altaxo.Serialization.SupportsSerializationVersioningAttribute));
						if(null==suppVersioning)
							continue; // this assembly don't support this, so skip it

						Type[] definedtypes = assembly.GetTypes();
						foreach(Type definedtype in definedtypes)
						{
							Attribute[] attributes = Attribute.GetCustomAttributes(definedtype,typeof(SerializationSurrogateAttribute));
							// compare with assembly version and search for the serialization
							// surrogate with the highest version where the version is lower than the
							// file version
							SerializationSurrogateAttribute bestattribute=null;
							int bestversion=-1;
							object hashversion = versionList[definedtype.FullName];
							int objversion = null==hashversion ? 0 : (int)hashversion;
							foreach(SerializationSurrogateAttribute att in attributes)
							{
								if(att.Version<=objversion && att.Version>bestversion)
								{
									bestattribute = att;
									bestversion = att.Version;
								}
							}
							if(null!=bestattribute)
							{
								ss.AddSurrogate(definedtype,formatter.Context, bestattribute.Surrogate);
							}
						}
					}

			
					/*
					AltaxoAdditionalContext additionalContext = new AltaxoAdditionalContext();
					additionalContext.m_SurrogateSelector = ss;
					additionalContext.m_FormatterType = formatter.GetType();
			
					System.Runtime.Serialization.StreamingContext context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.All,additionalContext);
					formatter.Context = context;
					*/
					
					formatter.SurrogateSelector=ss;

					m_SurrogateSelector = ss;
					object obj = formatter.Deserialize(myStream);
					m_SurrogateSelector = null;	
					document = (AltaxoDocument)obj;
					document.OnDeserialization(new DeserializationFinisher(this));
					// document.RestoreWindowsAfterDeserialization();
					// Code to write the stream goes here.

					myStream.Close();
				}
			}

		}
	}
}
