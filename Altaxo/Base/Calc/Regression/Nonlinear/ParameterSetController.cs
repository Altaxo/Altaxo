using System;
using Altaxo.Main.GUI;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  public class ParameterSetController : Altaxo.Main.GUI.MultiChildController
  {
    ParameterSet _doc;

    public ParameterSetController(ParameterSet doc)
    {
      _doc = doc;
      _doc.InitializationFinished += new EventHandler(EhInitializationFinished);

      IMVCAController[] childs = new IMVCAController[_doc.Count];
      for(int i=0;i<childs.Length;i++)
        childs[i] = (IMVCAController)Current.GUIFactoryService.GetControllerAndControl(new object[]{_doc[i]},typeof(IMVCAController));

      base.Initialize(childs);
    }

    private void EhInitializationFinished(object sender, EventArgs e)
    {
      IMVCAController[] childs = new IMVCAController[_doc.Count];
      for(int i=0;i<childs.Length;i++)
        childs[i] = (IMVCAController)Current.GUIFactoryService.GetControllerAndControl(new object[]{_doc[i]},typeof(IMVCAController));

      base.Initialize(childs);
    }
  }
}
