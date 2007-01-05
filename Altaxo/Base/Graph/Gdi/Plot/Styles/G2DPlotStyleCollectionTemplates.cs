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
using System.Text;
using System.Collections;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  public class G2DPlotStyleCollectionTemplates
  {

    #region Inner Classes

    public delegate G2DPlotStyleCollection CreateCollectionProcedure();

    class TypeArray
    {
      System.Type[] _types;
      public TypeArray(System.Type[] types)
      {
        _types = types;
      }

      public override int GetHashCode()
      {
        int result = 0;
        for (int i = 0; i < _types.Length; i++)
          result ^= _types[i].GetHashCode();
        return result;
      }
      public override bool Equals(object obj)
      {
        if (!(obj is TypeArray))
          return false;

        TypeArray from = (TypeArray)obj;
        for (int i = 0; i < _types.Length; i++)
          if (this._types[i] != from._types[i])
            return false;

        return true;
      }
    }
    #endregion

    static Hashtable _NamesByTypeArray ;
    static Hashtable _CreationProcByName;
    static ArrayList _NamesInOrder;
    static G2DPlotStyleCollectionTemplates()
    {
      _NamesByTypeArray = new Hashtable();
      _CreationProcByName = new Hashtable();
      _NamesInOrder = new ArrayList();

      Add("Line", new CreateCollectionProcedure(CreateLineStyle));
      Add("Scatter", new CreateCollectionProcedure(CreateScatterStyle));
      Add("Scatter+Line", new CreateCollectionProcedure(CreateScatterAndLineStyle));
      Add("Label+Scatter+Line",new CreateCollectionProcedure(CreateLabelAndScatterAndLineStyle));
      Add("Label only", new CreateCollectionProcedure(CreateLabelStyle));
    }

    public static string GetName(G2DPlotStyleCollection coll)
    {
      return (string)_NamesByTypeArray[GetTypeArray(coll)];
    }

    static TypeArray GetTypeArray(G2DPlotStyleCollection coll)
    {
      System.Type[] types = new Type[coll.Count];
      for (int i = 0; i < types.Length; i++)
        types[i] = coll[i].GetType();

      return new TypeArray(types);
    }

    public static string[] GetAvailableNames()
    {
      return (string[])_NamesInOrder.ToArray(typeof(string));
    }

    public static string[] GetAvailableNamesPlusCustom()
    {
      string[] result = new string[_NamesInOrder.Count + 1];
      result[0] = "Custom";
      for (int i = 1; i < result.Length; i++)
        result[i] = (string)_NamesInOrder[i - 1];
      return result;
    }

    public static int GetIndexOfAvailableNamesPlusCustom(string name)
    {
      int result = _NamesInOrder.IndexOf(name);
      return result < 0 ? 0 : result + 1;
    }

    public static int GetIndexOfAvailableNamesPlusCustom(G2DPlotStyleCollection coll)
    {
      string name = GetName(coll);
      if (null == name)
        return 0;

      int result = _NamesInOrder.IndexOf(name);
      return result < 0 ? 0 : result + 1;
    }

    public static G2DPlotStyleCollection GetTemplate(string name)
    {
      CreateCollectionProcedure proc = (CreateCollectionProcedure)_CreationProcByName[name];
      if (null != proc)
        return proc();
      else
        return null;
    }

    public static G2DPlotStyleCollection GetTemplate(int idx)
    {
      return GetTemplate((string)_NamesInOrder[idx]);
    }

    public static void Add(string name, CreateCollectionProcedure procedure)
    {
      if (_CreationProcByName.ContainsKey(name))
        throw new Exception(string.Format("Template {0} is already present in the template collection", name));

      G2DPlotStyleCollection coll = procedure();
      if (coll == null || coll.Count == 0)
        throw new Exception(string.Format("Procedure for template {0} creates no or an empty collection.", name));

      _NamesInOrder.Add(name);
      _CreationProcByName.Add(name, procedure);
      _NamesByTypeArray.Add(GetTypeArray(coll), name);
    }


    static G2DPlotStyleCollection CreateLineStyle()
    {
      G2DPlotStyleCollection coll = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new LinePlotStyle());
      return coll;
    }
    static G2DPlotStyleCollection CreateScatterStyle()
    {
      G2DPlotStyleCollection coll = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new ScatterPlotStyle());
      return coll;
    }
    static G2DPlotStyleCollection CreateScatterAndLineStyle()
    {
      G2DPlotStyleCollection coll = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new ScatterPlotStyle());
      coll.Add(new LinePlotStyle());
      return coll;
    }

    static G2DPlotStyleCollection CreateLabelAndScatterAndLineStyle()
    {
      G2DPlotStyleCollection coll = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new LabelPlotStyle());
      coll.Add(new ScatterPlotStyle());
      coll.Add(new LinePlotStyle());
      return coll;
    }

    static G2DPlotStyleCollection CreateLabelStyle()
    {
      G2DPlotStyleCollection coll = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new LabelPlotStyle());
      return coll;
    }
  }
}
