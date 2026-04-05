#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// A thread-safe service container class.
  /// </summary>
  public sealed class AltaxoServiceContainer : IServiceProvider, IServiceContainer, IDisposable
  {
    private readonly ConcurrentStack<IServiceProvider> _fallbackServiceProviders = new ConcurrentStack<IServiceProvider>();
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
    private readonly List<Type> servicesToDispose = new List<Type>();
    private readonly Dictionary<Type, object> _taskCompletionSources = new Dictionary<Type, object>(); // object = TaskCompletionSource<T> for various T

    /// <summary>
    /// Initializes a new instance of the <see cref="AltaxoServiceContainer"/> class.
    /// </summary>
    public AltaxoServiceContainer()
    {
      _services.Add(typeof(AltaxoServiceContainer), this);
      _services.Add(typeof(IServiceContainer), this);
    }

    /// <summary>
    /// Adds a fallback service provider that is queried when the service is not found in this container.
    /// </summary>
    /// <param name="provider">The fallback service provider to add.</param>
    public void AddFallbackProvider(IServiceProvider provider)
    {
      _fallbackServiceProviders.Push(provider);
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
      object? instance;
      lock (_services)
      {
        if (_services.TryGetValue(serviceType, out instance))
        {
          if (instance is ServiceCreatorCallback callback)
          {
            instance = callback(this, serviceType);
            if (instance is not null)
            {
              _services[serviceType] = instance;
              OnServiceInitialized(serviceType, instance);
            }
            else
            {
              _services.Remove(serviceType);
            }
          }
        }
      }
      if (instance is not null)
        return instance;
      foreach (var fallbackProvider in _fallbackServiceProviders)
      {
        instance = fallbackProvider.GetService(serviceType);
        if (instance is not null)
          return instance;
      }
      return null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      Type[] disposableTypes;
      lock (_services)
      {
        disposableTypes = servicesToDispose.ToArray();
        //services.Clear();
        servicesToDispose.Clear();
      }
      // dispose services in reverse order of their creation
      for (int i = disposableTypes.Length - 1; i >= 0; i--)
      {
        IDisposable? disposable = null;
        lock (_services)
        {
          if (_services.TryGetValue(disposableTypes[i], out var serviceInstance))
          {
            disposable = serviceInstance as IDisposable;
            if (disposable is not null)
              _services.Remove(disposableTypes[i]);
          }
        }
        disposable?.Dispose();
      }
    }

    private void OnServiceInitialized(Type serviceType, object serviceInstance)
    {
      if (serviceInstance is IDisposable disposableService)
        servicesToDispose.Add(serviceType);

      if (_taskCompletionSources.TryGetValue(serviceType, out dynamic? taskCompletionSource))
      {
        _taskCompletionSources.Remove(serviceType);
        taskCompletionSource.SetResult(serviceInstance);
      }
    }

    /// <inheritdoc/>
    public void AddService(Type serviceType, object serviceInstance)
    {
      lock (_services)
      {
        _services.Add(serviceType, serviceInstance);
        OnServiceInitialized(serviceType, serviceInstance);
      }
    }

    /// <inheritdoc/>
    public void AddService(Type serviceType, object serviceInstance, bool promote)
    {
      AddService(serviceType, serviceInstance);
    }

    /// <inheritdoc/>
    public void AddService(Type serviceType, ServiceCreatorCallback callback)
    {
      lock (_services)
      {
        _services.Add(serviceType, callback);
      }
    }

    /// <inheritdoc/>
    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
      AddService(serviceType, callback);
    }

    /// <inheritdoc/>
    public void RemoveService(Type serviceType)
    {
      lock (_services)
      {
        if (_services.TryGetValue(serviceType, out var instance))
        {
          _services.Remove(serviceType);
          var disposableInstance = instance as IDisposable;
          if (disposableInstance is not null)
            servicesToDispose.Remove(serviceType);
        }
      }
    }

    /// <inheritdoc/>
    public void RemoveService(Type serviceType, bool promote)
    {
      RemoveService(serviceType);
    }

    /// <summary>
    /// Gets a task that completes when a service of the specified type becomes available.
    /// </summary>
    /// <typeparam name="T">The type of service to await.</typeparam>
    /// <returns>A task that returns the requested service instance when it becomes available, or <c>null</c>.</returns>
    public Task<T?> GetFutureService<T>() where T : class
    {
      Type serviceType = typeof(T);
      lock (_services)
      {
        if (_services.ContainsKey(serviceType))
        {
          return Task.FromResult((T?)GetService(serviceType));
        }
        else
        {
          if (_taskCompletionSources.TryGetValue(serviceType, out var taskCompletionSource))
          {
            return ((TaskCompletionSource<T?>)taskCompletionSource).Task;
          }
          else
          {
            var tcs = new TaskCompletionSource<T?>();
            _taskCompletionSources.Add(serviceType, tcs);
            return tcs.Task;
          }
        }
      }
    }
  }
}
