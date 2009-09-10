using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Provides methode to separate identifier in the directory path and the name part.
	/// </summary>
	public static class NameHelper
	{
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
				return string.Empty;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0)
				return string.Empty;
			return fullName.Substring(0, lastIndex + 1);
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
			return GetDirectoryPart(oldFullNameOrDir) + newName;
		}

		/// <summary>
		/// Gets the name portion of a full name. This is the last part of fullName which does not contain a DirectorySeparatorChar.
		/// If the last char is a DirectorySeparatorChar, an empty string will be returned.
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static string GetNamePart(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
				return string.Empty;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0)
				return fullName;
			else
				return fullName.Substring(lastIndex+1, fullName.Length-(lastIndex+1));
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
			directoryPart = string.Empty;
			namePart = string.Empty;

			if (string.IsNullOrEmpty(fullName))
				return;

			int lastIndex = fullName.LastIndexOf(DirectorySeparatorChar);
			if (lastIndex < 0) // no DirectorySeparatorChar
			{
				namePart = fullName;
			}
			else
			{
				namePart = fullName.Substring(lastIndex + 1, fullName.Length - (lastIndex + 1));
				directoryPart = fullName.Substring(0, lastIndex + 1);
			}
		}

	}
}
