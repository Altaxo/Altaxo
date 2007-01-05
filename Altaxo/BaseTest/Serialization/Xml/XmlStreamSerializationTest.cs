#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Altaxo.Serialization.Xml;

#if false

namespace Altaxo.Test.Serialization.Xml
{
#region Serialization test classes
  [System.Xml.Serialization.XmlRoot]
  public class Foo
  {
    [System.Xml.Serialization.XmlArrayItem("row",typeof(double))]
    public double[] m_Array;

    string m_String;

    const string testString = "Dies<ist>ein=fieser-Ströng_mit&lt-und_&gt-und\\sogar\"mittendrin";

    [XmlSerializationSurrogateFor(typeof(Foo),0)]
      public class Ser1 : IXmlSerializationSurrogate
    {
      public object Deserialize(object o, IXmlDeserializationInfo info, object parent)
      {
        Foo foo = o!=null ? (Foo)o : new Foo();
        int val = info.GetInt32();

        int count = info.GetInt32Attribute("Count");
        foo.m_Array = new Double[count];
        // for(int i=0;i<count;i++) foo.m_Array[i] = info.GetDouble();
        info.GetArray(foo.m_Array,count);
        foo.m_String = info.GetString();

        return foo;
      }

      public void Serialize(object o, IXmlSerializationInfo info)
      {
        Foo obj = (Foo)o;

        info.AddValue("Part2",32);
        info.AddArray("DoubleArray",obj.m_Array,obj.m_Array.Length);
        info.AddValue("String",obj.m_String);

      }
    }


    public Foo()
    {
    }

    public void Fill(int number)
    {
      m_Array = new double[number];
      for(int i=0;i<m_Array.Length;i++)
      {
        m_Array[i] = Math.PI*Math.Sqrt(i);
      }

      m_String = testString;
    }

      
    public bool EnsureEquality(int len)
    {
      bool bEnabled=true;

      if(m_Array.Length!=len)
        Console.WriteLine("Achtung die Längen sind unterschiedlich!");

      for(int i=0;i<m_Array.Length;i++)
      {
        double soll = Math.PI*Math.Sqrt(i);
        if(0!=m_Array[i]-soll)
        {
          if(bEnabled)
          {
            Console.WriteLine("Zahlen unterschiedlich bei {0}, ist={1}, soll={2}",i,m_Array[i],soll);
            bEnabled=false;
          }
        }
      }
      

      if(m_String!=testString)
      {
        Console.WriteLine("String different: actual: {0} but should be: {1}",m_String,testString);
        bEnabled=false;

      }

      if(bEnabled)
      {
        Console.WriteLine("All very fine");
        return true;
      }
      else
        return false;
    }
  }
#endregion




#region Tests
  [NUnit.Framework.TestFixture]
  public class TestXmlSerialization
  {
    public static void TestUnzipped()
    {
      DateTime t1, t2;
      TimeSpan dt;


      Foo o = new Foo();
      o.Fill(100000);
      
      t1 = DateTime.Now;
      XmlDocumentSerializationInfo info = new XmlDocumentSerializationInfo();
      info.AddValue("FooNode",o);
      info.Doc.Save(@"C:\temp\xmltest03.xml");
      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document saved, duration {0}.",dt);

      t1 = DateTime.Now;
      XmlDocument doc3 = new XmlDocument();
      doc3.Load(@"C:\TEMP\xmltest03.xml");
      XmlDocumentSerializationInfo info3 = new XmlDocumentSerializationInfo(doc3);
      Foo o3 = (Foo)info3.GetValue(null);
      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document restored, duration {0}.",dt);
    }


    [NUnit.Framework.Test]
    public  void TestUnzippedStreamZeroXml()
    {
      TestUnzippedStream(0,XmlArrayEncoding.Xml);
    }
    [NUnit.Framework.Test]
    public  void TestUnzippedStreamTenXml()
    {
      TestUnzippedStream(10,XmlArrayEncoding.Xml);
    }
    [NUnit.Framework.Test]
    public  void TestUnzippedStreamHundretThousandXml()
    {
      TestUnzippedStream(100000,XmlArrayEncoding.Xml);
    }

    [NUnit.Framework.Test]
    public  void TestUnzippedStreamZeroBase64()
    {
      TestUnzippedStream(0,XmlArrayEncoding.Base64);
    }
    [NUnit.Framework.Test]
    public void TestUnzippedStreamTenBase64()
    {
      TestUnzippedStream(10,XmlArrayEncoding.Base64);
    }
    [NUnit.Framework.Test]
    public void TestUnzippedStreamHundretThousandBase64()
    {
      TestUnzippedStream(100000,XmlArrayEncoding.Base64);
    }

    [NUnit.Framework.Test]
    public void TestUnzippedStreamZeroBinHex()
    {
      TestUnzippedStream(0,XmlArrayEncoding.BinHex);
    }
    [NUnit.Framework.Test]
    public void TestUnzippedStreamTenBinHex()
    {
      TestUnzippedStream(10,XmlArrayEncoding.BinHex);
    }
    [NUnit.Framework.Test]
    public void TestUnzippedStreamHundretThousandBinHex()
    {
      TestUnzippedStream(100000,XmlArrayEncoding.BinHex);
    }


    public static void TestUnzippedStream(int len, XmlArrayEncoding encoding)
    {
      DateTime t1, t2;
      TimeSpan dt;


      Foo o = new Foo();
      o.Fill(len);
      
      
      System.IO.FileStream outfile = System.IO.File.Create(@"C:\temp\xmlteststream01.xml");
      XmlStreamSerializationInfo info = new XmlStreamSerializationInfo();
      info.DefaultArrayEncoding = encoding;
      t1 = DateTime.Now;
      info.BeginWriting(outfile);
      info.AddValue("FooNode",o);
      info.EndWriting();
      outfile.Close();

      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document saved, duration {0}.",dt);

      
      t1 = DateTime.Now;
      System.IO.FileStream inpstream = System.IO.File.Open(@"C:\temp\xmlteststream01.xml",System.IO.FileMode.Open);
      XmlStreamDeserializationInfo info3 = new XmlStreamDeserializationInfo();
      info3.BeginReading(inpstream);
      Foo o3 = (Foo)info3.GetValue(null);
      info3.EndReading();
      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document restored, duration {0}.",dt);
      NUnit.Framework.Assertion.Assert(o3.EnsureEquality(len));
      
    }

    public static void TestZippedStream(int len, int ziplevel)
    {
      DateTime t1, t2;
      TimeSpan dt;


      Foo o = new Foo();
      o.Fill(len);
      
      
      System.IO.FileStream zipoutfile = System.IO.File.Create(@"C:\temp\xmlteststream01.xml.zip");
      ZipOutputStream ZipStream = new ZipOutputStream(zipoutfile);
      ZipEntry ZipEntry = new ZipEntry("Table/Table1.xml");
      ZipStream.PutNextEntry(ZipEntry);
      ZipStream.SetLevel(ziplevel);

      XmlStreamSerializationInfo info = new XmlStreamSerializationInfo();
      t1 = DateTime.Now;
      info.BeginWriting(ZipStream);
      info.AddValue("FooNode",o);
      info.EndWriting();
      ZipStream.Finish();
      ZipStream.Close();
      zipoutfile.Close();

      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document saved, duration {0}.",dt);

      
      t1 = DateTime.Now;
      ZipFile zipfile = new ZipFile(@"C:\temp\xmlteststream01.xml.zip");
      System.IO.Stream zipinpstream = zipfile.GetInputStream(new ZipEntry("Table/Table1.xml"));
      XmlStreamDeserializationInfo info3 = new XmlStreamDeserializationInfo();
      info3.BeginReading(zipinpstream);
      Foo o3 = (Foo)info3.GetValue(null);
      info3.EndReading();
      zipinpstream.Close();
      zipfile.Close();
      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document restored, duration {0}.",dt);
      o3.EnsureEquality(len);
      
    }


    public static void TestZipped()
    {
      DateTime t1, t2;
      TimeSpan dt;


      Foo o = new Foo();
      o.Fill(100000);

      
      t1 = DateTime.Now;
      XmlDocumentSerializationInfo info = new XmlDocumentSerializationInfo();
      info.AddValue("FooNode",o);
      
      System.IO.FileStream zipoutfile = System.IO.File.Create(@"C:\temp\xmltest03.xml.zip");
      ZipOutputStream ZipStream = new ZipOutputStream(zipoutfile);
      ZipEntry ZipEntry = new ZipEntry("Table/Table1.xml");
      ZipStream.PutNextEntry(ZipEntry);
      ZipStream.SetLevel(7);
      info.Doc.Save(ZipStream);
      ZipStream.Finish();
      ZipStream.Close();
      zipoutfile.Close();


      t2 = DateTime.Now;
      dt = t2-t1;
      Console.WriteLine("Document saved, duration {0}.",dt);


      t1 = DateTime.Now;
      ZipFile zipfile = new ZipFile(@"C:\temp\xmltest03.xml.zip");
      System.IO.Stream zipinpstream = zipfile.GetInputStream(new ZipEntry("Table/Table1.xml"));
      XmlDocument doc3 = new XmlDocument();
      doc3.Load(zipinpstream);
      XmlDocumentSerializationInfo info3 = new XmlDocumentSerializationInfo(doc3);
      Foo o3 = (Foo)info3.GetValue(null);
      zipinpstream.Close();
      zipfile.Close();
      t2 = DateTime.Now;
      dt = t2-t1;

      Console.WriteLine("Document restored, duration {0}.",dt);
      
    }

    public static void TestConvert()
    {
      int number = 100000;
      double [] m_Array = new double[number];
      for(int i=0;i<m_Array.Length;i++)
      {
        m_Array[i] = Math.PI*Math.Sqrt(i);
      }

      System.Text.StringBuilder sb = new System.Text.StringBuilder(24*number);

      DateTime t1, t2;
      TimeSpan dt;

      t1 = DateTime.Now;
      for(int i=0;i<m_Array.Length;i++)
      {
        
        sb.Append("<t>");
        sb.Append(System.Xml.XmlConvert.ToString(m_Array[i]));
        sb.Append("/t");
        
        
        
      }

      t2 = DateTime.Now;
      dt = t2-t1;

      Console.WriteLine("Duration: {0}, SB-Length:{1}.",dt,sb.Length);
    }


    public static void TestBinarySerializationUnzipped(int len)
    {
      DateTime t1;
      TimeSpan dt;
      double[] arr = new double[len];
      for(int i=0;i<arr.Length;i++)
      {
        arr[i] = Math.PI*Math.Sqrt(i);
      }

      FileStream fs = new FileStream(@"C:\TEMP\testbinformat.bin", FileMode.Create);

      // Construct a BinaryFormatter and use it to serialize the data to the stream.
      System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

      t1 = DateTime.Now;
      try 
      {
        formatter.Serialize(fs, arr);
      }
      catch (SerializationException e) 
      {
        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
        throw;
      }
      finally 
      {
        fs.Flush();
        fs.Close();
      }
      dt = DateTime.Now - t1;
      Console.WriteLine("Duration: {0}",dt);


    }


    public static void TestBinarySerializationZipped(int len)
    {
      DateTime t1;
      TimeSpan dt;
      double[] arr = new double[len];
      for(int i=0;i<arr.Length;i++)
      {
        arr[i] = Math.PI*Math.Sqrt(i);
      }

      //  FileStream fs = new FileStream(@"C:\TEMP\testbinformat.bin", FileMode.Create);
      System.IO.FileStream zipoutfile = System.IO.File.Create(@"C:\temp\xmltest03.xml.zip");
      ZipOutputStream fs = new ZipOutputStream(zipoutfile);
      ZipEntry ZipEntry = new ZipEntry("Table/Table1.xml");
      fs.PutNextEntry(ZipEntry);
      fs.SetLevel(0);


      // Construct a BinaryFormatter and use it to serialize the data to the stream.
      System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new BinaryFormatter();

      t1 = DateTime.Now;
      try 
      {
        formatter.Serialize(fs, arr);
      }
      catch (SerializationException e) 
      {
        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
        throw;
      }
      finally 
      {
        fs.Flush();
        fs.Close();
      }
      dt = DateTime.Now - t1;
      Console.WriteLine("Duration: {0}",dt);


    }

    public static void TestConv()
    {
      double[] arr = new double[4];
      byte [] bytes = new byte[32];
      arr.CopyTo(bytes,0);

      Console.WriteLine("Thats it");

    }

  }
#endregion


}

#endif