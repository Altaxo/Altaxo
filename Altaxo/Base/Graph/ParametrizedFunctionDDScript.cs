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

namespace Altaxo.Graph
{

    public interface IParametrizedFunctionDDScriptText : IScriptText
    {
        /// <summary>
        /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
        /// If the script object exists, the function "Evaluate" will be called.
        /// </summary>
        /// <param name="x">The function argument.</param>
        /// <returns>The function value.</returns>
        /// <remarks>If exceptions were thrown during execution, the return value is double.NaN.
        /// </remarks>
        double Evaluate(double x);

      double Evaluate(double x, double[] parameters);

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
    }

    /// <summary>
    /// Holds the text, the module (=executable), and some properties of a property column script. 
    /// </summary>

    public class ParametrizedFunctionDDScript : AbstractScript, IParametrizedFunctionDDScriptText
    {
        /// <summary>
        /// Number of Parameters
        /// </summary>
        int _NumberOfParameters;

        /// <summary>
        /// True if we use user defined parameter names in the script.
        /// </summary>
        bool _IsUsingUserDefinedParameterNames;

        /// <summary>
        /// Names of the parameters. This is set to null if no parameter names where provided.
        /// </summary>
        string[] _UserDefinedParameterNames;
        /// <summary>
        /// Values of the parameters.
        /// </summary>
        double[] _parameterValues;

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
                  "namespace Altaxo.Calc\r\n" +
                  "{\r\n" +
                  "\tpublic class MyParametrizedFunctionDDScript : Altaxo.Calc.FunctionEvaluationScriptBase\r\n" +
                  "\t{\r\n" +
                  "\t\tpublic override double EvaluateFunctionValue(double x, double[]P)\r\n" +
                  "\t\t{\r\n"+
                  ParameterRegionStart+
                  ParameterRegionEnd;
            }
        }

        public string ParameterRegionStart
        {
          get
          {
            return "#region Start of parameter naming\r\n";
          }
        }
      public string ParameterRegionEnd
      {
        get
        {
          return "#endregion // parameter naming\r\n";
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
                  "\t\t\treturn P[0]+P[1]*Sin(x);\r\n" +
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
          this._IsUsingUserDefinedParameterNames = true;
          this._NumberOfParameters = value.Length;
          this._UserDefinedParameterNames = (string[])value.Clone();

          string[] pnames = value;
          int first = this.ScriptText.IndexOf(this.ParameterRegionStart);
          if (first < 0)
            throw new ApplicationException("The script text seems to no longer contain a parameter start region");
          first += this.ParameterRegionStart.Length;
          int last = this.ScriptText.IndexOf(this.ParameterRegionEnd);
          System.Text.StringBuilder sb = new System.Text.StringBuilder();
          sb.Append(this.ScriptText.Substring(0,first));
          for(int i=0;i<pnames.Length;i++)
            sb.Append(string.Format("{0}=P[{1}];\r\n",pnames[i],i));
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
      public double Evaluate(double x)
      {
        return Evaluate(x,this._parameterValues);
      }
    } // end of class
}
