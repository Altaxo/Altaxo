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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Altaxo.Main.Services.PropertyReflection
{
  /// <summary>
  /// Base class for property-grid items that support property-change notification and deterministic disposal.
  /// </summary>
  /// <remarks>
  /// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
  /// </remarks>
  public abstract class Item : INotifyPropertyChanged, IDisposable
  {
    #region Notify Property Changed Members

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property.
    /// </summary>
    protected void NotifyPropertyChanged(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion Notify Property Changed Members

    #region IDisposable Members

    private bool _disposed = false;

    /// <summary>
    /// Gets a value indicating whether this instance has been disposed.
    /// </summary>
    protected bool Disposed
    {
      get { return _disposed; }
    }

    /// <summary>
    /// Releases the resources used by this instance.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        _disposed = true;
      }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes the instance.
    /// </summary>
    ~Item()
    {
      Dispose(false);
    }

    #endregion IDisposable Members
  }
}
