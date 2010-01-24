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


        result = ProjectFolder.GetDirectoryPart(fullname);
        Assert.That(result == dirpart, ReportNameError("GetDirectoryPart",i));

				result = ProjectFolder.GetNamePart(fullname);
        Assert.That(result == namepart, ReportNameError("GetNamePart",i));

				ProjectFolder.SplitIntoDirectoryAndNamePart(fullname, out result, out result1);
        Assert.That(result == dirpart && result1 == namepart, ReportNameError("SplitIntoDirectoryAndNamePart",i));
      }
    }

		string ReportNameError(string function, int i)
		{
			return string.Format("{0} failed at i={1}, fullname=<<{2}>>", function, i, _testNames[i,0]);
		}

  }
}
