using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Gui.Common.MultiRename;

namespace Altaxo.Main.Commands
{
	/// <summary>
	/// Supports actions to rename documents like worksheet, graph etc.
	/// </summary>
	public static class MultiRenameDocuments
	{
		/// <summary>
		/// Shows a dialog in which one can rename a list of documents (graphs, worksheets).
		/// </summary>
		/// <param name="objectsToRename">The documents to rename.</param>
		public static void ShowRenameDocumentsDialog(IEnumerable<object> objectsToRename)
		{
			var renameData = new Altaxo.Gui.Common.MultiRename.MultiRenameData();
			renameData.AddObjectsToRename(objectsToRename);

			renameData.RegisterListColumn("Full name", Main.Commands.MultiRenameDocuments.GetFullName);
			renameData.RegisterListColumn("New name", null);
			renameData.RegisterListColumn("Creation date", Main.Commands.MultiRenameDocuments.GetCreationDateString);

			Main.Commands.MultiRenameDocuments.RegisterCommonDocumentShortcuts(renameData);

			renameData.DefaultPatternString = "[N]";


			renameData.RegisterRenameActionHandler(RenameDocuments);

			var controller = new Altaxo.Gui.Common.MultiRename.MultiRenameController();
			controller.InitializeDocument(renameData);

			Current.Gui.ShowDialog(controller, "Rename items");
		}


		/// <summary>
		/// Renames the documents using the items and functions registered in <paramref name="renameData"/>.
		/// </summary>
		/// <param name="renameData">Data that provideds the items to rename, the functions etc.</param>
		/// <returns>If the action was successfull, an empty list. If the action was only partially successfull, the returned list contains those objects, for which the rename action failed.</returns>
		public static List<object> RenameDocuments(Altaxo.Gui.Common.MultiRename.MultiRenameData renameData)
		{
			List<object> renameFailedObjects = new List<object>();

			// first, give all items a new, random name
			for (int i = 0; i < renameData.ObjectsToRenameCount; ++i)
			{
				object o = renameData.GetObjectToRename(i);
				string newName = System.Guid.NewGuid().ToString();
				SetName(o, newName);
			}

			// now, try to set the new assigned name
			for (int i = 0; i < renameData.ObjectsToRenameCount; ++i)
			{
				object o = renameData.GetObjectToRename(i);
				string newName = renameData.GetNewNameForObject(i);
				try
				{
					SetName(o, newName);
				}
				catch (Exception)
				{
					renameFailedObjects.Add(o);
				}
			}

			return renameFailedObjects;
		}


		/// <summary>
		/// Register common shortcuts that are available for all documents (worksheets and graphs), like name, path, creation date etc.
		/// </summary>
		/// <param name="renameData">Rename data structure in which the shortcuts have to be registered.</param>
		public static void RegisterCommonDocumentShortcuts(MultiRenameData renameData)
		{
			renameData.RegisterStringShortcut("N", GetFullName, "Name of the object (full name, with path)");
			renameData.RegisterStringShortcut("SN", GetShortName, "Short name of the object (without path");
			renameData.RegisterStringShortcut("PN", GetFolderName, "Path name of the object");
			renameData.RegisterIntegerShortcut("C", GetCounter, "Index of the object in the list");

			renameData.RegisterDateTimeShortcut("CD", GetCreationDate, "Creation date of the object");

			renameData.RegisterStringArrayShortcut("NA", GetNamePartArray, "Name array, i.e. full name split into individual path pieces");
			renameData.RegisterStringArrayShortcut("PA", GetFolderPartArray, "Path array, i.e. path name of the object split into individual path pieces");
		}

		#region Sub-functions for shortcuts

		static int GetCounter(object o, int i)
		{
			return i;
		}

		/// <summary>
		/// Gets the short name of a document. See the class documentation for which type of documents can be used as argument.
		/// </summary>
		/// <param name="o">Document object.</param>
		/// <param name="i">This parameter is ignored.</param>
		/// <returns>Short name of the document.</returns>
		public static string GetShortName(object o, int i)
		{
			var name = GetFullName(o, i);
			return Altaxo.Main.ProjectFolder.GetNamePart(name);
		}

		/// <summary>
		/// Gets the folder name of a document (with trailing DirectorySeparatorChar). See the class documentation for which type of documents can be used as argument.
		/// </summary>
		/// <param name="o">Document object.</param>
		/// <param name="i">This parameter is ignored.</param>
		/// <returns>Folder name of the document.</returns>
		public static string GetFolderName(object o, int i)
		{
			var name = GetFullName(o, i);
			return Altaxo.Main.ProjectFolder.GetFolderPart(name);
		}

		static string[] GetNamePartArray(object o, int i)
		{
			var name = GetFullName(o, i);
			return name.Split(new char[] { Altaxo.Main.ProjectFolder.DirectorySeparatorChar });
		}

		static string[] GetFolderPartArray(object o, int i)
		{
			var name = GetFolderName(o, i);
			if (name.EndsWith("" + Altaxo.Main.ProjectFolder.DirectorySeparatorChar))
				name = name.Substring(0, name.Length - 1);
			return name.Split(new char[] { Altaxo.Main.ProjectFolder.DirectorySeparatorChar });
		}

		/// <summary>
		/// Gets the full name of a document. See the class documentation for which type of documents can be used as argument.
		/// </summary>
		/// <param name="o">Document object.</param>
		/// <param name="i">This parameter is ignored.</param>
		/// <returns>Full name of the document.</returns>
		public static string GetFullName(object o, int i) { return GetFullName(o); }

		/// <summary>
		/// Gets the full name of a document. See the class documentation for which type of documents can be used as argument.
		/// </summary>
		/// <param name="o">Document object.</param>
		/// <returns>Full name of the document.</returns>
		public static string GetFullName(object o)
		{
			if (o is Altaxo.Data.DataTable)
			{
				return ((Altaxo.Data.DataTable)o).Name;
			}
			else if (o is Altaxo.Graph.Gdi.GraphDocument)
			{
				return ((Altaxo.Graph.Gdi.GraphDocument)o).Name;
			}
			else
			{
				throw new ApplicationException("Unknown item type");
			}
		}

		static void SetName(object o, string newName)
		{
			if (o is Altaxo.Data.DataTable)
			{
				((Altaxo.Data.DataTable)o).Name = newName;
			}
			else if (o is Altaxo.Graph.Gdi.GraphDocument)
			{
				((Altaxo.Graph.Gdi.GraphDocument)o).Name = newName;
			}
			else
			{
				throw new ApplicationException("Unknown item type");
			}
		}

		static DateTime GetCreationDate(object o, int i)
		{
			return GetCreationDate(o).ToLocalTime();
		}

		/// <summary>
		/// Gets the creation date of a document. See the class documentation for which type of documents can be used as argument.
		/// </summary>
		/// <param name="o">Document object.</param>
		/// <returns>Creation date of the document.</returns>
		public static string GetCreationDateString(object o)
		{
			return GetCreationDate(o).ToLocalTime().ToString();
		}

		static DateTime GetCreationDate(object o)
		{
			if (o is Altaxo.Data.DataTable)
			{
				return ((Altaxo.Data.DataTable)o).CreationTimeUtc;
			}
			else if (o is Altaxo.Graph.Gdi.GraphDocument)
			{
				return ((Altaxo.Graph.Gdi.GraphDocument)o).CreationTimeUtc;
			}
			else
			{
				throw new ApplicationException("Unknown item type");
			}
		}

		#endregion

	}
}
