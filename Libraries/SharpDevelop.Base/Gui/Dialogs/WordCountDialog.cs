// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class WordCountDialog : BaseSharpDevelopForm
	{
		ArrayList items;
		Report total;
		
		internal class Report
		{
			public string name;
			public long chars;
			public long words;
			public long lines;
			
			public Report(string name, long chars, long words, long lines)
			{
				this.name  = name;
				this.chars = chars;
				this.words = words;
				this.lines = lines;
			}
			
			public ListViewItem ToListItem()
			{
				return new ListViewItem(new string[] {Path.GetFileName(name), chars.ToString(), words.ToString(), lines.ToString()});
			}
			
			public static Report operator+(Report r, Report s)
			{
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				Report tmp = new Report(resourceService.GetString("Dialog.WordCountDialog.TotalText"), s.chars, s.words, s.lines);
				tmp.chars += r.chars;
				tmp.words += r.words;
				tmp.lines += r.lines;
				return tmp;
			}
		}
		
		Report GetReport(string filename)
		{
			long numLines = 0;
			long numWords = 0;
			long numChars = 0;
			
			if (!File.Exists(filename)) return null;
			
			FileStream istream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(istream);
			string line = sr.ReadLine();
			while (line != null) {
				++numLines;
				numChars += line.Length;
				string[] words = line.Split(null);
				numWords += words.Length;
				line = sr.ReadLine();
			}
			
			sr.Close();
			return new Report(filename, numChars, numWords, numLines);
		}
		
		void startEvent(object sender, System.EventArgs e)
		{
			items = new ArrayList();
			total = null;
			
			switch (((ComboBox)ControlDictionary["locationComboBox"]).SelectedIndex) {
				case 0: {// current file
					IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					if (window != null) {
						if (window.ViewContent.ContentName == null) {
							MessageService.ShowWarning("${res:Dialog.WordCountDialog.SaveTheFileWarning}");
						} else {
							Report r = GetReport(window.ViewContent.ContentName);
							if (r != null) items.Add(r);
							// ((ListView)ControlDictionary["resultListView"]).Items.Add(r.ToListItem());
						}
					}
					break;
				}
				case 1: {// all open files
				if (WorkbenchSingleton.Workbench.ViewContentCollection.Count > 0) {
					bool dirty = false;
					
					total = new Report(StringParserService.Parse("${res:Dialog.WordCountDialog.TotalText}"), 0, 0, 0);
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.ContentName == null) {
							MessageService.ShowWarning("${res:Dialog.WordCountDialog.SaveAllFileWarning}");
							continue;
						} else {
							Report r = GetReport(content.ContentName);
							if (r != null) {
								if (content.IsDirty) dirty = true;
								total += r;
								items.Add(r);
								// ((ListView)ControlDictionary["resultListView"]).Items.Add(r.ToListItem());
							}
						}
					}
					
					if (dirty) {
						MessageService.ShowWarning("${res:Dialog.WordCountDialog.DirtyWarning}");
					}
					
					// ((ListView)ControlDictionary["resultListView"]).Items.Add(new ListViewItem(""));
					// ((ListView)ControlDictionary["resultListView"]).Items.Add(all.ToListItem());
				}
				break;
				}
				case 2: {// whole project
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					
					if (projectService.CurrentOpenCombine == null) {
						MessageService.ShowError("${res:Dialog.WordCountDialog.MustBeInProtectedModeWarning}");
						break;
					}
					total = new Report(StringParserService.Parse("${res:Dialog.WordCountDialog.TotalText}"), 0, 0, 0);
					CountCombine(projectService.CurrentOpenCombine, ref total);
					// ((ListView)ControlDictionary["resultListView"]).Items.Add(new ListViewItem(""));
					// ((ListView)ControlDictionary["resultListView"]).Items.Add(all.ToListItem());
					break;
				}
			}
			UpdateList(0);
		}
		
		void CountCombine(Combine combine, ref Report all)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry.Entry is IProject) {
					// string tmp = "";
					foreach (ProjectFile finfo in ((IProject)entry.Entry).ProjectFiles) {
						if (finfo.Subtype != Subtype.Directory && 
						    finfo.BuildAction == BuildAction.Compile) {
							Report r = GetReport(finfo.Name);
							all += r;
							items.Add(r);
							// ((ListView)ControlDictionary["resultListView"]).Items.Add(r.ToListItem());
						}
					}
				} else
					CountCombine((Combine)entry.Entry, ref all);
			}
		}
		
		void UpdateList(int SortKey)
		{
			if (items == null) {
				return;
			}
			((ListView)ControlDictionary["resultListView"]).BeginUpdate();
			((ListView)ControlDictionary["resultListView"]).Items.Clear();
			
			if (items.Count == 0) {
				goto endupdate;
			}
			
			ReportComparer rc = new ReportComparer(SortKey);
			items.Sort(rc);
			
			for (int i = 0; i < items.Count; ++i) {
				((ListView)ControlDictionary["resultListView"]).Items.Add(((Report)items[i]).ToListItem());
			}
			
			if (total != null) {
				((ListView)ControlDictionary["resultListView"]).Items.Add(new ListViewItem(""));
				((ListView)ControlDictionary["resultListView"]).Items.Add(total.ToListItem());
			}
			
		endupdate:
			((ListView)ControlDictionary["resultListView"]).EndUpdate();
			
		}		
		
		internal class ReportComparer : IComparer
		{
			int sortKey;
		
			public ReportComparer(int SortKey)
			{
				sortKey = SortKey;
			}
			
			public int Compare(object x, object y)
			{
				Report xr = x as Report;
				Report yr = y as Report;
				
				if (x == null || y == null) return 1;
				
				switch (sortKey) {
					case 0:  // files
						return String.Compare(xr.name, yr.name);
					case 1:  // chars
						return xr.chars.CompareTo(yr.chars);
					case 2:  // words
						return xr.words.CompareTo(yr.words);
					case 3:  // lines
						return xr.lines.CompareTo(yr.lines);
					default:
						return 1;
				}
			}
		}
		
		void SortEvt(object sender, ColumnClickEventArgs e)
		{
			UpdateList(e.Column);
		}
		
		public WordCountDialog()
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\WordCountDialog.xfrm"));
			((Button)ControlDictionary["startButton"]).Click += new System.EventHandler(startEvent);
			((ListView)ControlDictionary["resultListView"]).ColumnClick += new ColumnClickEventHandler(SortEvt);
			
			Icon  = IconService.GetIcon("Icons.16x16.FindIcon");
			Owner = (Form)WorkbenchSingleton.Workbench;
			
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParserService.Parse("${res:Global.Location.currentfile}"));
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParserService.Parse("${res:Global.Location.allopenfiles}"));
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParserService.Parse("${res:Global.Location.wholeproject}"));
			((ComboBox)ControlDictionary["locationComboBox"]).SelectedIndex = 0;
		}
	}
}
