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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace Altaxo.Main
{
  [TestFixture]
  public class ProjectFolder_Test
  {
    static string[,] _testNames = new string[,]
     {
      {"",null,""},
      {"A",null,"A"},
      {"ABCD",null,"ABCD"},
      {@"AB\CD","AB","CD"},
      {@"AB\CD\EF",@"AB\CD","EF"},
      {@"\AB","","AB"},
      {@"AB\","AB",""},
      {@"\AB\",@"\AB",""},
      {@"\AB\CD\EF",@"\AB\CD","EF"},
      {@"\AB\CD\EF\",@"\AB\CD\EF",""},
      {@"\\",@"\",""}
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
        Assert.That(result == dirpart, ReportNameError("GetDirectoryPart",i));

				result = ProjectFolder.GetNamePart(fullname);
        Assert.That(result == namepart, ReportNameError("GetNamePart",i));

				ProjectFolder.SplitIntoFolderAndNamePart(fullname, out result, out result1);
        Assert.That(result == dirpart && result1 == namepart, ReportNameError("SplitIntoDirectoryAndNamePart",i));
      }
    }

		string ReportNameError(string function, int i)
		{
			return string.Format("{0} failed at i={1}, fullname=<<{2}>>", function, i, _testNames[i,0]);
		}

  }
}
