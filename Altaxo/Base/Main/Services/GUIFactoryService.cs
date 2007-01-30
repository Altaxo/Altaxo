#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Reflection;
using System.Collections;
using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Creates the appropriate GUI object for a given document type.
  /// </summary>
  public class GUIFactoryService
  {
    private System.Windows.Forms.PageSetupDialog _pageSetupDialog;
    private System.Windows.Forms.PrintDialog _printDialog;



    /// <summary>
    /// Gets an <see cref="IMVCController" />  for a given document type.
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If not found, null is returned.</returns>
    public IMVCController GetController(object[] args, System.Type expectedControllerType)
    {
      return GetController(args, null, expectedControllerType,UseDocument.Copy);
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
        if (ReflectionService.IsSubClassOfOrImplements(definedType, typeof(IMVCANController)))
        {
          IMVCANController mvcan = (IMVCANController)Activator.CreateInstance(definedType);
          mvcan.UseDocumentCopy = copyDocument;
          if(mvcan.InitializeDocument(creationArgs))
            result = mvcan;
        }
        else
        {
          result = ReflectionService.CreateInstanceFromList(list, new Type[] { expectedControllerType }, creationArgs);
        }

        if (result is IMVCController)
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

      if (!ReflectionService.IsSubClassOfOrImplements(expectedControllerType, typeof(IMVCController)))
        throw new ArgumentException("Expected controller type has to be IMVCController or a subclass or derived class of this");

      IMVCController controller = GetController(args, overrideArg0Type, typeof(IMVCController), copyDocument);
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
    /// <remarks>This function is used externally, so we add the restriction here that the expected type
    /// has to be a System.Windows.Forms.Control.</remarks>
    public object FindAndAttachControlTo(IMVCController controller, System.Type expectedType)
    {
      return ReflectionService.GetClassForClassInstanceByAttribute(
        typeof(UserControlForControllerAttribute),
        new Type[] { typeof(System.Windows.Forms.Control), expectedType },
        new object[] { controller });
    }


    /// <summary>
    /// Searchs for a appropriate control for a given controller and attaches the control to the controller.
    /// </summary>
    /// <param name="controller">The controller a control is searched for.</param>
    public void FindAndAttachControlTo(IMVCController controller)
    {
      // if the controller has 
      System.Type ct = controller.GetType();
      object[] viewattributes = ct.GetCustomAttributes(typeof(ExpectedTypeOfViewAttribute), false);

      if (viewattributes != null && viewattributes.Length > 0)
      {
        if (viewattributes.Length > 1)
        {
          Array.Sort(viewattributes);
        }

        foreach (ExpectedTypeOfViewAttribute attr in viewattributes)
        {
          Type[] controltypes = ReflectionService.GetNonAbstractSubclassesOf(new System.Type[] { typeof(System.Windows.Forms.Control), attr.TargetType });
          foreach (Type controltype in controltypes)
          {
            // test if the control has a special preference for a controller...
            object[] controlForAttributes = controltype.GetCustomAttributes(typeof(UserControlForControllerAttribute), false);
            if (controlForAttributes.Length > 0)
            {
              bool containsControllerType = false;
              foreach (UserControlForControllerAttribute controlForAttr in controlForAttributes)
              {
                if (ReflectionService.IsSubClassOfOrImplements(ct,controlForAttr.TargetType))
                {
                  containsControllerType = true;
                  break;
                }
              }
              if (!containsControllerType)
                continue; // then this view is not intended for our controller
            }

            // all seems ok, so we can try to create the control
            object controlinstance = System.Activator.CreateInstance(controltype);
            if (null == controlinstance)
              throw new ApplicationException(string.Format("Searching a control for controller of type {0}. Find control type {1}, but it was not possible to create a instance of this type.", ct, controltype));

            controller.ViewObject = controlinstance;

            if (controller.ViewObject == null)
              throw new ApplicationException(string.Format("Searching a control for controller of type {0}. Find control type {1}, but the controller did not accept this control.", ct, controltype));

            return;
          }
        }
      }
      else // Controller has no ExpectedTypeOfView attribute
      {
        System.Windows.Forms.UserControl control =
          (System.Windows.Forms.UserControl)ReflectionService.GetClassForClassInstanceByAttribute(
          typeof(UserControlForControllerAttribute),
          new System.Type[] { typeof(System.Windows.Forms.UserControl) },
          new object[] { controller });

        if (control == null)
          return;

        controller.ViewObject = control;
      }
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
    public bool ShowDialog(IMVCAController controller, string title, bool showApplyButton)
    {

      if (controller.ViewObject == null)
      {
        FindAndAttachControlTo(controller);
      }

      if (controller.ViewObject == null)
        throw new ArgumentException("Can't find a view object for controller of type " + controller.GetType());

      if (controller is Altaxo.Gui.Scripting.IScriptController)
      {
        System.Windows.Forms.Form dlgctrl = new Altaxo.Gui.Scripting.ScriptExecutionDialog((Altaxo.Gui.Scripting.IScriptController)controller);
        return (System.Windows.Forms.DialogResult.OK == dlgctrl.ShowDialog(Current.MainWindow));
      }
      else
      {
        DialogShellController dlgctrl = new DialogShellController(new DialogShellView((System.Windows.Forms.UserControl)controller.ViewObject), controller, title, showApplyButton);
        return (dlgctrl.ShowDialog(Current.MainWindow));
      }
    }




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
      IMVCAController controller = (IMVCAController)Current.Gui.GetControllerAndControl(args, typeof(IMVCAController));

      if (null == controller)
        return false;

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
      object[] args = new object[1];
      args[0] = arg;
      bool result = Current.Gui.ShowDialog(args, title, showApplyButton);
      arg = args[0];
      return result;
    }


    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    public void ErrorMessageBox(string errortxt)
    {
      System.Windows.Forms.MessageBox.Show(Current.MainWindow, errortxt, "Error(s)!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
    }

    /// <summary>
    /// Shows a message box with a question to be answered either yes or no.
    /// </summary>
    /// <param name="txt">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultanswer">If true, the default answer is "yes", otherwise "no".</param>
    /// <returns>True if the user answered with Yes, otherwise false.</returns>
    public bool YesNoMessageBox(string txt, string caption, bool defaultanswer)
    {
      return System.Windows.Forms.DialogResult.Yes == System.Windows.Forms.MessageBox.Show(Current.MainWindow, txt, caption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, defaultanswer ? System.Windows.Forms.MessageBoxDefaultButton.Button1 : System.Windows.Forms.MessageBoxDefaultButton.Button2);
    }


    public bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.ThreadStart threadstart, IExternalDrivenBackgroundMonitor monitor)
    {
      System.Threading.Thread t = new System.Threading.Thread(threadstart);
      t.Start();
      return ShowBackgroundCancelDialog(millisecondsDelay, t, monitor);
    }

    public bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor)
    {
      for (int i = 0; i < millisecondsDelay && thread.IsAlive; i += 10)
        System.Threading.Thread.Sleep(10);

      if (thread.IsAlive)
      {
        Altaxo.Gui.Common.BackgroundCancelDialog dlg = new Altaxo.Gui.Common.BackgroundCancelDialog(thread, monitor);

        if (thread.IsAlive)
        {
          System.Windows.Forms.DialogResult r = dlg.ShowDialog(Current.MainWindow);
          return r == System.Windows.Forms.DialogResult.OK ? false : true;
        }
      }
      return false;
    }

    /// <summary>
    /// Shows a page setup dialog.
    /// </summary>
    /// <returns>True if the user close the dialog with OK, false otherwise.</returns>
    public bool ShowPageSetupDialog()
      {
      bool result = System.Windows.Forms.DialogResult.OK == _pageSetupDialog.ShowDialog(Current.MainWindow);
      if (true==result)
      {
        Current.PrintingService.UpdateBoundsAndMargins();
      }
      return result;
    }
    /// <summary>
    /// Shows a print dialog.
    /// </summary>
    /// <returns>True if the user close the dialog with OK, false otherwise.</returns>
    public bool ShowPrintDialog()
    {
      return System.Windows.Forms.DialogResult.OK == _printDialog.ShowDialog(Current.MainWindow);
    }

    /// <summary>
    /// Sets the print document for both the page setup dialog and the print dialog.
    /// </summary>
    /// <param name="printDocument">The document to set.</param>
    public void SetPrintDocument(System.Drawing.Printing.PrintDocument printDocument)
    {
      if (_pageSetupDialog == null)
        _pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
      if (_printDialog == null)
        _printDialog = new System.Windows.Forms.PrintDialog();

      _pageSetupDialog.Document = printDocument;
      _printDialog.Document = printDocument;
    }


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
        result = Current.ResourceService.GetString("ClassNames." + t.FullName+"."+Enum.GetName(t,value));
      }
      catch (Exception)
      {
      }
      if (null != result)
        return result;

      FieldInfo fi = value.GetType().GetField(value.ToString());
      System.ComponentModel.DescriptionAttribute[] attributes =
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
      System.ComponentModel.DescriptionAttribute[] attributes =
        (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
      return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }


    #region static Windows Form methods

    public static void InitComboBox(System.Windows.Forms.ComboBox box, Altaxo.Collections.SelectableListNodeList names)
    {
      box.BeginUpdate();
      box.Items.Clear();
      foreach (Altaxo.Collections.SelectableListNode node in names)
      {
        box.Items.Add(node);
      }
      foreach (Altaxo.Collections.SelectableListNode node in names)
      {
        if (node.Selected)
        {
          box.SelectedItem = node;
          break;
        }
      }
      box.EndUpdate();
    }
    public static void SynchronizeSelectableListNodes(System.Windows.Forms.ComboBox box)
    {
      object sel = box.SelectedItem;
      foreach (Altaxo.Collections.SelectableListNode node in box.Items)
      {
        node.Selected = object.ReferenceEquals(sel, node);
      }
    }

    #endregion
  }
}
