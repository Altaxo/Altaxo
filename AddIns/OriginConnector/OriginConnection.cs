using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Altaxo.Addins.OriginConnector
{
  public class OriginConnection
  {
    private Origin.IOApplication _originApp;				// the Origin object reference

    string _originSaveProjectFileName;

    const int nWorksheetOriginType = 2;		// 2 internally in Origin indicates worksheet window


    public Origin.IOApplication Application
    {
      get
      {
        if (null == _originApp)
          throw new InvalidOperationException("Not connected to Origin");

        return _originApp;
      }
    }


    public bool IsConnected()
    {
      return _originApp != null;
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
        ShowOriginWindow(true);		// show or not Origin window

        _originSaveProjectFileName = GetSaveAsFileName();	// init file name for saving
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

    public Int32 GetInt32(string variableName)
    {
      return (Int32)_originApp.get_LTVar(variableName);
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


 


   
    public bool ExistsWorksheet(string wksName)
    {
      return ExistsOriginObject(wksName, nWorksheetOriginType);
    }

    /// <summary>
    /// Get a list of all worksheet names that are available in the connected origin application.
    /// </summary>
    /// <returns></returns>
    public List<string> GetExistingWorksheetNames()
    {
      List<string> result = new List<string>();
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

   
  }
}
