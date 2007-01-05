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
using System.Runtime.Serialization;
using Altaxo.Serialization;
using Altaxo.Main;

using Altaxo.Graph.Gdi;

namespace Altaxo
{
  /// <summary>
  /// Summary description for AltaxoDocument.
  /// </summary>
  [SerializationSurrogate(0,typeof(AltaxoDocument.SerializationSurrogate0))]
  [SerializationVersion(0,"Initial version of the main document only contains m_DataSet")]
  public class AltaxoDocument 
    : 
    IDeserializationCallback,
    Main.INamedObjectCollection ,
    Main.IChildChangedEventSink 
  {
    protected Altaxo.Data.DataTableCollection m_DataSet = null; // The root of all the data

    protected Altaxo.Graph.Gdi.GraphDocumentCollection m_GraphSet = null; // all graphs are stored here

    protected Altaxo.Worksheet.WorksheetLayoutCollection m_TableLayoutList = null;

    private Altaxo.Scripting.FitFunctionScriptCollection _FitFunctionScripts;


    //  protected System.Collections.ArrayList m_Worksheets;
    /// <summary>The list of GraphForms for the document.</summary>
    //  protected System.Collections.ArrayList m_GraphForms;

    [NonSerialized]
    protected bool m_IsDirty=false;
    public event EventHandler DirtyChanged;
    [NonSerialized]
    private bool m_DeserializationFinished=false;

    public AltaxoDocument()
    {
      m_DataSet = new Altaxo.Data.DataTableCollection(this);
      m_GraphSet = new GraphDocumentCollection(this);
      m_TableLayoutList = new Altaxo.Worksheet.WorksheetLayoutCollection(this);
      _FitFunctionScripts = new Altaxo.Scripting.FitFunctionScriptCollection();
      //  m_Worksheets = new System.Collections.ArrayList();
      //  m_GraphForms = new System.Collections.ArrayList();
    }

    #region Serialization
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        AltaxoDocument s = (AltaxoDocument)obj;
        info.AddValue("DataTableCollection",s.m_DataSet);
        //info.AddValue("Worksheets",s.m_Worksheets);
        //  info.AddValue("GraphForms",s.m_GraphForms);
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        AltaxoDocument s = (AltaxoDocument)obj;
        s.m_DataSet = (Altaxo.Data.DataTableCollection)info.GetValue("DataTableCollection",typeof(Altaxo.Data.DataTableCollection));
        // s.tstObj    = (AltaxoTestObject02)info.GetValue("TstObj",typeof(AltaxoTestObject02));
        //s.m_Worksheets = (System.Collections.ArrayList)info.GetValue("Worksheets",typeof(System.Collections.ArrayList));
        //  s.m_GraphForms = (System.Collections.ArrayList)info.GetValue("GraphForms",typeof(System.Collections.ArrayList));
        s.m_IsDirty = false;
        return s;
      }
    }

    public void OnDeserialization(object obj)
    {
      if(!m_DeserializationFinished && obj is DeserializationFinisher)
      {
        m_DeserializationFinished=true;
        DeserializationFinisher finisher = new DeserializationFinisher(this);
      
        m_DataSet.ParentObject = this;
        m_DataSet.OnDeserialization(finisher);

        /*
          for(int i=0;i<m_Worksheets.Count;i++)
          {
            m_Worksheets[i] = ((IDeserializationSubstitute)m_Worksheets[i]).GetRealObject(App.Current.View.Form);
            ((System.Windows.Forms.Form)m_Worksheets[i]).Show();
          }
          for(int i=0;i<m_GraphForms.Count;i++)
          {
            m_GraphForms[i] = ((IDeserializationSubstitute)m_GraphForms[i]).GetRealObject(App.Current.View.Form);
            ((System.Windows.Forms.Form)m_GraphForms[i]).Show();
          }
          */
      }
    }

    public void RestoreWindowsAfterDeserialization()
    {
    }


    public void SaveToZippedFile(ICompressedFileContainerStream zippedStream, Altaxo.Serialization.Xml.XmlStreamSerializationInfo info)
    {
      System.Text.StringBuilder errorText = new System.Text.StringBuilder();

      // first, we save all tables into the tables subdirectory
      foreach(Altaxo.Data.DataTable table in this.m_DataSet)
      {
        try
        {
          zippedStream.StartFile("Tables/"+table.Name+".xml",0);
          //ZipEntry ZipEntry = new ZipEntry("Tables/"+table.Name+".xml");
          //zippedStream.PutNextEntry(ZipEntry);
          //zippedStream.SetLevel(0);
          info.BeginWriting(zippedStream.Stream);
          info.AddValue("Table",table);
          info.EndWriting();
        }
        catch(Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }

      // second, we save all graphs into the Graphs subdirectory
      foreach(GraphDocument graph in this.m_GraphSet)
      {
        try
        {
          zippedStream.StartFile("Graphs/"+graph.Name+".xml",0);
          //ZipEntry ZipEntry = new ZipEntry("Graphs/"+graph.Name+".xml");
          //zippedStream.PutNextEntry(ZipEntry);
          //zippedStream.SetLevel(0);
          info.BeginWriting(zippedStream.Stream);
          info.AddValue("Graph",graph);
          info.EndWriting();
        }
        catch(Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }

      // third, we save all TableLayouts into the TableLayouts subdirectory
      foreach(Altaxo.Worksheet.WorksheetLayout layout in this.m_TableLayoutList)
      {
        try 
        {
          zippedStream.StartFile("TableLayouts/"+layout.Name+".xml",0);
          //ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
          //zippedStream.PutNextEntry(ZipEntry);
          //zippedStream.SetLevel(0);
          info.BeginWriting(zippedStream.Stream);
          info.AddValue("WorksheetLayout",layout);
          info.EndWriting();
        }
        catch(Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }

      // 4th, we save all FitFunctions into the FitFunctions subdirectory
      foreach(Altaxo.Scripting.FitFunctionScript fit in this._FitFunctionScripts)
      {
        try 
        {
          zippedStream.StartFile("FitFunctionScripts/"+fit.CreationTime.ToString()+".xml",0);
          //ZipEntry ZipEntry = new ZipEntry("TableLayouts/"+layout.Name+".xml");
          //zippedStream.PutNextEntry(ZipEntry);
          //zippedStream.SetLevel(0);
          info.BeginWriting(zippedStream.Stream);
          info.AddValue("FitFunctionScript",fit);
          info.EndWriting();
        }
        catch(Exception exc)
        {
          errorText.Append(exc.ToString());
        }
      }


      if(errorText.Length!=0)
        throw new ApplicationException(errorText.ToString());
    }


    public void RestoreFromZippedFile(ICompressedFileContainer zipFile, Altaxo.Serialization.Xml.XmlStreamDeserializationInfo info)
    {
      System.Text.StringBuilder errorText = new System.Text.StringBuilder();

      foreach(IFileContainerItem zipEntry in zipFile)
      {
        try
        {
          if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Tables/"))
          {
            System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
            info.BeginReading(zipinpstream);
            object readedobject = info.GetValue("Table",this);
            if(readedobject is Altaxo.Data.DataTable)
              this.m_DataSet.Add((Altaxo.Data.DataTable)readedobject);
            info.EndReading();
        
          }
          else if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("Graphs/"))
          {
            System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
            info.BeginReading(zipinpstream);
            object readedobject = info.GetValue("Graph",this);
            if(readedobject is GraphDocument)
              this.m_GraphSet.Add((GraphDocument)readedobject);
            info.EndReading();
          
          }
          else if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("TableLayouts/"))
          {
            System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
            info.BeginReading(zipinpstream);
            object readedobject = info.GetValue("WorksheetLayout",this);
            if(readedobject is Altaxo.Worksheet.WorksheetLayout)
              this.m_TableLayoutList.Add((Altaxo.Worksheet.WorksheetLayout)readedobject);
            info.EndReading();
          
          }
          else if(!zipEntry.IsDirectory && zipEntry.Name.StartsWith("FitFunctionScripts/"))
          {
            System.IO.Stream zipinpstream =zipFile.GetInputStream(zipEntry);
            info.BeginReading(zipinpstream);
            object readedobject = info.GetValue("FitFunctionScript",this);
            if(readedobject is Altaxo.Scripting.FitFunctionScript)
              this._FitFunctionScripts.Add((Altaxo.Scripting.FitFunctionScript)readedobject);
            info.EndReading();
          
          }
        }
        catch(Exception exc)
        {
          errorText.Append("Error deserializing ");
          errorText.Append(zipEntry.Name);
          errorText.Append(", ");
          errorText.Append(exc.ToString());
        }
      }

      try
      {
        info.AnnounceDeserializationEnd(this);
      }
      catch(Exception exc)
      {
        errorText.Append(exc.ToString());
      }


      if(errorText.Length!=0)
        throw new ApplicationException(errorText.ToString());

    }


    #endregion

    
    public Altaxo.Data.DataTableCollection DataTableCollection
    {
      get { return m_DataSet; }
    }
    public Altaxo.Graph.Gdi.GraphDocumentCollection GraphDocumentCollection
    {
      get { return m_GraphSet; }
    }

    public Altaxo.Worksheet.WorksheetLayoutCollection TableLayouts
    {
      get { return this.m_TableLayoutList; }
    }
    public Altaxo.Scripting.FitFunctionScriptCollection FitFunctionScripts
    {
      get { return _FitFunctionScripts; }
    }

    protected virtual void OnDirtyChanged()
    {
      if(null!=DirtyChanged)
        DirtyChanged(this,EventArgs.Empty);
    }

    public bool IsDirty
    {
      get { return m_IsDirty; }
      set 
      {
        bool oldValue = m_IsDirty;
        m_IsDirty = value;
        if(oldValue!=m_IsDirty)
        {
          OnDirtyChanged();
        }

      }
    }

    public void EhChildChanged(object sender, EventArgs e)
    {
      this.IsDirty = true;
    }

    public Altaxo.Data.DataTable CreateNewTable(string worksheetName, bool bCreateDefaultColumns)
    {
      Altaxo.Data.DataTable dt1 = new Altaxo.Data.DataTable(worksheetName);


      if(bCreateDefaultColumns)
      {
        dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(),"A",Altaxo.Data.ColumnKind.X);
        dt1.DataColumns.Add(new Altaxo.Data.DoubleColumn(),"B");
      }

      DataTableCollection.Add(dt1);

      return dt1;
    }

    public Altaxo.Graph.Gdi.GraphDocument CreateNewGraphDocument()
    {
      GraphDocument doc = new GraphDocument();
      GraphDocumentCollection.Add(doc);

      return doc;
    }

  


    public Altaxo.Worksheet.WorksheetLayout CreateNewTableLayout(Altaxo.Data.DataTable table)
    {
      Altaxo.Worksheet.WorksheetLayout layout = new Altaxo.Worksheet.WorksheetLayout(table);
      this.m_TableLayoutList.Add(layout);
      return layout;
    }

    public object GetChildObjectNamed(string name)
    {
      switch(name)
      {
        case "Tables":
          return this.m_DataSet;
        case "Graphs":
          return this.m_GraphSet;
        case "TableLayouts":
          return this.m_TableLayoutList;
        case "FitFunctionScripts":
          return this._FitFunctionScripts;
      }
      return null;
    }

    public string GetNameOfChildObject(object o)
    {
      if(null==o)
        return null;
      else if(o.Equals(this.m_DataSet))
        return "Tables";
      else if(o.Equals(this.m_GraphSet))
        return "Graphs";
      else if (o.Equals(this._FitFunctionScripts))
        return "FitFunctionScripts";
      else
        return null;
    }
  
  }
}
