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
using System.Collections.Generic;
using System.IO;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// IMessageService implementation that writes messages to a text writer.
  /// User input is not implemented by this service.
  /// </summary>
  public class InfoWarningErrorTextMessageService : IInfoWarningErrorTextMessageService
  {
    private Action<InfoWarningErrorTextMessageItem>? _messageAdded;

    /// <summary>
    /// Temporary cache for the items as long as nobody has subscribed to this service. If this member is null,
    /// there was already a subscriber how has acquired the cached items.
    /// </summary>
    private List<InfoWarningErrorTextMessageItem>? _startupListOfItems = new List<InfoWarningErrorTextMessageItem>();

    /// <summary>
    /// Occurs when a message is available. The first subscriber how subscribe to this event will receive all messages
    /// that were up-to-now stored in this call.
    /// </summary>
    public event Action<InfoWarningErrorTextMessageItem> MessageAdded
    {
      add
      {
        bool wasEmptyBefore = null == _messageAdded;

        _messageAdded += value;

        var list = _startupListOfItems;
        if (wasEmptyBefore && null != _messageAdded && null != list)
        {
          _startupListOfItems = null;
          foreach (var item in list)
            _messageAdded(item);
        }
      }
      remove
      {
        _messageAdded -= value;
      }
    }

    public void WriteLine(MessageLevel messageLevel, string source, string text)
    {
      var msg = new InfoWarningErrorTextMessageItem(level: messageLevel,
          source: source,
          message: text,
          timeUtc: DateTime.UtcNow
          );

      var act = _messageAdded;

      if (null != act)
      {
        _messageAdded?.Invoke(msg);
      }
      else if (_startupListOfItems != null)
      {
        _startupListOfItems.Add(msg);
      }
    }

    public void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args)
    {
      WriteLine(messageLevel, source, string.Format(format, args));
    }

    public void WriteLine(MessageLevel messageLevel, string source, System.IFormatProvider provider, string format, params object[] args)
    {
      WriteLine(messageLevel, source, string.Format(provider, format, args));
    }
  }
}
