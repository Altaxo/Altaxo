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

#nullable enable
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main.Services.ScriptCompilation;

namespace Altaxo.Scripting
{
  public interface IFitFunctionScriptText : IScriptText, IFitFunction
  {
    /// <summary>
    /// Returns true if the script uses user defined parameter names instead of using P[0], P[1] ...
    /// </summary>
    bool IsUsingUserDefinedParameterNames { get; set; }

    /// <summary>
    /// Get / sets the user defined parameter names. If setting, this also sets the property
    /// <see cref="IsUsingUserDefinedParameterNames" /> to true, and the <see cref="NumberOfParameters" /> to the given number
    /// of user defined parameters.
    /// </summary>
    string[]? UserDefinedParameterNames { get; set; }

    string[] DependentVariablesNames { set; }

    string[] IndependentVariablesNames { set; }

    new int NumberOfParameters { get; set; }

    void CopyFrom(IFitFunctionScriptText from, bool forModification);

    /// <summary>
    /// Date/Time of creating this fit function.
    /// </summary>
    DateTime CreationTime { get; }

    /// <summary>
    /// Name of the fit function.
    /// </summary>
    string FitFunctionName { get; set; }

    /// <summary>
    /// Category of the fit function. If the fit function category contains PathSeparatorChars, than the
    /// parts of the string that are separated by these chars are treated as separate sub-categories.
    /// </summary>
    string FitFunctionCategory { get; set; }

    /// <summary>
    /// Descriptional text for that fit function.
    /// </summary>
    string FitFunctionDescription { get; set; }
  }

  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a property column script.
  /// </summary>

  public class FitFunctionScript : AbstractScript, IFitFunctionScriptText, IFitFunction
  {
    /// <summary>
    /// True if we use user defined parameter names in the script.
    /// </summary>
    private bool _IsUsingUserDefinedParameterNames = true;

    /// <summary>
    /// Number of Parameters
    /// </summary>
    private int _NumberOfParameters = 2;

    /// <summary>
    /// Names of the parameters. This is set to null if no parameter names where provided.
    /// </summary>
    private string[]? _UserDefinedParameterNames = new string[] { "A", "B" };

    private string[] _IndependentVariablesNames = new string[] { "x" };
    private string[] _DependentVariablesNames = new string[] { "y" };

    private string _fitFunctionName = "User1";
    private string _fitFunctionCategory = "UserCat";
    private DateTime _fitFunctionCreationTime = DateTime.Now;
    private string _fitFunctionDescription = string.Empty;

    public DateTime CreationTime
    {
      get { return _fitFunctionCreationTime; }
    }

    /// <summary>
    /// Name of the fit function.
    /// </summary>
    public string FitFunctionName
    {
      get { return _fitFunctionName; }
      set { _fitFunctionName = value; }
    }

    /// <summary>
    /// Category of the fit function. If the fit function category contains PathSeparatorChars, than the
    /// parts of the string that are separated by these chars are treated as separate sub-categories.
    /// </summary>
    public string FitFunctionCategory
    {
      get { return _fitFunctionCategory; }
      set { _fitFunctionCategory = value; }
    }

    /// <summary>
    /// Descriptional text for that fit function.
    /// </summary>
    public string FitFunctionDescription
    {
      get { return _fitFunctionDescription; }
      set { _fitFunctionDescription = value; }
    }

    public override bool Equals(object? obj)
    {
      return obj is FitFunctionScript from &&
                      base.Equals(from) &&
                      this.FitFunctionCategory == from.FitFunctionCategory &&
                      this.FitFunctionName == from.FitFunctionName;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + _fitFunctionCategory.GetHashCode() + _fitFunctionName.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("FitFunctionScript {0} (created {1})", FitFunctionName, CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private FitFunctionScript? _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbstractScript)obj;

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FitFunctionScript?)o ?? new FitFunctionScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        var surr = new XmlSerializationSurrogate0
        {
          _deserializedObject = s
        };
        info.DeserializationFinished += surr.EhDeserializationFinished;

        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinalCall)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);

        if (documentRoot is AltaxoDocument doc)
        {
          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject!);
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private FitFunctionScript? _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitFunctionScript)obj;

        // Update the user defined paramter names
        if (s._scriptObject is not null && s.IsUsingUserDefinedParameterNames)
        {
          var ff = (IFitFunction)s._scriptObject;
          if (s._UserDefinedParameterNames is null || s._UserDefinedParameterNames.Length != ff.NumberOfParameters)
            s._UserDefinedParameterNames = new string[ff.NumberOfParameters];
          for (int i = 0; i < ff.NumberOfParameters; ++i)
            s._UserDefinedParameterNames[i] = ff.ParameterName(i);
        }

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));

        info.AddValue("Category", s.FitFunctionCategory);
        info.AddValue("Name", s.FitFunctionName);
        info.AddValue("CreationTime", s._fitFunctionCreationTime);

        info.AddValue("NumberOfParameters", s.NumberOfParameters);
        info.AddValue("UserDefinedParameters", s.IsUsingUserDefinedParameterNames);
        if (s.IsUsingUserDefinedParameterNames)
        {
          info.AddArray("UserDefinedParameterNames", s._UserDefinedParameterNames, s._UserDefinedParameterNames.Length);
        }

        info.AddArray("IndependentVariableNames", s._IndependentVariablesNames, s._IndependentVariablesNames.Length);
        info.AddArray("DependentVariableNames", s._DependentVariablesNames, s._DependentVariablesNames.Length);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FitFunctionScript?)o ?? new FitFunctionScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        s._fitFunctionCategory = info.GetString("Category");
        s._fitFunctionName = info.GetString("Name");
        s._fitFunctionCreationTime = info.GetDateTime("CreationTime");
        s._NumberOfParameters = info.GetInt32("NumberOfParameters");
        s._IsUsingUserDefinedParameterNames = info.GetBoolean("UserDefinedParameters");
        if (s._IsUsingUserDefinedParameterNames)
          s._UserDefinedParameterNames = info.GetArrayOfStrings("UserDefinedParameterNames");
        s._IndependentVariablesNames = info.GetArrayOfStrings("IndependentVariableNames");
        s._DependentVariablesNames = info.GetArrayOfStrings("DependentVariableNames");

        var surr = new XmlSerializationSurrogate1
        {
          _deserializedObject = s
        };
        info.DeserializationFinished += surr.info_DeserializationFinished;

        if (s.IsUsingUserDefinedParameterNames && s._NumberOfParameters != s._UserDefinedParameterNames.Length)
          s.Compile(); // dirty quick fix in the case that the userdefined parameters where not updated before serialization

        return s;
      }

      private void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(info_DeserializationFinished);

        if (documentRoot is AltaxoDocument doc)
        {
          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject!);
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private FitFunctionScript? _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitFunctionScript)obj;

        // Update the user defined paramter names
        if (s._scriptObject is not null && s.IsUsingUserDefinedParameterNames)
        {
          var ff = (IFitFunction)s._scriptObject;
          if (s._UserDefinedParameterNames is null || s._UserDefinedParameterNames.Length != ff.NumberOfParameters)
            s._UserDefinedParameterNames = new string[ff.NumberOfParameters];
          for (int i = 0; i < ff.NumberOfParameters; ++i)
            s._UserDefinedParameterNames[i] = ff.ParameterName(i);
        }

        info.AddValue("Category", s.FitFunctionCategory);
        info.AddValue("Name", s.FitFunctionName);
        info.AddValue("CreationTime", s._fitFunctionCreationTime);
        info.AddValue("Description", s.FitFunctionName);

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));

        info.AddValue("NumberOfParameters", s.NumberOfParameters);
        info.AddValue("UserDefinedParameters", s.IsUsingUserDefinedParameterNames);
        if (s.IsUsingUserDefinedParameterNames)
        {
          info.AddArray("UserDefinedParameterNames", s._UserDefinedParameterNames, s._UserDefinedParameterNames.Length);
        }

        info.AddArray("IndependentVariableNames", s._IndependentVariablesNames, s._IndependentVariablesNames.Length);
        info.AddArray("DependentVariableNames", s._DependentVariablesNames, s._DependentVariablesNames.Length);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FitFunctionScript?)o ?? new FitFunctionScript();

        s._fitFunctionCategory = info.GetString("Category");
        s._fitFunctionName = info.GetString("Name");
        s._fitFunctionCreationTime = info.GetDateTime("CreationTime");
        s._fitFunctionDescription = info.GetString("Description");

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        s._NumberOfParameters = info.GetInt32("NumberOfParameters");
        s._IsUsingUserDefinedParameterNames = info.GetBoolean("UserDefinedParameters");
        if (s._IsUsingUserDefinedParameterNames)
          s._UserDefinedParameterNames = info.GetArrayOfStrings("UserDefinedParameterNames");
        s._IndependentVariablesNames = info.GetArrayOfStrings("IndependentVariableNames");
        s._DependentVariablesNames = info.GetArrayOfStrings("DependentVariableNames");

        var surr = new XmlSerializationSurrogate2
        {
          _deserializedObject = s
        };
        info.DeserializationFinished += surr.EhDeserializationFinished;

        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinalCall)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);

        if (documentRoot is AltaxoDocument doc)
        {
          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject!);
        }
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates an empty script.
    /// </summary>
    public FitFunctionScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public FitFunctionScript(FitFunctionScript b)
        : base(b, false)
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="from">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public FitFunctionScript(FitFunctionScript from, bool forModification)
        : base(from, forModification)
    {
      CopyInstanceMembersFrom(from);
    }

    private void CopyInstanceMembersFrom(FitFunctionScript from)
    {
      _IsUsingUserDefinedParameterNames = from._IsUsingUserDefinedParameterNames;
      _NumberOfParameters = from._NumberOfParameters;
      _UserDefinedParameterNames = from._UserDefinedParameterNames is null ? null : (string[])from._UserDefinedParameterNames.Clone();
      _IndependentVariablesNames = (string[])from._IndependentVariablesNames.Clone();
      _DependentVariablesNames = (string[])from._DependentVariablesNames.Clone();
      _fitFunctionName = from._fitFunctionName;
      _fitFunctionCategory = from.FitFunctionCategory;
      _fitFunctionCreationTime = from._fitFunctionCreationTime;
    }

    public void CopyFrom(FitFunctionScript from, bool forModification)
    {
      if (ReferenceEquals(this, from))
        return;

      base.CopyFrom(from, forModification);
      CopyInstanceMembersFrom(from);
    }

    void IFitFunctionScriptText.CopyFrom(IFitFunctionScriptText from, bool forModification)
    {
      CopyFrom((FitFunctionScript)from, forModification);
    }

    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.MyFitFunction"; }
    }

    public override string ScriptText
    {
      get
      {
        return base.ScriptText;
      }
      set
      {
        if (!IsReadOnly && _scriptText != value)
          _fitFunctionCreationTime = DateTime.Now;
        base.ScriptText = value;
      }
    }

    private static bool AreEqual<T>(T a, T b) where T : IEquatable<T>
    {
      if (!object.ReferenceEquals(null, a))
        return a.Equals(b);
      else if (!object.ReferenceEquals(null, b))
        return b.Equals(a);
      else // both null
        return true;
    }

    private static bool ObservedAssign<T>(ref T toSet, T source) where T : IEquatable<T>
    {
      if (!(AreEqual(toSet, source)))
      {
        toSet = source;
        return true;
      }
      else
      {
        return false;
      }
    }

    private static bool ObservedAllocateArray<T>(ref T[] existingArray, int newCount)
    {
      if (existingArray is null || existingArray.Length != newCount)
      {
        existingArray = new T[newCount];
        return true;
      }
      else
      {
        return false;
      }
    }

    public override bool Compile()
    {
      bool success = base.Compile();

      if (success && (_scriptObject is IFitFunction))
      {
        var ff = (IFitFunction)_scriptObject;

        bool hasChanged = false;

        hasChanged |= ObservedAssign(ref _NumberOfParameters, ff.NumberOfParameters);

        if (IsUsingUserDefinedParameterNames)
        {
          hasChanged = ObservedAllocateArray(ref _UserDefinedParameterNames, ff.NumberOfParameters);
          for (int i = 0; i < ff.NumberOfParameters; ++i)
            hasChanged = ObservedAssign(ref _UserDefinedParameterNames[i], ff.ParameterName(i));
        }

        hasChanged = ObservedAllocateArray(ref _IndependentVariablesNames, ff.NumberOfIndependentVariables);
        for (int i = 0; i < _IndependentVariablesNames.Length; ++i)
          hasChanged = ObservedAssign(ref _IndependentVariablesNames[i], ff.IndependentVariableName(i));

        hasChanged = ObservedAllocateArray(ref _DependentVariablesNames, ff.NumberOfDependentVariables);
        for (int i = 0; i < _DependentVariablesNames.Length; ++i)
          hasChanged = ObservedAssign(ref _DependentVariablesNames[i], ff.DependentVariableName(i));

        if (hasChanged)
          EhSelfChanged(EventArgs.Empty);
      }

      return success;
    }

    #region Text Definitions

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public override string CodeHeader
    {
      get
      {
        return
            "#region ScriptHeader\r\n" +
            "using System;\r\n" +
            "using Altaxo;\r\n" +
            "using Altaxo.Calc;\r\n" +
            "using Altaxo.Data;\r\n" +
            "using Altaxo.Calc.Regression.Nonlinear;\r\n" +
            "namespace Altaxo.Calc\r\n" +
            "{\r\n" +
            "\tpublic class MyFitFunction : Altaxo.Calc.FitFunctionExeBase\r\n" +
            "\t{\r\n" +
            "\t\tpublic MyFitFunction()\r\n" +
            "\t\t{\r\n" +
            DefinitionRegionIndentation + IndependentDefinitionRegionStart +
            IndependentDefinitionRegionCore +
            DefinitionRegionIndentation + IndependentDefinitionRegionEnd +
            "\r\n" +
            DefinitionRegionIndentation + DependentDefinitionRegionStart +
            DependentDefinitionRegionCore +
            DefinitionRegionIndentation + DependentDefinitionRegionEnd +
            "\r\n" +
            DefinitionRegionIndentation + ParameterDefinitionRegionStart +
            ParameterDefinitionRegionCore +
            DefinitionRegionIndentation + ParameterDefinitionRegionEnd +
            "\t\t}\r\n" +
            "\r\n" +
            "\t\tpublic override void Evaluate(double[] X, double[] P, double[] Y)\r\n" +
            "\t\t{\r\n" +
            AssignmentRegionIndentation + IndependentAssignmentRegionStart +
            IndependentAssignmentRegionCore +
            AssignmentRegionIndentation + IndependentAssignmentRegionEnd +
            "\r\n" +
            AssignmentRegionIndentation + ParameterAssignmentRegionStart +
            ParameterAssignmentRegionCore +
            AssignmentRegionIndentation + ParameterAssignmentRegionEnd +
            "\r\n" +
            AssignmentRegionIndentation + DependentDeclarationRegionStart +
            DependentDeclarationRegionCore +
            AssignmentRegionIndentation + DependentDeclarationRegionEnd;
      }
    }

    public string DefinitionRegionIndentation { get => "\t\t\t"; }

    public string IndependentDefinitionRegionStart
    {
      get
      {
        return "#region Independent Variable Definition\r\n";
      }
    }

    public string IndependentDefinitionRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        stb.Append(DefinitionRegionIndentation);
        stb.Append("_independentVariableNames = new string[]{");

        for (int i = 0; i < _IndependentVariablesNames.Length; i++)
        {
          stb.Append("\"" + _IndependentVariablesNames[i] + "\"");
          if ((i + 1) == _IndependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }

        return stb.ToString();
      }
    }

    public string IndependentDefinitionRegionEnd
    {
      get
      {
        return "#endregion //  Independent Variable Definition\r\n";
      }
    }

    public string DependentDefinitionRegionStart
    {
      get
      {
        return "#region Dependent Variable Definition\r\n";
      }
    }

    public string DependentDefinitionRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        stb.Append(DefinitionRegionIndentation);
        stb.Append("_dependentVariableNames = new string[]{");

        for (int i = 0; i < _DependentVariablesNames.Length; i++)
        {
          stb.Append("\"" + _DependentVariablesNames[i] + "\"");
          if ((i + 1) == _DependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }

        return stb.ToString();
      }
    }

    public string DependentDefinitionRegionEnd
    {
      get
      {
        return "#endregion //  Dependent Variable Definition\r\n";
      }
    }

    public string ParameterDefinitionRegionStart
    {
      get
      {
        return "#region Parameter Definition\r\n";
      }
    }

    public string ParameterDefinitionRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        stb.Append(DefinitionRegionIndentation);
        stb.Append("_parameterNames = new string[]{");

        for (int i = 0; i < NumberOfParameters; i++)
        {
          stb.Append("\"" + ParameterName(i, false) + "\"");
          if ((i + 1) < NumberOfParameters)
            stb.Append(",");
        }
        stb.Append("};\r\n");

        return stb.ToString();
      }
    }

    public string ParameterDefinitionRegionEnd
    {
      get
      {
        return "#endregion // Parameter Definition\r\n";
      }
    }

    public string AssignmentRegionIndentation { get => "\t\t\t"; }

    public string IndependentAssignmentRegionStart
    {
      get
      {
        return "#region Independent Variable Assignment\r\n";
      }
    }

    public string IndependentAssignmentRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        for (int i = 0; i < _IndependentVariablesNames.Length; i++)
        {
          stb.Append(AssignmentRegionIndentation);
          stb.Append("double ");
          stb.Append(_IndependentVariablesNames[i]);
          stb.Append(" = X[");
          stb.Append(i.ToString());
          stb.Append("];\r\n");
        }
        return stb.ToString();
      }
    }

    public string IndependentAssignmentRegionEnd
    {
      get
      {
        return "#endregion //  Independent Variable Assignment\r\n";
      }
    }

    public string ParameterAssignmentRegionStart
    {
      get
      {
        return "#region Parameter Assignment\r\n";
      }
    }

    public string ParameterAssignmentRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        if (IsUsingUserDefinedParameterNames)
        {
          for (int i = 0; i < NumberOfParameters; i++)
          {
            stb.Append(AssignmentRegionIndentation);
            stb.Append("double ");
            stb.Append(ParameterName(i));
            stb.Append(" = P[");
            stb.Append(i.ToString());
            stb.Append("];\r\n");
          }
        }
        return stb.ToString();
      }
    }

    public string ParameterAssignmentRegionEnd
    {
      get
      {
        return "#endregion // Parameter Assignment\r\n";
      }
    }

    public string DependentDeclarationRegionStart
    {
      get
      {
        return "#region ExpertsOnly - Dependent Variable Declaration\r\n";
      }
    }

    public string DependentDeclarationRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        for (int i = 0; i < _DependentVariablesNames.Length; i++)
        {
          stb.Append(AssignmentRegionIndentation);
          stb.Append("double ");
          stb.Append(_DependentVariablesNames[i]);
          stb.Append(";\r\n");
        }
        return stb.ToString();
      }
    }

    public string DependentDeclarationRegionEnd
    {
      get
      {
        return "#endregion // Dependent Variable Declaration\r\n";
      }
    }

    public string DependentAssignmentRegionStart
    {
      get
      {
        return "#region Dependent Variable Assignment\r\n";
      }
    }

    public string DependentAssignmentRegionCore
    {
      get
      {
        var stb = new System.Text.StringBuilder();
        for (int i = 0; i < _DependentVariablesNames.Length; i++)
        {
          stb.Append(AssignmentRegionIndentation);
          stb.Append("Y[" + i.ToString() + "] = ");
          stb.Append(_DependentVariablesNames[i]);
          stb.Append(";\r\n");
        }
        return stb.ToString();
      }
    }

    public string DependentAssignmentRegionEnd
    {
      get
      {
        return "#endregion // Dependent Variable Assignment\r\n";
      }
    }

    public override string CodeStart
    {
      get
      {
        return
            "#endregion // ScriptHeader\r\n" +
            "\t\t\t// ----- add your script below this line -----\r\n";
      }
    }

    public override string CodeUserDefault
    {
      get
      {
        return
            "\t\t\t\r\n" +
            "\t\t\ty = P[0]+P[1]*Sin(x);\r\n" +
            "\t\t\t\r\n"
            ;
      }
    }

    public override string CodeEnd
    {
      get
      {
        return
            "\t\t\t// ----- add your script above this line -----\r\n" +
            "#region ScriptFooter\r\n";
      }
    }

    /// <summary>
    /// Get the ending text of the script, dependent on the ScriptStyle.
    /// </summary>
    public override string CodeTail
    {
      get
      {
        return

            AssignmentRegionIndentation + DependentAssignmentRegionStart +
            DependentAssignmentRegionCore +
            AssignmentRegionIndentation + DependentAssignmentRegionEnd +
            "\t\t} // method\r\n" +
            "\t} // class\r\n" +
            "} //namespace\r\n" +
            "#endregion\r\n";
      }
    }

    #endregion Text Definitions

    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public override object Clone()
    {
      return new FitFunctionScript(this, true);
    }

    public string[] DependentVariablesNames
    {
      set
      {
        System.Text.StringBuilder sb;
        int first, last;

        _DependentVariablesNames = (string[])value.Clone();
        string[] names = value;

        first = ScriptText.IndexOf(DependentDefinitionRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables start region");
        first += DependentDefinitionRegionStart.Length;
        last = ScriptText.IndexOf(DependentDefinitionRegionEnd);
        if (last < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable definition end region");
        sb = new System.Text.StringBuilder();
        sb.Append(ScriptText.Substring(0, first));
        sb.Append(DependentDefinitionRegionCore);
        sb.Append(DefinitionRegionIndentation);
        sb.Append(ScriptText.Substring(last));
        ScriptText = sb.ToString();

        first = ScriptText.IndexOf(DependentDeclarationRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain an dependent variables declaration start region");
        first += DependentDeclarationRegionStart.Length;
        last = ScriptText.IndexOf(DependentDeclarationRegionEnd);
        if (last < 0)
          throw new ApplicationException("The script text seems to no longer contain an dependent variable declaration end region");

        sb = new System.Text.StringBuilder();
        sb.Append(ScriptText.Substring(0, first));
        sb.Append(DependentDeclarationRegionCore);
        sb.Append(DefinitionRegionIndentation);
        sb.Append(ScriptText.Substring(last));
        ScriptText = sb.ToString();

        first = ScriptText.IndexOf(DependentAssignmentRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables assignment start region");
        first += DependentAssignmentRegionStart.Length;
        last = ScriptText.IndexOf(DependentAssignmentRegionEnd);
        if (last < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable assignment end region");
        sb = new System.Text.StringBuilder();
        sb.Append(ScriptText.Substring(0, first));
        sb.Append(DependentAssignmentRegionCore);
        sb.Append(AssignmentRegionIndentation);
        sb.Append(ScriptText.Substring(last));
        ScriptText = sb.ToString();
      }
    }

    public string[] IndependentVariablesNames
    {
      set
      {
        System.Text.StringBuilder sb;
        int first, last;
        _IndependentVariablesNames = (string[])value.Clone();

        string[] names = value;

        first = ScriptText.IndexOf(IndependentDefinitionRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain an independent variables definition start region");
        first += IndependentDefinitionRegionStart.Length;
        last = ScriptText.IndexOf(IndependentDefinitionRegionEnd);
        if (last < 0)
          throw new ApplicationException("The script text seems to no longer contain an independent variable definition end region");

        sb = new System.Text.StringBuilder();
        sb.Append(ScriptText.Substring(0, first));
        sb.Append(IndependentDefinitionRegionCore);
        sb.Append(DefinitionRegionIndentation);
        sb.Append(ScriptText.Substring(last));
        ScriptText = sb.ToString();

        first = ScriptText.IndexOf(IndependentAssignmentRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables assignment start region");
        first += IndependentAssignmentRegionStart.Length;
        last = ScriptText.IndexOf(IndependentAssignmentRegionEnd);
        if (last < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable assignment end region");

        sb = new System.Text.StringBuilder();
        sb.Append(ScriptText.Substring(0, first));
        sb.Append(IndependentAssignmentRegionCore);
        sb.Append(AssignmentRegionIndentation);
        sb.Append(ScriptText.Substring(last));
        ScriptText = sb.ToString();
      }
    }

    public bool IsUsingUserDefinedParameterNames
    {
      [MemberNotNullWhen(true, nameof(_UserDefinedParameterNames))]
      get
      {
        return _IsUsingUserDefinedParameterNames && !(_UserDefinedParameterNames is null);
      }
      set
      {
        if (value == true && _IsUsingUserDefinedParameterNames == false)
        {
          var oldNames = _UserDefinedParameterNames;
          if (oldNames is null)
            oldNames = new string[0];

          string[] newNames = new string[_NumberOfParameters];

          int len = Math.Min(oldNames.Length, newNames.Length);
          for (int i = 0; i < len; ++i)
            newNames[i] = oldNames[i];
          for (int i = len; i < newNames.Length; ++i)
            newNames[i] = "P" + i.ToString();

          UserDefinedParameterNames = newNames;
        }

        _IsUsingUserDefinedParameterNames = value;
        SetParametersInScript();
      }
    }

    private void SetParametersInScript()
    {
      System.Text.StringBuilder sb;
      int first, last;

      first = ScriptText.IndexOf(ParameterDefinitionRegionStart);
      if (first < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter definition start region");
      first += ParameterDefinitionRegionStart.Length;
      last = ScriptText.IndexOf(ParameterDefinitionRegionEnd);
      if (last < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter definition end region");
      sb = new System.Text.StringBuilder();
      sb.Append(ScriptText.Substring(0, first));
      sb.Append(ParameterDefinitionRegionCore);
      sb.Append(DefinitionRegionIndentation);
      sb.Append(ScriptText.Substring(last));
      ScriptText = sb.ToString();

      first = ScriptText.IndexOf(ParameterAssignmentRegionStart);
      if (first < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter assignment start region");
      first += ParameterAssignmentRegionStart.Length;
      last = ScriptText.IndexOf(ParameterAssignmentRegionEnd);
      if (last < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter assignment end region");
      sb = new System.Text.StringBuilder();
      sb.Append(ScriptText.Substring(0, first));
      sb.Append(ParameterAssignmentRegionCore);
      sb.Append(AssignmentRegionIndentation);
      sb.Append(ScriptText.Substring(last));
      ScriptText = sb.ToString();
    }

    public string[]? UserDefinedParameterNames
    {
      get
      {
        if (IsUsingUserDefinedParameterNames)
          return (string[])_UserDefinedParameterNames.Clone();
        else
          return null;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        var sb = new System.Text.StringBuilder();

        _IsUsingUserDefinedParameterNames = true;
        _NumberOfParameters = value.Length;
        _UserDefinedParameterNames = (string[])value.Clone();

        SetParametersInScript();
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="x">The main function argument.</param>
    /// <param name="parameters">The parameters used for evalulation of the function.</param>
    /// <returns></returns>
    public double Evaluate(double x, double[] parameters)
    {
      MakeSureWasTriedToCompile();

      if (_scriptObject is null)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
        return double.NaN;
      }

      try
      {
        return ((Altaxo.Calc.IParametrizedScalarFunctionDD)_scriptObject).Evaluate(x, parameters);
      }
      catch (Exception)
      {
        return double.NaN;
      }
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        if (_scriptObject is not null)
          return ((IFitFunction)_scriptObject).NumberOfIndependentVariables;
        else
          return _IndependentVariablesNames.Length;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        if (_scriptObject is not null)
          return ((IFitFunction)_scriptObject).NumberOfDependentVariables;
        else
          return _DependentVariablesNames.Length;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        if (_scriptObject is not null)
          return ((IFitFunction)_scriptObject).NumberOfParameters;
        else
          return _NumberOfParameters;
      }
      set
      {
        if (_scriptObject is not null)
          throw new ApplicationException("Number of parameters can not be changed after successfull compilation");
        else
        {
          _IsUsingUserDefinedParameterNames = false;
          _NumberOfParameters = value;
          SetParametersInScript();
        }
      }
    }

    public string IndependentVariableName(int i)
    {
      if (_scriptObject is not null)
        return ((IFitFunction)_scriptObject).IndependentVariableName(i);
      else
        return _IndependentVariablesNames[i];
    }

    public string DependentVariableName(int i)
    {
      if (_scriptObject is not null)
        return ((IFitFunction)_scriptObject).DependentVariableName(i);
      else
        return _DependentVariablesNames[i];
    }

    public string ParameterName(int i)
    {
      return ParameterName(i, true);
    }

    public string ParameterName(int i, bool tryUseCompiledObject)
    {
      // try to avoid an exception if the script object is not compiled
      // if (tryUseCompiledObject && IsUsingUserDefinedParameterNames && (_UserDefinedParameterNames == null || i >= this._UserDefinedParameterNames.Length))
      //   MakeSureWasTriedToCompile();

      string result;

      if (_scriptObject is not null)
      {
        result = ((IFitFunction)_scriptObject).ParameterName(i);
      }
      else
      {
        if (IsUsingUserDefinedParameterNames)
        {
          result = _UserDefinedParameterNames[i];
        }
        else
        {
          result = "P[" + i.ToString() + "]";
        }
      }
      return result;
    }

    public double DefaultParameterValue(int i)
    {
      if (_scriptObject is not null)
        return ((IFitFunction)_scriptObject).DefaultParameterValue(i);
      else
        return 0;
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      if (_scriptObject is not null)
        return ((IFitFunction)_scriptObject).DefaultVarianceScaling(i);
      else
        return null;
    }

    void Altaxo.Calc.Regression.Nonlinear.IFitFunction.Evaluate(double[] independent, double[] parameters, double[] result)
    {
      MakeSureWasTriedToCompile();

      if (_scriptObject is null)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
        return;
      }

      try
      {
        ((IFitFunction)_scriptObject).Evaluate(independent, parameters, result);
        return;
      }
      catch (Exception)
      {
        return;
      }
    }

    #endregion IFitFunction Members
  } // end of class
}
