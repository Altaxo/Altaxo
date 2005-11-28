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
using System.Configuration;
using System.Collections;
using System.Xml;

namespace ICSharpCode.SharpDevelop
{
  public class AddInSettingsHandler : System.Configuration.IConfigurationSectionHandler
  {
    public AddInSettingsHandler()
    {
    }

    public object Create(object parent, object configContext, System.Xml.XmlNode section)
    {
      ArrayList addInDirectories = new ArrayList();
      XmlNode attr = section.Attributes.GetNamedItem("ignoreDefaultPath");
      if (attr != null) 
      {
        try 
        {
          addInDirectories.Add(Convert.ToBoolean(attr.Value));
        } 
        catch (InvalidCastException) 
        {
          addInDirectories.Add(false);
        }
      } 
      else 
      {

        addInDirectories.Add(false);
      }
      
      XmlNodeList addInDirList = section.SelectNodes("AddInDirectory");
      foreach (XmlNode addInDir in addInDirList) 
      {
        XmlNode path = addInDir.Attributes.GetNamedItem("path");
        if (path != null) 
        {

          addInDirectories.Add(path.Value);
        }
      }
      return addInDirectories;
    }

    public static string[] GetAddInDirectories(out bool ignoreDefaultPath)
    {
      ArrayList addInDirs = System.Configuration.ConfigurationSettings.GetConfig("AddInDirectories") as ArrayList;
      if (addInDirs != null) 
      {
        int count = addInDirs.Count;
        if (count <= 1) 
        {

          ignoreDefaultPath = false;
          return null;
        }
        ignoreDefaultPath = (bool) addInDirs[0];
        string [] directories = new string[count-1];
        for (int i = 0; i < count - 1; i++) 
        {

          directories[i] = addInDirs[i+1] as string;
        }
        return directories;
      }
      ignoreDefaultPath = false;
      return null;
    }
  }
}
