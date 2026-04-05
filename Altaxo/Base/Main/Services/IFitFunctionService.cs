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
using System.Text;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Responsible for retrieving and storing user defined fit functions.
  /// </summary>
  public interface IFitFunctionService
  {
    /// <summary>
    /// This will get all user defined fit functions.
    /// </summary>
    /// <returns>Array of information about the user defined fit functions.</returns>
    FileBasedFitFunctionInformation[] GetUserDefinedFitFunctions();

    /// <summary>
    /// This removes a fit function that is stored in xml format onto disc.
    /// </summary>
    /// <param name="info">The fit function information (only the file name is used from it).</param>
    void RemoveUserDefinedFitFunction(Main.Services.FileBasedFitFunctionInformation info);

    /// <summary>
    /// This will get all application wide defined fit functions.
    /// </summary>
    /// <returns>Array of information about the application wide defined fit functions.</returns>
    FileBasedFitFunctionInformation[] GetApplicationFitFunctions();

    /// <summary>
    /// This will get all builtin fit functions.
    /// </summary>
    /// <returns>Array of information about the builtin fit functions.</returns>
    BuiltinFitFunctionInformation[] GetBuiltinFitFunctions();

    /// <summary>
    /// This will get all current document fit functions.
    /// </summary>
    /// <returns>Array of document fit function informations.</returns>
    DocumentFitFunctionInformation[] GetDocumentFitFunctions();

    /// <summary>
    /// Saves a user defined fit function in the user's application directory. The user is prompted
    /// by a message box if the function already exists.
    /// </summary>
    /// <param name="doc">The fit function script to save.</param>
    /// <returns>True if the function is saved, otherwise (error or user action) returns false.</returns>
    bool SaveUserDefinedFitFunction(Altaxo.Scripting.FitFunctionScript doc);
  }

  /// <summary>
  /// Provides metadata and factory methods for a fit function.
  /// </summary>
  public interface IFitFunctionInformation
  {
    /// <summary>
    /// Name of the fit function.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Category of the fit function. Can contain DirectorySeparatorChars for dividing in sub-categories.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Returns description text of the fit function. This text can be plain text or can be text with RTF format tags (but without header and trailer). It can
    /// also contain MathML1.0 formulas.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the creation time of this fit function, or null if no creation time is available (e.g. for builtin fit functions).
    /// </summary>
    /// <value>
    /// The creation time, or null.
    /// </value>
    DateTime? CreationTime { get; }

    /// <summary>
    /// Creates an instances of the fit function this info belongs to.
    /// </summary>
    /// <returns>Instance of the fit function. An exception is thrown if the fit function could not be created.</returns>
    Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction();
  }

  /// <summary>
  /// Holds information about file based user defined fit function scripts.
  /// </summary>
  public class FileBasedFitFunctionInformation : IFitFunctionInformation
  {
    private string _name;
    private string _category;
    private DateTime _creationTime;
    private string _description;
    private string _fileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileBasedFitFunctionInformation"/> class.
    /// </summary>
    /// <param name="category">The fit function category.</param>
    /// <param name="name">The fit function name.</param>
    /// <param name="creationTime">The creation time.</param>
    /// <param name="description">The fit function description.</param>
    /// <param name="fullfilename">The file name that stores the fit function.</param>
    public FileBasedFitFunctionInformation(string category, string name, DateTime creationTime, string description, string fullfilename)
    {
      _category = category;
      _name = name;
      _creationTime = creationTime;
      _description = description;
      _fileName = fullfilename;
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return _name; }
    }

    /// <inheritdoc/>
    public string Category
    {
      get { return _category; }
    }

    /// <inheritdoc/>
    public DateTime? CreationTime
    {
      get { return _creationTime; }
    }

    /// <inheritdoc/>
    public string Description
    {
      get { return _description; }
    }

    /// <summary>
    /// Gets the full file name of the fit function script.
    /// </summary>
    public string FileName
    {
      get { return _fileName; }
    }

    /// <inheritdoc/>
    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      return FitFunctionService.ReadUserDefinedFitFunction(this) ??
        throw new InvalidOperationException($"Could not create fit function from file {_fileName}");
    }
  }

  /// <summary>
  /// Holds information about built-in fit functions.
  /// </summary>
  public class BuiltinFitFunctionInformation : IFitFunctionInformation
  {
    private Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute _creatorAttrib;
    private System.Reflection.MethodInfo _method;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuiltinFitFunctionInformation"/> class.
    /// </summary>
    /// <param name="creatorattrib">The creator attribute.</param>
    /// <param name="method">The method that creates the fit function.</param>
    public BuiltinFitFunctionInformation(Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute creatorattrib, System.Reflection.MethodInfo method)
    {
      _creatorAttrib = creatorattrib;
      _method = method;
    }

    /// <summary>
    /// Gets the creator attribute that describes the built-in fit function.
    /// </summary>
    public Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute CreatorAttrib
    {
      get { return _creatorAttrib; }
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return _creatorAttrib.Name; }
    }

    /// <inheritdoc/>
    public string Category
    {
      get { return _creatorAttrib.Category; }
    }

    /// <inheritdoc/>
    public DateTime? CreationTime
    {
      get { return null; }
    }

    /// <summary>
    /// Gets the method that creates the fit function.
    /// </summary>
    public System.Reflection.MethodInfo Method
    {
      get { return _method; }
    }

    /// <inheritdoc/>
    public string Description
    {
      get
      {
        object[] attribs = _method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        return (attribs.Length == 0) ? string.Empty : StringParser.Parse(((System.ComponentModel.DescriptionAttribute)attribs[0]).Description);
      }
    }

    /// <inheritdoc/>
    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      var result = _method.Invoke(null, new object[] { });
      if (result is Altaxo.Calc.Regression.Nonlinear.IFitFunction fitfunction)
        return fitfunction;
      else
        throw new InvalidProgramException($"Method that should create fit function actually returned {result}");

    }
  }

  /// <summary>
  /// Holds information about fit function scripts stored in the current document.
  /// </summary>
  public class DocumentFitFunctionInformation : IFitFunctionInformation
  {
    private Altaxo.Scripting.FitFunctionScript _fitFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentFitFunctionInformation"/> class.
    /// </summary>
    /// <param name="script">The fit function script.</param>
    public DocumentFitFunctionInformation(Altaxo.Scripting.FitFunctionScript script)
    {
      _fitFunction = script;
    }

    /// <inheritdoc/>
    public string Name
    {
      get { return _fitFunction.FitFunctionName; }
    }

    /// <inheritdoc/>
    public string Category
    {
      get { return _fitFunction.FitFunctionCategory; }
    }

    /// <inheritdoc/>
    public DateTime? CreationTime
    {
      get { return _fitFunction.CreationTime; }
    }

    /// <inheritdoc/>
    public string Description
    {
      get { return _fitFunction.FitFunctionDescription; }
    }

    /// <summary>
    /// Gets the underlying fit function script.
    /// </summary>
    public Altaxo.Scripting.FitFunctionScript FitFunction
    {
      get { return _fitFunction; }
    }

    /// <inheritdoc/>
    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      return _fitFunction;
    }
  }
}
