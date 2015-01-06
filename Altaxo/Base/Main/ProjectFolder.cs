#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Immutable class that holds a project folder name. Note that the root folder name is an empty string, and
	/// that all other folders have to end with an <see cref="DirectorySeparatorChar"/>. This rule also holds when splitting a full folder name into
	/// parts: the parts also have to end with a <see cref="DirectorySeparatorChar"/> or have to be the empty string.
	/// </summary>
	public class ProjectFolder : INamedObject
	{
		/// <summary>
		/// Creates a project folder with the given name.
		/// </summary>
		/// <param name="name">Full name of the project folder. The value null is associated with the root folder.</param>
		public ProjectFolder(string name)
		{
			ThrowExceptionOnInvalidFullFolderPath(name);
			Name = name;
		}

		/// <summary>Gets the full folder name. The root folder is represented by an empty string. All other returned names end with an <see cref="DirectorySeparatorChar"/>.</summary>
		public string Name { get; private set; }

		/// <summary>Returns true if the folder name is the root folder (empty string).</summary>
		public bool IsRootFolder
		{
			get { return RootFolderName == Name; }
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var a = obj as ProjectFolder;
			if (null != a)
				return a.Name == this.Name;

			var b = obj as string;
			if (null != b)
				return b == this.Name;

			return false;
		}

		#region Static ProjectFolder

		private static ProjectFolder _rootFolder = new ProjectFolder(RootFolderName);

		/// <summary>
		/// Returns the root folder instance.
		/// </summary>
		public static ProjectFolder RootFolder
		{
			get
			{
				return _rootFolder;
			}
		}

		/// <summary>
		/// Gets the string that is associated with the root folder (here: an empty string).
		/// </summary>
		public static string RootFolderName
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Determines if the given string is the name of the root folder.
		/// </summary>
		/// <param name="folder">Name to test.</param>
		/// <returns>True if the given name is the root folder name (i.e. is an empty string).</returns>
		public static bool IsRootFolderName(string folder)
		{
			return folder == string.Empty;
		}

		#endregion Static ProjectFolder

		#region Static Name functions

		/// <summary>Char used to separate project subfolders.</summary>
		public const char DirectorySeparatorChar = '\\';

		/// <summary>Char used to separate project subfolders, here as a string for convenience..</summary>
		public const string DirectorySeparatorString = "\\";

		/// <summary>
		/// Test for the validity of a single folder name. The folderName either has to be an empty string  (representing the root folder) or a string ending with a directory separator char and containing no other directory separator chars.
		/// If the string is null, an <see cref="ArgumentNullException"/> is thrown. Otherwise, if the provided string is not a valid folder name, a <see cref="ArgumentException"/> will be thrown.
		/// </summary>
		/// <param name="folderName">Folder name to test.</param>
		public static void ThrowExceptionOnInvalidSingleFolderName(string folderName)
		{
			if (null == folderName)
			{
				throw new ArgumentNullException("folderName is null");
			}
			if (folderName.Length > 0)
			{
				int idx = folderName.IndexOf(DirectorySeparatorChar);
				if (idx + 1 != folderName.Length)
					throw new ArgumentException(string.Format("folderName has to end with a directory separator char and must not contain any other directory separator char! The provided folderName is: {0}", folderName));
			}
		}

		/// <summary>
		/// Test for the validity of a folder name of any level. The argument folderPath either has to be an empty string (representing the root folder) or a string ending with a directory separator char.
		/// If the argument is null, an <see cref="ArgumentNullException"/> is thrown. Otherwise, if the provided string is not a valid folder path, a <see cref="ArgumentException"/> will be thrown.
		/// </summary>
		/// <param name="folderPath">Folder path to test.</param>
		public static void ThrowExceptionOnInvalidFullFolderPath(string folderPath)
		{
			if (null == folderPath)
				throw new ArgumentNullException("folderPath is null");
			if (folderPath.Length > 0 && folderPath[folderPath.Length - 1] != DirectorySeparatorChar)
				throw new ArgumentException(string.Format("folderPath has to end with a directory separator char! The provided folderPath is: {0}", folderPath));
		}

		/// <summary>
		/// Gets the directory part of a full qualified name. Can either be the name of an item (worksheet, graph) or a full folder name.
		/// If the name of a item is provided, the item's directory name is returned.
		/// If a full folder path is provided  (i.e. either an empty string or a string ending with a <see cref="DirectorySeparatorChar"/>), the unchanged argument is returned.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static string GetFolderPart(string fullName)
		{
			if (null == fullName)
				throw new ArgumentNullException("fullName");

			if (IsRootFolderName(fullName))
				return RootFolderName;

			if (fullName[fullName.Length - 1] == DirectorySeparatorChar)
				return fullName;
			else
			{
				int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
				if (lastIndex < 0)
					return RootFolderName;
				else
					return fullName.Substring(0, lastIndex + 1);
			}
		}

		/// <summary>
		/// Intended for scripting purposes only!
		/// Gets the directory part of a full qualified name, but without the trailing DirectorySeparatorChar.
		/// No exception is thrown if the directory part is the root folder, thus the names '\Alice' and 'Alice' both return an empty string.
		/// The full qualified name can either be the name of an item (worksheet, graph) or a full folder name.
		/// If the name of a item is provided, the item's directory name (without trailing DirectorySeparatorChar) is returned.
		/// If a full folder path is provided  (i.e. either an empty string or a string ending with a <see cref="DirectorySeparatorChar"/>), the name of the parent folder (without trailing DirectorySeparatorChar)  is returned.
		/// </summary>
		/// <param name="fullName">The full qualified name of (either an item's name or a full folder name).</param>
		/// <returns>Name of the parent folder, but without trailing DirectorySeparatorChar.</returns>
		public static string GetFolderPartWithoutTrailingDirectorySeparatorChar(string fullName)
		{
			string result = GetFolderPart(fullName);
			if (IsRootFolderName(result))
				return result;
			else
				return result.Substring(0, result.Length - 1);
		}

		/// <summary>
		/// Gets the name portion of a full name.  Can either be the name of an item (worksheet, graph) or a full folder name.
		/// If the name of a item (worksheet or graph) is provided, the item's name without folder name is returned (thus containing no <see cref="DirectorySeparatorChar"/>)directory name is returned.
		/// If a full folder path is provided (i.e. either an empty string or a string ending with a <see cref="DirectorySeparatorChar"/>), the name of the last folder of the full folder path (!) is returned.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static string GetNamePart(string fullName)
		{
			if (null == fullName)
				throw new ArgumentNullException("fullName");
			if (IsRootFolderName(fullName))
				return string.Empty;

			if (fullName[fullName.Length - 1] == DirectorySeparatorChar)
				return string.Empty; // name part of a full name that end with directory separator char is an empty string
			else
			{
				int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
				if (lastIndex < 0)
					return fullName;
				else
					return fullName.Substring(lastIndex + 1);
			}
		}

		/// <summary>
		/// Splits into directory and name portion of a full name. The full name can either be the name of an item (worksheet, graph) or a full folder name.
		/// If the name of a item (worksheet or graph) is provided, it is splitted into the item's folder name and the item's name.
		/// If a full folder path is provided (i.e. either an empty string or a string ending with a <see cref="DirectorySeparatorChar"/>), it is splitted into the name of the parent folder and the name of the last folder.
		/// </summary>
		/// <param name="fullName">The fullName to split.</param>
		/// <param name="directoryPart">On return, contains the directory part information (either an empty string for the root folder or a folder name with trailing <see cref="DirectorySeparatorChar"/>.</param>
		/// <param name="namePart">On return, contains the name part information, i.e. the last part of the fullName.
		/// If the provided fullName was the name of an item, the name portion doesn't contain any <see cref="DirectorySeparatorChar"/>.
		/// if the provided fullName was the name of a project folder, the name portion is either a empty string (the root folder name), or has a trailing <see cref="DirectorySeparatorChar"/>, but no other <see cref="DirectorySeparatorChar"/>s.
		/// </param>
		public static void SplitIntoFolderAndNamePart(string fullName, out string directoryPart, out string namePart)
		{
			if (null == fullName)
				throw new ArgumentNullException("fullName");

			if (IsRootFolderName(fullName) || fullName[fullName.Length - 1] == DirectorySeparatorChar)
			{
				directoryPart = fullName;
				namePart = string.Empty;
			}
			else
			{
				int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);

				if (lastIndex < 0) // no DirectorySeparatorChar
				{
					directoryPart = RootFolderName;
					namePart = fullName;
				}
				else
				{
					directoryPart = fullName.Substring(0, lastIndex + 1);
					namePart = fullName.Substring(lastIndex + 1);
				}
			}
		}

		/// <summary>
		/// Combines a folder name (with trailing <see cref="DirectorySeparatorChar"/>) with an item name,
		/// returning the full name of the item. If the trailing  <see cref="DirectorySeparatorChar"/> is missed in the directoryPart,
		/// it is automatically appended to the directoryPart.
		/// </summary>
		/// <param name="directoryPart">Folder name (normally with trailing <see cref="DirectorySeparatorChar"/>, but here it is tolerated also without trailing <see cref="DirectorySeparatorChar"/>).</param>
		/// <param name="namePart">Name part of the item (without any <see cref="DirectorySeparatorChar"/>)s.</param>
		/// <returns>The full name of the item. (directoryPart + <see cref="DirectorySeparatorChar"/> + namePart.</returns>
		public static string Combine(string directoryPart, string namePart)
		{
			if (null != directoryPart && directoryPart.Length > 0 && directoryPart[directoryPart.Length - 1] != DirectorySeparatorChar)
				directoryPart += DirectorySeparatorChar;

			return (directoryPart ?? string.Empty) + namePart;
		}

		/// <summary>
		/// Splits a full folder name name into the parent directory and the single directory name.
		/// </summary>
		/// <param name="fullName">The full name of the directory.</param>
		/// <param name="parentDir">Parent directory name.</param>
		/// <param name="currDir">Single directory name.</param>
		public static void SplitFolderIntoParentFolderAndLastFolderPart(string fullName, out string parentDir, out string currDir)
		{
			parentDir = GetFoldersParentFolder(fullName);
			currDir = fullName.Substring(parentDir.Length);
		}

		/// <summary>
		/// Returns the name of the last folder from a folder path of arbitrary depth.
		/// </summary>
		/// <param name="fullName">Folder path of arbitrary depth. Must be a valid full folder path.</param>
		/// <returns>Last folder name.If the provided folder path was the root folder, the returned value is the <see cref="RootFolderName"/>.
		/// Else the returned value is the last part of the string, ending with the <see cref="DirectorySeparatorChar"/>, but containing no other <see cref="DirectorySeparatorChar"/>s.</returns>
		/// <example>Given the full folder name "foo\bar\", the string "bar\" is returned.</example>
		public static string GetFoldersLastFolderPart(string fullName)
		{
			var parentDir = GetFoldersParentFolder(fullName);
			return fullName.Substring(parentDir.Length);
		}

		/// <summary>
		/// Gets the parent directory of the provided directory.
		/// </summary>
		/// <param name="dirName">The provided directory name.</param>
		/// <returns>The parent directory. If no parent directory exists, the function returns null.</returns>
		public static string GetFoldersParentFolder(string dirName)
		{
			ThrowExceptionOnInvalidFullFolderPath(dirName);
			if (dirName == RootFolderName)
				throw new InvalidOperationException("Can not get the parent directory of the root folder");

			int lastIndex = dirName.Length < 2 ? -1 : dirName.LastIndexOf(DirectorySeparatorChar, dirName.Length - 2);

			if (lastIndex < 0)
				return RootFolderName;
			else
				return dirName.Substring(0, lastIndex + 1);
		}

		/// <summary>
		/// Creates a new name starting from a oldfullNameOrDir and a newName. Retrieves the directory information
		/// from the oldFullNameOrDir and combines it with the newName to form a new full name.
		/// </summary>
		/// <param name="oldFullNameOrDir"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		public static string CreateFullName(string oldFullNameOrDir, string newName)
		{
			return Combine(GetFolderPart(oldFullNameOrDir), newName);
		}

		/// <summary>
		/// Prepends a string to a full name (by prepending to the name part only).
		/// </summary>
		/// <param name="oldFullName">The full original name.</param>
		/// <param name="prependString">A string that is to be prepended to the name part of the original name.</param>
		/// <returns></returns>
		public static string PrependToName(string oldFullName, string prependString)
		{
			return CreateFullName(oldFullName, prependString + GetNamePart(oldFullName));
		}

		/// <summary>
		/// Appends a string to a full name (by appending to the name part only).
		/// </summary>
		/// <param name="oldFullName">The full original name.</param>
		/// <param name="prependString">A string that is to be prepended to the name part of the original name.</param>
		/// <returns></returns>
		public static string AppendToName(string oldFullName, string prependString)
		{
			return CreateFullName(oldFullName, GetNamePart(oldFullName) + prependString);
		}

		#region Gui Helpers

		/// <summary>
		/// Converts a folder name (i.e. either an empty string or a string with a trailing <see cref="DirectorySeparatorChar"/>) to a
		/// name which can be used to display on the display.
		/// </summary>
		/// <param name="folderName">The name of the folder.</param>
		/// <returns>A name that can be displayed. The root folder name is converted to "\", and all other folder names are stripped off the trailing <see cref="DirectorySeparatorChar"/>.</returns>
		public static string ConvertFolderNameToDisplayFolderName(string folderName)
		{
			ThrowExceptionOnInvalidFullFolderPath(folderName);
			if (folderName.Length > 0)
				return folderName.Substring(0, folderName.Length - 1);
			else
				return "" + DirectorySeparatorChar;
		}

		/// <summary>
		/// Converts a folder name (i.e. either an empty string or a string with a trailing <see cref="DirectorySeparatorChar"/>) to a
		/// name which can be used to display. Only the last part of the folder name is returned here.
		/// </summary>
		/// <param name="folderName">The name of the folder.</param>
		/// <returns>The last part of a full folder name name that can be displayed. The root folder name is converted an empty string, and all other folder names are stripped off the trailing <see cref="DirectorySeparatorChar"/>.</returns>
		public static string ConvertFolderNameToDisplayFolderLastPart(string folderName)
		{
			var name = ProjectFolder.GetFoldersLastFolderPart(folderName);
			if (name.Length > 0 && name[name.Length - 1] == ProjectFolder.DirectorySeparatorChar)
				name = name.Substring(0, name.Length - 1);
			return name;
		}

		/// <summary>
		/// Converts a string that was used by the Gui or entered by the user (and contains no trailing <see cref="DirectorySeparatorChar"/>) to a
		/// valid folder name.
		/// </summary>
		/// <param name="displayFolderName">String used by the Gui or entered by the user.</param>
		/// <returns>Valid folder name, i.e. either an empty string (root folder name) or a string with a trailing <see cref="DirectorySeparatorChar"/>.</returns>
		public static string ConvertDisplayFolderNameToFolderName(string displayFolderName)
		{
			if (string.IsNullOrEmpty(displayFolderName))
				return RootFolderName;
			else
				return displayFolderName + DirectorySeparatorChar;
		}

		#endregion Gui Helpers

		#region other helpers

		/// <summary>Determines whether all names in argument <paramref name="itemNames"/> are from a single project folder.</summary>
		/// <param name="itemNames">Enumeration of names (usually of project items).</param>
		/// <param name="folderName">On return, if all names belong to one folder, the name of this folder is returned (with trailing directory separator char). Otherwise, an empty string is returned.</param>
		/// <returns><c>True</c> if all names belong to one project folder; otherwise <c>False</c>.</returns>
		public static bool AreAllNamesFromOneFolder(IEnumerable<string> itemNames, out string folderName)
		{
			var folderHash = new HashSet<string>();

			foreach (var name in itemNames)
			{
				folderHash.Add(GetFolderPart(name));
			}

			if (folderHash.Count == 1)
			{
				folderName = folderHash.First();
				return true;
			}
			else
			{
				folderName = string.Empty;
				return false;
			}
		}

		/// <summary>
		/// Gets the common folder of the provided item names.
		/// </summary>
		/// <param name="itemNames">The item names.</param>
		/// <returns>The common folder of the items. This it at least the root folder (see <see cref="RootFolderName"/>).</returns>
		/// <exception cref="System.ArgumentException">itemNames enumeration was empty</exception>
		public static string GetCommonFolderOfNames(IEnumerable<string> itemNames)
		{
			if (null == itemNames)
				throw new ArgumentNullException("itemNames");

			IList<string> nameList;

			if (itemNames is IList<string>)
				nameList = (IList<string>)itemNames;
			else
				nameList = new List<string>(itemNames);

			if (nameList.Count == 0)
				throw new ArgumentException("itemNames enumeration was empty");

			var firstNamesFolder = GetFolderPart(nameList[0]);

			while (firstNamesFolder.Length > 0)
			{
				int i;
				for (i = nameList.Count - 1; i >= 1; --i)
				{
					if (!(nameList[i].StartsWith(firstNamesFolder)))
						break;
				}
				if (i == 0)
					break;

				firstNamesFolder = GetFoldersParentFolder(firstNamesFolder);
			}

			return firstNamesFolder;
		}

		#endregion other helpers

		#endregion Static Name functions
	}
}