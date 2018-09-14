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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Creates the appropriate GUI object for a given document type.
  /// </summary>
  public abstract class GUIFactoryService : IGuiFactoryService
  {
    /// <summary>
    /// Gets the window handle of the main window;
    /// </summary>
    public abstract IntPtr MainWindowHandle { get; }

    /// <summary>
    /// Gets the main window as object. Depending on the Gui technology that is used, this is either a Wpf window, or something else.
    /// </summary>
    /// <value>
    /// The main window.
    /// </value>
    public abstract object MainWindowObject { get; }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If not found, null is returned.</returns>
    public IMVCController GetController(object[] args, System.Type expectedControllerType)
    {
      return GetController(args, null, expectedControllerType, UseDocument.Copy);
    }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <param name="copyDocument">Determines whether to work directly with the document or use a copy.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If not found, null is returned.</returns>
    public IMVCController GetController(object[] args, System.Type expectedControllerType, UseDocument copyDocument)
    {
      return GetController(args, null, expectedControllerType, copyDocument);
    }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type.
    /// </summary>
    /// <param name="creationArgs">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="overrideArg0Type">If this parameter is not null, this given type is used instead of determining the type of the <c>arg[0]</c> argument.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <param name="copyDocument">Determines wether to use the document directly or use a clone of the document.</param>
    /// <returns>The controller for that document when found.</returns>
    public IMVCController GetController(object[] creationArgs, System.Type overrideArg0Type, System.Type expectedControllerType, UseDocument copyDocument)
    {
      if (!ReflectionService.IsSubClassOfOrImplements(expectedControllerType, typeof(IMVCController)))
        throw new ArgumentException("Expected controller type has to be IMVCController or a subclass or derived class of this");

      object result = null;

      // 1st search for all classes that wear the UserControllerForObject attribute
      ReflectionService.IAttributeForClassList list = ReflectionService.GetAttributeInstancesAndClassTypesForClass(typeof(UserControllerForObjectAttribute), creationArgs[0], overrideArg0Type);

      foreach (Type definedType in list.Types)
      {
        if (typeof(IMVCANController).IsAssignableFrom(definedType) && expectedControllerType.IsAssignableFrom(definedType))
        {
          var mvcan = (IMVCANController)Activator.CreateInstance(definedType);
          mvcan.UseDocumentCopy = copyDocument;
          if (mvcan.InitializeDocument(creationArgs))
            result = mvcan;
        }
        else
        {
          result = ReflectionService.CreateInstanceFromList(list, new Type[] { expectedControllerType }, creationArgs);
        }

        if (expectedControllerType.IsAssignableFrom(result?.GetType()))
          break;
      }

      return (IMVCController)result;
    }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type, and finding the right GUI user control for it.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If no controller is found for the document, or if no GUI control is found for the controller, the return value is null.</returns>
    public IMVCController GetControllerAndControl(object[] args, System.Type expectedControllerType)
    {
      return GetControllerAndControl(args, null, expectedControllerType, UseDocument.Copy);
    }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type, and finding the right GUI user control for it.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <param name="copyDocument">Determines whether to use the document directly or a cloned copy of the document.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If no controller is found for the document, or if no GUI control is found for the controller, the return value is null.</returns>
    public IMVCController GetControllerAndControl(object[] args, System.Type expectedControllerType, UseDocument copyDocument)
    {
      return GetControllerAndControl(args, null, expectedControllerType, copyDocument);
    }

    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type, and finding the right GUI user control for it.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="overrideArg0Type">If this parameter is not null, this given type is used instead of determining the type of the <c>arg[0]</c> argument.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <param name="copyDocument">Determines whether to use the document directly or a cloned copy.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If no controller is found for the document, or if no GUI control is found for the controller, the return value is null.</returns>
    public IMVCController GetControllerAndControl(object[] args, System.Type overrideArg0Type, System.Type expectedControllerType, UseDocument copyDocument)
    {
      if (!typeof(IMVCController).IsAssignableFrom(expectedControllerType))
        throw new ArgumentException("Expected controller type has to be IMVCController or a subclass or derived class of this");

      IMVCController controller = GetController(args, overrideArg0Type, expectedControllerType, copyDocument);
      if (controller == null)
        return null;

      FindAndAttachControlTo(controller);
      return controller;
    }

    /// <summary>
    /// Searchs for a appropriate control for a given controller with restriction to a given type.
    /// The control is not (!) attached to the controller. You have to do this manually.
    /// </summary>
    /// <param name="controller">The controller a control is searched for.</param>
    /// <param name="expectedType">The expected type of the control.</param>
    /// <returns>The control with the type provided as expectedType argument, or null if no such controller was found.</returns>
    public object FindAndAttachControlTo(IMVCController controller, System.Type expectedType)
    {
      object result = null;

      foreach (var guiControlType in RegisteredGuiTechnologies)
      {
        result = ReflectionService.GetClassForClassInstanceByAttribute(
          typeof(UserControlForControllerAttribute),
          new Type[] { guiControlType, expectedType },
          new object[] { controller });
        if (null != result)
          break;
      }
      return result;
    }

    /// <summary>
    /// Searchs for a appropriate control for a given controller and attaches the control to the controller.
    /// </summary>
    /// <param name="controller">The controller a control is searched for.</param>
    public void FindAndAttachControlTo(IMVCController controller)
    {
      foreach (var guiType in RegisteredGuiTechnologies)
      {
        InternalFindAndAttachControlUsingGuiType(controller, guiType);
        if (controller.ViewObject != null)
          break;
      }
    }

    /// <summary>
    /// Try to attach a control to the controller. To determine the type of gui, the viewTemplate is analysed.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="viewTemplate"></param>
    public void FindAndAttachControlUsingGuiTemplate(IMVCController controller, object viewTemplate)
    {
      foreach (var guiType in RegisteredGuiTechnologies)
      {
        if (ReflectionService.IsSubClassOfOrImplements(viewTemplate.GetType(), guiType))
        {
          InternalFindAndAttachControlUsingGuiType(controller, guiType);
          if (controller.ViewObject != null)
            return;
        }
      }

      if (controller.ViewObject == null)
        FindAndAttachControlTo(controller);
    }

    /// <summary>
    /// Searchs for a appropriate control for a given controller and attaches the control to the controller.
    /// </summary>
    /// <param name="controller">The controller a control is searched for.</param>
    /// <param name="guiControlType">Base type of the underlying Gui system.</param>
    private void InternalFindAndAttachControlUsingGuiType(IMVCController controller, System.Type guiControlType)
    {
      // if the controller has
      System.Type ct = controller.GetType();
      object[] viewattributes = ct.GetCustomAttributes(typeof(ExpectedTypeOfViewAttribute), false);
      if (null == viewattributes || viewattributes.Length == 0)
        viewattributes = ct.GetCustomAttributes(typeof(ExpectedTypeOfViewAttribute), true);

      bool isInvokeRequired = Current.Dispatcher.InvokeRequired;

      if (viewattributes != null && viewattributes.Length > 0)
      {
        if (viewattributes.Length > 1)
        {
          Array.Sort(viewattributes);
        }

        foreach (ExpectedTypeOfViewAttribute attr in viewattributes)
        {
          Type[] controltypes = ReflectionService.GetNonAbstractSubclassesOf(new System.Type[] { guiControlType, attr.TargetType });
          foreach (Type controltype in controltypes)
          {
            // test if the control has a special preference for a controller...
            object[] controlForAttributes = controltype.GetCustomAttributes(typeof(UserControlForControllerAttribute), false);
            if (controlForAttributes.Length > 0)
            {
              bool containsControllerType = false;
              foreach (UserControlForControllerAttribute controlForAttr in controlForAttributes)
              {
                if (ReflectionService.IsSubClassOfOrImplements(ct, controlForAttr.TargetType))
                {
                  containsControllerType = true;
                  break;
                }
              }
              if (!containsControllerType)
                continue; // then this view is not intended for our controller
            }

            // all seems ok, so we can try to create the control

            if (isInvokeRequired)
              Current.Dispatcher.InvokeIfRequired(CreateAndAttachControlOfType, controller, controltype);
            else
              CreateAndAttachControlOfType(controller, controltype);
            return;
          }
        }
      }
      else // Controller has no ExpectedTypeOfView attribute
      {
        if (isInvokeRequired)
          Current.Dispatcher.InvokeIfRequired(CreateControlForControllerWithNoExpectedTypeOfViewAttribute, controller, guiControlType);
        else
          CreateControlForControllerWithNoExpectedTypeOfViewAttribute(controller, guiControlType);
      }
    }

    private static void CreateControlForControllerWithNoExpectedTypeOfViewAttribute(IMVCController controller, System.Type guiControlType)
    {
      object control =
        ReflectionService.GetClassForClassInstanceByAttribute(
        typeof(UserControlForControllerAttribute),
        new System.Type[] { guiControlType },
        new object[] { controller });
      if (null != control)
        controller.ViewObject = control;
    }

    private static void CreateAndAttachControlOfType(IMVCController controller, System.Type controltype)
    {
      object controlinstance = System.Activator.CreateInstance(controltype);
      if (null == controlinstance)
        throw new ApplicationException(string.Format("Searching a control for controller of type {0}. Find control type {1}, but it was not possible to create a instance of this type.", controller.GetType(), controltype));
      controller.ViewObject = controlinstance;
    }

    /// <summary>
    /// Shows a configuration dialog for an object (without "Apply" button).
    /// </summary>
    /// <param name="controller">The controller to show in the dialog</param>
    /// <param name="title">The title of the dialog to show.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    public bool ShowDialog(IMVCAController controller, string title)
    {
      return ShowDialog(controller, title, false);
    }

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="controller">The controller to show in the dialog</param>
    /// <param name="title">The title of the dialog to show.</param>
    /// <param name="showApplyButton">If true, the "Apply" button is visible on the dialog.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    public abstract bool ShowDialog(IMVCAController controller, string title, bool showApplyButton);

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="args">Hierarchy of objects. Args[0] contain the object for which the configuration dialog is searched.
    /// args[1].. can contain the parents of this object (in most cases this is not necessary.
    /// If the return value is true, args[0] contains the configured object. </param>
    /// <param name="title">The title of the dialog to show.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The presumtions to get this function working are:
    /// <list>
    /// <item>A controller which implements <see cref="IMVCAController" /> has to exist.</item>
    /// <item>A <see cref="UserControllerForObjectAttribute" /> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see cref="UserControlForControllerAttribute" /> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    public bool ShowDialog(object[] args, string title)
    {
      return ShowDialog(args, title, false);
    }

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="args">Hierarchy of objects. Args[0] contain the object for which the configuration dialog is searched.
    /// args[1].. can contain the parents of this object (in most cases this is not necessary.
    /// If the return value is true, args[0] contains the configured object. </param>
    /// <param name="title">The title of the dialog to show.</param>
    /// <param name="showApplyButton">If true, the apply button is shown also.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The presumtions to get this function working are:
    /// <list>
    /// <item>A controller which implements <see cref="IMVCAController" /> has to exist.</item>
    /// <item>A <see cref="UserControllerForObjectAttribute" /> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see cref="UserControlForControllerAttribute" /> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    public bool ShowDialog(object[] args, string title, bool showApplyButton)
    {
      var controller = (IMVCAController)GetControllerAndControl(args, typeof(IMVCAController));
      try
      {
        if (null == controller)
        {
          if (args[0] is System.Enum)
          {
            var arge = (System.Enum)args[0];
            return ShowDialog(ref arge, title);
          }

          // we can try to use a property grid
          controller = new PropertyController(args[0]);
          FindAndAttachControlTo(controller);
        }

        if (ShowDialog(controller, title, showApplyButton))
        {
          args[0] = controller.ModelObject;
          return true;
        }
        else
        {
          return false;
        }
      }
      finally
      {
        var c = System.Threading.Interlocked.Exchange(ref controller, null);
        if (null != c)
          c.Dispose();
      }
    }

    #region Commands

    /// <inheritdoc/>
    public abstract ICommand NewRelayCommand(Action execute, Func<bool> canExecute = null);

    /// <inheritdoc/>
    public abstract ICommand NewRelayCommand(Action<object> execute, Predicate<object> canExecute = null);

    /// <inheritdoc/>
    public abstract void RegisterRequerySuggestedHandler(EventHandler handler);

    /// <inheritdoc/>
    public abstract void UnregisterRequerySuggestedHandler(EventHandler handler);

    #endregion Commands

    public bool ShowDialog(ref System.Enum arg, string title)
    {
      System.Type type = arg.GetType();
      System.Array arr = System.Enum.GetValues(type);
      int i;
      for (i = 0; i < arr.Length; ++i)
        if (arg.ToString() == arr.GetValue(i).ToString())
          break;
      if (i == arr.Length)
        throw new ArgumentException("The enumeration is not of integer type", "arg");

      object obj = new SingleChoiceObject(System.Enum.GetNames(type), i);

      if (ShowDialog(ref obj, title))
      {
        arg = (System.Enum)arr.GetValue((obj as SingleChoiceObject).Selection);
        return true;
      }
      else
        return false;
    }

    public bool ShowDialogForEnumFlag(ref System.Enum arg, string title)
    {
      System.Type type = arg.GetType();
      System.Array arr = System.Enum.GetValues(type);

      var ctrl = new Altaxo.Gui.Common.EnumFlagController();
      ctrl.InitializeDocument(new object[] { arg });

      if (ShowDialog(ctrl, title))
      {
        arg = (System.Enum)ctrl.ModelObject;
        return true;
      }
      else
        return false;
    }

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
    public bool ShowDialog(ref object arg, string title)
    {
      return ShowDialog(ref arg, title, false);
    }

    /// <summary>
    /// Shows a configuration dialog for an object.
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
    public bool ShowDialog(ref object arg, string title, bool showApplyButton)
    {
      if (arg is System.Enum)
      {
        var arge = (System.Enum)arg;
        return ShowDialog(ref arge, title);
      }

      object[] args = new object[1];
      args[0] = arg;
      bool result = ShowDialog(args, title, showApplyButton);
      if (result)
        arg = args[0];
      return result;
    }

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
    public bool ShowDialog<T>(ref T arg, string title, bool showApplyButton)
    {
      object[] args = new object[1];
      args[0] = arg;
      bool result = ShowDialog(args, title, showApplyButton);
      if (result)
        arg = (T)args[0];
      return result;
    }

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    /// <param name="title">The title of the message box.</param>
    public abstract void ErrorMessageBox(string errortxt, string title);

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    public void ErrorMessageBox(string errortxt)
    {
      ErrorMessageBox(errortxt, null);
    }

    /// <summary>
    /// Shows a message box with an informational text.
    /// </summary>
    /// <param name="infotxt">The info text.</param>
    /// <param name="title">The title of the message box.</param>
    public abstract void InfoMessageBox(string infotxt, string title);

    /// <summary>
    /// Shows a message box with an informational text.
    /// </summary>
    /// <param name="infotxt">The info text.</param>
    public void InfoMessageBox(string infotxt)
    {
      InfoMessageBox(infotxt, null);
    }

    /// <summary>
    /// Shows a message box with a question to be answered either yes or no.
    /// </summary>
    /// <param name="txt">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultanswer">If true, the default answer is "yes", otherwise "no".</param>
    /// <returns>True if the user answered with Yes, otherwise false.</returns>
    public abstract bool YesNoMessageBox(string txt, string caption, bool defaultanswer);

    /// <summary>
    /// Shows a message box with a questtion to be answered either by YES, NO, or CANCEL.
    /// </summary>
    /// <param name="text">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultAnswer">If true, the default answer is "yes", if false the default answer is "no", if null the default answer is "Cancel".</param>
    /// <returns>True if the user answered with Yes, false if the user answered No, null if the user pressed Cancel.</returns>
    public abstract bool? YesNoCancelMessageBox(string text, string caption, bool? defaultAnswer);

    /// <summary>
    /// Gets a user friendly class name. See remarks for a detailed description how it is been obtained.
    /// </summary>
    /// <param name="definedtype">The type of the class for which to obtain the user friendly class name.</param>
    /// <returns>The user friendly class name. See remarks.</returns>
    /// <remarks>
    /// The strategy for obtaining a user friendly class name is as follows:
    /// <list type="">
    /// <item>If there is a resource string "ClassNames.type" (type is replaced by the type of the class), then the resource string is returned.</item>
    /// <item>If there is a description attribute for that class, the description string is returned.</item>
    /// <item>The name of the class (without namespace), followed by the namespace name in paranthesis is returned.</item>
    /// </list>
    /// </remarks>
    public string GetUserFriendlyClassName(System.Type definedtype)
    {
      string result = null;

      try
      {
        result = Current.ResourceService.GetString("ClassNames." + definedtype.FullName);
      }
      catch (Exception)
      {
      }
      if (result != null)
        return result;

      Attribute[] attributes = Attribute.GetCustomAttributes(definedtype, typeof(System.ComponentModel.DescriptionAttribute));
      if (attributes.Length > 0)
        return ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;

      return string.Format("{0} ({1})", definedtype.Name, definedtype.Namespace);
    }

    /// <summary>
    /// This retrieves user friendly class name for an array of types.
    /// </summary>
    /// <param name="types">Array of types for whose to return an user friendly class name.</param>
    /// <param name="withStartingNone">If true, the first item will be a none (the returned string array then has one element more than the provided array).</param>
    /// <returns>Array of strings with the user friendly class names.</returns>
    public string[] GetUserFriendlyClassName(System.Type[] types, bool withStartingNone)
    {
      string[] result = new string[types.Length + (withStartingNone ? 1 : 0)];

      int offset = 0;
      if (withStartingNone)
      {
        offset = 1;
        result[0] = "None";
      }

      for (int i = 0; i < types.Length; ++i)
        result[i + offset] = GetUserFriendlyClassName(types[i]);

      return result;
    }

    /// <summary>
    /// Retrieves the description of the enumeration value <b>value</b>.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <returns>The description of this value. If no description is available, the ToString() method is used
    /// to return the name of the value.</returns>
    public string GetUserFriendlyName(Enum value)
    {
      string result = null;
      try
      {
        Type t = value.GetType();
        result = Current.ResourceService.GetString("ClassNames." + t.FullName + "." + Enum.GetName(t, value));
      }
      catch (Exception)
      {
      }
      if (null != result)
        return result;

      FieldInfo fi = value.GetType().GetField(value.ToString());
      var attributes =
        (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
      return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    /// <summary>
    /// Retrieves the description of the enumeration value <b>value</b>.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <returns>The description of this value. If no description is available, the ToString() method is used
    /// to return the name of the value.</returns>
    public static string GetDescription(Enum value)
    {
      FieldInfo fi = value.GetType().GetField(value.ToString());
      var attributes =
        (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
      return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    /// <summary>
    /// Retrieves information about a screen area.
    /// </summary>
    /// <param name="virtual_x">The virtual screen x coordinate of the point on the virtual screen.</param>
    /// <param name="virtual_y">The virtual screen x coordinate of the point on the virtual screen.</param>
    /// <returns>True if the function has successfully retrieved information, false otherwise.</returns>
    public abstract RectangleD2D GetScreenInformation(double virtual_x, double virtual_y);

    /// <summary>Gets the screen resolution that is set in windows in dots per inch.</summary>
    public abstract PointD2D ScreenResolutionDpi { get; }

    public abstract bool ShowOpenFileDialog(OpenFileOptions options);

    public abstract bool ShowSaveFileDialog(SaveFileOptions options);

    /// <summary>
    /// Get a new clipboard data object. You can use this to put data on the clipboard.
    /// </summary>
    /// <returns>A newly created data object.</returns>
    public abstract IClipboardSetDataObject GetNewClipboardDataObject();

    /// <summary>
    /// Opens the clipboard data object. You can use this to see which data are on the clipboard.
    /// </summary>
    /// <returns>A data object to see which data are on the clipboard.</returns>
    public abstract IClipboardGetDataObject OpenClipboardDataObject();

    /// <summary>
    /// Sets the data stored in the dataObject on the clipboard for temporary usage.
    /// </summary>
    /// <param name="dataObject">Object used to store the data.</param>
    public virtual void SetClipboardDataObject(IClipboardSetDataObject dataObject)
    {
      SetClipboardDataObject(dataObject, false);
    }

    /// <summary>
    /// Sets the data stored in the dataObject on the clipboard for temporary or permanent usage.
    /// </summary>
    /// <param name="dataObject">Object used to store the data.</param>
    /// <param name="value">If true, the data remains on the clipboard when the application is closed. If false, the data will be removed from the clipboard when the application is closed.</param>
    public abstract void SetClipboardDataObject(IClipboardSetDataObject dataObject, bool value);

    #region static Windows Form methods

    /// <summary>
    /// Dictionary of context menu providers. Key is the type of the gui root element (Winforms: Control, Wpf: UIElement).
    /// Value is an Action with the following parameters: object parent (Gui parent element), object owner (Owner of the addin element), string addInPath, double x, double y)
    /// </summary>
    public Dictionary<System.Type, Action<object, object, string, double, double>> RegistedContextMenuProviders = new Dictionary<Type, Action<object, object, string, double, double>>();

    public List<System.Type> RegisteredGuiTechnologies = new List<Type>();

    /// <summary>
    /// Creates and shows a context menu.
    /// </summary>
    /// <param name="parent">The parent gui element.</param>
    /// <param name="owner">The object that will be owner of this context menu.</param>
    /// <param name="addInTreePath">Add in tree path used to build the context menu.</param>
    /// <param name="x">The x coordinate of the location where to show the context menu.</param>
    /// <param name="y">The y coordinate of the location where to show the context menu.</param>
    /// <returns>The context menu. Returns Null if there is no registered context menu provider</returns>
    public abstract void ShowContextMenu(object parent, object owner, string addInTreePath, double x, double y);

    public virtual bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.ThreadStart threadstart, IExternalDrivenBackgroundMonitor monitor)
    {
      var t = new System.Threading.Thread(threadstart);
      t.Start();
      return ShowBackgroundCancelDialog(millisecondsDelay, t, monitor);
    }

    public abstract bool ShowBackgroundCancelDialog(int millisecondsDelay, Thread thread, IExternalDrivenBackgroundMonitor monitor);

    public struct ScreenInformation
    {
      public double WorkAreaX;
      public double WorkAreaY;
      public double WorkAreaWidth;
      public double WorkAreaHeight;
    }

    #endregion static Windows Form methods
  }
}
