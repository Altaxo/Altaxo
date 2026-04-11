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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Gui;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Provides factory and dialog services for GUI-related controllers and views.
  /// </summary>
  public interface IGuiFactoryService
  {
    /// <summary>
    /// Gets the window handle of the main window.
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

    /// <summary>
    /// Finds and attaches a suitable control to the specified controller.
    /// </summary>
    /// <param name="controller">The controller to attach a control to.</param>
    void FindAndAttachControlTo(IMVCController controller);

    /// <summary>
    /// Finds and attaches a suitable control of the expected type to the specified controller.
    /// </summary>
    /// <param name="controller">The controller to attach a control to.</param>
    /// <param name="expectedType">The expected control type.</param>
    /// <returns>The attached control, or <c>null</c> if no suitable control was found.</returns>
    object? FindAndAttachControlTo(IMVCController controller, Type expectedType);

    /// <summary>
    /// Gets a controller for the specified arguments.
    /// </summary>
    /// <param name="args">The controller creation arguments.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <returns>The controller, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetController(object[] args, Type expectedControllerType);

    /// <summary>
    /// Gets a controller for the specified arguments and document usage mode.
    /// </summary>
    /// <param name="args">The controller creation arguments.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <param name="copyDocument">Specifies how the document should be used.</param>
    /// <returns>The controller, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetController(object[] args, Type expectedControllerType, UseDocument copyDocument);

    /// <summary>
    /// Gets a controller for the specified arguments while overriding the type of the first argument.
    /// </summary>
    /// <param name="creationArgs">The controller creation arguments.</param>
    /// <param name="overrideArg0Type">The override type for the first argument.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <param name="copyDocument">Specifies how the document should be used.</param>
    /// <returns>The controller, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetController(object[] creationArgs, Type overrideArg0Type, Type expectedControllerType, UseDocument copyDocument);

    /// <summary>
    /// Gets a controller together with an attached control for the specified arguments.
    /// </summary>
    /// <param name="args">The controller creation arguments.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <returns>The controller with an attached control, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetControllerAndControl(object[] args, Type expectedControllerType);

    /// <summary>
    /// Gets the required controller and control. Throws an exception if either controller or control could not be retrieved.
    /// </summary>
    /// <typeparam name="T">The type of expected controller.</typeparam>
    /// <param name="arg">The first argument. Always required.</param>
    /// <param name="args">Additional arguments.</param>
    /// <returns>The controller for the provided arguments. A control is already set.</returns>
    T GetRequiredControllerAndControl<T>(object arg, params object?[]? args) where T : class, IMVCController;

    /// <summary>
    /// Gets a controller together with an attached control for the specified arguments and document usage mode.
    /// </summary>
    /// <param name="args">The controller creation arguments.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <param name="copyDocument">Specifies how the document should be used.</param>
    /// <returns>The controller with an attached control, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetControllerAndControl(object[] args, Type expectedControllerType, UseDocument copyDocument);

    /// <summary>
    /// Gets a controller together with an attached control while overriding the type of the first argument.
    /// </summary>
    /// <param name="args">The controller creation arguments.</param>
    /// <param name="overrideArg0Type">The override type for the first argument.</param>
    /// <param name="expectedControllerType">The expected controller type.</param>
    /// <param name="copyDocument">Specifies how the document should be used.</param>
    /// <returns>The controller with an attached control, or <see langword="null"/> if no suitable controller was found.</returns>
    IMVCController? GetControllerAndControl(object[] args, Type overrideArg0Type, Type expectedControllerType, UseDocument copyDocument);

    /// <summary>
    /// Gets a user-friendly class name for the specified type.
    /// </summary>
    /// <param name="definedtype">The type to describe.</param>
    /// <returns>A user-friendly class name.</returns>
    string GetUserFriendlyClassName(Type definedtype);

    /// <summary>
    /// Gets user-friendly class names for the specified types.
    /// </summary>
    /// <param name="types">The types to describe.</param>
    /// <param name="withStartingNone">If set to <c>true</c>, a leading `None` entry is included.</param>
    /// <returns>The user-friendly class names.</returns>
    string[] GetUserFriendlyClassName(Type[] types, bool withStartingNone);

    /// <summary>
    /// Gets a user-friendly name for the specified enumeration value.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <returns>A user-friendly name.</returns>
    string GetUserFriendlyName(Enum value);

    /// <summary>
    /// Shows a cancel dialog for a background thread that is started by the service.
    /// </summary>
    /// <param name="millisecondsDelay">The delay before showing the dialog.</param>
    /// <param name="threadstart">The thread entry point.</param>
    /// <param name="monitor">The monitor that reports progress and cancellation state.</param>
    /// <returns><see langword="true"/> if the operation completed successfully; otherwise, <see langword="false"/>.</returns>
    bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.ThreadStart threadstart, IExternalDrivenBackgroundMonitor monitor);

    /// <summary>
    /// Shows a cancel dialog for an already created background thread.
    /// </summary>
    /// <param name="millisecondsDelay">The delay before showing the dialog.</param>
    /// <param name="thread">The existing thread to monitor.</param>
    /// <param name="monitor">The monitor that reports progress and cancellation state.</param>
    /// <returns><see langword="true"/> if the operation completed successfully; otherwise, <see langword="false"/>.</returns>
    bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor);

    /// <summary>
    /// Shows a cancel dialog for a task.
    /// </summary>
    /// <param name="millisecondsDelay">The delay before showing the dialog.</param>
    /// <param name="task">The task to monitor.</param>
    /// <param name="monitor">The monitor that reports progress and cancellation state.</param>
    /// <returns><see langword="true"/> if the operation completed successfully; otherwise, <see langword="false"/>.</returns>
    bool ShowTaskCancelDialog(int millisecondsDelay, System.Threading.Tasks.Task task, IExternalDrivenBackgroundMonitor monitor);

    /// <summary>
    /// Executes an action that can be cancelled by the user.
    /// </summary>
    /// <param name="millisecondsDelay">The delay before showing cancellation UI.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The exception thrown by the action, or <see langword="null"/> if execution finished without an exception.</returns>
    Exception? ExecuteAsUserCancellable(int millisecondsDelay, Action<IProgressReporter> action);

    /// <summary>
    /// Creates and shows a context menu.
    /// </summary>
    /// <param name="parent">The parent gui element.</param>
    /// <param name="owner">The object that will be owner of this context menu.</param>
    /// <param name="addInTreePath">Add in tree path used to build the context menu.</param>
    /// <param name="x">The x coordinate of the location where to show the context menu.</param>
    /// <param name="y">The y coordinate of the location where to show the context menu.</param>
    /// <returns>The context menu. Returns <c>null</c> if there is no registered context menu provider.</returns>
    void ShowContextMenu(object parent, object owner, string addInTreePath, double x, double y);

    /// <summary>
    /// Shows a dialog for the specified controller.
    /// </summary>
    /// <param name="controller">The controller to show.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialog(IMVCAController controller, string title);

    /// <summary>
    /// Shows a dialog for selecting an enumeration value.
    /// </summary>
    /// <param name="arg">The enumeration value to edit.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialog(ref Enum arg, string title);

    /// <summary>
    /// Shows a dialog for editing enumeration flags.
    /// </summary>
    /// <param name="arg">The enumeration value to edit.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialogForEnumFlag(ref System.Enum arg, string title);

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="arg">The object to configure.
    /// If the return value is true, arg contains the configured object. </param>
    /// <param name="title">The title of the dialog.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The prerequisites for this function are:
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
    /// <typeparam name="T">The type of the object to configure.</typeparam>
    /// <param name="arg">The object to configure.
    /// If the return value is true, arg contains the configured object. </param>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="showApplyButton">If true, the Apply button is shown.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The prerequisites for this function are:
    /// <list>
    /// <item>A controller which implements <see cref="IMVCAController" /> has to exist.</item>
    /// <item>A <see cref="UserControllerForObjectAttribute" /> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see cref="UserControlForControllerAttribute" /> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    bool ShowDialog<T>([DisallowNull][NotNull] ref T arg, string title, bool showApplyButton);

    /// <summary>
    /// Shows a dialog for the specified controller and controls whether the Apply button is visible.
    /// </summary>
    /// <param name="controller">The controller to show.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="showApplyButton"><see langword="true"/> to show the Apply button; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialog(IMVCAController controller, string title, bool showApplyButton);

    /// <summary>
    /// Shows a dialog for the specified arguments and controls whether the Apply button is visible.
    /// </summary>
    /// <param name="args">The dialog arguments.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="showApplyButton"><see langword="true"/> to show the Apply button; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialog(object[] args, string title, bool showApplyButton);

    /// <summary>
    /// Shows a dialog for the specified arguments.
    /// </summary>
    /// <param name="args">The dialog arguments.</param>
    /// <param name="title">The dialog title.</param>
    /// <returns><see langword="true"/> if the dialog was accepted; otherwise, <see langword="false"/>.</returns>
    bool ShowDialog(object[] args, string title);

    /// <summary>
    /// Shows an open-file dialog.
    /// </summary>
    /// <param name="options">The dialog options.</param>
    /// <returns><see langword="true"/> if the user accepted the dialog; otherwise, <see langword="false"/>.</returns>
    bool ShowOpenFileDialog(OpenFileOptions options);

    /// <summary>
    /// Shows a save-file dialog.
    /// </summary>
    /// <param name="options">The dialog options.</param>
    /// <returns><see langword="true"/> if the user accepted the dialog; otherwise, <see langword="false"/>.</returns>
    bool ShowSaveFileDialog(SaveFileOptions options);

    /// <summary>
    /// Shows a folder-selection dialog.
    /// </summary>
    /// <param name="options">The dialog options.</param>
    /// <returns><see langword="true"/> if the user accepted the dialog; otherwise, <see langword="false"/>.</returns>
    bool ShowFolderDialog(FolderChoiceOptions options);

    /// <summary>
    /// Shows a yes/no message box.
    /// </summary>
    /// <param name="txt">The question text.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="defaultanswer"><see langword="true"/> to default to Yes; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the user answered Yes; otherwise, <see langword="false"/>.</returns>
    bool YesNoMessageBox(string txt, string caption, bool defaultanswer);

    /// <summary>
    /// Shows a message box with a question to be answered either by YES, NO, or CANCEL.
    /// </summary>
    /// <param name="text">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultAnswer">If true, the default answer is "yes", if false the default answer is "no", if null the default answer is "Cancel".</param>
    /// <returns>True if the user answered with Yes, false if the user answered No, null if the user pressed Cancel.</returns>
    bool? YesNoCancelMessageBox(string text, string caption, bool? defaultAnswer);

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    void ErrorMessageBox(string errortxt);

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    /// <param name="title">The title of the message box.</param>
    void ErrorMessageBox(string errortxt, string title);

    /// <summary>
    /// Shows an information message box.
    /// </summary>
    /// <param name="errortxt">The information text.</param>
    void InfoMessageBox(string errortxt);

    /// <summary>
    /// Shows an information message box with the specified title.
    /// </summary>
    /// <param name="errortxt">The information text.</param>
    /// <param name="title">The title of the message box.</param>
    void InfoMessageBox(string errortxt, string title);

    #region Clipboard

    /// <summary>
    /// Gets a new clipboard data object. You can use this to put data on the clipboard.
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
    /// Invalidates command requery suggestions.
    /// </summary>
    void InvalidateRequerySuggested();

    /// <summary>
    /// Gets a command that executes an action and evaluates the <paramref name="canExecute"/> condition every time when
    /// something in the GUI has changed.
    /// </summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The canExecute function that evaluates if the execute action can be executed under the current conditions. May be null (in this case, it is considered to return true).</param>
    /// <returns>A command that can be used, for example for binding to the GUI.</returns>
    ICommand NewRelayCommand(Action execute, Func<bool>? canExecute = null);

    /// <summary>
    /// Gets a command that executes an action and evaluates the <paramref name="canExecute"/> condition every time when
    /// something in the GUI has changed.
    /// </summary>
    /// <param name="execute">The execute action.</param>
    /// <param name="canExecute">The canExecute function that evaluates if the execute action can be executed under the current conditions. May be null (in this case, it is considered to return true).</param>
    /// <returns>A command that can be used, for example for binding to the GUI.</returns>
    ICommand NewRelayCommand(Action<object> execute, Predicate<object>? canExecute = null);

    /// <summary>
    /// Registers a handler that will be called back if something in the Gui has changed so that
    /// a requery of `CanExecute()` functions might be necessary. This handler will be bound weakly to the event.
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
