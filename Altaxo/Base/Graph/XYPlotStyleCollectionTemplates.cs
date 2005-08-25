using System;
using System.Text;
using System.Collections;

namespace Altaxo.Graph
{
  public class XYPlotStyleCollectionTemplates
  {

    #region Inner Classes

    public delegate XYPlotStyleCollection CreateCollectionProcedure();

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
    static XYPlotStyleCollectionTemplates()
    {
    _NamesByTypeArray = new Hashtable();
    _CreationProcByName = new Hashtable();
    _NamesInOrder = new ArrayList();

    Add("Line", new CreateCollectionProcedure(CreateLineStyle));
    Add("Scatter", new CreateCollectionProcedure(CreateScatterStyle));
    Add("Line+Scatter", new CreateCollectionProcedure(CreateLineAndScatterStyle));
    Add("Line+Scatter+Label",new CreateCollectionProcedure(CreateLineAndScatterAndLabelStyle));
    Add("Label only", new CreateCollectionProcedure(CreateLabelStyle));
  }

    public static string GetName(XYPlotStyleCollection coll)
    {
      return (string)_NamesByTypeArray[GetTypeArray(coll)];
    }

    static TypeArray GetTypeArray(XYPlotStyleCollection coll)
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

    public static int GetIndexOfAvailableNamesPlusCustom(XYPlotStyleCollection coll)
    {
      string name = GetName(coll);
      if (null == name)
        return 0;

      int result = _NamesInOrder.IndexOf(name);
      return result < 0 ? 0 : result + 1;
    }

    public static XYPlotStyleCollection GetTemplate(string name)
    {
      CreateCollectionProcedure proc = (CreateCollectionProcedure)_CreationProcByName[name];
      if (null != proc)
        return proc();
      else
        return null;
    }

    public static XYPlotStyleCollection GetTemplate(int idx)
    {
      return GetTemplate((string)_NamesInOrder[idx]);
    }

    public static void Add(string name, CreateCollectionProcedure procedure)
    {
      if (_CreationProcByName.ContainsKey(name))
        throw new Exception(string.Format("Template {0} is already present in the template collection", name));

      XYPlotStyleCollection coll = procedure();
      if (coll == null || coll.Count == 0)
        throw new Exception(string.Format("Procedure for template {0} creates no or an empty collection.", name));

      _NamesInOrder.Add(name);
      _CreationProcByName.Add(name, procedure);
      _NamesByTypeArray.Add(GetTypeArray(coll), name);
    }


    static XYPlotStyleCollection CreateLineStyle()
    {
      return new XYPlotStyleCollection(LineScatterPlotStyleKind.Line); 
    }
    static XYPlotStyleCollection CreateScatterStyle()
    {
      return new XYPlotStyleCollection(LineScatterPlotStyleKind.Scatter);
    }
    static XYPlotStyleCollection CreateLineAndScatterStyle()
    {
      return new XYPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
    }

    static XYPlotStyleCollection CreateLineAndScatterAndLabelStyle()
    {
      XYPlotStyleCollection coll = new XYPlotStyleCollection(LineScatterPlotStyleKind.LineAndScatter);
      coll.Add(new XYPlotLabelStyle());
      return coll;
    }

    static XYPlotStyleCollection CreateLabelStyle()
    {
      XYPlotStyleCollection coll = new XYPlotStyleCollection(LineScatterPlotStyleKind.Empty);
      coll.Add(new XYPlotLabelStyle());
      return coll;
    }
  }
}
