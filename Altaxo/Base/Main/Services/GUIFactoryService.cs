using System;
using System.Reflection;
using System.Collections;
using Altaxo.Main.GUI;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Summary description for GUIFactoryService.
	/// </summary>
	public class GUIFactoryService
	{
    public IMVCController GetController(object[] args, System.Type expectedControllerType)
    {
      return (IMVCController)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControllerForObjectAttribute),typeof(IMVCController),args);
    }

    public IMVCController GetControllerAndControl(object[] args, System.Type expectedControllerType)
    {
      IMVCController controller = (IMVCController)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControllerForObjectAttribute),typeof(IMVCController),args);
      if(controller==null)
        return null;

      System.Windows.Forms.UserControl control = (System.Windows.Forms.UserControl)ReflectionService.GetClassForClassInstanceByAttribute(typeof(UserControlForControllerAttribute),typeof(System.Windows.Forms.UserControl),new object[]{controller});

      if(control==null)
        return null;

      controller.ViewObject = control;

      return controller;
    }
    
	}
}
