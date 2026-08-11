#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Linq;
using Xunit;

namespace Altaxo.Serialization.NamePropertyExtraction
{
  public class BasicTypes_Tests
  {
    [Fact]
    public void TestFileNameSplitter_DriveLetter()
    {
      var path = @"F:\Foo\Bar\Baz\Qux.txt";
      var testObj = new PathToFileNameSplitter();
      var result = testObj.Split(path);
      Assert.Single(result);
      Assert.Equal("Qux", result[0]);
    }

    [Fact]
    public void TestFileNameSplitter_UNCPath()
    {
      var path = @"\\Server\Share\Foo\Bar\Baz\Qux.txt";
      var testObj = new PathToFileNameSplitter();
      var result = testObj.Split(path);
      Assert.Single(result);
      Assert.Equal("Qux", result[0]);
    }

    [Fact]
    public void TestPathSplitter_DriveLetter()
    {
      var path = @"F:\Foo\Bar\Baz\Qux.txt";
      var testObj = new PathSplitter();
      var result = testObj.Split(path);
      Assert.Equal(6, result.Count);
      Assert.Equal("F:\\", result[0]);
      Assert.Equal("Foo", result[1]);
      Assert.Equal("Bar", result[2]);
      Assert.Equal("Baz", result[3]);
      Assert.Equal("Qux", result[4]);
      Assert.Equal(".txt", result[5]);
    }
    [Fact]
    public void TestPathSplitter_UNCPath()
    {
      var path = @"\\Server\Share\Foo\Bar\Baz\Qux.txt";
      var testObj = new PathSplitter();
      var result = testObj.Split(path);
      Assert.Equal(6, result.Count);
      Assert.Equal("\\\\Server\\Share", result[0]);
      Assert.Equal("Foo", result[1]);
      Assert.Equal("Bar", result[2]);
      Assert.Equal("Baz", result[3]);
      Assert.Equal("Qux", result[4]);
      Assert.Equal(".txt", result[5]);
    }

    [Fact]
    public void TestNameSplitterBySeparatorStrings()
    {
      var path = @"F:\Foo\Bar\Baz\Qux.txt";
      var testObj = new NameSplitterBySeparatorStrings
      {
        Separators = ["\\"],
        RemoveEmptyEntries = false
      };
      var result = testObj.Split(path);
      Assert.Equal(5, result.Count);
      Assert.Equal("F:", result[0]);
      Assert.Equal("Foo", result[1]);
      Assert.Equal("Bar", result[2]);
      Assert.Equal("Baz", result[3]);
      Assert.Equal("Qux.txt", result[4]);
    }

    [Fact]
    public void TestNameSplitterBySeparatorStrings_2Strings()
    {
      var path = @"F:\Foo\Bar\Baz\Qux.txt";
      var testObj = new NameSplitterBySeparatorStrings
      {
        Separators = ["\\", ":"],
        RemoveEmptyEntries = false
      };
      var result = testObj.Split(path);
      Assert.Equal(6, result.Count);
      Assert.Equal("F", result[0]);
      Assert.Equal("", result[1]);
      Assert.Equal("Foo", result[2]);
      Assert.Equal("Bar", result[3]);
      Assert.Equal("Baz", result[4]);
      Assert.Equal("Qux.txt", result[5]);
    }

    [Fact]
    public void TestNameSplitterBySeparatorStrings_2Strings_RemoveEmpty()
    {
      var path = @"F:\Foo\Bar\Baz\Qux.txt";
      var testObj = new NameSplitterBySeparatorStrings
      {
        Separators = ["\\", ":"],
        RemoveEmptyEntries = true
      };
      var result = testObj.Split(path);
      Assert.Equal(5, result.Count);
      Assert.Equal("F", result[0]);
      Assert.Equal("Foo", result[1]);
      Assert.Equal("Bar", result[2]);
      Assert.Equal("Baz", result[3]);
      Assert.Equal("Qux.txt", result[4]);
    }

    [Fact]
    public void Test_StringPropertyEvaluator()
    {
      var testObj = new StringPropertyEvaluator() { PropertyName = "Foo" };
      var result = testObj.Evaluate("Hello World");
      Assert.Equal("Foo", result.PropertyName);
      Assert.Equal("Hello World", result.PropertyValue);
    }

    [Fact]
    public void Test_IntegerPropertyEvaluator()
    {
      var testObj = new IntegerPropertyEvaluator() { PropertyName = "Foo" };
      var result = testObj.Evaluate("42");
      Assert.Equal("Foo", result.PropertyName);
      Assert.Equal(42, result.PropertyValue);
    }

    [Fact]
    public void Test_DoublePropertyEvaluator()
    {
      var testObj = new DoublePropertyEvaluator() { PropertyName = "Foo" };
      var result = testObj.Evaluate("42.25");
      Assert.Equal("Foo", result.PropertyName);
      Assert.Equal(42.25, result.PropertyValue);
    }

    [Fact]
    public void Test_TreeParsing()
    {
      var path = @"F:\Foo\Bar\Baz\SampleName_42.5_33.txt";

      var n2 = new NameSplitterBySeparatorStrings
      {
        Separators = ["_"],
        RemoveEmptyEntries = true,
        Children = [
        ( 0, new StringPropertyEvaluator { PropertyName = "Name" } ),
        ( 1, new DoublePropertyEvaluator { PropertyName = "Value1" } ),
        ( 2, new IntegerPropertyEvaluator { PropertyName = "Value2" } )
        ]
      };

      var n1 = new PathToFileNameSplitter
      {
        Children = [
          ( 0, n2 )
        ]
      };

      var result = ((IPropertyExtractionTreeNode)n1).ExtractProperties(path).ToList();

      Assert.Equal(3, result.Count);
      Assert.Equal("Name", result[0].PropertyName);
      Assert.Equal("SampleName", result[0].PropertyValue);
      Assert.Equal("Value1", result[1].PropertyName);
      Assert.Equal(42.5, result[1].PropertyValue);
      Assert.Equal("Value2", result[2].PropertyName);
      Assert.Equal(33, result[2].PropertyValue);
    }
  }
}
