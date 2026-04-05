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

#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces

  /// <summary>
  /// View interface for editing fit function scripts.
  /// </summary>
  public interface IFitFunctionScriptView
  {
    /// <summary>
    /// Gets or sets the controller that handles view events.
    /// </summary>
    IFitFunctionScriptViewEventSink Controller { get; set; }

    /// <summary>
    /// Closes the view.
    /// </summary>
    /// <param name="withOK"><see langword="true"/> to close with acceptance; otherwise, <see langword="false"/>.</param>
    void Close(bool withOK);

    /// <summary>
    /// Sets the embedded script view.
    /// </summary>
    /// <param name="scriptView">The script view object.</param>
    void SetScriptView(object scriptView);

    /// <summary>
    /// Sets whether user-defined parameters are enabled.
    /// </summary>
    /// <param name="useUserDefParameters"><see langword="true"/> to use user-defined parameters; otherwise, <see langword="false"/>.</param>
    void SetCheckUseUserDefinedParameters(bool useUserDefParameters);

    /// <summary>
    /// Sets the parameter text.
    /// </summary>
    /// <param name="text">The parameter text.</param>
    /// <param name="enable"><see langword="true"/> to enable editing; otherwise, <see langword="false"/>.</param>
    void SetParameterText(string text, bool enable);

    /// <summary>
    /// Sets the independent-variable text.
    /// </summary>
    /// <param name="text">The text to display.</param>
    void SetIndependentVariableText(string text);

    /// <summary>
    /// Sets the dependent-variable text.
    /// </summary>
    /// <param name="text">The text to display.</param>
    void SetDependentVariableText(string text);

    /// <summary>
    /// Sets the number of parameters.
    /// </summary>
    /// <param name="numberOfParameters">The number of parameters.</param>
    /// <param name="enable"><see langword="true"/> to enable editing; otherwise, <see langword="false"/>.</param>
    void SetNumberOfParameters(int numberOfParameters, bool enable);

    /// <summary>
    /// Enables or disables the script view.
    /// </summary>
    /// <param name="view">The view object.</param>
    /// <param name="enable"><see langword="true"/> to enable the view; otherwise, <see langword="false"/>.</param>
    void EnableScriptView(object view, bool enable);
  }

  /// <summary>
  /// Event sink interface for <see cref="IFitFunctionScriptView"/>.
  /// </summary>
  public interface IFitFunctionScriptViewEventSink
  {
    /// <summary>
    /// Handles changes to the number of parameters.
    /// </summary>
    /// <param name="numParameter">The new number of parameters.</param>
    void EhView_NumberOfParameterChanged(int numParameter);

    /// <summary>
    /// Handles changes to the user-defined-parameter setting.
    /// </summary>
    /// <param name="userDefinedParameters"><see langword="true"/> if user-defined parameters are enabled; otherwise, <see langword="false"/>.</param>
    void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters);

    /// <summary>
    /// Handles changes to the user-defined parameter text.
    /// </summary>
    /// <param name="parameterNames">The entered parameter names.</param>
    void EhView_UserDefinedParameterTextChanged(string parameterNames);

    /// <summary>
    /// Handles changes to the independent-variable text.
    /// </summary>
    /// <param name="val">The entered value.</param>
    void EhView_IndependentVariableTextChanged(string val);

    /// <summary>
    /// Handles changes to the dependent-variable text.
    /// </summary>
    /// <param name="val">The entered value.</param>
    void EhView_DependentVariableTextChanged(string val);

    /// <summary>
    /// Commits the pending view changes.
    /// </summary>
    void EhView_CommitChanges();

    /// <summary>
    /// Reverts the pending view changes.
    /// </summary>
    void EhView_RevertChanges();
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for editing <see cref="IFitFunctionScriptText"/> documents.
  /// </summary>
  [UserControllerForObject(typeof(IFitFunctionScriptText), 300)]
  [ExpectedTypeOfView(typeof(IFitFunctionScriptView))]
  public class FitFunctionScriptController : IFitFunctionScriptViewEventSink, IScriptController
  {
    /// <summary>
    /// Holds the script execution handler.
    /// </summary>
    protected ScriptExecutionHandler m_ScriptExecution;

    /// <summary>
    /// Holds the original script document.
    /// </summary>
    public IFitFunctionScriptText m_Script;

    /// <summary>
    /// Holds the temporary editable script document.
    /// </summary>
    public IFitFunctionScriptText m_TempScript;

    /// <summary>
    /// Holds the embedded script controller.
    /// </summary>
    public IScriptController _scriptController;

    /// <summary>
    /// Holds the attached view.
    /// </summary>
    protected IFitFunctionScriptView m_View;

    private int _tempNumberOfParameters;
    private bool _tempIsUsingUserDefinedParameters;
    private string _tempIndependentVariables;
    private string _tempDependentVariables;
    private string _tempUserDefinedParameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionScriptController"/> class.
    /// </summary>
    public FitFunctionScriptController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionScriptController"/> class.
    /// </summary>
    /// <param name="script">The script document to edit.</param>
    public FitFunctionScriptController(IFitFunctionScriptText script)
    {
      InitializeDocument(script);
    }

    #region IMVCANController Members

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
        return false;
      var doc = args[0] as IFitFunctionScriptText;
      if (doc is null)
        return false;

      m_Script = doc;
      m_TempScript = (IFitFunctionScriptText)m_Script.CloneForModification();

      SetElements(true);

      return true;
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    #endregion IMVCANController Members

    /// <summary>
    /// Initializes and updates view elements from the current temporary model.
    /// </summary>
    /// <param name="bInit">If set to <c>true</c>, the embedded script controller is created.</param>
    protected void SetElements(bool bInit)
    {
      if (bInit)
      {
        _scriptController = new ScriptController(m_TempScript);
        Current.Gui.FindAndAttachControlTo(_scriptController);

        //View.ScriptName = m_TempScript.ScriptName;
        //_scriptController.ScriptCursorLocation = m_TempScript.UserAreaScriptOffset;
      }

      if (View is not null)
      {
        View.SetScriptView(_scriptController.ViewObject);

        _tempIsUsingUserDefinedParameters = m_TempScript.IsUsingUserDefinedParameterNames;
        _tempNumberOfParameters = m_TempScript.NumberOfParameters;
        _tempUserDefinedParameters = GetParametersAsLine(_tempIsUsingUserDefinedParameters);
        _tempIndependentVariables = GetIndependentVariablesAsLine();
        _tempDependentVariables = GetDependentVariablesAsLine();

        View.SetCheckUseUserDefinedParameters(_tempIsUsingUserDefinedParameters);
        View.SetNumberOfParameters(_tempNumberOfParameters, !_tempIsUsingUserDefinedParameters);
        View.SetParameterText(_tempUserDefinedParameters, _tempIsUsingUserDefinedParameters);
        View.SetIndependentVariableText(_tempIndependentVariables);
        View.SetDependentVariableText(_tempDependentVariables);
      }
    }

    /// <summary>
    /// Gets or sets the attached view.
    /// </summary>
    public IFitFunctionScriptView View
    {
      get
      {
        return m_View;
      }
      set
      {
        if (m_View is not null)
          m_View.Controller = null;

        m_View = value;

        if (m_View is not null)
        {
          m_View.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    /// <inheritdoc/>
    public object ViewObject
    {
      get { return View; }
      set { View = value as IFitFunctionScriptView; }
    }

    private bool IsValidVariableName(string name)
    {
      if (name is null)
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
      var stb = new System.Text.StringBuilder();
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
      var stb = new System.Text.StringBuilder();
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

    /// <summary>
    /// Gets the independent-variable names as a comma-separated string.
    /// </summary>
    /// <returns>The formatted independent-variable names.</returns>
    public string GetIndependentVariablesAsLine()
    {
      var stb = new System.Text.StringBuilder();
      for (int i = 0; i < m_TempScript.NumberOfIndependentVariables; ++i)
      {
        stb.Append(m_TempScript.IndependentVariableName(i));
        if ((i + 1) < m_TempScript.NumberOfIndependentVariables)
          stb.Append(", ");
      }
      return stb.ToString();
    }

    /// <summary>
    /// Gets the dependent-variable names as a comma-separated string.
    /// </summary>
    /// <returns>The formatted dependent-variable names.</returns>
    public string GetDependentVariablesAsLine()
    {
      var stb = new System.Text.StringBuilder();
      for (int i = 0; i < m_TempScript.NumberOfDependentVariables; ++i)
      {
        stb.Append(m_TempScript.DependentVariableName(i));
        if ((i + 1) < m_TempScript.NumberOfDependentVariables)
          stb.Append(", ");
      }
      return stb.ToString();
    }

    /// <inheritdoc/>
    public void EhView_NumberOfParameterChanged(int numParameter)
    {
      _scriptController.Update();
      _tempNumberOfParameters = numParameter;

      if (View is not null)
        View.EnableScriptView(_scriptController.ViewObject, false);

      if (View is not null && !_tempIsUsingUserDefinedParameters)
        View.SetParameterText(GetDefaultParametersAsLine(_tempNumberOfParameters), _tempIsUsingUserDefinedParameters);
    }

    /// <inheritdoc/>
    public void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters)
    {
      _scriptController.Update();
      _tempIsUsingUserDefinedParameters = userDefinedParameters;
      if (View is not null)
        View.EnableScriptView(_scriptController.ViewObject, false);

      if (View is not null)
      {
        if (!_tempIsUsingUserDefinedParameters)
          View.SetParameterText(GetDefaultParametersAsLine(_tempNumberOfParameters), _tempIsUsingUserDefinedParameters);
      }
    }

    /// <inheritdoc/>
    public void EhView_UserDefinedParameterTextChanged(string parameterNames)
    {
      _scriptController.Update();
      _tempUserDefinedParameters = parameterNames;
      if (View is not null)
        View.EnableScriptView(_scriptController.ViewObject, false);
    }

    /// <inheritdoc/>
    public void EhView_IndependentVariableTextChanged(string val)
    {
      _scriptController.Update();
      _tempIndependentVariables = val;
      if (View is not null)
        View.EnableScriptView(_scriptController.ViewObject, false);
    }

    /// <inheritdoc/>
    public void EhView_DependentVariableTextChanged(string val)
    {
      _scriptController.Update();
      _tempDependentVariables = val;
      if (View is not null)
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

    /// <inheritdoc/>
    public void EhView_CommitChanges()
    {
      bool successI = true, successD = true, successP = true;

      string[] parameterNames = null;
      successI = ParseIndependentVariables(_tempIndependentVariables, out var independentNames);
      successD = ParseDependentVariables(_tempDependentVariables, out var dependentNames);
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

    /// <inheritdoc/>
    public void EhView_RevertChanges()
    {
      SetElements(false);
      View.EnableScriptView(_scriptController.ViewObject, true);
    }

    #region IMVCController Members

    /// <inheritdoc/>
    public object ModelObject
    {
      get
      {
        return m_Script;
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    #endregion IMVCController Members

    #region IApplyController Members

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      if (_scriptController.Apply(disposeController))
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
    /// Tries to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>true</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IApplyController Members

    #region IScriptController Members

    /// <inheritdoc/>
    public void SetText(string text)
    {
      _scriptController.SetText(text);
    }

    /// <inheritdoc/>
    public Task Compile(CancellationToken cancellationToken)
    {
      return _scriptController.Compile(cancellationToken);
    }

    /// <inheritdoc/>
    public void Update()
    {
      _scriptController.Update();
    }

    /// <inheritdoc/>
    public void Cancel()
    {
    }

    /// <inheritdoc/>
    public void Execute(IProgressReporter reporter)
    {
    }

    /// <inheritdoc/>
    public bool HasExecutionErrors()
    {
      return false;
    }

    #endregion IScriptController Members
  }
}
