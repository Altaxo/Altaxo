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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo.Serialization;
using Altaxo.Data;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Scripting
{

  public interface IFitFunctionScriptText : IScriptText, IFitFunction
  {
    /// <summary>
    /// Returns true if the script uses user defined parameter names instead of using P[0], P[1] ...
    /// </summary>
    bool IsUsingUserDefinedParameterNames { get; set;}

    /// <summary>
    /// Get / sets the user defined parameter names. If setting, this also sets the property
    /// <see cref="IsUsingUserDefinedParameterNames" /> to true, and the <see cref="NumberOfParameters" /> to the given number
    /// of user defined parameters.
    /// </summary>
    string[] UserDefinedParameterNames { get; set; }

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
    string FitFunctionName {get; set;}

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
    bool _IsUsingUserDefinedParameterNames=true;

    /// <summary>
    /// Number of Parameters
    /// </summary>
    int _NumberOfParameters=2;
    /// <summary>
    /// Names of the parameters. This is set to null if no parameter names where provided.
    /// </summary>
    string[] _UserDefinedParameterNames = new string[]{"A","B"};
  
    string[] _IndependentVariablesNames = new string[]{"x"};
    string[] _DependentVariablesNames = new string[]{"y"};

    string _fitFunctionName = "User1";
    string _fitFunctionCategory = "UserCat";
    DateTime _fitFunctionCreationTime = DateTime.Now;
    string _fitFunctionDescription = string.Empty;

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

    public override bool Equals(object obj)
    {
      if(!(obj is FitFunctionScript))
        return base.Equals (obj);

      FitFunctionScript from = (FitFunctionScript)obj;

      if(!base.Equals(from))
        return false;

      if(this.FitFunctionCategory != from.FitFunctionCategory)
        return false;
      if(this.FitFunctionName != from.FitFunctionName)
        return false;
      
      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + this._fitFunctionCategory.GetHashCode()+this._fitFunctionName.GetHashCode();
    }


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      FitFunctionScript _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AbstractScript s = (AbstractScript)obj;

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitFunctionScript s = null != o ? (FitFunctionScript)o : new FitFunctionScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
        surr._deserializedObject = s;
        info.DeserializationFinished +=new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);

        return s;
      }

      private void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(info_DeserializationFinished);

        if(documentRoot is AltaxoDocument)
        {
          AltaxoDocument doc = documentRoot as AltaxoDocument;

          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject);
        }
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 1)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      FitFunctionScript _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FitFunctionScript s = (FitFunctionScript)obj;

        // Update the user defined paramter names
        if (s.m_ScriptObject != null && s.IsUsingUserDefinedParameterNames)
        { 
          IFitFunction ff = (IFitFunction)s.m_ScriptObject;
          if(s._UserDefinedParameterNames==null || s._UserDefinedParameterNames.Length!=ff.NumberOfParameters)
            s._UserDefinedParameterNames = new string[ff.NumberOfParameters];
          for(int i=0;i<ff.NumberOfParameters;++i)
            s._UserDefinedParameterNames[i] = ff.ParameterName(i);
        }

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));

        info.AddValue("Category",s.FitFunctionCategory);
        info.AddValue("Name",s.FitFunctionName);
        info.AddValue("CreationTime",s._fitFunctionCreationTime);

        info.AddValue("NumberOfParameters",s.NumberOfParameters);
        info.AddValue("UserDefinedParameters",s.IsUsingUserDefinedParameterNames);
        if(s.IsUsingUserDefinedParameterNames)
        {
          info.AddArray("UserDefinedParameterNames",s._UserDefinedParameterNames,s._UserDefinedParameterNames.Length);
        }

        info.AddArray("IndependentVariableNames",s._IndependentVariablesNames,s._IndependentVariablesNames.Length);
        info.AddArray("DependentVariableNames",s._DependentVariablesNames,s._DependentVariablesNames.Length);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitFunctionScript s = null != o ? (FitFunctionScript)o : new FitFunctionScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        s._fitFunctionCategory = info.GetString("Category");
        s._fitFunctionName = info.GetString("Name");
        s._fitFunctionCreationTime = info.GetDateTime("CreationTime");
        s._NumberOfParameters = info.GetInt32("NumberOfParameters");
        s._IsUsingUserDefinedParameterNames = info.GetBoolean("UserDefinedParameters");
        if(s._IsUsingUserDefinedParameterNames)
          info.GetArray("UserDefinedParameterNames",out s._UserDefinedParameterNames);
        info.GetArray("IndependentVariableNames",out s._IndependentVariablesNames);
        info.GetArray("DependentVariableNames",out s._DependentVariablesNames);

        XmlSerializationSurrogate1 surr = new XmlSerializationSurrogate1();
        surr._deserializedObject = s;
        info.DeserializationFinished +=new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);

        return s;
      }

      private void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(info_DeserializationFinished);

        if(documentRoot is AltaxoDocument)
        {
          AltaxoDocument doc = documentRoot as AltaxoDocument;

          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject);
        }
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionScript), 2)]
    class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      FitFunctionScript _deserializedObject;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FitFunctionScript s = (FitFunctionScript)obj;

        // Update the user defined paramter names
        if (s.m_ScriptObject != null && s.IsUsingUserDefinedParameterNames)
        {
          IFitFunction ff = (IFitFunction)s.m_ScriptObject;
          if (s._UserDefinedParameterNames == null || s._UserDefinedParameterNames.Length != ff.NumberOfParameters)
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
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitFunctionScript s = null != o ? (FitFunctionScript)o : new FitFunctionScript();


        s._fitFunctionCategory = info.GetString("Category");
        s._fitFunctionName = info.GetString("Name");
        s._fitFunctionCreationTime = info.GetDateTime("CreationTime");
        s._fitFunctionDescription = info.GetString("Description");

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        s._NumberOfParameters = info.GetInt32("NumberOfParameters");
        s._IsUsingUserDefinedParameterNames = info.GetBoolean("UserDefinedParameters");
        if (s._IsUsingUserDefinedParameterNames)
          info.GetArray("UserDefinedParameterNames", out s._UserDefinedParameterNames);
        info.GetArray("IndependentVariableNames", out s._IndependentVariablesNames);
        info.GetArray("DependentVariableNames", out s._DependentVariablesNames);

        XmlSerializationSurrogate2 surr = new XmlSerializationSurrogate2();
        surr._deserializedObject = s;
        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.info_DeserializationFinished);

        return s;
      }

      private void info_DeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(info_DeserializationFinished);

        if (documentRoot is AltaxoDocument)
        {
          AltaxoDocument doc = documentRoot as AltaxoDocument;

          // add this script to the collection of scripts
          doc.FitFunctionScripts.Add(_deserializedObject);
        }
      }
    }


    #endregion


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
      : base(b,false)
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
      this._IsUsingUserDefinedParameterNames=from._IsUsingUserDefinedParameterNames;
      this._NumberOfParameters = from._NumberOfParameters;
      this._UserDefinedParameterNames = null==from._UserDefinedParameterNames ? null : (string[])from._UserDefinedParameterNames.Clone();
      this._IndependentVariablesNames = (string[])from._IndependentVariablesNames.Clone();
      this._DependentVariablesNames = (string[])from._DependentVariablesNames.Clone();
      this._fitFunctionName = from._fitFunctionName;
      this._fitFunctionCategory = from.FitFunctionCategory;
      this._fitFunctionCreationTime = from._fitFunctionCreationTime;
    }

    public void CopyFrom(FitFunctionScript from, bool forModification)
    {
      base.CopyFrom(from,forModification);
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

    public override bool Compile()
    {
      bool success = base.Compile ();

      if(success && (this.m_ScriptObject is IFitFunction))
      {
        IFitFunction ff = (IFitFunction)m_ScriptObject;

        this._NumberOfParameters = ff.NumberOfParameters;
        this._fitFunctionCreationTime = DateTime.Now;
        if(this.IsUsingUserDefinedParameterNames)
        {
          if(_UserDefinedParameterNames==null || _UserDefinedParameterNames.Length!=ff.NumberOfParameters)
            _UserDefinedParameterNames = new string[ff.NumberOfParameters];
          for(int i=0;i<ff.NumberOfParameters;++i)
            _UserDefinedParameterNames[i] = ff.ParameterName(i);
        }

        this._IndependentVariablesNames = new string[ff.NumberOfIndependentVariables];
        for(int i=0;i<this._IndependentVariablesNames.Length;++i)
          this._IndependentVariablesNames[i] = ff.IndependentVariableName(i);

        this._DependentVariablesNames = new string[ff.NumberOfDependentVariables];
        for(int i=0;i<this._DependentVariablesNames.Length;++i)
          this._DependentVariablesNames[i] = ff.DependentVariableName(i);
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
          "#region ScriptHeader\r\n"+
          "using System;\r\n" +
          "using Altaxo;\r\n" +
          "using Altaxo.Calc;\r\n" +
          "using Altaxo.Data;\r\n" +
          "using Altaxo.Calc.Regression.Nonlinear;\r\n" + 
          "namespace Altaxo.Calc\r\n" +
          "{\r\n" +
          "\tpublic class MyFitFunction : Altaxo.Calc.FitFunctionExeBase\r\n" +
          "\t{\r\n" +
          "\t\tpublic MyFitFunction()\r\n"+
          "\t\t{\r\n"+
          this.IndependentDefinitionRegionStart+
          this.IndependentDefinitionRegionCore+
          this.IndependentDefinitionRegionEnd+
          this.DependentDefinitionRegionStart+
          this.DependentDefinitionRegionCore+
          this.DependentDefinitionRegionEnd+
          this.ParameterDefinitionRegionStart+
          this.ParameterDefinitionRegionCore+
          this.ParameterDefinitionRegionEnd+
          "\t\t}\r\n"+
          "\r\n"+
          "\t\tpublic override void Evaluate(double[] X, double[] P, double[] Y)\r\n" +
          "\t\t{\r\n"+
          IndependentAssignmentRegionStart+
          IndependentAssignmentRegionCore+
          IndependentAssignmentRegionEnd+
          ParameterAssignmentRegionStart+
          ParameterAssignmentRegionCore+
          ParameterAssignmentRegionEnd+
          DependentDeclarationRegionStart+
          DependentDeclarationRegionCore+
          DependentDeclarationRegionEnd;
      }
    }
    public string ParameterDefinitionRegionStart
    {
      get
      {
        return "\t\t#region Parameter Definition\r\n";
      }
    }
    public string ParameterDefinitionRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        stb.Append(
          "\t\t_parameterNames = new string[]{");
      

        for(int i=0;i<this.NumberOfParameters;i++)
        {
          stb.Append("\"" + this.ParameterName(i,false) + "\"");
          if((i+1)<this.NumberOfParameters)
            stb.Append(",");
        }
        stb.Append("};\r\n");

        /*
        stb.Append(
          "\t\tpublic int NumberOfParameters\r\n" +
          "\t\t{\r\n"+
          "\t\t\tget\r\n"+
          "\t\t\t{\r\n"+
          "\t\t\t\treturn _parameterName.Length;\r\n"+
          "\t\t\t}\r\n"+
          "\t\t}\r\n"+
          "\t\tpublic string ParameterName(int i)\r\n"+
          "\t\t{\r\n"+
          "\t\t\treturn _parameterName[i];\r\n"+
          "\t\t}\r\n"
          );
          */

        return stb.ToString();
      }
    }
    

    public string ParameterDefinitionRegionEnd
    {
      get
      {
        return "\t\t#endregion // Parameter Definition\r\n\r\n";
      }
    }
    public string ParameterAssignmentRegionStart
    {
      get
      {
        return "\t\t\t#region Parameter Assignment\r\n";
      }
    }
    public string ParameterAssignmentRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        if(this.IsUsingUserDefinedParameterNames)
        {
          for(int i=0;i<this.NumberOfParameters;i++)
          {
            stb.Append("\t\t\t");
            stb.Append("double ");
            stb.Append(this.ParameterName(i));
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
        return "\t\t\t#endregion // Parameter Assignment\r\n\r\n";
      }
    }



    public string DependentDefinitionRegionStart
    {
      get
      {
        return "\t\t#region Dependent Variable Definition\r\n";
      }
    }
    public string DependentDefinitionRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        stb.Append(
          "\t\t_dependentVariableNames = new string[]{");
      

        for(int i=0;i<this._DependentVariablesNames.Length;i++)
        {
          stb.Append("\"" + this._DependentVariablesNames[i] + "\"");
          if((i+1)==this._DependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }

        /*
                stb.Append(
                  "\t\tpublic int NumberOfDependentVariables\r\n" +
                  "\t\t{\r\n"+
                  "\t\t\tget\r\n"+
                  "\t\t\t{\r\n"+
                  "\t\t\t\treturn _dependentVariableName.Length;\r\n"+
                  "\t\t\t}\r\n"+
                  "\t\t}\r\n"+
                  "\t\tpublic string DependentVariableName(int i)\r\n"+
                  "\t\t{\r\n"+
                  "\t\t\treturn _dependentVariableName[i];\r\n"+
                  "\t\t}\r\n"
                  );
        */
        return stb.ToString();
      }
    }
    public string DependentDefinitionRegionEnd
    {
      get
      {
        return "\t\t#endregion //  Dependent Variable Definition\r\n\r\n";
      }
    }

    public string DependentDeclarationRegionStart
    {
      get
      {
        return "\t\t\t#region ExpertsOnly - Dependent Variable Declaration\r\n";
      }
    }
    public string DependentDeclarationRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        for(int i=0;i<this._DependentVariablesNames.Length;i++)
        {
          stb.Append("\t\t\tdouble ");
          stb.Append(this._DependentVariablesNames[i]);
          stb.Append(";\r\n");
        }
        return stb.ToString();
      }
    }
    

    public string DependentDeclarationRegionEnd
    {
      get
      {
        return "\t\t\t#endregion // Dependent Variable Declaration\r\n\r\n";
      }
    }

    public string DependentAssignmentRegionStart
    {
      get
      {
        return "\t\t\t#region Dependent Variable Assignment\r\n";
      }
    }
    public string DependentAssignmentRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        for(int i=0;i<this._DependentVariablesNames.Length;i++)
        {
          stb.Append("\t\t\t");
          stb.Append("Y["+i.ToString()+"] = ");
          stb.Append(this._DependentVariablesNames[i]);
          stb.Append(";\r\n");
        }
        return stb.ToString();
      }
    }
    public string DependentAssignmentRegionEnd
    {
      get
      {
        return "\t\t\t#endregion // Dependent Variable Assignment\r\n\r\n";
      }
    }

    
    public string IndependentDefinitionRegionStart
    {
      get
      {
        return "\t\t#region Independent Variable Definition\r\n";
      }
    }
    public string IndependentDefinitionRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        stb.Append(
          "\t\t_independentVariableNames = new string[]{");
      

        for(int i=0;i<this._IndependentVariablesNames.Length;i++)
        {
          stb.Append("\"" + this._IndependentVariablesNames[i] + "\"");
          if((i+1)==this._IndependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }

        /*
                stb.Append(
                  "\t\tpublic int NumberOfIndependentVariables\r\n" +
                  "\t\t{\r\n"+
                  "\t\t\tget\r\n"+
                  "\t\t\t{\r\n"+
                  "\t\t\t\treturn _independentVariableName.Length;\r\n"+
                  "\t\t\t}\r\n"+
                  "\t\t}\r\n"+
                  "\t\tpublic string IndependentVariableName(int i)\r\n"+
                  "\t\t{\r\n"+
                  "\t\t\treturn _independentVariableName[i];\r\n"+
                  "\t\t}\r\n"
                  );
        */
        return stb.ToString();
      }
    }
    public string IndependentDefinitionRegionEnd
    {
      get
      {
        return "\t\t#endregion //  Independent Variable Definition\r\n\r\n";
      }
    }
    public string IndependentAssignmentRegionStart
    {
      get
      {
        return "\t\t\t#region Independent Variable Assignment\r\n";
      }
    }
    public string IndependentAssignmentRegionCore
    {
      get
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        for(int i=0;i<this._IndependentVariablesNames.Length;i++)
        {
          stb.Append("\t\t\t");
          stb.Append("double ");
          stb.Append(this._IndependentVariablesNames[i]);
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
        return "\t\t\t#endregion //  Independent Variable Assignment\r\n\r\n";
      }
    }




    public override string CodeStart
    {
      get
      {
        return
          "#endregion // ScriptHeader\r\n"+
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
          "\t\t\t// ----- add your script above this line -----\r\n"+
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
          DependentAssignmentRegionStart+
          DependentAssignmentRegionCore+
          DependentAssignmentRegionEnd+
          "\t\t} // method\r\n" +
          "\t} // class\r\n" +
          "} //namespace\r\n"+
          "#endregion\r\n";
      }
    }

    #endregion

    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public override object Clone()
    {
      return new FitFunctionScript(this,true);
    }

    

    
    public string[] DependentVariablesNames 
    {
      set
      {
        System.Text.StringBuilder sb;
        int first,last;

        this._DependentVariablesNames = (string[])value.Clone();
        string[] names = value;

        first = this.ScriptText.IndexOf(this.DependentDefinitionRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables start region");
        first += this.DependentDefinitionRegionStart.Length;
        last = this.ScriptText.IndexOf(this.DependentDefinitionRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable definition end region");
        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.DependentDefinitionRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();


        first = this.ScriptText.IndexOf(this.DependentDeclarationRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain an dependent variables declaration start region");
        first += this.DependentDeclarationRegionStart.Length;
        last = this.ScriptText.IndexOf(this.DependentDeclarationRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain an dependent variable declaration end region");

        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.DependentDeclarationRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();


      
        first = this.ScriptText.IndexOf(this.DependentAssignmentRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables assignment start region");
        first += this.DependentAssignmentRegionStart.Length;
        last = this.ScriptText.IndexOf(this.DependentAssignmentRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable assignment end region");
        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.DependentAssignmentRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();

      }
    }

    public string[] IndependentVariablesNames 
    {
      set
      {
        System.Text.StringBuilder sb;
        int first,last;
        this._IndependentVariablesNames = (string[])value.Clone();

        string[] names = value;

        first = this.ScriptText.IndexOf(this.IndependentDefinitionRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain an independent variables definition start region");
        first += this.IndependentDefinitionRegionStart.Length;
        last = this.ScriptText.IndexOf(this.IndependentDefinitionRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain an independent variable definition end region");

        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.IndependentDefinitionRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();

        
        first = this.ScriptText.IndexOf(this.IndependentAssignmentRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variables assignment start region");
        first += this.IndependentAssignmentRegionStart.Length;
        last = this.ScriptText.IndexOf(this.IndependentAssignmentRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain a dependent variable assignment end region");

        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.IndependentAssignmentRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();

      
      }
    }

    public bool IsUsingUserDefinedParameterNames
    {
      get 
      {
        return this._IsUsingUserDefinedParameterNames;
      }
      set
      {
        if (value == true && _IsUsingUserDefinedParameterNames==false)
        {
          string[] oldNames = this._UserDefinedParameterNames;
          if(oldNames==null) 
            oldNames = new string[0];

          string[] newNames= new string[_NumberOfParameters];

          int len = Math.Min(oldNames.Length,newNames.Length);
          for(int i=0;i<len;++i)
            newNames[i] = oldNames[i];
          for(int i=len;i<newNames.Length;++i)
            newNames[i] = "P"+i.ToString();

          this.UserDefinedParameterNames = newNames;
        }

        _IsUsingUserDefinedParameterNames = value;
        SetParametersInScript();
      }
      
    }

    void SetParametersInScript()
    {
      System.Text.StringBuilder sb;
      int first, last;

      first = this.ScriptText.IndexOf(this.ParameterDefinitionRegionStart);
      if (first < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter definition start region");
      first += this.ParameterDefinitionRegionStart.Length;
      last = this.ScriptText.IndexOf(this.ParameterDefinitionRegionEnd);
      if (last < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter definition end region");
      sb = new System.Text.StringBuilder();
      sb.Append(this.ScriptText.Substring(0, first));
      sb.Append(this.ParameterDefinitionRegionCore);
      sb.Append(this.ScriptText.Substring(last));
      this.ScriptText = sb.ToString();

      first = this.ScriptText.IndexOf(this.ParameterAssignmentRegionStart);
      if (first < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter assignment start region");
      first += this.ParameterAssignmentRegionStart.Length;
      last = this.ScriptText.IndexOf(this.ParameterAssignmentRegionEnd);
      if (last < 0)
        throw new ApplicationException("The script text seems to no longer contain a parameter assignment end region");
      sb = new System.Text.StringBuilder();
      sb.Append(this.ScriptText.Substring(0, first));
      sb.Append(this.ParameterAssignmentRegionCore);
      sb.Append(this.ScriptText.Substring(last));
      this.ScriptText = sb.ToString();

    }

    public string[] UserDefinedParameterNames
    {
      get
      {
        if(this._IsUsingUserDefinedParameterNames)
          return (string[])this._UserDefinedParameterNames.Clone();
        else
          return null;
      }
      set
      {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        this._IsUsingUserDefinedParameterNames = true;
        this._NumberOfParameters = value.Length;
        this._UserDefinedParameterNames = (string[])value.Clone();

        this.SetParametersInScript();
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

      if (null == m_ScriptObject)
      {
        m_Errors = new string[1] { "Script Object is null" };
        return double.NaN;
      }

      try
      {
        return ((Altaxo.Calc.IParametrizedScalarFunctionDD)m_ScriptObject).Evaluate(x,parameters);
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
       

        if (this.m_ScriptObject != null)
          return ((IFitFunction)m_ScriptObject).NumberOfIndependentVariables;
        else
          return this._IndependentVariablesNames.Length;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        

        if (this.m_ScriptObject != null)
          return ((IFitFunction)m_ScriptObject).NumberOfDependentVariables;
        else
          return this._DependentVariablesNames.Length;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        

        if (this.m_ScriptObject != null)
          return ((IFitFunction)m_ScriptObject).NumberOfParameters;
        else
          return this._NumberOfParameters;
      }
      set
      {
        if (this.m_ScriptObject != null)
          throw new ApplicationException("Number of parameters can not be changed after successfull compilation");
        else
        {
          this._IsUsingUserDefinedParameterNames = false;
          this._NumberOfParameters = value;
          this.SetParametersInScript();
        }
      }
    }

    public string IndependentVariableName(int i)
    {
      

      if (this.m_ScriptObject != null)
        return ((IFitFunction)m_ScriptObject).IndependentVariableName(i);
      else
        return this._IndependentVariablesNames[i];
    }

    public string DependentVariableName(int i)
    {
      

      if (this.m_ScriptObject != null)
        return ((IFitFunction)m_ScriptObject).DependentVariableName(i);
      else
        return this._DependentVariablesNames[i];
    }

    public string ParameterName(int i)
    {
      return ParameterName(i, true);
    }

    public string ParameterName(int i, bool tryUseCompiledObject)
    {
      // try to avoid a exception if the script object is not compiled
     // if (tryUseCompiledObject && IsUsingUserDefinedParameterNames && (_UserDefinedParameterNames == null || i >= this._UserDefinedParameterNames.Length))
     //   MakeSureWasTriedToCompile();

      if (this.m_ScriptObject != null)
      {
        return ((IFitFunction)m_ScriptObject).ParameterName(i);
      }
      else
      {
        if (IsUsingUserDefinedParameterNames)
        {
          return this._UserDefinedParameterNames[i];
        }
        else
          return "P[" + i.ToString() + "]";
      }
    }

    public double DefaultParameterValue(int i)
    {

      if (this.m_ScriptObject != null)
        return ((IFitFunction)m_ScriptObject).DefaultParameterValue(i);
      else
        return 0;
    }


    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      if (this.m_ScriptObject != null)
        return ((IFitFunction)m_ScriptObject).DefaultVarianceScaling(i);
      else
        return null;
    }

    void Altaxo.Calc.Regression.Nonlinear.IFitFunction.Evaluate(double[] independent, double[] parameters, double[] result)
    {
      MakeSureWasTriedToCompile();

      if (null == m_ScriptObject)
      {
        m_Errors = new string[1] { "Script Object is null" };
        return;
      }

      try
      {
        ((IFitFunction)m_ScriptObject).Evaluate(independent,parameters,result);
        return;
      }
      catch (Exception)
      {
        return;
      }
    }

    #endregion
  } // end of class
}
