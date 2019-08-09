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
using System.ComponentModel;
using Altaxo.Graph.Graph3D;
using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.Pads.LightingPad
{
  public interface ILightingView
  {
    LightSettings Lighting { get; set; }
    bool IsEnabled { get; set; }

    event EventHandler LightingChanged;
  }

  /// <summary>
  /// Responsible for showing the notes of worksheets and graph windows.
  /// </summary>
  [ExpectedTypeOfView(typeof(ILightingView))]
  public class LightingController : AbstractPadContent
  {
    private ILightingView _view;

    /// <summary>The currently active view content to which the text belongs.</summary>
    private WeakReference _currentActiveViewContent = new WeakReference(null);

    public LightingController()
    {
      Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchActiveViewContentChanged, handler => Current.Workbench.ActiveViewContentChanged -= handler);
    }

    private void EhView_LightingChanged(object sender, EventArgs e)
    {
      if (_currentActiveViewContent.IsAlive && object.ReferenceEquals(_currentActiveViewContent.Target, Current.Workbench.ActiveViewContent))
      {
        var ctrl = ActiveViewContentAsGraph3DController;
        if (null != ctrl)
        {
          ctrl.Doc.Lighting = _view.Lighting;
        }
      }
    }

    private Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController ActiveViewContentAsGraph3DController
    {
      get
      {
        var cnt = Current.Workbench.ActiveViewContent;
        if (cnt is Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController grctrl)
          return grctrl;
        else
          return null;
      }
    }

    private void EhWorkbenchActiveViewContentChanged(object sender, EventArgs e)
    {
      if (null == _view)
        return; // happens during shutdown or during creation

      _currentActiveViewContent = new WeakReference(Current.Workbench.ActiveViewContent);

      bool enable = true;

      var ctrl = ActiveViewContentAsGraph3DController;

      if (null != ctrl)
      {
        _view.Lighting = ctrl.Doc.Lighting;
        enable = true;
      }
      else
      {
        _view.Lighting = null;
        enable = false;
      }

      _view.IsEnabled = enable;

      // uncomment the following if we want to activate the lighting pad each time a Graph3D window is activated
      /*
			if (enable && _view.Lighting != null)
			{
				ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;

				var pad = WorkbenchSingleton.Workbench.GetPad(this.GetType());
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(pad);

				// now focus back to the formerly active workbench window.
				ww.SelectWindow();
			}
			*/
    }

    /// <summary>
    /// Stores the text in the text control back to the graph document or worksheet.
    /// </summary>

    #region IPadContent Members

    private void AttachView()
    {
      _view.LightingChanged += EhView_LightingChanged;
      _view.IsEnabled = false;

      EhWorkbenchActiveViewContentChanged(Current.Workbench, EventArgs.Empty); // Find out if the active workbench window is a Graph3d
    }

    private void DetachView()
    {
      _view.LightingChanged -= EhView_LightingChanged;
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(value, _view))
        {
          if (null != _view)
            DetachView();

          _view = value as ILightingView;

          if (null != _view)
            AttachView();
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

    #region IDisposable Members

    public override void Dispose()
    {
      _view = null;
    }

    #endregion IDisposable Members
  }
}
