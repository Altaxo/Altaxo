// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Linq;
using System.Collections.Generic;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

using System.Collections.Generic;

using System.Windows.Shell;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// </summary>
	public class RecentOpen : IRecentOpen
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		private int MAX_LENGTH = 10;

		private IList<FileName> recentFiles = new List<FileName>();
		private IList<FileName> recentProjects = new List<FileName>();

		public static readonly Altaxo.Main.Properties.PropertyKey<IList<FileName>> PropertyKeyRecentFiles =
			new Altaxo.Main.Properties.PropertyKey<IList<FileName>>(
				"E89CEABA-079C-4523-A594-CCDB67023BA7",
				"App\\RecentFiles",
				Altaxo.Main.Properties.PropertyLevel.Application);

		public static readonly Altaxo.Main.Properties.PropertyKey<IList<FileName>> PropertyKeyRecentProjects =
		new Altaxo.Main.Properties.PropertyKey<IList<FileName>>(
			"0C3B80BC-BE6B-4270-B59A-AAF5BA9CF00C",
			"App\\RecentProjects",
			Altaxo.Main.Properties.PropertyLevel.Application);

		#region Serialization

		/// <summary>
		///
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(List<FileName>), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (List<FileName>)obj;

				var count = s.Count;

				info.CreateArray("Properties", count);
				foreach (var entry in s)
				{
					info.AddValue("e", (string)entry);
				}
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = o as List<FileName> ?? new List<FileName>();
				s.Clear();
				int count = info.OpenArray("Properties");

				for (int i = 0; i < count; ++i)
				{
					s.Add(FileName.Create(info.GetString("e")));
				}
				info.CloseArray(count);
				return s;
			}
		}

		#endregion Serialization

		public IReadOnlyList<FileName> RecentFiles
		{
			get { return recentFiles.AsReadOnly(); }
		}

		public IReadOnlyList<FileName> RecentProjects
		{
			get { return recentProjects.AsReadOnly(); }
		}

		public RecentOpen()
		{
			recentProjects = Current.PropertyService.GetValue(PropertyKeyRecentProjects, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>());
			recentFiles = Current.PropertyService.GetValue(PropertyKeyRecentFiles, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>());
		}

		public void AddRecentFile(FileName name)
		{
			recentFiles.Remove(name); // remove if the filename is already in the list

			while (recentFiles.Count >= MAX_LENGTH)
			{
				recentFiles.RemoveAt(recentFiles.Count - 1);
			}

			recentFiles.Insert(0, name);

			Current.PropertyService.SetValue(PropertyKeyRecentFiles, recentFiles);
		}

		public void ClearRecentFiles()
		{
			recentFiles.Clear();
			Current.PropertyService.SetValue(PropertyKeyRecentFiles, recentFiles);
		}

		public void ClearRecentProjects()
		{
			recentProjects.Clear();
			Current.PropertyService.SetValue(PropertyKeyRecentProjects, recentProjects);
		}

		public void RemoveRecentProject(FileName name)
		{
			recentProjects.Remove(name);
			Current.PropertyService.SetValue(PropertyKeyRecentProjects, recentProjects);
		}

		public void AddRecentProject(FileName name)
		{
			recentProjects.Remove(name);

			while (recentProjects.Count >= MAX_LENGTH)
			{
				recentProjects.RemoveAt(recentProjects.Count - 1);
			}

			recentProjects.Insert(0, name);
			JumpList.AddToRecentCategory(name);

			Current.PropertyService.SetValue(PropertyKeyRecentProjects, recentProjects);
		}

		internal void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < recentFiles.Count; ++i)
			{
				string file = recentFiles[i].ToString();
				if (e.FileName == file)
				{
					recentFiles.RemoveAt(i);
					break;
				}
			}
		}

		internal void FileRenamed(object sender, FileRenameEventArgs e)
		{
			for (int i = 0; i < recentFiles.Count; ++i)
			{
				string file = recentFiles[i].ToString();
				if (e.SourceFile == file)
				{
					recentFiles.RemoveAt(i);
					recentFiles.Insert(i, FileName.Create(e.TargetFile));
					break;
				}
			}
		}
	}
}