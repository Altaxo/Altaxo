using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo.Serialization;

namespace Altaxo.Data
{
	/// <summary>
	/// Summary description for ColumnScript.
	/// </summary>
	[SerializationSurrogate(0,typeof(Altaxo.Data.ColumnScript.SerializationSurrogate0))]
	[SerializationVersion(0)]
	[Serializable()]
	public class ColumnScript : ICloneable, System.Runtime.Serialization.IDeserializationCallback
	{
		public enum ScriptStyle { SetColumnValues, SetColumn, FreeStyle };

		protected ScriptStyle m_ScriptStyle = ScriptStyle.SetColumn;

		// public DataColumn m_Column; // the name of the column

		public string m_ScriptText; // the text of the script

		protected string m_RowFrom="0";

		protected string m_RowCondition="<";
		
		protected string m_RowTo="10";
		
		protected string m_RowInc="++"; // the values for the loop conditions

		[NonSerialized()]
		protected bool  m_IsDirty; // true when text changed, can be reseted

		[NonSerialized()]
		protected bool m_Compiled; // false when text changed and not compiled already

		[NonSerialized()]
		protected System.Reflection.Assembly m_ScriptAssembly;

		[NonSerialized()]
		protected Altaxo.Calc.ColScriptExeBase m_ScriptObject; // the compiled and created script object

		[NonSerialized()]
		protected string[] m_Errors=null;



		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				Altaxo.Data.ColumnScript s = (Altaxo.Data.ColumnScript)obj;
				// I decided _not_ to serialize the parent object, since if we only want
				// to serialize this column, we would otherwise serialize the entire object
				// graph
				// info.AddValue("Parent",s.m_Table); // 
				info.AddValue("Style",(int)s.m_ScriptStyle);
				info.AddValue("Text",s.m_ScriptText);
				info.AddValue("From",s.m_RowFrom);
				info.AddValue("Cond",s.m_RowCondition);
				info.AddValue("To",s.m_RowTo);
				info.AddValue("Inc",s.m_RowInc);
			}
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

		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion


		public ColumnScript()
		{
		}

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


		public string[] Errors
		{
			get { return m_Errors; }
		}


		public bool IsDirty
		{
			get { return m_IsDirty; }
			set { m_IsDirty = value; }
		}

			public ScriptStyle Style
		{
			get { return m_ScriptStyle; }
			set { m_ScriptStyle = value; m_IsDirty=true; m_Compiled=false; }
		}


		public string ForFrom
		{
			get { return m_RowFrom; }
			set { m_RowFrom = value; m_IsDirty=true; m_Compiled=false; }
		}

		public string ForCondition
		{
			get { return m_RowCondition; }
			set { m_RowCondition = value; m_IsDirty=true; m_Compiled=false; }
		}

		public string ForEnd
		{
			get { return m_RowTo; }
			set { m_RowTo = value; m_IsDirty=true; m_Compiled=false; }
		}

		public string ForInc
		{
			get { return m_RowInc; }
			set { m_RowInc = value; m_IsDirty=true; m_Compiled=false; }
		}

		public string ScriptBody
		{
			get { return m_ScriptText; }
			set { m_ScriptText = value; m_IsDirty=true; m_Compiled=false; }
		}

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
													"Altaxo.Data.DataSet   tab = (null!=col)? col.ParentDataSet:null;\r\n"+
													"for(int i=" + m_RowFrom + ";i" + m_RowCondition + m_RowTo + ";i" + m_RowInc + ") {\r\n";
						//codestart  =  "cts[i]=";
						//codetail = "} /*for*/ } /*Execute*/  } /*class*/  } /*namespace*/"; 
						break;
					case ScriptStyle.SetColumn:
						codeheader =  "namespace Altaxo {\r\npublic class SetColVal : Altaxo.Calc.ColScriptExeBase {\r\n" +
													"public override void Execute(Altaxo.Data.DataColumn myColumn) {\r\n" +
													"Altaxo.Data.DataTable col = (null!=myColumn)? myColumn.ParentTable:null;\r\n"+
													"Altaxo.Data.DataSet   tab = (null!=col)? col.ParentDataSet:null;\r\n";
													
						//codestart = "col[\"" + dataColumn.ColumnName + "\"]=";
						//codetail = "} /*Execute*/ } /*class*/ } /*namespace*/";
						break;
					case ScriptStyle.FreeStyle:
						codeheader =	"namespace Altaxo {\r\npublic class SetColVal : Altaxo.Calc.ColScriptExeBase {\r\n"+
													"public override void Execute(Altaxo.Data.DataColumn myColumn) {\r\n" +
													"Altaxo.Data.DataTable col = (null!=myColumn)? myColumn.ParentTable:null;\r\n"+
													"Altaxo.Data.DataSet   tab = (null!=col)? col.ParentDataSet:null;\r\n";
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



		public object Clone()
		{
			return new ColumnScript(this);
		}


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

		public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataColumn myColumn)
		{
			bool bSucceeded=true;
			Altaxo.Data.DataTable myTable=null;
			Altaxo.Data.DataSet   myDataSet=null;

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


	[Serializable]
	public class ColumnScriptCollection : System.Collections.Hashtable
	{
		public ColumnScriptCollection()
			: base()
		{
		}
		
		public ColumnScriptCollection(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
			: base(info,context)
		{
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
		{
			base.GetObjectData(info,context);
		}

		public ColumnScript this[DataColumn dc]
		{
			get { return base[dc] as ColumnScript; }
			set {	base[dc]=value;	}
		}
	}

}
