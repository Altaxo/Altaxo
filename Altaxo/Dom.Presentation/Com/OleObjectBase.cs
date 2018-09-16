#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
  using UnmanagedApi.Ole32;

  /// <summary>
  /// Contains almost all methods of the IOleObject interface, and by derivation from DataObjectBase, also almost all methods of the IDataObject interface.
  /// </summary>
  public abstract class OleObjectBase : DataObjectBase
  {
    // Manages our communication with the container.
    protected IOleClientSite _clientSite;

    protected ManagedOleAdviseHolderFM _oleAdviseHolder;

    /// <summary>
    /// Value indicating whether the internal document is out of sync with the document linked or embedded in the container application.
    /// </summary>
    public bool _isDocumentDirty;

    /// <summary>
    /// The document moniker, or null if there is not any.
    /// </summary>
    protected IMoniker _documentMoniker;

    /// <summary>
    /// The document moniker rot cookie. This is the value returned by the RunningObjectTable when registering the Moniker. If no document moniker is registered, this value is 0.
    /// </summary>
    protected int _documentMonikerRotCookie;

    public OleObjectBase(ComManager comManager)
      : base(comManager)
    {
    }

    #region SendAdvise functions

    /// <summary>
    /// This method sends IAdviseSink::OnSave notifications to all advisory sinks registered with the Ole advise holder.
    /// You can call SendOnSave whenever you save the object the advise holder is associated with.
    /// </summary>
    /// <remarks></remarks>
    public virtual void SendAdvise_Saved()
    {
      _oleAdviseHolder.SendOnSave();
    }

    /// <summary>
    /// This method sends notification that the object is closed to all advisory sinks currently registered with the advise holder.
    /// You can call SendOnClose when you determine that a Close operation has been successful.
    /// </summary>
    public virtual void SendAdvise_Closed()
    {
      _oleAdviseHolder.SendOnClose();
    }

    /// <summary>
    /// This method sends IAdviseSink::OnRename notifications to all advisory sinks registered with the advise holder.
    /// You can call SendOnRename in the implementation of IOleObject::SetMoniker, when you have determined that the operation is successful.
    /// </summary>
    public virtual void SendAdvise_Renamed()
    {
      var moniker = _documentMoniker;
      if (null != moniker)
      {
        _oleAdviseHolder.SendOnRename(moniker);
      }
    }

    /// <summary>
    /// This method sends IAdviseSink::OnSave notifications to all advisory sinks registered with the advise holder.
    /// You can call SendOnSave whenever you save the object the advise holder is associated with.
    /// </summary>
    public virtual void SendAdvise_SaveObject()
    {
      if (_isDocumentDirty && null != _clientSite)
      {
        ComDebug.ReportInfo("{0}.SendAdvise.SaveObject -> calling IOleClientSite.SaveObject()", GetType().Name);
        _clientSite.SaveObject();
      }
      else
      {
        ComDebug.ReportInfo("{0}.SendAdvise.SaveObject -> NOT DONE! isDirty={1}, isClientSiteNull={2} )", GetType().Name, _isDocumentDirty, null == _clientSite);
      }
    }

    public override void SendAdvise_DataChanged()
    {
      base.SendAdvise_DataChanged();

      // we must also note the change time to the running object table, see
      // Brockschmidt, Inside Ole 2nd ed., page 989
      var rotCookie = _documentMonikerRotCookie;
      if (rotCookie != 0)
      {
        var ft = new System.Runtime.InteropServices.ComTypes.FILETIME();
        Ole32Func.CoFileTimeNow(out ft);
        RunningObjectTableHelper.GetROT().NoteChangeTime(rotCookie, ref ft);
      }
    }

    /// <summary>
    /// Notifies a container when an embedded object's window is about to become visible.
    /// The container uses this information to shade the object's client site when the object is displayed in a window.
    /// A shaded object, having received this notification, knows that it already has an open window and therefore can respond to being double-clicked by bringing this window quickly to the top, instead of launching its application in order to obtain a new one.
    /// </summary>
    public virtual void SendAdvise_ShowWindow()
    {
      ComDebug.ReportInfo("{0}.SendAdvise.ShowWindow -> Calling IOleClientSite.OnShowWindow(true)", GetType().Name);
      if (null != _clientSite)
      {
        _clientSite.OnShowWindow(true);
      }
    }

    /// <summary>
    /// Notifies a container when an embedded object's window is about to become invisible.
    /// The container uses this information to remove the shading when the object is not visible any more.
    /// </summary>
    public virtual void SendAdvise_HideWindow()
    {
      ComDebug.ReportInfo("{0}.SendAdvise.HideWindow -> Calling IOleClientSite.OnShowWindow(false)", GetType().Name);
      try
      {
        if (null != _clientSite)
        {
          _clientSite.OnShowWindow(false);
        }
      }
      catch (Exception ex)
      {
        ComDebug.ReportError("{0}.SendAdvise.HideWindow -> Exception while calling _clientSite.OnShowWindow(false), Details: {0}", GetType().Name, ex.Message);
      }
    }

    /// <summary>
    /// Asks a container to display its object to the user. This method ensures that the container itself is visible and not minimized.
    /// </summary>
    /// <remarks>
    /// <para>After a link client binds to a link source, it commonly calls IOleObject::DoVerb on the link source, usually
    /// requesting the source to perform some action requiring that it display itself to the user. As part of its implementation
    /// of IOleObject::DoVerb, the link source can call ShowObject, which forces the client to show the link source as best it can.
    /// If the link source's container is itself an embedded object, it will recursively invoke ShowObject on its own container.</para>
    /// <para>Having called the ShowObject method, a link source has no guarantee of being appropriately displayed because its container
    /// may not be able to do so at the time of the call. The ShowObject method does not guarantee visibility, only that the container will do the best it can.</para>
    /// </remarks>
    public virtual void SendAdvise_ShowObject()
    {
      ComDebug.ReportInfo("{0}.SendAdvise.ShowObject -> Calling IOleClientSite.ShowObject()", GetType().Name);
      if (null != _clientSite)
        _clientSite.ShowObject();
    }

    #endregion SendAdvise functions

    #region IOleObject members

    public int SetClientSite(IOleClientSite pClientSite)
    {
      ComDebug.ReportInfo("{0}.IOleObject.SetClientSite", GetType().Name);
      _clientSite = pClientSite;
      return ComReturnValue.NOERROR;
    }

    public IOleClientSite GetClientSite()
    {
      ComDebug.ReportInfo("{0}.IOleObject.GetClientSite", GetType().Name);

      return _clientSite;
    }

    public int SetHostNames(string containerApplicationName, string containerDocumentName)
    {
      // see Brockschmidt, Inside Ole 2nd ed. page 992
      // calling SetHostNames is the only sign that our object is embedded (and thus not linked)
      // this means that we have to switch the user interface from within this function

      ComDebug.ReportInfo("{0}.IOleObject.SetHostNames ContainerAppName={1}, ContainerDocName={2}", GetType().Name, containerApplicationName, containerDocumentName);

      _comManager.SetHostNames(containerApplicationName, containerDocumentName);
      return ComReturnValue.NOERROR;
    }

    public int GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
    {
      // Brockschmidt Inside Ole 2nd ed. page 994
      ComDebug.ReportInfo("{0}.IOleObject.GetMoniker", GetType().Name);
      if (null != _documentMoniker)
      {
        moniker = _documentMoniker;
        return ComReturnValue.S_OK;
      }
      // see Brockschmidt if we want to support linking to embedding
      moniker = null;
      return ComReturnValue.E_FAIL;
    }

    public int SetMoniker(int dwWhichMoniker, object pmk)
    {
      // Brockschmidt Inside Ole 2nd ed. page 993
      // see there if you want to support linking to embedding
      ComDebug.ReportWarning("{0}.IOleObject.SetMoniker => not implemented!", GetType().Name);
      return ComReturnValue.E_NOTIMPL;
    }

    public int InitFromData(System.Runtime.InteropServices.ComTypes.IDataObject pDataObject, int fCreation, int dwReserved)
    {
      ComDebug.ReportWarning("{0}.IOleObject.InitFromData => not implemented!", GetType().Name);
      return ComReturnValue.E_NOTIMPL;
    }

    public int GetClipboardData(int dwReserved, out System.Runtime.InteropServices.ComTypes.IDataObject data)
    {
      ComDebug.ReportInfo("{0}.IOleObject.GetClipboardData => not implemented!", GetType().Name);
      data = null;
      return ComReturnValue.E_NOTIMPL;
    }

    public int EnumVerbs(out IEnumOLEVERB e)
    {
      ComDebug.ReportInfo("{0}.IOleObject.EnumVerbs -> use registry", GetType().Name);
      e = null;
      return ComReturnValue.OLE_S_USEREG;
    }

    public int OleUpdate()
    {
      ComDebug.ReportInfo("{0}.IOleObject.OleUpdate", GetType().Name);
      return ComReturnValue.NOERROR;
    }

    public int IsUpToDate()
    {
      ComDebug.ReportInfo("{0}.IOleObject.IsUpToDate", GetType().Name);
      return ComReturnValue.NOERROR;
    }

    public int GetUserClassID(ref Guid pClsid)
    {
      ComDebug.ReportInfo("{0}.IOleObject.GetUserClassID", GetType().Name);
      pClsid = GetType().GUID;
      return ComReturnValue.NOERROR;
    }

    public int GetUserType(int dwFormOfType, out string userType)
    {
      ComDebug.ReportInfo("{0}.IOleObject.GetUserType -> use registry.", GetType().Name);
      userType = null;
      return ComReturnValue.OLE_S_USEREG;
    }

    public int Advise(System.Runtime.InteropServices.ComTypes.IAdviseSink pAdvSink, out int cookie)
    {
      ComDebug.ReportInfo("{0}.IOleObject.Advise", GetType().Name);
      try
      {
        _oleAdviseHolder.Advise(pAdvSink, out cookie);
        return ComReturnValue.NOERROR;
      }
      catch (Exception e)
      {
        ComDebug.ReportError("{0}.IOleObject.Advise caused an exception: {1}", GetType().Name, e);
        throw;
      }
    }

    public int Unadvise(int dwConnection)
    {
      ComDebug.ReportInfo("{0}.IOleObject.Unadvise", GetType().Name);
      try
      {
        _oleAdviseHolder.Unadvise(dwConnection);
        return ComReturnValue.NOERROR;
      }
      catch (Exception e)
      {
        ComDebug.ReportError("{0}.IOleObject.Unadvise threw an exception: {0}", GetType().Name, e);
        throw;
      }
    }

    public int EnumAdvise(out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA e)
    {
      ComDebug.ReportInfo("{0}.IOleObject.EnumAdvise", GetType().Name);

      e = _oleAdviseHolder.EnumAdvise();
      return ComReturnValue.NOERROR;
    }

    public int SetColorScheme(tagLOGPALETTE pLogpal)
    {
      ComDebug.ReportInfo("{0}.IOleObject.SetColorScheme (not implemented)", GetType().Name);
      return ComReturnValue.E_NOTIMPL;
    }

    #endregion IOleObject members
  }
}
