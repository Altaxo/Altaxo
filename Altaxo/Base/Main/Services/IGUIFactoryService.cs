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

using System;
using System.Threading;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Gui;

namespace Altaxo.Main.Services
{
  public interface IGuiFactoryService
  {
    /// <summary>
    /// Gets the window handle of the main window;
    /// </summary>
    IntPtr MainWindowHandle { get; }

    /// <summary>
    /// Gets the main window as object. Depending on the Gui technology that is used, this is either a Wpf window, or something else.
    /// </summary>
    /// <value>
    /// The main window.
    /// </value>
    object MainWindowObject { get; }

    /// <summary>
    /// Retrieves information about a screen area.
    /// </summary>
    /// <param name="virtual_x">The virtual screen x coordinate of the point on the virtual screen.</param>
    /// <param name="virtual_y">The virtual screen x coordinate of the point on the virtual screen.</param>
    /// <returns>True if the function has successfully retrieved information, false otherwise.</returns>
    RectangleD2D GetScreenInformation(double virtual_x, double virtual_y);

    /// <summary>Gets the screen resolution that is set in windows in dots per inch.</summary>
    PointD2D ScreenResolutionDpi { get; }

    void FindAndAttachControlTo(IMVCController controller);

    object FindAndAttachControlTo(IMVCController controller, Type expectedType);

    IMVCController GetController(object[] args, Type expectedControllerType);

    IMVCController GetController(object[] args, Type expectedControllerType, UseDocument copyDocument);

    IMVCController GetController(object[] creationArgs, Type overrideArg0Type, Type expectedControllerType, UseDocument copyDocument);

    IMVCController GetControllerAndControl(object[] args, Type expectedControllerType);

    IMVCController GetControllerAndControl(object[] args, Type expectedControllerType, UseDocument copyDocument);

    IMVCController GetControllerAndControl(object[] args, Type overrideArg0Type, Type expectedControllerType, UseDocument copyDocument);

    string GetUserFriendlyClassName(Type definedtype);

    string[] GetUserFriendlyClassName(Type[] types, bool withStartingNone);

    string GetUserFriendlyName(Enum value);

    bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.ThreadStart threadstart, IExternalDrivenBackgroundMonitor monitor);

    bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor);

    /// <summary>
    /// Creates and shows a context menu.
    /// </summary>
    /// <param name="parent">The parent gui element.</param>
    /// <param name="owner">The object that will be owner of this context menu.</param>
    /// <param name="addInTreePath">Add in tree path used to build the context menu.</param>
    /// <param name="x">The x coordinate of the location where to show the context menu.</param>
    /// <param name="y">The y coordinate of the location where to show the context menu.</param>
    /// <returns>The context menu. Returns Null if there is no registered context menu provider</returns>
    void ShowContextMenu(object parent, object owner, string addInTreePath, double x, double y);

    bool ShowDialog(IMVCAController controller, string title);

    bool ShowDialog(ref Enum arg, string title);

    bool ShowDialogForEnumFlag(ref System.Enum arg, string title);

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="arg">The object to configure.
    /// If the return value is true, arg contains the configured object. </param>
    /// <param name="title">The title of the dialog.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The presumtions to get this function working are:
    /// <list>
    /// <item>A controller which implements <see cref="IMVCAController" /> has to exist.</item>
    /// <item>A <see cref="UserControllerForObjectAttribute" /> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see cref="UserControlForControllerAttribute" /> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    bool ShowDialog(ref object arg, string title);

    /// <summary>
    /// Shows a configuration dialog for any item.
    /// </summary>
    /// <param name="arg">The object to configure.
    /// If the return value is true, arg contains the configured object. </param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="showApplyButton">If true, the Apply button is shown.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The presumtions to get this function working are:
    /// <list>
    /// <item>A controller which implements <see cref="IMVCAController" /> has to exist.</item>
    /// <item>A <see cref="UserControllerForObjectAttribute" /> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see cref="UserControlForControllerAttribute" /> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    bool ShowDialog<T>(ref T arg, string title, bool showApplyButton);

    bool ShowDialog(IMVCAController controller, string title, bool showApplyButton);

    bool ShowDialog(object[] args, string title, bool showApplyButton);

    bool ShowDialog(object[] args, string title);

    bool ShowOpenFileDialog(OpenFileOptions options);

    bool ShowSaveFileDialog(SaveFileOptions options);

    bool YesNoMessageBox(string txt, string caption, bool defaultanswer);

    /// <summary>
    /// Shows a message box with a questtion to be answered either by YES, NO, or CANCEL.
    /// </summary>
    /// <param name="text">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultAnswer">If true, the default answer is "yes", if false the default answer is "no", if null the default answer is "Cancel".</param>
    /// <returns>True if the user answered with Yes, false if the user answered No, null if the user pressed Cancel.</returns>
    bool? YesNoCancelMessageBox(string text, string caption, bool? defaultAnswer);

    void ErrorMessageBox(string errortxt);

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    /// <param name="title">The title of the message box.</param>
    void ErrorMessageBox(string errortxt, string title);

    void InfoMessageBox(string errortxt);

    void InfoMessageBox(string errortxt, string title);

    #region Clipboard

    /// <summary>
    /// Get a new clipboard data object. You can use this to put data on the clipboard.
    /// </summary>
    /// <returns>A newly created data object.</returns>
    IClipboardSetDataObject GetNewClipboardDataObject();

    /// <summary>
    /// Opens the clipboard data object. You can use this to see which data are on the clipboard.
    /// </summary>
    /// <returns>A data object to see which data are on the clipboard.</returns>
    IClipboardGetDataObject OpenClipboardDataObject();

    /// <summary>
    /// Sets the data stored in the dataObject on the clipboard for temporary usage.
    /// </summary>
    /// <param name="dataObject">Object used to store the data.</param>
    void SetClipboardDataObject(IClipboardSetDataObject dataObject);

    /// <summary>
    /// Sets the data stored in the dataObject on the clipboard for temporary or permanent usage.
    /// </summary>
    /// <param name="dataObject">Object used to store the data.</param>
    /// <param name="value">If true, the data remains on the clipboard when the application is closed. If false, the data will be removed from the clipboard when the application is closed.</param>
    void SetClipboardDataObject(IClipboardSetDataObject dataObject, bool value);

    #endregion Clipboard

    #region Commands

    /// <summary>
    /// Gets a command that executes an action and evaluates the <paramref name="canExecute"/> condition every time when
    /// something in the Gui has changed.
    /// </summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The canExecute function that evaluates if the execute action can be executed under the current conditions. May be null (in this case, it is considered to return true).</param>
    /// <returns>A command that can be used, e.g. for binding to the Gui.</returns>
    ICommand NewRelayCommand(Action execute, Func<bool> canExecute = null);

    /// <summary>
    /// Gets a command that executes an action and evaluates the <paramref name="canExecute"/> condition every time when
    /// something in the Gui has changed.
    /// </summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The canExecute function that evaluates if the execute action can be executed under the current conditions. May be null (in this case, it is considered to return true).</param>
    /// <returns>A command that can be used, e.g. for binding to the Gui.</returns>
    ICommand NewRelayCommand(Action<object> execute, Predicate<object> canExecute = null);

    /// <summary>
    /// Registers a handler that will be called back if something in the Gui has changed so that
    /// a requery of CanExecute() functions might be neccessary. This handler will be bound weak to the event.
    /// Unregister by using <see cref="UnregisterRequerySuggestedHandler(EventHandler)"/>.
    /// </summary>
    /// <param name="handler">The handler.</param>
    void RegisterRequerySuggestedHandler(EventHandler handler);

    /// <summary>
    /// Unregisters the handler that was registered with <see cref="RegisterRequerySuggestedHandler(EventHandler)"/>.
    /// </summary>
    /// <param name="handler">The handler.</param>
    void UnregisterRequerySuggestedHandler(EventHandler handler);

    #endregion Commands
  }
}
