#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{

  public interface IParametrizedFunctionDDScriptText : IScriptText
  {
       

    /// <summary>
    /// Get / sets the number of parameters. If setting the number of parameters with this property,
    /// the property <see>IsUsingUserDefinedParameterNames</see> is set to false.
    /// </summary>
    int NumberOfParameters { get; set; }

    /// <summary>
    /// Returns true if the script uses user defined parameter names instead of using P[0], P[1] ...
    /// </summary>
    bool IsUsingUserDefinedParameterNames { get; }

    /// <summary>
    /// Get / sets the user defined parameter names. If setting, this also sets the property
    /// <see>IsUsingUserDefinedParameterNames</see> to true, and the <see>NumberOfParameters</see> to the given number
    /// of user defined parameters.
    /// </summary>
    string[] UserDefinedParameterNames { get; set; }

    string[] DependentVariablesNames { set; }

    string[] IndependentVariablesNames { set; }
  }

  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a property column script. 
  /// </summary>

  public class ParametrizedFunctionDDScript : AbstractScript, IParametrizedFunctionDDScriptText, IFitFunction
  {
    /// <summary>
    /// Number of Parameters
    /// </summary>
    int _NumberOfParameters;

    string[] _IndependentVariablesNames = new string[]{"x"};
    string[] _DependentVariablesNames = new string[]{"y"};


    /// <summary>
    /// True if we use user defined parameter names in the script.
    /// </summary>
    bool _IsUsingUserDefinedParameterNames;

    /// <summary>
    /// Names of the parameters. This is set to null if no parameter names where provided.
    /// </summary>
    string[] _UserDefinedParameterNames;
  

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Graph.ParametrizedFunctionDDScript), 0)]
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Altaxo.Data.AbstractScript s = (Altaxo.Data.AbstractScript)obj;

        info.AddBaseValueEmbedded(s, typeof(Altaxo.Data.AbstractScript));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Graph.ParametrizedFunctionDDScript s = null != o ? (Altaxo.Graph.ParametrizedFunctionDDScript)o : new Altaxo.Graph.ParametrizedFunctionDDScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(Altaxo.Data.AbstractScript), parent);

        return s;
      }
    }


    #endregion


    /// <summary>
    /// Creates an empty script.
    /// </summary>
    public ParametrizedFunctionDDScript()
    {
      _IndependentVariablesNames = new string[]{"x"};
      _DependentVariablesNames = new string[]{"y"};
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public ParametrizedFunctionDDScript(ParametrizedFunctionDDScript b)
      : base(b)
    {
    }

    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.MyParametrizedFunctionDDScript"; }
    }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public override string CodeHeader
    {
      get
      {
        return
          "using System;\r\n" +
          "using Altaxo;\r\n" +
          "using Altaxo.Calc;\r\n" +
          "using Altaxo.Data;\r\n" +
          "using Altaxo.Calc.Regression.Nonlinear;\r\n" + 
          "namespace Altaxo.Calc\r\n" +
          "{\r\n" +
          "\tpublic class MyParametrizedFunctionDDScript : Altaxo.Calc.ScriptExecutionBase, IFitFunction\r\n" +
          "\t{\r\n" +
          this.IndependentDefinitionRegionStart+
          this.IndependentDefinitionRegionCore+
          this.IndependentDefinitionRegionEnd+
          this.DependentDefinitionRegionStart+
          this.DependentDefinitionRegionCore+
          this.DependentDefinitionRegionEnd+
          this.ParameterDefinitionRegionStart+
          this.ParameterDefinitionRegionCore+
          this.ParameterDefinitionRegionEnd+
          "\t\tpublic void Evaluate(double[] X, double[] P, double[] Y)\r\n" +
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
          "\t\tprivate string[] _parameterName = new string[]{");
      

        for(int i=0;i<this.NumberOfParameters;i++)
        {
          stb.Append("\"" + this.ParameterName(i) + "\"");
          if((i+1)<this.NumberOfParameters)
            stb.Append(",");
        }
        stb.Append("};\r\n");

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
        return "";
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
          "\t\tprivate string[] _dependentVariableName = new string[]{");
      

        for(int i=0;i<this._DependentVariablesNames.Length;i++)
        {
          stb.Append("\"" + this._DependentVariablesNames[i] + "\"");
          if((i+1)==this._DependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }


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
          "\t\tprivate string[] _independentVariableName = new string[]{");
      

        for(int i=0;i<this._IndependentVariablesNames.Length;i++)
        {
          stb.Append("\"" + this._IndependentVariablesNames[i] + "\"");
          if((i+1)==this._IndependentVariablesNames.Length)
            stb.Append("};\r\n");
          else
            stb.Append(",");
        }


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
          "\t\t\t// ----- add your script above this line -----\r\n";
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
          "} //namespace\r\n";
      }
    }



    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public override object Clone()
    {
      return new Altaxo.Graph.ParametrizedFunctionDDScript(this);
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
        sb.Append(this.DependentAssignmentRegionCore);
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
        sb.Append(this.IndependentAssignmentRegionCore);
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
        System.Text.StringBuilder sb;
        int first,last;

        this._IsUsingUserDefinedParameterNames = true;
        this._NumberOfParameters = value.Length;
        this._UserDefinedParameterNames = (string[])value.Clone();

        string[] pnames = value;
        
        first = this.ScriptText.IndexOf(this.ParameterDefinitionRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a parameter definition start region");
        first += this.ParameterDefinitionRegionStart.Length;
        last = this.ScriptText.IndexOf(this.ParameterDefinitionRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain a parameter definition end region");
        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.ParameterDefinitionRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();

        
        first = this.ScriptText.IndexOf(this.ParameterAssignmentRegionStart);
        if (first < 0)
          throw new ApplicationException("The script text seems to no longer contain a parameter assignment start region");
        first += this.ParameterAssignmentRegionStart.Length;
        last = this.ScriptText.IndexOf(this.ParameterAssignmentRegionEnd);
        if(last<0)
          throw new ApplicationException("The script text seems to no longer contain a parameter assignment end region");
        sb = new System.Text.StringBuilder();
        sb.Append(this.ScriptText.Substring(0,first));
        sb.Append(this.ParameterDefinitionRegionCore);
        sb.Append(this.ScriptText.Substring(last));
        this.ScriptText = sb.ToString();

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
          
        return this._IndependentVariablesNames.Length;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return this._DependentVariablesNames.Length;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return this._NumberOfParameters;
      }
      set
      {
        this._IsUsingUserDefinedParameterNames = false;
        this._NumberOfParameters = value;
      }
    }

    public string IndependentVariableName(int i)
    {
      return this._IndependentVariablesNames[i];
    }

    public string DependentVariableName(int i)
    {
      return this._DependentVariablesNames[i];
    }

    public string ParameterName(int i)
    {
      if(IsUsingUserDefinedParameterNames)
        return this._UserDefinedParameterNames[i];
      else
        return "P"+i.ToString();
    }

    void Altaxo.Calc.Regression.Nonlinear.IFitFunction.Evaluate(double[] independent, double[] parameters, double[] result)
    {
      if (null == m_ScriptObject)
      {
        m_Errors = new string[1] { "Script Object is null" };
        return;
      }

      try
      {
        ((Altaxo.Calc.IParametrizedFunctionDDD)m_ScriptObject).Evaluate(independent,parameters,result);
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
