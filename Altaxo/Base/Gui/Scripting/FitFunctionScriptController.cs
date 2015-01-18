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

using Altaxo.Graph.Gdi;
using Altaxo.Scripting;
using System;
using System.Text.RegularExpressions;

namespace Altaxo.Gui.Scripting
{
	#region Interfaces

	public interface IFitFunctionScriptView
	{
		IFitFunctionScriptViewEventSink Controller { get; set; }

		void Close(bool withOK);

		void SetScriptView(object scriptView);

		void SetCheckUseUserDefinedParameters(bool useUserDefParameters);

		void SetParameterText(string text, bool enable);

		void SetIndependentVariableText(string text);

		void SetDependentVariableText(string text);

		void SetNumberOfParameters(int numberOfParameters, bool enable);

		void EnableScriptView(object view, bool enable);
	}

	public interface IFitFunctionScriptViewEventSink
	{
		void EhView_NumberOfParameterChanged(int numParameter);

		void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters);

		void EhView_UserDefinedParameterTextChanged(string parameterNames);

		void EhView_IndependentVariableTextChanged(string val);

		void EhView_DependentVariableTextChanged(string val);

		void EhView_CommitChanges();

		void EhView_RevertChanges();
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for TableScriptController.
	/// </summary>
	[UserControllerForObject(typeof(IFitFunctionScriptText), 300)]
	[ExpectedTypeOfView(typeof(IFitFunctionScriptView))]
	public class FitFunctionScriptController : IFitFunctionScriptViewEventSink, IScriptController
	{
		protected ScriptExecutionHandler m_ScriptExecution;
		public IFitFunctionScriptText m_Script;
		public IFitFunctionScriptText m_TempScript;

		public IScriptController _scriptController;

		protected IFitFunctionScriptView m_View;

		private int _tempNumberOfParameters;
		private bool _tempIsUsingUserDefinedParameters;
		private string _tempIndependentVariables;
		private string _tempDependentVariables;
		private string _tempUserDefinedParameters;

		public FitFunctionScriptController()
		{
		}

		public FitFunctionScriptController(IFitFunctionScriptText script)
		{
			InitializeDocument(script);
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;
			IFitFunctionScriptText doc = args[0] as IFitFunctionScriptText;
			if (doc == null)
				return false;

			this.m_Script = doc;
			m_TempScript = (IFitFunctionScriptText)m_Script.CloneForModification();

			SetElements(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion IMVCANController Members

		protected void SetElements(bool bInit)
		{
			if (bInit)
			{
				_scriptController = new ScriptController(m_TempScript);
				Current.Gui.FindAndAttachControlTo(_scriptController);

				//View.ScriptName = m_TempScript.ScriptName;
				//_scriptController.ScriptCursorLocation = m_TempScript.UserAreaScriptOffset;
			}

			if (null != View)
			{
				View.SetScriptView(_scriptController.ViewObject);

				_tempIsUsingUserDefinedParameters = m_TempScript.IsUsingUserDefinedParameterNames;
				_tempNumberOfParameters = m_TempScript.NumberOfParameters;
				_tempUserDefinedParameters = this.GetParametersAsLine(_tempIsUsingUserDefinedParameters);
				_tempIndependentVariables = this.GetIndependentVariablesAsLine();
				_tempDependentVariables = this.GetDependentVariablesAsLine();

				View.SetCheckUseUserDefinedParameters(_tempIsUsingUserDefinedParameters);
				View.SetNumberOfParameters(_tempNumberOfParameters, !_tempIsUsingUserDefinedParameters);
				View.SetParameterText(_tempUserDefinedParameters, _tempIsUsingUserDefinedParameters);
				View.SetIndependentVariableText(_tempIndependentVariables);
				View.SetDependentVariableText(_tempDependentVariables);
			}
		}

		public IFitFunctionScriptView View
		{
			get
			{
				return m_View;
			}
			set
			{
				if (null != m_View)
					m_View.Controller = null;

				m_View = value;

				if (null != m_View)
				{
					m_View.Controller = this;
					SetElements(false); // set only the view elements, dont't initialize the variables
				}
			}
		}

		public object ViewObject
		{
			get { return View; }
			set { View = value as IFitFunctionScriptView; }
		}

		private bool IsValidVariableName(string name)
		{
			if (name == null)
				return false;
			if (name == string.Empty)
				return false;
			if (!char.IsLetter(name[0]))
				return false;
			for (int i = 1; i < name.Length; i++)
				if (!char.IsLetterOrDigit(name[i]) && !('_' == name[i]))
					return false;

			return true;
		}

		private string GetDefaultParametersAsLine(int numberOfParameters)
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			int max = Math.Min(numberOfParameters, 9);
			for (int i = 0; i < max; i++)
			{
				stb.Append("P[" + i.ToString() + "]");

				if ((i + 1) < max)
					stb.Append(", ");
			}
			if (numberOfParameters > max)
			{
				stb.Append("..");
				stb.Append("P[" + (numberOfParameters - 1).ToString() + "]");
			}

			return stb.ToString();
		}

		private string GetParametersAsLine(bool isUsingUserDefinedParameters)
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			int i;
			int numberOfParameters = m_TempScript.NumberOfParameters;
			bool userDef = isUsingUserDefinedParameters;

			if (userDef)
			{
				for (i = 0; i < numberOfParameters; ++i)
				{
					stb.Append(m_TempScript.ParameterName(i));
					if ((i + 1) < numberOfParameters)
						stb.Append(", ");
				}
			}
			else
			{
				return GetDefaultParametersAsLine(numberOfParameters);
			}
			return stb.ToString();
		}

		public string GetIndependentVariablesAsLine()
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			for (int i = 0; i < m_TempScript.NumberOfIndependentVariables; ++i)
			{
				stb.Append(m_TempScript.IndependentVariableName(i));
				if ((i + 1) < m_TempScript.NumberOfIndependentVariables)
					stb.Append(", ");
			}
			return stb.ToString();
		}

		public string GetDependentVariablesAsLine()
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			for (int i = 0; i < m_TempScript.NumberOfDependentVariables; ++i)
			{
				stb.Append(m_TempScript.DependentVariableName(i));
				if ((i + 1) < m_TempScript.NumberOfDependentVariables)
					stb.Append(", ");
			}
			return stb.ToString();
		}

		public void EhView_NumberOfParameterChanged(int numParameter)
		{
			_scriptController.Update();
			_tempNumberOfParameters = numParameter;

			if (null != View)
				View.EnableScriptView(_scriptController.ViewObject, false);

			if (View != null && !_tempIsUsingUserDefinedParameters)
				View.SetParameterText(GetDefaultParametersAsLine(_tempNumberOfParameters), _tempIsUsingUserDefinedParameters);
		}

		public void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters)
		{
			_scriptController.Update();
			_tempIsUsingUserDefinedParameters = userDefinedParameters;
			if (null != View)
				View.EnableScriptView(_scriptController.ViewObject, false);

			if (View != null)
			{
				if (!_tempIsUsingUserDefinedParameters)
					View.SetParameterText(GetDefaultParametersAsLine(_tempNumberOfParameters), _tempIsUsingUserDefinedParameters);
			}
		}

		public void EhView_UserDefinedParameterTextChanged(string parameterNames)
		{
			_scriptController.Update();
			_tempUserDefinedParameters = parameterNames;
			if (null != View)
				View.EnableScriptView(_scriptController.ViewObject, false);
		}

		public void EhView_IndependentVariableTextChanged(string val)
		{
			_scriptController.Update();
			_tempIndependentVariables = val;
			if (null != View)
				View.EnableScriptView(_scriptController.ViewObject, false);
		}

		public void EhView_DependentVariableTextChanged(string val)
		{
			_scriptController.Update();
			_tempDependentVariables = val;
			if (null != View)
				View.EnableScriptView(_scriptController.ViewObject, false);
		}

		private bool ParseIndependentVariables(string val, out string[] validNames)
		{
			// parse the parameter string
			string[] vars = val.Split(new char[] { '.', ',', ';', ':', ' ' });
			// test if all parameters start with a numeric character and have only
			// alphanumeric characters
			bool[] valid = new bool[vars.Length];
			int numberValid = 0;
			for (int i = 0; i < vars.Length; i++)
			{
				valid[i] = IsValidVariableName(vars[i]);
				if (valid[i])
					++numberValid;
			}

			validNames = new string[numberValid];
			for (int i = 0, j = 0; j < vars.Length; j++)
			{
				if (valid[j])
					validNames[i++] = vars[j];
			}

			return numberValid > 0;
		}

		private bool ParseDependentVariables(string val, out string[] validNames)
		{
			// parse the parameter string
			string[] vars = val.Split(new char[] { '.', ',', ';', ':', ' ' });
			// test if all parameters start with a numeric character and have only
			// alphanumeric characters
			bool[] valid = new bool[vars.Length];
			int numberValid = 0;
			for (int i = 0; i < vars.Length; i++)
			{
				valid[i] = IsValidVariableName(vars[i]);
				if (valid[i])
					++numberValid;
			}

			validNames = new string[numberValid];
			for (int i = 0, j = 0; j < vars.Length; j++)
			{
				if (valid[j])
					validNames[i++] = vars[j];
			}

			return numberValid > 0;
		}

		private bool ParseUserDefinedParameters(string parameterNames, out string[] validParameterNames)
		{
			// parse the parameter string
			string[] parameters = parameterNames.Split(new char[] { '.', ',', ';', ':', ' ' });
			// test if all parameters start with a numeric character and have only
			// alphanumeric characters
			bool[] parameterValid = new bool[parameters.Length];
			int numberValidParameters = 0;
			for (int i = 0; i < parameters.Length; i++)
			{
				parameterValid[i] = IsValidVariableName(parameters[i]);
				if (parameterValid[i])
					++numberValidParameters;
			}

			validParameterNames = new string[numberValidParameters];
			for (int i = 0, j = 0; j < parameters.Length; j++)
			{
				if (parameterValid[j])
					validParameterNames[i++] = parameters[j];
			}

			return numberValidParameters > 0;
		}

		public void EhView_CommitChanges()
		{
			bool successI = true, successD = true, successP = true;

			string[] independentNames, dependentNames, parameterNames = null;
			successI = ParseIndependentVariables(this._tempIndependentVariables, out independentNames);
			successD = ParseDependentVariables(_tempDependentVariables, out dependentNames);
			if (_tempIsUsingUserDefinedParameters)
				successP = ParseUserDefinedParameters(_tempUserDefinedParameters, out parameterNames);

			if (successI && successD && successP)
			{
				m_TempScript.IndependentVariablesNames = independentNames;
				m_TempScript.DependentVariablesNames = dependentNames;
				m_TempScript.IsUsingUserDefinedParameterNames = _tempIsUsingUserDefinedParameters;
				if (_tempIsUsingUserDefinedParameters)
					m_TempScript.UserDefinedParameterNames = parameterNames;
				else
					m_TempScript.NumberOfParameters = _tempNumberOfParameters;

				_scriptController.SetText(m_TempScript.ScriptText);
				_scriptController.Update();

				SetElements(false);
				View.EnableScriptView(_scriptController.ViewObject, true);
			}
			else // something has gone wrong
			{
				if (!successI)
					Current.Gui.ErrorMessageBox("Invalid independent variable name(s). Check that each independent variable has a valid name (letter followed by letters, digits or underscores).");
				if (!successD)
					Current.Gui.ErrorMessageBox("Invalid dependent variable name(s). Check that each dependent variable has a valid name (letter followed by letters, digits or underscores).");
				if (!successP)
					Current.Gui.ErrorMessageBox("Invalid parameter name(s). Check that each parameter has a valid name (letter followed by letters, digits or underscores).");
			}
		}

		public void EhView_RevertChanges()
		{
			SetElements(false);
			View.EnableScriptView(_scriptController.ViewObject, true);
		}

		#region IMVCController Members

		public object ModelObject
		{
			get
			{
				return m_Script;
			}
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (this._scriptController.Apply(disposeController))
			{
				m_TempScript = (IFitFunctionScriptText)_scriptController.ModelObject;

				m_Script.CopyFrom(m_TempScript, false);
				m_TempScript = (IFitFunctionScriptText)m_Script.CloneForModification();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members

		#region IScriptController Members

		public void SetText(string text)
		{
			this._scriptController.SetText(text);
		}

		public void Compile()
		{
			this._scriptController.Compile();
		}

		public void Update()
		{
			this._scriptController.Update();
		}

		public void Cancel()
		{
		}

		public void Execute(IProgressReporter reporter)
		{
		}

		public bool HasExecutionErrors()
		{
			return false;
		}

		#endregion IScriptController Members
	}
}