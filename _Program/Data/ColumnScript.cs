/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo.Serialization;

namespace Altaxo.Data
{
	/// <summary>
	/// Holds the text, the module (=executable), and some properties of a column script. 
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.ColumnScript.SerializationSurrogate0))]
	[SerializationVersion(0)]
	[Serializable()]
	public class ColumnScript : ICloneable, System.Runtime.Serialization.IDeserializationCallback
	{
		/// <summary>
		/// ScriptStyle enumerates the style of the column script.
		/// </summary>
		public enum ScriptStyle 
		{
			/// <summary>
			/// ColumnValues are set by indexing the target column, i.e. targetcol[i]=...
			/// The values calculated by the script must therefore be scalar values.
			/// </summary>
			SetColumnValues, 
			/// <summary>
			/// ColumnValues are set by setting the column at once, i.e. targetcol=...<para/>
			/// The values calculated by the script must therefore be columns (1-dimensional arrays).
			/// </summary>
			SetColumn,
			/// <summary>
			/// With this style, you can write code outside the function <see cref="Altaxo.Calc.ColScriptExeBase.Execute"/>
			/// You can even define your own classes and functions for use by the column script.
			///	</summary>
			FreeStyle };

		/// <summary>
		/// Holds the <see cref="ScriptStyle"/> of the column script.
		/// </summary>
		protected ScriptStyle m_ScriptStyle = ScriptStyle.SetColumn;

		/// <summary>
		/// The text of the column script.
		/// </summary>
		public string m_ScriptText; // the text of the script

		/// <summary>
		/// holds the first row if ScriptStyle is SetColumnValues, otherwise not used.
		/// </summary>
		protected string m_RowFrom="0";

		/// <summary>
		/// Holds the condition of the for-loop if ScriptStyle is SetColumnValues, otherwise not used.
		/// </summary>
		protected string m_RowCondition="<";
		
		/// <summary>
		/// Holds the end value of the for-loop if ScriptStyle is SetColumnValues, otherwise not used.
		/// </summary>
		protected string m_RowTo="10";
		
		/// <summary>
		/// The increment statement of the for-loop (can also be decrement or other) if ScriptStyle is SetColumnValues, otherwise not used.
		/// </summary>
		protected string m_RowInc="++"; // the values for the loop conditions

		/// <summary>
		/// True when the text changed from last time this flag was reseted.
		/// </summary>
		[NonSerialized()]
		protected bool  m_IsDirty; // true when text changed, can be reseted

		/// <summary>
		/// False when the text has changed and was not compiled already.
		/// </summary>
		[NonSerialized()]
		protected bool m_Compiled; // false when text changed and not compiled already

		/// <summary>
		/// The assembly that holds the created script class.
		/// </summary>
		[NonSerialized()]
		protected System.Reflection.Assembly m_ScriptAssembly;

		/// <summary>
		/// The script object. This is a instance of the newly created script class, which is derived class of <see cref="Altaxo.Calc.ColScriptExeBase"/>
		/// </summary>
		[NonSerialized()]
		protected Altaxo.Calc.ColScriptExeBase m_ScriptObject; // the compiled and created script object

		/// <summary>
		/// Holds error messages created by the compiler.
		/// </summary>
		/// <remarks>The column script is compiled, if it is dirty. This can happen not only
		/// during the use of the column script dialog, but anytime when you changed the script text and
		/// try to execute the script. That's the reason for holding the compiler error messages
		/// here and not in the script dialog.</remarks>
		[NonSerialized()]
		protected string[] m_Errors=null;



		#region Serialization
		/// <summary>
		/// Responsible for serialization of the column script version 0.
		/// </summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes the column script
			/// </summary>
			/// <param name="obj">The column script to serialize.</param>
			/// <param name="info">Serialization info.</param>
			/// <param name="context">Streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.ColumnScript s = (Altaxo.Data.ColumnScript)obj;
				info.AddValue("Style",(int)s.m_ScriptStyle);
				info.AddValue("Text",s.m_ScriptText);
				info.AddValue("From",s.m_RowFrom);
				info.AddValue("Cond",s.m_RowCondition);
				info.AddValue("To",s.m_RowTo);
				info.AddValue("Inc",s.m_RowInc);
			}

			/// <summary>
			/// Deserialized the column script.
			/// </summary>
			/// <param name="obj">The uninitialized column script instance.</param>
			/// <param name="info">Serialization info.</param>
			/// <param name="context">Streaming context.</param>
			/// <param name="selector">Surrogate selector.</param>
			/// <returns></returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				Altaxo.Data.ColumnScript s = (Altaxo.Data.ColumnScript)obj;
				// s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
				s.m_ScriptStyle = (ScriptStyle)info.GetInt32("Style");
				s.m_ScriptText = info.GetString("Text");
				s.m_RowFrom = info.GetString("From");
				s.m_RowCondition = info.GetString("Cond");
				s.m_RowTo = info.GetString("To");
				s.m_RowInc = info.GetString("Inc");
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.ColumnScript),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				Altaxo.Data.ColumnScript s = (Altaxo.Data.ColumnScript)obj;
		
				info.AddValue("Style",(int)s.m_ScriptStyle);
				info.AddValue("Text",s.m_ScriptText);
				info.AddValue("From",s.m_RowFrom);
				info.AddValue("Cond",s.m_RowCondition);
				info.AddValue("To",s.m_RowTo);
				info.AddValue("Inc",s.m_RowInc);
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info, object parent)
			{
				Altaxo.Data.ColumnScript s = null!=o ? (Altaxo.Data.ColumnScript)o : new Altaxo.Data.ColumnScript();
				
				info.OpenInnerContent();

				s.m_ScriptStyle = (ScriptStyle)info.GetInt32("Style");
				s.m_ScriptText = info.GetString("Text");
				s.m_RowFrom = info.GetString("From");
				s.m_RowCondition = info.GetString("Cond");
				s.m_RowTo = info.GetString("To");
				s.m_RowInc = info.GetString("Inc");
				return s;
			}
		}

		/// <summary>
		/// Is called when deserialization has finished.
		/// </summary>
		/// <param name="obj"></param>
		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion


		/// <summary>
		/// Creates an empty column script. Default Style is "Set Column".
		/// </summary>
		public ColumnScript()
		{
		}

		/// <summary>
		/// Creates a column script as a copy from another script.
		/// </summary>
		/// <param name="b">The script to copy from.</param>
		public ColumnScript(ColumnScript b)
		{
			this.m_ScriptStyle = b.m_ScriptStyle;
			this.m_ScriptText  = b.m_ScriptText;
			this.m_ScriptAssembly = b.m_ScriptAssembly;
			this.m_ScriptObject   = b.m_ScriptObject;
			this.m_RowFrom = b.m_RowFrom;
			this.m_RowCondition = b.m_RowCondition;
			this.m_RowTo   = b.m_RowTo;
			this.m_RowInc  = b.m_RowInc;
			this.m_IsDirty = b.m_IsDirty;
			this.m_Compiled = b.m_Compiled;
			this.m_Errors   = null==b.m_Errors ? null: (string[])b.m_Errors.Clone();
		}


		/// <summary>
		/// Returns the compiler errors as array of strings.
		/// </summary>
		public string[] Errors
		{
			get { return m_Errors; }
		}


		/// <summary>
		/// True if the column script is dirty, i.e. the text changed since the last reset of IsDirty.
		/// </summary>
		public bool IsDirty
		{
			get { return m_IsDirty; }
			set { m_IsDirty = value; }
		}

		/// <summary>
		/// Get / sets the script style.
		/// </summary>
		public ScriptStyle Style
		{
			get { return m_ScriptStyle; }
			set { m_ScriptStyle = value; m_IsDirty=true; m_Compiled=false; }
		}


		/// <summary>
		/// Get / sets the start value of the for-loop (used only if ScriptStyle is "SetColumnValues")
		/// </summary>
		public string ForFrom
		{
			get { return m_RowFrom; }
			set { m_RowFrom = value; m_IsDirty=true; m_Compiled=false; }
		}

		/// <summary>
		/// Get / sets the end condition of the for-loop (used only if ScriptStyle is "SetColumnValues")
		/// </summary>
		public string ForCondition
		{
			get { return m_RowCondition; }
			set { m_RowCondition = value; m_IsDirty=true; m_Compiled=false; }
		}

		/// <summary>
		/// Get / sets the end value of the for-loop (used only if ScriptStyle is "SetColumnValues")
		/// </summary>
		public string ForEnd
		{
			get { return m_RowTo; }
			set { m_RowTo = value; m_IsDirty=true; m_Compiled=false; }
		}

		/// <summary>
		/// Get / sets the increment statement of the for-loop (used only if ScriptStyle is "SetColumnValues")
		/// </summary>
		public string ForInc
		{
			get { return m_RowInc; }
			set { m_RowInc = value; m_IsDirty=true; m_Compiled=false; }
		}

		/// <summary>
		/// Get / sets the script text
		/// </summary>
		public string ScriptBody
		{
			get { return m_ScriptText; }
			set { m_ScriptText = value; m_IsDirty=true; m_Compiled=false; }
		}

		/// <summary>
		/// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
		/// </summary>
		public string CodeHeader
		{
			get
			{
				string codeheader;
				switch(m_ScriptStyle)
				{
					case ScriptStyle.SetColumnValues:
						codeheader =  "namespace Altaxo{\r\npublic class SetColVal : Altaxo.Calc.ColScriptExeBase{\r\n" +
							"public override void Execute(Altaxo.Data.DataColumn myColumn) {\r\n" +
							"Altaxo.Data.DataTable col = (null!=myColumn)? myColumn.ParentTable:null;\r\n" +
							"Altaxo.Data.TableSet   tab = (null!=col)? col.ParentDataSet:null;\r\n"+
							"for(int i=" + m_RowFrom + ";i" + m_RowCondition + m_RowTo + ";i" + m_RowInc + ") {\r\n";
						//codestart  =  "cts[i]=";
						//codetail = "} /*for*/ } /*Execute*/  } /*class*/  } /*namespace*/"; 
						break;
					case ScriptStyle.SetColumn:
						codeheader =  "namespace Altaxo {\r\npublic class SetColVal : Altaxo.Calc.ColScriptExeBase {\r\n" +
							"public override void Execute(Altaxo.Data.DataColumn myColumn) {\r\n" +
							"Altaxo.Data.DataTable col = (null!=myColumn)? myColumn.ParentTable:null;\r\n"+
							"Altaxo.Data.TableSet   tab = (null!=col)? col.ParentDataSet:null;\r\n";
													
						//codestart = "col[\"" + dataColumn.ColumnName + "\"]=";
						//codetail = "} /*Execute*/ } /*class*/ } /*namespace*/";
						break;
					case ScriptStyle.FreeStyle:
						codeheader =	"namespace Altaxo {\r\npublic class SetColVal : Altaxo.Calc.ColScriptExeBase {\r\n"+
							"public override void Execute(Altaxo.Data.DataColumn myColumn) {\r\n" +
							"Altaxo.Data.DataTable col = (null!=myColumn)? myColumn.ParentTable:null;\r\n"+
							"Altaxo.Data.TableSet   tab = (null!=col)? col.ParentDataSet:null;\r\n";
						//codestart = "public override void Execute(Altaxo.Data.DataTable col) {\n";
						//codetail = " } /*class*/ } /*namespace*/ \n// You have to provide the end brace of Execute(...), after this you can add own member functions";
						break;				
					default:
						codeheader="";
						//codestart="";
						//codetail="";
						break;
				} // end case 
				return codeheader;
			}
		}


		/// <summary>
		/// Get the assignment statement for the column value or the column, depending on the script style.
		/// </summary>
		public string CodeStart
		{
			get
			{
				string codestart;
				switch(m_ScriptStyle)
				{
					case ScriptStyle.SetColumnValues:
						//codeheader =  "namespace Altaxo { public class SetColVal : ColScriptExeBase {\n";
						//codeheader +=	"public override void Execute(Altaxo.Data.DataTable col) {\n";
						//codeheader += "Altaxo.Data.DataColumn cts = col[\"" + dataColumn.ColumnName + "\"];\n";
						//codeheader += "for(int i=" + m_RowFrom + ";i" + m_RowCondition + m_RowTo + ";i" + m_RowInc + ") {\n";
						codestart  =  "myColumn[i]=";
						//codetail = "} /*for*/ } /*Execute*/  } /*class*/  } /*namespace*/"; 
						break;
					case ScriptStyle.SetColumn:
						//codeheader =  "namespace Altaxo { public class SetColVal : ColScriptExeBase {";
						//codeheader += "public override void Execute(Altaxo.Data.DataTable col) {\n";
						codestart = "myColumn.Data=";
						//codetail = "} /*Execute*/ } /*class*/ } /*namespace*/";
						break;
					case ScriptStyle.FreeStyle:
						//codeheader = "namespace Altaxo { public class SetColVal : ColScriptExeBase { ";
						codestart = "";
						//codetail = " } /*class*/ } /*namespace*/ \n// You have to provide the end brace of Execute(...), after this you can add own member functions";
						break;				
					default:
						//codeheader="";
						codestart="";
						//codetail="";
						break;
				} // end case 
				return codestart;
			}
		}


		/// <summary>
		/// Get the ending text of the script, dependent on the ScriptStyle.
		/// </summary>
		public string CodeTail
		{
			get
			{
				string codetail;
				switch(m_ScriptStyle)
				{
					case ScriptStyle.SetColumnValues:
						//codeheader =  "namespace Altaxo { public class SetColVal : ColScriptExeBase {\n";
						//codeheader +=	"public override void Execute(Altaxo.Data.DataTable col) {\n";
						//codeheader += "Altaxo.Data.DataColumn cts = col[\"" + dataColumn.ColumnName + "\"];\n";
						//codeheader += "for(int i=" + m_RowFrom + ";i" + m_RowCondition + m_RowTo + ";i" + m_RowInc + ") {\n";
						//codestart  =  "cts[i]=";
						codetail = "} /*for*/ } /*Execute*/  } /*class*/  } /*namespace*/"; 
						break;
					case ScriptStyle.SetColumn:
						//codeheader =  "namespace Altaxo { public class SetColVal : ColScriptExeBase {";
						//codeheader += "public override void Execute(Altaxo.Data.DataTable col) {\n";
						//codestart = "col[\"" + dataColumn.ColumnName + "\"]=";
						codetail = "} /*Execute*/ } /*class*/ } /*namespace*/";
						break;
					case ScriptStyle.FreeStyle:
						//codeheader = "namespace Altaxo { public class SetColVal : ColScriptExeBase { ";
						//codestart = "public override void Execute(Altaxo.Data.DataTable col) {\n";
						codetail = " } /*class*/ } /*namespace*/ \n// You have to provide the end brace of Execute(...), after this you can add own member functions";
						break;				
					default:
						//codeheader="";
						//codestart="";
						codetail="";
						break;
				} // end case 
				return codetail;
			}
		}



		/// <summary>
		/// Clones the column script.
		/// </summary>
		/// <returns>The cloned object.</returns>
		public object Clone()
		{
			return new ColumnScript(this);
		}


		/// <summary>
		/// Does the compilation of the script into an assembly.
		/// If it was not compiled before or is dirty, it is compiled first.
		/// From the compiled assembly, a new instance of the newly created script class is created
		/// and stored in m_ScriptObject.
		/// </summary>
		/// <returns>True if successfully compiles, otherwise false.</returns>
		public bool Compile()
		{
			bool bSucceeded=true;

			// we need nothing to do if not dirty and assembly and object is valid
			if(this.m_Compiled && null!=m_ScriptAssembly && null!=m_ScriptObject)
			{
				return false; // keep the error since a compiler error was detected before
			}

			this.m_Compiled = true;

			Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();

			// For Visual Basic Compiler try this :
			//Microsoft.VisualBasic.VBCodeProvider

			System.CodeDom.Compiler.ICodeCompiler compiler = codeProvider.CreateCompiler();
			System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();

			parameters.GenerateInMemory = true;
			parameters.IncludeDebugInformation = true;

			// Add available assemblies including the application itself 
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) 
			{
				parameters.ReferencedAssemblies.Add(asm.Location);
			}
			String code = CodeHeader + CodeStart + ScriptBody + CodeTail;
			

			CompilerResults results = compiler.CompileAssemblyFromSource(parameters, code);

			if (results.Errors.Count > 0) 
			{
				bSucceeded = false;
				this.m_ScriptAssembly = null;
				this.m_ScriptObject = null;

				m_Errors = new string[results.Errors.Count];
				int i=0;
				foreach (CompilerError err in results.Errors) 
				{
					m_Errors[i++] = err.ToString();
				}
			}
			else	
			{
				// try to execute application
				this.m_ScriptAssembly = results.CompiledAssembly;
				this.m_ScriptObject = null;

				try
				{
					this.m_ScriptObject = (Altaxo.Calc.ColScriptExeBase)results.CompiledAssembly.CreateInstance("Altaxo.SetColVal");
				}
				catch (Exception ex) 
				{
					bSucceeded = false;
					m_Errors = new string[1];
					m_Errors[0] = "Unable to create Scripting object, have you missed it?\n" + ex.ToString(); 
				}
			}
			return bSucceeded;
		}


		/// <summary>
		/// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
		/// If the script object exists, the Execute function of this script object is called.
		/// </summary>
		/// <param name="myColumn">The data column this script is working on.</param>
		/// <returns>True if executed without exceptions, otherwise false.</returns>
		/// <remarks>If exceptions were thrown during execution, the exception messages are stored
		/// inside the column script and can be recalled by the Errors property.</remarks>
		public bool Execute(Altaxo.Data.DataColumn myColumn)
		{
			if(null==m_ScriptObject)
			{
				m_Errors = new string[1]{"Script Object is null"};
				return false;
			}

			try
			{
				m_ScriptObject.Execute(myColumn);
			}
			catch(Exception ex)
			{
				m_Errors = new string[1];
				m_Errors[0] = ex.ToString();
				return false;
			}
			return true;
		}


		/// <summary>
		/// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
		/// If the script object exists, the data change notifications will be switched of (for all tables).
		/// Then the Execute function of this script object is called. Afterwards, the data changed notifications are switched on again.
		/// </summary>
		/// <param name="myColumn">The data column this script is working on.</param>
		/// <returns>True if executed without exceptions, otherwise false.</returns>
		/// <remarks>If exceptions were thrown during execution, the exception messages are stored
		/// inside the column script and can be recalled by the Errors property.</remarks>
		public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataColumn myColumn)
		{
			bool bSucceeded=true;
			Altaxo.Data.DataTable myTable=null;
			Altaxo.Data.TableSet   myDataSet=null;

			// first, test some preconditions
			if(null==m_ScriptObject)
			{
				m_Errors = new string[1]{"Script Object is null"};
				return false;
			}

			if(null!=myColumn) myTable=myColumn.ParentTable;
			if(null!=myTable)  myDataSet = myTable.ParentDataSet;


			if(null!=myDataSet) 
				myDataSet.SuspendDataChangedNotifications();
			else if(null!=myTable)
				myTable.SuspendDataChangedNotifications();
			else if(null!=myColumn)
				myColumn.SuspendDataChangedNotifications();

			try
			{
				m_ScriptObject.Execute(myColumn);
			}
			catch(Exception ex)
			{
				bSucceeded = false;
				m_Errors = new string[1];
				m_Errors[0] = ex.ToString();
			}
			finally
			{
				if(null!=myDataSet) 
					myDataSet.ResumeDataChangedNotifications();
				else if(null!=myTable)
					myTable.ResumeDataChangedNotifications();
				else if(null!=myColumn)
					myColumn.ResumeDataChangedNotifications();
			}

			return bSucceeded; 
		}

	} // end of class ColumnScript


	/// <summary>
	/// Holds  column scripts in a hash table. The hash value is the data column the script belongs to.
	/// </summary>
	[Serializable]
	public class ColumnScriptCollection : System.Collections.Hashtable
	{
		/// <summary>
		/// Constructs a empty ColumnScriptCollection
		/// </summary>
		public ColumnScriptCollection()
			: base()
		{
		}
		
		/// <summary>
		/// Special deserialization constructor.
		/// </summary>
		/// <param name="info">Serialization info.</param>
		/// <param name="context">Streaming context.</param>
		public ColumnScriptCollection(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			: base(info,context)
		{
		}

		/// <summary>
		/// Serialization function.
		/// </summary>
		/// <param name="info">Serialization info.</param>
		/// <param name="context">Streaming context.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
		{
			base.GetObjectData(info,context);
		}

		/// <summary>
		/// get / set the column scripts associated with the correspondig columns.
		/// </summary>
		public ColumnScript this[DataColumn dc]
		{
			get { return base[dc] as ColumnScript; }
			set {	base[dc]=value;	}
		}
	}

}
