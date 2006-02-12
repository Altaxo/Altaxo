#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
namespace Altaxo.Main.Services
{
  /// <summary>
  /// Responsible for retrieving and storing user defined fit functions.
  /// </summary>
  public interface IFitFunctionService
  {
    FitFunctionInformation[] GetUserDefinedFitFunctions();
    bool SaveUserDefinedFitFunction(Altaxo.Scripting.FitFunctionScript doc);
  }

  /// <summary>
  /// Holds information about file based user defined fit function scripts.
  /// </summary>
  public class FitFunctionInformation
  {
    string _name;
    string _category;
    DateTime _creationTime;
    string _fileName;

    public FitFunctionInformation(string category, string name, DateTime creationTime, string filename)
    {
      _category = category;
      _name = name;
      _creationTime = creationTime;
      _fileName = filename;
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

    public string FileName
    {
      get { return _fileName; }

    }

  }
}
