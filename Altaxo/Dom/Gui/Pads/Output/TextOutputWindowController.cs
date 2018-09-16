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

using System;
using Altaxo.Gui.Workbench;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Pads.Output
{
  public interface ITextOutputWindowView
  {
    string Text { get; set; }

    void AppendText(string text);
  }

  /// <summary>
  /// Controls the Output window pad which shows the Altaxo text output.
  /// </summary>
  [ExpectedTypeOfView(typeof(ITextOutputWindowView))]
  public class TextOutputWindowController :
    AbstractPadContent, // is a workbench pad
    ITextOutputService, // implements ITextOutPutService
    IInfoWarningErrorTextMessageService // by the way also implements this service
  {
    private ITextOutputWindowView _view;

    private string _initialText;

    public event Action<InfoWarningErrorTextMessageItem> MessageAdded;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextOutputWindowController"/> class.
    /// Since this constructor is usually called during creation of the pads, it is not registed as a service with the normal
    /// service registration codons. Thus we register it as a service here is this constructor.
    /// </summary>
    public TextOutputWindowController()
    {
      // since this is usually called during creation of the pads, it is not registed as a service with the normal
      // service registration codons
      // thus we register it as a service here

      var oldTextOutputService = Current.GetService<ITextOutputService>();
      if (oldTextOutputService is TextOutputServiceTemporary tempTextOutputService)
        _initialText = tempTextOutputService.Text;
      if (null != oldTextOutputService)
        Current.RemoveService<ITextOutputService>();
      Current.AddService<ITextOutputService>(this);

      // ----------- register also as IInfoWarningErrorTextMessageService if not already registered ----------
      var oldMessageService = Current.GetService<IInfoWarningErrorTextMessageService>();
      if (null == oldMessageService)
      {
        Current.AddService<IInfoWarningErrorTextMessageService>(this);
      }
    }

    #region IPadContent Members

    private void Initialize(bool initData)
    {
    }

    private void AttachView()
    {
      _view.Text = _initialText;
      _initialText = null;
    }

    private void DetachView()
    {
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
        {
          if (null != _view)
            DetachView();

          _view = value as ITextOutputWindowView;

          if (null != _view)
          {
            Initialize(false);
            AttachView();
          }
        }
      }
    }

    public override object ModelObject
    {
      get
      {
        return null;
      }
    }

    #endregion IPadContent Members

    #region IOutputService Members

    private void Write_GuiThreadOnly(string text)
    {
      _view?.AppendText(text);

      if (!(IsSelected && IsVisible))
      {
        var ww = Current.Workbench.ActiveContent;

        // bring this pad to front
        IsActive = true;
        IsSelected = true;
        IsVisible = true;

        // afterwards, bring originally view content to view
        if (ww is IViewContent)
        {
          ww.IsActive = true;
          ww.IsSelected = true;
        }
      }
    }

    public void Write(string text)
    {
      Current.Dispatcher.InvokeIfRequired(Write_GuiThreadOnly, text);
    }

    public void WriteLine()
    {
      Write(System.Environment.NewLine);
    }

    public void WriteLine(string text)
    {
      Write(text + System.Environment.NewLine);
    }

    public void WriteLine(string format, params object[] args)
    {
      Write(string.Format(format, args) + System.Environment.NewLine);
    }

    public void WriteLine(System.IFormatProvider provider, string format, params object[] args)
    {
      Write(string.Format(provider, format, args) + System.Environment.NewLine);
    }

    public void Write(string format, params object[] args)
    {
      Write(string.Format(format, args));
    }

    public void Write(System.IFormatProvider provider, string format, params object[] args)
    {
      Write(string.Format(provider, format, args));
    }

    public void WriteLine(MessageLevel messageLevel, string source, string message)
    {
      Write(string.Format("{0} (source: {1}) : {2}{3}", messageLevel, source, message, System.Environment.NewLine));
    }

    public void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args)
    {
      WriteLine(messageLevel, source, string.Format(format, args));
    }

    public void WriteLine(MessageLevel messageLevel, string source, IFormatProvider provider, string format, params object[] args)
    {
      WriteLine(messageLevel, source, string.Format(provider, format, args));
    }

    #endregion IOutputService Members
  }
}
