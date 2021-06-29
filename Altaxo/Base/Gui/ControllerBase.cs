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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Gui
{
  /// <summary>
  /// Provides some basic properties common for all controllers.
  /// </summary>
  public class ControllerBase : INotifyPropertyChanged, IDisposable
  {
    /// <summary>Get a value indication whether  this controller is already disposed.</summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;


    protected InvalidOperationException NoDocumentException =>
      new InvalidOperationException($"Controller {GetType()} was not initialized with a document");


    protected InvalidOperationException NoViewException =>
      new InvalidOperationException($"Controller {GetType()} currently has no view.");

    protected InvalidOperationException NotInitializedException =>
      new InvalidOperationException($"Controller {GetType()} is not property initialized.");


    protected virtual void CheckDocumentInitialized<T>([AllowNull][NotNull] ref T doc)
    {
      if (doc is null)
        throw NoDocumentException;
    }


    public virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    public virtual void Dispose(bool isDisposing)
    {
      IsDisposed = true;
      PropertyChanged = null;
    }

    #region Helper functions

    /// <summary>
    /// Helper function to dispose a member and set it to null.
    /// </summary>
    /// <typeparam name="T">Type of the object to dispose.</typeparam>
    /// <param name="objectToDispose">The object to dispose.</param>
    protected static void DisposeAndSetToNull<T>(ref T? objectToDispose) where T : class, IDisposable
    {
      if (objectToDispose is not null)
      {
        objectToDispose.Dispose();
        objectToDispose = null;
      }
    }

    #endregion Helper functions

  }
}
