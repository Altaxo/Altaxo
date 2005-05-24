using System;
using System.Reflection;
using System.Collections;
using Altaxo.Main.GUI;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Creates the appropriate GUI object for a given document type.
  /// </summary>
  public class GUIFactoryService
  {
    /// <summary>
    /// Gets an <see>IMVCController for a given document type.</see>
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If not found, null is returned.</returns>
    public IMVCController GetController(object[] args, System.Type expectedControllerType)
    {
      return (IMVCController)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControllerForObjectAttribute),typeof(IMVCController),args);
    }

    /// <summary>
    /// Gets an <see>IMVCController for a given document type, and finding the right GUI user control for it.</see>
    /// </summary>
    /// <param name="args">The argument list. The first element args[0] is the document for which the controller is searched. The following elements are
    /// optional, and are usually the parents of this document.</param>
    /// <param name="expectedControllerType">Type of controller that you expect to return.</param>
    /// <returns>The controller for that document when found. The controller is already initialized with the document. If no controller is found for the document, or if no GUI control is found for the controller, the return value is null.</returns>
    public IMVCController GetControllerAndControl(object[] args, System.Type expectedControllerType)
    {

      if(!ReflectionService.IsSubClassOfOrImplements(expectedControllerType,typeof(IMVCController)))
        throw new ArgumentException("Expected controller type has to be IMVCController or a subclass or derived class of this");

      IMVCController controller = (IMVCController)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControllerForObjectAttribute),expectedControllerType,args);
      if(controller==null)
        return null;

      

      System.Windows.Forms.UserControl control = (System.Windows.Forms.UserControl)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControlForControllerAttribute),typeof(System.Windows.Forms.UserControl),new object[]{controller});

      if(control==null)
        return null;

      controller.ViewObject = control;

      return controller;
    }


    /// <summary>
    /// Searchs for a appropriate control for a given controller and attaches the control to the controller.
    /// </summary>
    /// <param name="controller">The controller a control is searched for.</param>
    public void GetControl(IMVCController controller)
    {
      System.Windows.Forms.UserControl control = (System.Windows.Forms.UserControl)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControlForControllerAttribute),typeof(System.Windows.Forms.UserControl),new object[]{controller});

      if(control==null)
        return;

      controller.ViewObject = control;
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
    /// <item>A controller which implements <see>Altaxo.Main.GUI.IMVCAController has to exist.</see></item>
    /// <item>A <see>Altaxo.Main.GUI.UserControllerForObjectAttribute</see> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see>Altaxo.Main.GUI.UserControlForControllerAttribute</see> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    public bool ShowDialog(object[] args, string title)
    {
      Main.GUI.IMVCAController controller = (Main.GUI.IMVCAController)Current.GUIFactoryService.GetControllerAndControl(args,typeof(Main.GUI.IMVCAController));
      
      if(null==controller)
        return false;

      Main.GUI.DialogShellController dlgctrl = new Altaxo.Main.GUI.DialogShellController(new Main.GUI.DialogShellView((System.Windows.Forms.UserControl)controller.ViewObject),controller,title,false);
      
      if(dlgctrl.ShowDialog(Current.MainWindow))
      {
        args[0] = controller.ModelObject;
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="arg">The object to configure.
    /// If the return value is true, arg contains the configured object. </param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    /// <remarks>The presumtions to get this function working are:
    /// <list>
    /// <item>A controller which implements <see>Altaxo.Main.GUI.IMVCAController has to exist.</see></item>
    /// <item>A <see>Altaxo.Main.GUI.UserControllerForObjectAttribute</see> has to be assigned to that controller, and the argument has to be the type of the object you want to configure.</item>
    /// <item>A GUI control (Windows Forms: UserControl) must exist, to which an <see>Altaxo.Main.GUI.UserControlForControllerAttribute</see> is assigned to, and the argument of that attribute has to be the type of the controller.</item>
    /// </list>
    /// </remarks>
    public bool ShowDialog(ref object arg, string title)
    {
      object[] args = new object[1];
      args[0] = arg;
      bool result = Current.GUIFactoryService.ShowDialog(args,title);
      arg = args[0];
      return result;
    }


    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    public void ErrorMessageBox(string errortxt)
    {
      System.Windows.Forms.MessageBox.Show(Current.MainWindow,errortxt,"Error(s)!", System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
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
      string result=null;
      
      try
      {
        result = Current.ResourceService.GetString("ClassNames."+definedtype.FullName);
      }
      catch(Exception)
      {
      }
      if(result!=null)
        return result;

      Attribute[] attributes = Attribute.GetCustomAttributes(definedtype,typeof(System.ComponentModel.DescriptionAttribute));
      if(attributes.Length>0)
        return ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;

      return string.Format("{0} ({1})",definedtype.Name,definedtype.Namespace);
    }

  }
}
