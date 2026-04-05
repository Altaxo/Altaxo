#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// Provides named templates for <see cref="G3DPlotStyleCollection"/> instances.
  /// </summary>
  public class G3DPlotStyleCollectionTemplates
  {
    #region Inner Classes

    /// <summary>
    /// Represents a factory method that creates a plot-style collection.
    /// </summary>
    /// <param name="context">The property context used for initialization.</param>
    /// <returns>A plot-style collection.</returns>
    public delegate G3DPlotStyleCollection CreateCollectionProcedure(Altaxo.Main.Properties.IReadOnlyPropertyBag context);

    private class TypeArray
    {
      private System.Type[] _types;

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

      public override bool Equals(object? obj)
      {
        if (!(obj is TypeArray))
          return false;

        var from = (TypeArray)obj;
        for (int i = 0; i < _types.Length; i++)
          if (_types[i] != from._types[i])
            return false;

        return true;
      }
    }

    #endregion Inner Classes

    private static Dictionary<TypeArray, string> _NamesByTypeArray;
    private static Dictionary<string, CreateCollectionProcedure> _CreationProcByName;
    private static List<string> _NamesInOrder;

    static G3DPlotStyleCollectionTemplates()
    {
      _NamesByTypeArray = new Dictionary<TypeArray, string>();
      _CreationProcByName = new Dictionary<string, CreateCollectionProcedure>();
      _NamesInOrder = new List<string>();

      //Add("Line", new CreateCollectionProcedure(CreateLineStyle));
      Add("Scatter", new CreateCollectionProcedure(CreateScatterStyle));
      //Add("Scatter+Line", new CreateCollectionProcedure(CreateScatterAndLineStyle));
      //Add("Label+Scatter+Line", new CreateCollectionProcedure(CreateLabelAndScatterAndLineStyle));
      //Add("Label only", new CreateCollectionProcedure(CreateLabelStyle));
    }

    /// <summary>
    /// Gets the template name for a given style collection.
    /// </summary>
    /// <param name="coll">The style collection.</param>
    /// <returns>The template name.</returns>
    public static string GetName(G3DPlotStyleCollection coll)
    {
      return (string)_NamesByTypeArray[GetTypeArray(coll)];
    }

    private static TypeArray GetTypeArray(G3DPlotStyleCollection coll)
    {
      var types = new Type[coll.Count];
      for (int i = 0; i < types.Length; i++)
        types[i] = coll[i].GetType();

      return new TypeArray(types);
    }

    /// <summary>
    /// Gets the available template names.
    /// </summary>
    /// <returns>The available template names.</returns>
    public static string[] GetAvailableNames()
    {
      return _NamesInOrder.ToArray();
    }

    /// <summary>
    /// Gets the available template names including the custom entry.
    /// </summary>
    /// <returns>The available template names including <c>Custom</c>.</returns>
    public static string[] GetAvailableNamesPlusCustom()
    {
      string[] result = new string[_NamesInOrder.Count + 1];
      result[0] = "Custom";
      for (int i = 1; i < result.Length; i++)
        result[i] = _NamesInOrder[i - 1];
      return result;
    }

    /// <summary>
    /// Gets the index of a template name in the list returned by <see cref="GetAvailableNamesPlusCustom"/>.
    /// </summary>
    /// <param name="name">The template name.</param>
    /// <returns>The template index.</returns>
    public static int GetIndexOfAvailableNamesPlusCustom(string name)
    {
      int result = _NamesInOrder.IndexOf(name);
      return result < 0 ? 0 : result + 1;
    }

    /// <summary>
    /// Gets the index of a style collection in the list returned by <see cref="GetAvailableNamesPlusCustom"/>.
    /// </summary>
    /// <param name="coll">The style collection.</param>
    /// <returns>The template index.</returns>
    public static int GetIndexOfAvailableNamesPlusCustom(G3DPlotStyleCollection coll)
    {
      string name = GetName(coll);
      if (name is null)
        return 0;

      int result = _NamesInOrder.IndexOf(name);
      return result < 0 ? 0 : result + 1;
    }

    /// <summary>
    /// Gets a style-collection template by name.
    /// </summary>
    /// <param name="name">The template name.</param>
    /// <param name="context">The property context.</param>
    /// <returns>The template collection, or <see langword="null"/>.</returns>
    public static G3DPlotStyleCollection? GetTemplate(string name, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_CreationProcByName.TryGetValue(name, out var proc))
        return proc(context);
      else
        return null;
    }

    /// <summary>
    /// Gets a style-collection template by index.
    /// </summary>
    /// <param name="idx">The template index.</param>
    /// <param name="context">The property context.</param>
    /// <returns>The template collection, or <see langword="null"/>.</returns>
    public static G3DPlotStyleCollection? GetTemplate(int idx, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      return GetTemplate(_NamesInOrder[idx], context);
    }

    /// <summary>
    /// Adds a new named template.
    /// </summary>
    /// <param name="name">The template name.</param>
    /// <param name="procedure">The template factory.</param>
    public static void Add(string name, CreateCollectionProcedure procedure)
    {
      if (_CreationProcByName.ContainsKey(name))
        throw new Exception(string.Format("Template {0} is already present in the template collection", name));

      var coll = procedure(PropertyExtensions.GetPropertyContextOfProject());
      if (coll is null || coll.Count == 0)
        throw new Exception(string.Format("Procedure for template {0} creates no or an empty collection.", name));

      _NamesInOrder.Add(name);
      _CreationProcByName.Add(name, procedure);
      _NamesByTypeArray.Add(GetTypeArray(coll), name);
    }

    private static G3DPlotStyleCollection CreateScatterStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      var coll = new G3DPlotStyleCollection
      {
        new ScatterPlotStyle(context)
      };
      return coll;
    }

    /*

        private static G3DPlotStyleCollection CreateLineStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
        {
            var coll = new G3DPlotStyleCollection(LineScatterPlotStyleKind.Empty, context);
            coll.Add(new LinePlotStyle(context));
            return coll;
        }

        private static G3DPlotStyleCollection CreateScatterAndLineStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
        {
            var coll = new G3DPlotStyleCollection(LineScatterPlotStyleKind.Empty, context);
            coll.Add(new ScatterPlotStyle(context));
            coll.Add(new LinePlotStyle(context));
            return coll;
        }

        private static G3DPlotStyleCollection CreateLabelAndScatterAndLineStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
        {
            var coll = new G3DPlotStyleCollection(LineScatterPlotStyleKind.Empty, context);
            coll.Add(new LabelPlotStyle(context));
            coll.Add(new ScatterPlotStyle(context));
            coll.Add(new LinePlotStyle(context));
            return coll;
        }

        private static G3DPlotStyleCollection CreateLabelStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
        {
            var coll = new G3DPlotStyleCollection(LineScatterPlotStyleKind.Empty, context);
            coll.Add(new LabelPlotStyle(context));
            return coll;
        }

    */
  }
}
