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
    string _name;
    string _category;
    DateTime _creationTime;
    string _description;
    string _fileName;

    public FileBasedFitFunctionInformation(string category, string name, DateTime creationTime, string description, string fullfilename)
    {
      _category = category;
      _name = name;
      _creationTime = creationTime;
      _description = description;
      _fileName = fullfilename;
    }

    public string Name
    {
      get { return _name; }

    }

    public string Category
    {
      get { return _category; }

    }

    public DateTime CreationTime
    {
      get { return _creationTime; }

    }

    public string Description
    {
      get { return _description; }

    }

    public string FileName
    {
      get { return _fileName; }

    }

    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      return FitFunctionService.ReadUserDefinedFitFunction(this);
    }

  }

  public class BuiltinFitFunctionInformation : IFitFunctionInformation
  {
    Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute _creatorAttrib;
    System.Reflection.MethodInfo _method;

    public BuiltinFitFunctionInformation(Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute creatorattrib, System.Reflection.MethodInfo method)
    {
      _creatorAttrib = creatorattrib;
      _method = method;
    }

    public Altaxo.Calc.Regression.Nonlinear.FitFunctionCreatorAttribute CreatorAttrib
    {
      get { return _creatorAttrib; }
    }

    public string Name
    {
      get { return _creatorAttrib.Name; }

    }

    public string Category
    {
      get { return _creatorAttrib.Category; }

    }

    public System.Reflection.MethodInfo Method
    {
      get { return _method; }
    }

    public string Description
    {
      get
      {
        object[] attribs = _method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        return GetDocumentationTextFromResource(attribs.Length == 0 ? string.Empty : ((System.ComponentModel.DescriptionAttribute)attribs[0]).Description);
      }
    }

    private string GetDocumentationTextFromResource(string resource)
    {
      string[] resources = resource.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

      System.Text.StringBuilder stb = new StringBuilder();

      foreach (string res in resources)
      {
        if (res.StartsWith("p:"))
        {
          // TODO : Load from a bitmap resource here and add the bitmap to the rtf text

        }
        else
        {
          string rawtext = Current.ResourceService.GetString(res);
          if (rawtext != null && rawtext.Length > 0)
            stb.Append(rawtext);
        }
      }

      return stb.ToString();
    }


    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      return  _method.Invoke(null, new object[] { }) as Altaxo.Calc.Regression.Nonlinear.IFitFunction;
    }

  }


  /// <summary>
  /// Holds information about file based user defined fit function scripts.
  /// </summary>
  public class DocumentFitFunctionInformation : IFitFunctionInformation
  {
    Altaxo.Scripting.FitFunctionScript _fitFunction;

    public DocumentFitFunctionInformation(Altaxo.Scripting.FitFunctionScript script)
    {
      _fitFunction = script;
    }

    public string Name
    {
      get { return _fitFunction.FitFunctionName; }

    }

    public string Category
    {
      get { return _fitFunction.FitFunctionCategory; }

    }

    public DateTime CreationTime
    {
      get { return _fitFunction.CreationTime; }

    }

    public string Description
    {
      get { return _fitFunction.FitFunctionDescription; }

    }

    public Altaxo.Scripting.FitFunctionScript FitFunction
    {
      get { return _fitFunction; }

    }

    public Altaxo.Calc.Regression.Nonlinear.IFitFunction CreateFitFunction()
    {
      return _fitFunction;
    }
  }

}
