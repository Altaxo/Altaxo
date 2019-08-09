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
using Altaxo.UnmanagedApi.Ole32;

namespace Altaxo.Com
{
  internal class ClassFactoryBase : IClassFactory
  {
    protected ComManager _comManager;

    protected uint _locked = 0;
    protected uint _classContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
    protected Guid _classId;
    protected uint _flags;
    protected uint _cookie;

    public ClassFactoryBase(ComManager comManager)
    {
      _comManager = comManager;
    }

    public virtual void InternalCreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
    {
      var nullPtr = new IntPtr(0);
      ppvObject = nullPtr;
    }

    public uint ClassContext
    {
      get
      {
        return _classContext;
      }
      set
      {
        _classContext = value;
      }
    }

    public Guid ClassId
    {
      get
      {
        return _classId;
      }
      set
      {
        _classId = value;
      }
    }

    public uint Flags
    {
      get
      {
        return _flags;
      }
      set
      {
        _flags = value;
      }
    }

    /// <summary>
    /// Registers the class factory.
    /// </summary>
    /// <returns></returns>
    public bool RegisterClassObject()
    {
      // Register the class factory
      int i = Ole32Func.CoRegisterClassObject
        (
        ref _classId,
        this,
        ClassContext,
        Flags,
        out _cookie
        );

      ComDebug.ReportInfo("{0}.RegisterClassObject, i={1}, _cookie={2}", GetType().Name, i, _cookie);

      if (i == 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Unregisters the class factory.
    /// </summary>
    /// <returns></returns>
    public bool RevokeClassObject()
    {
      int i = Ole32Func.CoRevokeClassObject(_cookie);

      if (i == 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public static bool ResumeClassObjects()
    {
      int i = Ole32Func.CoResumeClassObjects();

      if (i == 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    #region IClassFactory Implementations

    public void CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
    {
      InternalCreateInstance(pUnkOuter, ref riid, out ppvObject);
    }

    public void LockServer(bool bLock)
    {
      if (bLock)
      {
        _comManager.InterlockedIncrementServerLockCount();
      }
      else
      {
        _comManager.InterlockedDecrementServerLockCount();
      }

      // Always attempt to see if we need to shutdown this server application.
      _comManager.AttemptToTerminateServer();
    }

    #endregion IClassFactory Implementations
  }
}
