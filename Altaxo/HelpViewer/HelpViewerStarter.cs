#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Altaxo.Gui.HelpViewing
{
  /// <summary>
  /// Helper to start viewing help topics in another app domain. See remarks for why this is neccessary.
  /// </summary>
  /// <seealso cref="System.MarshalByRefObject" />
  /// <remarks>
  /// The reason why we need to start the help viewing in another AppDomain is that when we call <see cref="System.Windows.Forms.Help.ShowHelp"/> directly
  /// from Altaxo, the help window is always in the foreground of Altaxo, which makes working with Altaxo while the help topic is open rather difficult.
  /// That's why we load this Dll in another AppDomain, then by means of this helper class start a hidden Windows Forms App by calling <see cref="Start"/>,
  /// and then can make calls to <see cref="ShowHelpTopic(string, string)"/> to show the help topics. In that way we can switch between Altaxo and the Help Viewer
  /// and both Altaxo or the HelpViewer can be in the foreground.
  /// </remarks>
  public class HelpViewerStarter : MarshalByRefObject
  {
    private HiddenMainForm _hiddenMainForm;

    /// <summary>
    /// Starts the hidden windows form app. You must call this method from another app domain using a separate thread which must have ApartmentState STA.
    /// It would be advisable if this thread is a background thread, thus it stops when the main app stops.
    /// After this call, call <see cref="GetState"/> repeatedly until it returns isLoaded==true.
    /// </summary>
    public void Start()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      _hiddenMainForm = new HiddenMainForm();
      Application.Run(_hiddenMainForm);
    }

    public void GetState(out bool isDisposed, out bool isLoaded)
    {
      isDisposed = null == _hiddenMainForm || _hiddenMainForm.IsDisposed;

      isLoaded = null != _hiddenMainForm && !_hiddenMainForm.IsDisposed && _hiddenMainForm.IsLoaded;
    }

    /// <summary>
    /// Shows a help topic.
    /// </summary>
    /// <param name="chmFileName">Full name of the CHM file.</param>
    /// <param name="chmTopic">The CHM topic. This is usually a string that starts with 'html/' and ends with '.htm'</param>
    public void ShowHelpTopic(string chmFileName, string chmTopic)
    {
      if (_hiddenMainForm.InvokeRequired)
      {
        _hiddenMainForm.BeginInvoke(new Action(() => InternalShowHelpTopic(chmFileName, chmTopic)));
      }
      else
      {
        InternalShowHelpTopic(chmFileName, chmTopic);
      }
    }

    private void InternalShowHelpTopic(string helpFileName, string topic)
    {
      try
      {
        _hiddenMainForm.Hide();
        System.Windows.Forms.Help.ShowHelp(_hiddenMainForm, helpFileName, topic);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
