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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	[TestFixture]
	public class ProjectFolder_Test
	{
		// 1st: fullName, 2nd: directoryPart, 3rd: name part
		private static string[,] _testNames = new string[,]
     {
      {"", "",	""}, // RootFolder should split in root folder and empty short name
      {"A", "",	"A"},
      {"ABCD","","ABCD"},
      {@"AB\CD",	@"AB\","CD"},
      {@"AB\CD\EF",@"AB\CD\","EF"},
      {@"\AB",@"\","AB"},
      {@"AB\",@"AB\",""},
      {@"\AB\",@"\AB\",""},
      {@"\AB\CD\EF",@"\AB\CD\","EF"},
      {@"\AB\CD\EF\",@"\AB\CD\EF\",""},
      {@"\\",@"\\",""},
      {@"\",@"\",""}
     };

		[Test]
		public void TestSplitNames()
		{
			string result, result1;

			int len = _testNames.GetLength(0);

			for (int i = 0; i < len; i++)
			{
				string fullname = _testNames[i, 0];
				string dirpart = _testNames[i, 1];
				string namepart = _testNames[i, 2];

				result = ProjectFolder.GetFolderPart(fullname);
				Assert.That(result == dirpart, ReportNameError("GetDirectoryPart", i));

				result = ProjectFolder.GetNamePart(fullname);
				Assert.That(result == namepart, ReportNameError("GetNamePart", i));

				ProjectFolder.SplitIntoFolderAndNamePart(fullname, out result, out result1);
				Assert.That(result == dirpart && result1 == namepart, ReportNameError("SplitIntoFolderAndNamePart", i));

				result = ProjectFolder.Combine(result, result1);
				Assert.That(result == fullname, ReportNameError("Combine", i));
			}
		}

		private string ReportNameError(string function, int i)
		{
			return string.Format("{0} failed at i={1}, fullname=<<{2}>>", function, i, _testNames[i, 0]);
		}

		private string ReportNameError(string function, string[,] field, int i)
		{
			return string.Format("{0} failed at i={1}, fullname=<<{2}>>", function, i, field[i, 0]);
		}

		private static string[,] _testParentFolderData1 = new string[,]
     {
      {@"A\B\",@"A\"},
      {@"AB\",@""},
      {@"\AB\",@"\"},
      {@"\AB\CD\EF\",@"\AB\CD\"},
      {@"\\",@"\"},
      {@"\",@""}
		 };

		[Test]
		public void TestFoldersParentFolder()
		{
			string result;

			int len = _testParentFolderData1.GetLength(0);

			for (int i = 0; i < len; i++)
			{
				string folderName = _testParentFolderData1[i, 0];
				string parentFolderName = _testParentFolderData1[i, 1];

				result = ProjectFolder.GetFoldersParentFolder(folderName);
				Assert.That(result == parentFolderName, ReportNameError("GetFoldersParentFolder", _testParentFolderData1, i));
			}
		}
	}
}