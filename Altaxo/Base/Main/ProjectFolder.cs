using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Immutable class that holds a project folder name. Note that the root folder name is associated with a null string, and
	/// that an empty string is a valid name for a project folder.
	/// </summary>
	public class ProjectFolder
	{
		/// <summary>
		/// Creates a project folder with the given name.
		/// </summary>
		/// <param name="name">Full name of the project folder. The value null is associated with the root folder.</param>
		public ProjectFolder(string name)
		{
			Name = name;
		}

		/// <summary>Gets the full folder name.</summary>
		public string Name { get; private set; }

		/// <summary>Returns true if the folder name is the root folder (i.e. is null).</summary>
		public bool IsRootFolder
		{
			get { return null == Name; }
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return null == Name ? 0 : Name.GetHashCode();
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

		private static ProjectFolder _rootFolder = new ProjectFolder(null);
		
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
		/// Gets the string that is associated with the root folder (here: null).
		/// </summary>
		public static string RootFolderName
		{
			get { return null; }
		}

		/// <summary>
		/// Determines if the given string is the name of the root folder.
		/// </summary>
		/// <param name="folder">Name to test.</param>
		/// <returns>True if the given name is the root folder name (here: if it is null).</returns>
		public static bool IsRootFolderName(string folder)
		{
			return folder == null;
		}



		#endregion

		#region Static Name functions

		/// <summary>
		/// Char used to separate directories of a name. 
		/// </summary>
		public static readonly char DirectorySeparatorChar = '\\';

		/// <summary>
		/// Gets the directory part of a full qualified name. If the provided fullName doesn't contain a DirectorySeparatorChar, i.e. is without path information, an empty string is returned.
		/// Otherwise, the directory string (with trailing DirectorySeparatorChar) is returned.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static string GetDirectoryPart(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
				return null;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0)
				return null;
			else
				return fullName.Substring(0, lastIndex);
		}

		/// <summary>
		/// Gets the name portion of a full name. This is the last part of fullName which does not contain a DirectorySeparatorChar.
		/// If the last char is a DirectorySeparatorChar, an empty string will be returned.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static string GetNamePart(string fullName)
		{
			if (null == fullName)
				return null;
			if (string.Empty == fullName)
				return string.Empty;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0)
				return fullName;
			else
				return fullName.Substring(lastIndex + 1);
		}

		/// <summary>
		/// Splits a fullName into directory path information and the name part information.
		/// </summary>
		/// <param name="fullName">The fullName to split.</param>
		/// <param name="directoryPart">On return, contains the directory part information (with trailing DirectorySeparatorChar).
		/// Returns an empty string if fullName does not contain path information.</param>
		/// <param name="namePart">On return, contains the name part information, i.e. the last part of the fullName, which does not contain a DirectorySeparatorChar.</param>
		public static void SplitIntoDirectoryAndNamePart(string fullName, out string directoryPart, out string namePart)
		{
			directoryPart = null;
			namePart = null;
			if (null == fullName)
				return;

			namePart = string.Empty;
			if (string.Empty == fullName)
				return;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0) // no DirectorySeparatorChar
			{
				directoryPart = null;
				namePart = fullName;
			}
			else
			{
				directoryPart = fullName.Substring(0, lastIndex);
				namePart = fullName.Substring(lastIndex + 1);
			}
		}

		/// <summary>
		/// Combines a folder name (without trailing <see cref="DirectorySeparatorChar"/>) with an item name,
		/// returning the full name of the item.
		/// </summary>
		/// <param name="directoryPart">Folder name (without trailing <see cref="DirectorySeparatorChar"/>).</param>
		/// <param name="namePart">Name part of the item (without any <see cref="DirectorySeparatorChar"/>)s.</param>
		/// <returns>The full name of the item. (directoryPart + <see cref="DirectorySeparatorChar"/> + namePart.</returns>
		public static string Combine(string directoryPart, string namePart)
		{
			if (null == directoryPart)
				return namePart;
			else
				return directoryPart + DirectorySeparatorChar + namePart;
		}

		/// <summary>
		/// Splits a directory name into the parent directory and the single directory name.
		/// </summary>
		/// <param name="fullName">The full name of the directory.</param>
		/// <param name="parentDir">Parent directory name.</param>
		/// <param name="currDir">Single directory name.</param>
		public static void SplitDirectory(string fullName, out string parentDir, out string currDir)
		{
			SplitIntoDirectoryAndNamePart(fullName, out parentDir, out currDir);
		}

		/// <summary>
		/// Gets the parent directory of the provided directory.
		/// </summary>
		/// <param name="dirName">The provided directory name.</param>
		/// <returns>The parent directory. If no parent directory exists, the function returns null.</returns>
		public static string GetParentDirectory(string dirName)
		{
			return GetDirectoryPart(dirName);
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
			return Combine(GetDirectoryPart(oldFullNameOrDir), newName);
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


	

		#endregion

	}

}
