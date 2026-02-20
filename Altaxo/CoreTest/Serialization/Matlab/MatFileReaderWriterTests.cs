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

using System;
using System.IO;
using Xunit;

namespace Altaxo.Serialization.Matlab
{
  /// <summary>
  /// Unit tests for <see cref="MatFileV5Reader"/> <see cref="MatFileV5Writer"/>.
  /// </summary>
  public class MatFileReaderWriterTests
  {
    /// <summary>
    /// Directory that contains embedded MAT test files.
    /// </summary>
    public string TestFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serialization\\Matlab\\TestFiles");

    /// <summary>
    /// Opens a MAT test file for reading.
    /// </summary>
    /// <param name="fileName">File name inside <see cref="TestFilePath"/>.</param>
    public FileStream GetFileStream(string fileName)
    {
      return new FileStream(Path.Combine(TestFilePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    /// Verifies that a MATLAB char array is imported as a managed string.
    /// </summary>
    [Fact]
    public void Import_CharArray_AsString()
    {
      using var w = new MatFileV5Writer();
      w.WriteString("s", "Hello");
      var mat = w.ToArray();
      var file = MatFileV5Reader.Read(new MemoryStream(mat));

      var s = Assert.IsType<MatValue.String>(file["s"]);
      Assert.Equal("Hello", s.Value);
    }

    /// <summary>
    /// Verifies that a MATLAB logical scalar is imported as a boolean scalar.
    /// </summary>
    [Fact]
    public void Import_LogicalScalar()
    {
      using var w = new MatFileV5Writer();
      w.WriteLogicalScalar("b", true);
      var mat = w.ToArray();
      var file = MatFileV5Reader.Read(new MemoryStream(mat));

      var b = Assert.IsType<MatValue.LogicalScalar>(file["b"]);
      Assert.True(b.Value);
    }

    /// <summary>
    /// Verifies that all bundled MAT files can be read and contain at least one variable.
    /// </summary>
    [Fact]
    public void Test_AllFilesReadable()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("*.mat");
      Assert.NotEmpty(testFiles);
      foreach (var file in testFiles)
      {
        using var str = GetFileStream(file.FullName);
        var reader = MatFileV5Reader.Read(str);
        Assert.NotEmpty(reader.Variables);
      }
    }

    /// <summary>
    /// Verifies key variables and selected field shapes in <c>corn.mat</c>.
    /// </summary>
    [Fact]
    public void Test_File_Corn()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("corn.mat");
      Assert.Single(testFiles);
      var file = testFiles[0];
      using var str = GetFileStream(file.FullName);
      var reader = MatFileV5Reader.Read(str);
      Assert.Equal(8, reader.Variables.Count);
      Assert.Contains("information", reader.Variables);
      Assert.Contains("m5spec", reader.Variables);
      Assert.Contains("mp5spec", reader.Variables);
      Assert.Contains("mp6spec", reader.Variables);
      Assert.Contains("m5nbs", reader.Variables);
      Assert.Contains("mp5nbs", reader.Variables);
      Assert.Contains("mp6nbs", reader.Variables);
      Assert.Contains("propvals", reader.Variables);

      Assert.True(reader.Variables["m5spec"] is MatValue.StructArray or MatValue.ObjectArray);
      Assert.True(reader.Variables["mp5spec"] is MatValue.StructArray or MatValue.ObjectArray);
      Assert.True(reader.Variables["mp6spec"] is MatValue.StructArray or MatValue.ObjectArray);
      Assert.True(reader.Variables["m5nbs"] is MatValue.StructArray or MatValue.ObjectArray);
      Assert.True(reader.Variables["mp5nbs"] is MatValue.StructArray or MatValue.ObjectArray);
      Assert.True(reader.Variables["mp6nbs"] is MatValue.StructArray or MatValue.ObjectArray);

      Assert.True(reader.Variables["information"] is MatValue.String);
      string s = ((MatValue.String)reader.Variables["information"]).Value;
      Assert.StartsWith("This data set consists of 80 samples of corn measured on 3 different NIR spectrometers.", s);

      // ----------------------------------------------------------------------
      // Verify that the "m5spec" variable contains expected fields (as seen when loading in Octave).

      var m5spec = reader.Variables["m5spec"] as MatValue.ObjectArray;
      Assert.NotNull(m5spec);
      Assert.Contains("type", m5spec.Fields);
      Assert.Contains("date", m5spec.Fields);
      Assert.Contains("moddate", m5spec.Fields);
      Assert.Contains("date", m5spec.Fields);
      Assert.Contains("data", m5spec.Fields);
      Assert.Contains("label", m5spec.Fields);
      Assert.Contains("axisscale", m5spec.Fields);
      Assert.Contains("title", m5spec.Fields);
      Assert.Contains("class", m5spec.Fields);
      Assert.Contains("datasetversion", m5spec.Fields);


      // Verify a couple of key field types/sizes (as seen when loading in Octave)
      var m5Date = Assert.IsType<MatValue.Vector>(m5spec.Fields["date"][0]);
      Assert.Equal(6, m5Date.Data.Count);
      Assert.Equal(2005, m5Date.Data[0]);
      Assert.Equal(5, m5Date.Data[1]);
      Assert.Equal(13, m5Date.Data[2]);
      Assert.Equal(9, m5Date.Data[3]);
      Assert.Equal(50, m5Date.Data[4]);

      var m5Data = Assert.IsType<MatValue.Matrix>(m5spec.Fields["data"][0]);
      Assert.Equal(80, m5Data.RowCount);
      Assert.Equal(700, m5Data.ColumnCount);
      AssertEx.AreEqual(0.044495, m5Data[0, 0], 0, 1E-5);
      AssertEx.AreEqual(0.056716, m5Data[6, 0], 0, 1E-5);
      AssertEx.AreEqual(0.877, m5Data[74, 699], 0, 1E-5);
      AssertEx.AreEqual(0.72825, m5Data[79, 699], 0, 1E-5);

      // In Octave this is shown as a double matrix whose column count matches the axis scale.
      var axisScaleValue = m5spec.Fields["axisscale"][0];
      var axisScaleVector = axisScaleValue switch
      {
        MatValue.Vector v => v,
        MatValue.CellArray c when c.Elements.Length > 0 => c.Elements[0] as MatValue.Vector,
        _ => null
      };

      if (axisScaleVector is not null)
        Assert.Equal(axisScaleVector.Data.Count, m5Data.ColumnCount);

      // ----------------------------------------------------------------------
      // m5nbs

      var m5nbs = reader.Variables["m5nbs"] as MatValue.ObjectArray;
      var m5nbs_data = m5nbs.Fields["data"][0] as MatValue.Matrix;

      Assert.Equal(3, m5nbs_data.RowCount);
      Assert.Equal(700, m5nbs_data.ColumnCount);
      AssertEx.AreEqual(0.13404, m5nbs_data[0, 0], 0, 1E-4);
      AssertEx.AreEqual(0.14366, m5nbs_data[2, 0], 0, 1E-4);
      AssertEx.AreEqual(0.096172, m5nbs_data[1, 695], 0, 1E-4);
      AssertEx.AreEqual(0.10477, m5nbs_data[2, 699], 0, 1E-4);
    }

    /// <summary>
    /// Ensures the octane data MAT file can be imported without throwing.
    /// </summary>
    [Fact]
    public void Test_File_Octane()
    {
      var testFiles = new DirectoryInfo(TestFilePath).GetFiles("gasoline_octane_nir_data.mat");
      Assert.Single(testFiles);
      var file = testFiles[0];
      using var str = GetFileStream(file.FullName);
      var reader = MatFileV5Reader.Read(str);
      Assert.Equal(2, reader.Variables.Count);
      Assert.Contains("octane", reader.Variables);
      Assert.Contains("NIR", reader.Variables);
      Assert.True(reader.Variables["octane"] is MatValue.Vector);
      Assert.True(reader.Variables["NIR"] is MatValue.Matrix);

      var v = (MatValue.Vector)reader.Variables["octane"];
      var m = (MatValue.Matrix)reader.Variables["NIR"];

      Assert.Equal(60, v.Data.Count);
      Assert.Equal(60, m.RowCount);
      Assert.Equal(401, m.ColumnCount);

      AssertEx.AreEqual(85.3, v.Data[0], 0, 1E-14);
      AssertEx.AreEqual(85.25, v.Data[1], 0, 1E-14);
      AssertEx.AreEqual(89.6, v.Data[58], 0, 1E-14);
      AssertEx.AreEqual(87.1, v.Data[59], 0, 1E-14);

      AssertEx.AreEqual(-0.050193, m[0, 0], 0, 1E-5);
      AssertEx.AreEqual(-0.045903, m[0, 1], 0, 1E-5);
      AssertEx.AreEqual(-0.044227, m[1, 0], 0, 1E-5);
      AssertEx.AreEqual(-0.039602, m[1, 1], 0, 1E-5);
      AssertEx.AreEqual(1.1760, m[58, 399], 0, 1E-4);
      AssertEx.AreEqual(1.1547, m[58, 400], 0, 1E-4);
      AssertEx.AreEqual(1.1544, m[59, 399], 0, 1E-4);
      AssertEx.AreEqual(1.164, m[59, 400], 0, 1E-4);
    }

    /// <summary>
    /// Reads a MAT v5 file written by Octave and verifies all variable types supported by <see cref="MatValue"/>.
    /// </summary>
    /// <remarks>
    /// The file can be generated with Octave by running:
    /// <c>octave --no-gui --quiet generate_all_supported_types.m</c>
    /// in <c>CoreTest\Serialization\Matlab\TestFiles</c>.
    /// Use the following script to generate the file in Octave:
    /// </remarks>
    [Fact]
    public void Test_File_AllSupportedTypes_FromOctave()
    {
      var filePath = Path.Combine(TestFilePath, "all_supported_types.mat");
      if (!File.Exists(filePath))
        return;

      using var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      var reader = MatFileV5Reader.Read(str);

      AssertAllSupportedTypes(reader);
    }

    /// <summary>
    /// Generates the same set of variables as <c>all_supported_types.mat</c> using <see cref="MatFileV5Writer"/>,
    /// reads the generated file back using <see cref="MatlabImporter"/>, and verifies all values and structures.
    /// </summary>
    [Fact]
    public void Test_AllSupportedTypes_FromWriter_InMemory()
    {
      static double[] Range1To(int n)
      {
        var a = new double[n];
        for (int i = 0; i < n; i++)
          a[i] = i + 1;
        return a;
      }

      using var w = new MatFileV5Writer();

      w.Write("s", new MatValue.String("Hello"));
      w.Write("a", new MatValue.Scalar(42.5));
      w.Write("v", new MatValue.Vector(new double[] { 1, 2, 3 }));
      w.Write("m", new MatValue.Matrix(2, 3, new double[] { 1, 2, 3, 4, 5, 6 }, IsColumnMajor: true));

      w.Write("b", new MatValue.LogicalScalar(true));
      w.Write("lb", new MatValue.LogicalArray(new bool[] { true, false, false, true }, new int[] { 2, 2 }, IsColumnMajor: true));

      w.Write("na", new MatValue.NumericArray(Range1To(24), new int[] { 2, 3, 4 }, IsColumnMajor: true));

      w.Write(
        "c",
        new MatValue.CellArray(
          new MatValue[]
          {
            new MatValue.String("cellstr"),
            new MatValue.Scalar(7.5),
            new MatValue.Vector(new double[] { 1, 2 }),
            new MatValue.Matrix(2, 2, new double[] { 1, 3, 2, 4 }, IsColumnMajor: true)
          },
          new int[] { 1, 4 },
          IsColumnMajor: true));

      var stFields = new System.Collections.Generic.Dictionary<string, MatValue[]>(StringComparer.Ordinal)
      {
        ["fstr"] = new MatValue[] { new MatValue.String("abc") },
        ["fscalar"] = new MatValue[] { new MatValue.Scalar(3.14) },
        ["fvec"] = new MatValue[] { new MatValue.Vector(new double[] { 10, 20, 30 }) },
        ["fmat"] = new MatValue[] { new MatValue.Matrix(2, 2, new double[] { 11, 13, 12, 14 }, IsColumnMajor: true) },
        ["flog"] = new MatValue[] { new MatValue.LogicalScalar(false) },
        ["flogarr"] = new MatValue[] { new MatValue.LogicalArray(new bool[] { true, false, false, true }, new int[] { 2, 2 }, IsColumnMajor: true) },
        ["fnd"] = new MatValue[] { new MatValue.NumericArray(Range1To(24), new int[] { 2, 3, 4 }, IsColumnMajor: true) },
        ["fcell"] = new MatValue[]
        {
          new MatValue.CellArray(
            new MatValue[] { new MatValue.String("x"), new MatValue.Scalar(1) },
            new int[] { 1, 2 },
            IsColumnMajor: true)
        }
      };
      w.Write("st", new MatValue.StructArray(stFields, new int[] { 1, 1 }, IsColumnMajor: true));

      var objFields = new System.Collections.Generic.Dictionary<string, MatValue[]>(StringComparer.Ordinal)
      {
        ["x"] = new MatValue[] { new MatValue.Scalar(1) },
        ["y"] = new MatValue[] { new MatValue.Vector(new double[] { 1, 2, 3 }) },
      };
      w.Write("obj", new MatValue.ObjectArray("MyClass", objFields, new int[] { 1, 1 }, IsColumnMajor: true));

      var bytes = w.ToArray();
      var imported = MatFileV5Reader.Read(new MemoryStream(bytes));

      AssertAllSupportedTypes(imported);
    }

    private static void AssertAllSupportedTypes(MatFile reader)
    {
      Assert.Contains("s", reader.Variables);
      Assert.Contains("a", reader.Variables);
      Assert.Contains("v", reader.Variables);
      Assert.Contains("m", reader.Variables);
      Assert.Contains("b", reader.Variables);
      Assert.Contains("lb", reader.Variables);
      Assert.Contains("na", reader.Variables);
      Assert.Contains("c", reader.Variables);
      Assert.Contains("st", reader.Variables);
      Assert.Contains("obj", reader.Variables);

      var s = Assert.IsType<MatValue.String>(reader.Variables["s"]);
      Assert.Equal("Hello", s.Value);

      var a = Assert.IsType<MatValue.Scalar>(reader.Variables["a"]);
      Assert.Equal(42.5, a.Value, 12);

      var v = Assert.IsType<MatValue.Vector>(reader.Variables["v"]);
      Assert.Equal(3, v.Data.Count);
      AssertEx.AreEqual(1, v.Data[0], 0, 1E-14);
      AssertEx.AreEqual(2, v.Data[1], 0, 1E-14);
      AssertEx.AreEqual(3, v.Data[2], 0, 1E-14);

      var m = Assert.IsType<MatValue.Matrix>(reader.Variables["m"]);
      Assert.Equal(2, m.RowCount);
      Assert.Equal(3, m.ColumnCount);
      AssertEx.AreEqual(1, m[0, 0], 0, 1E-14);
      AssertEx.AreEqual(2, m[1, 0], 0, 1E-14);
      AssertEx.AreEqual(3, m[0, 1], 0, 1E-14);
      AssertEx.AreEqual(4, m[1, 1], 0, 1E-14);
      AssertEx.AreEqual(5, m[0, 2], 0, 1E-14);
      AssertEx.AreEqual(6, m[1, 2], 0, 1E-14);

      var b = Assert.IsType<MatValue.LogicalScalar>(reader.Variables["b"]);
      Assert.True(b.Value);

      var lb = Assert.IsType<MatValue.LogicalArray>(reader.Variables["lb"]);
      Assert.Equal(new[] { 2, 2 }, lb.Dimensions.ToArray());
      Assert.Equal(4, lb.Data.Length);
      Assert.True(lb.Data.Span[0]);
      Assert.False(lb.Data.Span[1]);
      Assert.False(lb.Data.Span[2]);
      Assert.True(lb.Data.Span[3]);

      var na = Assert.IsType<MatValue.NumericArray>(reader.Variables["na"]);
      Assert.Equal(new[] { 2, 3, 4 }, na.Dimensions.ToArray());
      Assert.Equal(24, na.Data.Length);
      AssertEx.AreEqual(1, na.Data.Span[0], 0, 1E-14);
      AssertEx.AreEqual(24, na.Data.Span[23], 0, 1E-14);

      var c = Assert.IsType<MatValue.CellArray>(reader.Variables["c"]);
      Assert.Equal(4, c.Elements.Length);
      Assert.IsType<MatValue.String>(c.Elements[0]);
      Assert.IsType<MatValue.Scalar>(c.Elements[1]);
      Assert.IsType<MatValue.Vector>(c.Elements[2]);
      Assert.IsType<MatValue.Matrix>(c.Elements[3]);

      var st = Assert.IsType<MatValue.StructArray>(reader.Variables["st"]);
      Assert.Equal(new[] { 1, 1 }, st.Dimensions.ToArray());
      Assert.Contains("fstr", st.Fields);
      Assert.Contains("fscalar", st.Fields);
      Assert.Contains("fvec", st.Fields);
      Assert.Contains("fmat", st.Fields);
      Assert.Contains("flog", st.Fields);
      Assert.Contains("flogarr", st.Fields);
      Assert.Contains("fnd", st.Fields);
      Assert.Contains("fcell", st.Fields);

      Assert.Equal("abc", Assert.IsType<MatValue.String>(st.Fields["fstr"][0]).Value);
      AssertEx.AreEqual(3.14, Assert.IsType<MatValue.Scalar>(st.Fields["fscalar"][0]).Value, 0, 1E-12);

      var stVec = Assert.IsType<MatValue.Vector>(st.Fields["fvec"][0]);
      Assert.Equal(3, stVec.Data.Count);
      AssertEx.AreEqual(10, stVec.Data[0], 0, 1E-14);
      AssertEx.AreEqual(30, stVec.Data[2], 0, 1E-14);

      var stMat = Assert.IsType<MatValue.Matrix>(st.Fields["fmat"][0]);
      Assert.Equal(2, stMat.RowCount);
      Assert.Equal(2, stMat.ColumnCount);
      AssertEx.AreEqual(11, stMat[0, 0], 0, 1E-14);
      AssertEx.AreEqual(14, stMat[1, 1], 0, 1E-14);

      Assert.False(Assert.IsType<MatValue.LogicalScalar>(st.Fields["flog"][0]).Value);
      Assert.IsType<MatValue.LogicalArray>(st.Fields["flogarr"][0]);
      Assert.IsType<MatValue.NumericArray>(st.Fields["fnd"][0]);
      Assert.IsType<MatValue.CellArray>(st.Fields["fcell"][0]);

      var obj = Assert.IsType<MatValue.ObjectArray>(reader.Variables["obj"]);
      Assert.Equal("MyClass", obj.ClassName);
      Assert.Contains("x", obj.Fields);
      Assert.Contains("y", obj.Fields);
      AssertEx.AreEqual(1, Assert.IsType<MatValue.Scalar>(obj.Fields["x"][0]).Value, 0, 1E-14);
      Assert.Equal(3, Assert.IsType<MatValue.Vector>(obj.Fields["y"][0]).Data.Count);
    }

    /// <summary>
    /// Verifies that a single 1x1 double matrix is imported as a scalar.
    /// </summary>
    [Fact]
    public void Import_ScalarDouble()
    {
      using var w = new MatFileV5Writer();
      w.WriteScalarDouble("a", 42.5);
      var mat = w.ToArray();
      var file = MatFileV5Reader.Read(new MemoryStream(mat));

      var v = Assert.IsType<MatValue.Scalar>(file["a"]);
      Assert.Equal(42.5, v.Value, 12);
    }

    /// <summary>
    /// Verifies that an Nx1 double matrix is imported as a vector.
    /// </summary>
    [Fact]
    public void Import_VectorDouble()
    {
      using var w = new MatFileV5Writer();
      w.WriteVectorDouble("v", new double[] { 1, 2, 3 });
      var mat = w.ToArray();
      var file = MatFileV5Reader.Read(new MemoryStream(mat));

      var v = Assert.IsType<MatValue.Vector>(file["v"]);
      Assert.Equal(new double[] { 1, 2, 3 }, v.Data.ToArray());
    }

    /// <summary>
    /// Verifies that 2D matrices are interpreted as column-major (MATLAB layout).
    /// </summary>
    [Fact]
    public void Import_MatrixDouble_ColumnMajor()
    {
      // 2x2 matrix in column-major: [1 3; 2 4] if data = [1,2,3,4]
      using var w = new MatFileV5Writer();
      w.WriteMatrixDouble("m", rows: 2, cols: 2, new double[] { 1, 2, 3, 4 });
      var mat = w.ToArray();
      var file = MatFileV5Reader.Read(new MemoryStream(mat));

      var m = Assert.IsType<MatValue.Matrix>(file["m"]);
      Assert.Equal(2, m.RowCount);
      Assert.Equal(2, m.ColumnCount);
      Assert.Equal(1, m[0, 0]);
      Assert.Equal(2, m[1, 0]);
      Assert.Equal(3, m[0, 1]);
      Assert.Equal(4, m[1, 1]);
    }
    // MAT v5 test file generation is provided by `MatFileV5Writer`.
  }
}
