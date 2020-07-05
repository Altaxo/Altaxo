// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  /// <summary>
  /// Static class that can be used to weakly bind to the <see cref="LanguageChanged"/> event that is fired if the Gui language has changed.
  /// </summary>
  public static class LanguageChangeWeakEventManager
  {
    private static WeakDelegate<Action> _languageChanged = new WeakDelegate<Action>();
    private static IResourceService? _resourceService;

    /// <summary>
    /// Occurs when the Gui language has changed. The event is hold weak, thus you can safely add your handler without running in memory leaks.
    /// </summary>
    public static event Action LanguageChanged
    {
      add
      {
        _languageChanged.Combine(value);
      }
      remove
      {
        _languageChanged.Remove(value);
      }
    }

    static LanguageChangeWeakEventManager()
    {
      Current.ServiceChanged += EhServiceChanged;
      EhServiceChanged();
    }

    private static void EhServiceChanged()
    {
      if (null != _resourceService)
      {
        _resourceService.LanguageChanged -= EhLanguageChanged;
      }

      _resourceService = Altaxo.Current.GetService<IResourceService>();

      if (null != _resourceService)
      {
        _resourceService.LanguageChanged += EhLanguageChanged;
      }
    }

    private static void EhLanguageChanged(object? sender, EventArgs e)
    {
      _languageChanged.Target?.Invoke();
    }
  }
}
