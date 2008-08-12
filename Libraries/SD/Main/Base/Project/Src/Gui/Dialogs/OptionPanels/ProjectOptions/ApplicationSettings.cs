// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2973 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class ApplicationSettings : AbstractProjectOptionPanel
	{
		ComboBox applicationManifestComboBox;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.ApplicationSettings.xfrm");
			
			InitializeHelper();
			
			ConnectBrowseButton("applicationIconBrowseButton", "applicationIconTextBox",
			                    "${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*",
			                    TextBoxEditMode.EditEvaluatedProperty);
			
			ConnectBrowseButton("win32ResourceFileBrowseButton", "win32ResourceFileTextBox",
			                    "Win32 Resource files|*.res|${res:SharpDevelop.FileFilter.AllFiles}|*.*",
			                    TextBoxEditMode.EditEvaluatedProperty);
			
			ConfigurationGuiBinding b;
			ChooseStorageLocationButton locationButton;
			b = helper.BindString("assemblyNameTextBox", "AssemblyName", TextBoxEditMode.EditEvaluatedProperty);
			b.CreateLocationButton("assemblyNameTextBox");
			Get<TextBox>("assemblyName").TextChanged += new EventHandler(RefreshOutputNameTextBox);
			
			b = helper.BindString("rootNamespaceTextBox", "RootNamespace", TextBoxEditMode.EditEvaluatedProperty);
			b.CreateLocationButton("rootNamespaceTextBox");
			
			b = helper.BindEnum<OutputType>("outputTypeComboBox", "OutputType");
			locationButton = b.CreateLocationButton("outputTypeComboBox");
			Get<ComboBox>("outputType").SelectedIndexChanged += RefreshOutputNameTextBox;
			Get<ComboBox>("outputType").SelectedIndexChanged += RefreshStartupObjectEnabled;
			
			b = helper.BindString("startupObjectComboBox", "StartupObject", TextBoxEditMode.EditEvaluatedProperty);
			b.RegisterLocationButton(locationButton);
			foreach (IClass c in GetPossibleStartupObjects(project)) {
				Get<ComboBox>("startupObject").Items.Add(c.FullyQualifiedName);
			}
			
			b = helper.BindString("applicationIconTextBox", "ApplicationIcon", TextBoxEditMode.EditEvaluatedProperty);
			Get<TextBox>("applicationIcon").TextChanged += new EventHandler(ApplicationIconTextBoxTextChanged);
			b.CreateLocationButton("applicationIconTextBox");
			
			b = helper.BindString("win32ResourceFileTextBox", "Win32Resource", TextBoxEditMode.EditEvaluatedProperty);
			b.CreateLocationButton("win32ResourceFileTextBox");
			
			applicationManifestComboBox = Get<ComboBox>("applicationManifest");
			applicationManifestComboBox.Items.Add("Embed default manifest");
			applicationManifestComboBox.Items.Add("Do not embed manifest");
			foreach (string fileName in Directory.GetFiles(project.Directory, "*.manifest")) {
				applicationManifestComboBox.Items.Add(Path.GetFileName(fileName));
			}
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
			applicationManifestComboBox.SelectedIndexChanged += ApplicationManifestComboBox_SelectedIndexChanged;
			
			b = new ManifestBinding(applicationManifestComboBox);
			helper.AddBinding("ApplicationManifest", b);
			b.CreateLocationButton(applicationManifestComboBox);
			applicationManifestComboBox.TextChanged += delegate { helper.IsDirty = true; };
			
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			// re-evaluate if the project has the minimum version whenever this options page gets visible
			// because the "convert project" button on the compiling tab page might have updated the MSBuild version.
			applicationManifestComboBox.VisibleChanged += delegate {
				applicationManifestComboBox.Enabled = project.MinimumSolutionVersion >= 10;
			};
			
			Get<TextBox>("projectFolder").Text = project.Directory;
			Get<TextBox>("projectFile").Text = Path.GetFileName(project.FileName);
			
			// maybe make this writable again? Needs special care when saving!
			Get<TextBox>("projectFile").ReadOnly = true;
			
			RefreshStartupObjectEnabled(null, EventArgs.Empty);
			RefreshOutputNameTextBox(null, EventArgs.Empty);
			
			helper.AddConfigurationSelector(this);
		}
		
		void ApplicationManifestComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 2) {
				BeginInvoke(new MethodInvoker(CreateManifest));
			} else if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 1) {
				BeginInvoke(new MethodInvoker(BrowseForManifest));
			}
		}
		
		void BrowseForManifest()
		{
			applicationManifestComboBox.SelectedIndex = -1;
			BrowseForFile(applicationManifestComboBox, "Manifest files|*.manifest|${res:SharpDevelop.FileFilter.AllFiles}|*.*", TextBoxEditMode.EditEvaluatedProperty);
		}
		
		void CreateManifest()
		{
			string manifestFile = Path.Combine(project.Directory, "app.manifest");
			if (!File.Exists(manifestFile)) {
				string defaultManifest;
				using (Stream stream = typeof(ApplicationSettings).Assembly.GetManifestResourceStream("Resources.DefaultManifest.manifest")) {
					if (stream == null)
						throw new ResourceNotFoundException("DefaultManifest.manifest");
					using (StreamReader r = new StreamReader(stream)) {
						defaultManifest = r.ReadToEnd();
					}
				}
				defaultManifest = defaultManifest.Replace("\t", DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.IndentationString);
				File.WriteAllText(manifestFile, defaultManifest, System.Text.Encoding.UTF8);
				FileService.FireFileCreated(manifestFile, false);
			}
			
			if (!project.IsFileInProject(manifestFile)) {
				FileProjectItem newItem = new FileProjectItem(project, ItemType.None);
				newItem.Include = "app.manifest";
				ProjectService.AddProjectItem(project, newItem);
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
			
			FileService.OpenFile(manifestFile);
			
			applicationManifestComboBox.Text = "app.manifest";
		}
		
		sealed class ManifestBinding : ConfigurationGuiBinding
		{
			ComboBox applicationManifestComboBox;
			
			public ManifestBinding(ComboBox applicationManifestComboBox)
			{
				this.applicationManifestComboBox = applicationManifestComboBox;
			}
			
			public override void Load()
			{
				string manifestFileName = Get("");
				if (string.IsNullOrEmpty(manifestFileName)) {
					if (Helper.GetProperty("NoWin32Manifest", false, true)) {
						// no manifest
						applicationManifestComboBox.SelectedIndex = 1;
					} else {
						// default manifest
						applicationManifestComboBox.SelectedIndex = 0;
					}
				} else {
					applicationManifestComboBox.Text = manifestFileName;
				}
			}
			
			public override bool Save()
			{
				if (applicationManifestComboBox.SelectedIndex == 0) {
					// Embed default manifest
					Set("");
					Helper.SetProperty("NoWin32Manifest", "", true, this.Location);
				} else if (applicationManifestComboBox.SelectedIndex == 1) {
					// No manifest
					Set("");
					Helper.SetProperty("NoWin32Manifest", true, true, this.Location);
				} else {
					Set(applicationManifestComboBox.Text);
					Helper.SetProperty("NoWin32Manifest", "", true, this.Location);
				}
				return true;
			}
		}
		
		public static IList<IClass> GetPossibleStartupObjects(IProject project)
		{
			List<IClass> results = new List<IClass>();
			IProjectContent pc = ParserService.GetProjectContent(project);
			if (pc != null) {
				foreach (IClass c in pc.Classes) {
					foreach (IMethod m in c.Methods) {
						if (m.IsStatic && m.Name == "Main") {
							results.Add(c);
						}
					}
				}
			}
			return results;
		}
		
		void RefreshStartupObjectEnabled(object sender, EventArgs e)
		{
			bool isLibrary = OutputType.Library == (OutputType)Get<ComboBox>("outputType").SelectedIndex;
			ControlDictionary["startupObjectComboBox"].Enabled = !isLibrary;
		}
		
		void RefreshOutputNameTextBox(object sender, EventArgs e)
		{
			Get<TextBox>("outputName").Text = Get<TextBox>("assemblyName").Text + CompilableProject.GetExtension((OutputType)Get<ComboBox>("outputType").SelectedIndex);
		}
		
		void ApplicationIconTextBoxTextChanged(object sender, EventArgs e)
		{
			if(FileUtility.IsValidPath(Get<TextBox>("applicationIcon").Text))
			{
				string applicationIcon = Path.Combine(baseDirectory, Get<TextBox>("applicationIcon").Text);
				if (File.Exists(applicationIcon)) {
					try {
						Get<PictureBox>("applicationIcon").Image = Image.FromFile(applicationIcon);
					} catch (OutOfMemoryException) {
						Get<PictureBox>("applicationIcon").Image = null;
						MessageService.ShowErrorFormatted("${res:Dialog.ProjectOptions.ApplicationSettings.InvalidIconFile}", FileUtility.NormalizePath(applicationIcon));
					}
				} else {
					Get<PictureBox>("applicationIcon").Image = null;
				}
			}
		}
	}
}
