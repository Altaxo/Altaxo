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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Addins.OriginConnector
{
  public class OriginConnection : IDisposable
  {
    private Origin.IOApplication _originApp;        // the Origin object reference

    private string _originSaveProjectFileName;

    private const int nWorksheetOriginType = 2;   // 2 internally in Origin indicates worksheet window

    public Origin.IOApplication Application
    {
      get
      {
        if (_originApp is null)
          throw new InvalidOperationException("Not connected to Origin");

        return _originApp;
      }
    }

    public bool IsConnected()
    {
      return _originApp is not null;
    }

    public bool Connect(bool bConnectExisting)
    {
      if (IsConnected())
        return true;

      try
      {
        if (bConnectExisting)
          _originApp = new Origin.ApplicationSIClass();
        else
          _originApp = new Origin.ApplicationClass();
      }
      catch (Exception e)
      {
        ShowErrorMessage(e.Message);
      }

      bool bConnected = IsConnected();

      if (bConnected)
      {
        ShowOriginWindow(true);   // show or not Origin window

        _originSaveProjectFileName = GetSaveAsFileName(); // init file name for saving
      }
      else
      {
        _originApp = null;
        ShowErrorMessage("Failed to access Origin instance!");
      }
      return bConnected;
    }

    public void Disconnect(bool saveOriginOnDisconnection, string fileName, bool exitOriginOnDisconnection)
    {
      if (IsConnected())
      {
        // Save the project if needed:
        if (saveOriginOnDisconnection)
        {
          string strFileName = fileName;
          if (!(strFileName.Length == 0))
            _originApp.Save(strFileName);
        }

        // Exit Origin if needed:
        if (exitOriginOnDisconnection)
        {
          ExecuteOriginCMD("exit");
        }

        System.Runtime.InteropServices.Marshal.ReleaseComObject(_originApp);
        _originApp = null;
      }

      return;
    }

    public bool ExecuteOriginCMD(string strCmd)
    {
      if (!IsConnected())
        return false;

      bool bb = _originApp.Execute(strCmd, "");
      return bb;
    }

    public double GetDouble(string variableName)
    {
      return _originApp.get_LTVar(variableName);
    }

    public int GetInt32(string variableName)
    {
      return (int)_originApp.get_LTVar(variableName);
    }

    public string GetString(string variableName)
    {
      return _originApp.get_LTStr(variableName);
    }

    private void ShowErrorMessage(string strMsg)
    {
      Altaxo.Current.Gui.ErrorMessageBox(strMsg);
    }

    public void ShowOriginWindow(bool bShow)
    {
      if (!IsConnected())
        return;

      string strCmd = "doc -mk ";
      strCmd += bShow ? "1" : "0";

      ExecuteOriginCMD(strCmd);
    }

    // Get appropriate file name for saving:
    private string GetSaveAsFileName()
    {
      string str = "";
      if (!IsConnected())
        return str;

      str = _originApp.get_LTStr("%Y");
      str += _originApp.get_LTStr("%G");

      return str;
    }

    /// <summary>
    /// Get a list of all worksheet names that are available in the connected origin application.
    /// </summary>
    /// <returns></returns>
    public List<string> GetExistingWorksheetNames()
    {
      var result = new List<string>();
      int cnt = _originApp.WorksheetPages.Count;
      for (int i = 0; i < cnt; i++)
        result.Add(_originApp.WorksheetPages[i].Name);

      return result;
    }

    public bool ExistsOriginObject(string objectName, int expectedOriginType)
    {
      // Check if specified worksheet exists:
      string str = "d=exist(" + objectName + ")";
      int nExists = 0;
      if (ExecuteOriginCMD(str))
      {
        // Get the value of variable "d":
        double rExist = _originApp.get_LTVar("d");
        nExists = (int)rExist;
      }

      return nExists == expectedOriginType;
    }

    public void Dispose()
    {
      Disconnect(false, string.Empty, false);
    }
  }
}
