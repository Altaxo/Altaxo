#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Reflection;
using System.Text;

#nullable enable

namespace Altaxo
{
  /// <summary>
  /// Can be used to build an event that binds the clients weak.
  /// </summary>
  /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
  /// <remarks>Credits and further info: Albahari, Joseph and Ben, C# 6.0 in a Nutshell - The definitive reference.</remarks>
  public class WeakDelegate<TDelegate> where TDelegate : class
  {
    private List<MethodTarget> _targets = new List<MethodTarget>();

    public WeakDelegate()
    {
      if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
        throw new InvalidOperationException("TDelegate must be a delegate type");
    }

    public void Combine(TDelegate? target)
    {
      if (target is Delegate targetDelegate)
      {
        foreach (Delegate d in targetDelegate.GetInvocationList())
          _targets.Add(new MethodTarget(d));
      }
    }

    public void Remove(TDelegate? target)
    {
      if (target is Delegate targetDelegate)
      {
        foreach (Delegate d in targetDelegate.GetInvocationList())
        {
          if (_targets.Find(w => Equals(d.Target, w.Reference?.Target) && Equals(d.Method.MethodHandle, w.Method.MethodHandle)) is { } methodTarget)
          {
            _targets.Remove(methodTarget);
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets the target.
    /// Gets all delegates that are still alive (and removes the dead targets by the way).
    /// If set, it removes the present targets and replaces them with the provided target(s).
    /// </summary>
    /// <value>
    /// The target.
    /// </value>
    public TDelegate? Target
    {
      get
      {
        Delegate? combinedTarget = null;
        foreach (MethodTarget methodTarget in _targets.ToArray())
        {
          var weakReference = methodTarget.Reference;
          //     Static target      ||    alive instance target
          if (weakReference is null || weakReference.Target is not null)
          {
            var newDelegate = Delegate.CreateDelegate(typeof(TDelegate), weakReference?.Target, methodTarget.Method);

            combinedTarget = Delegate.Combine(combinedTarget, newDelegate);
          }
          else // Target is already garbage collected
          {
            _targets.Remove(methodTarget); // thus remove it
          }
        }

        return combinedTarget as TDelegate;
      }
      set
      {
        if (!(value is null))
        {
          _targets.Clear();
          Combine(value);
        }
      }
    }

    #region Inner class

    private class MethodTarget
    {
      public readonly WeakReference Reference;
      public readonly MethodInfo Method;

      public MethodTarget(Delegate d)
      {
        Reference = new WeakReference(d.Target);
        Method = d.Method;
      }
    }

    #endregion Inner class
  }
}
