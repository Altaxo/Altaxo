using System;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// Summary description for SingleChoiceObjectController.
  /// </summary>
  [UserControllerForObject(typeof(ISingleChoiceObject),100)]
  public class SingleChoiceObjectController : SingleChoiceController
  {
    protected ISingleChoiceObject _choiceObject;
    public SingleChoiceObjectController(ISingleChoiceObject o)
      :
      base(o.Choices,o.Selection)
    {
      _choiceObject = o;
    }

    public override object ModelObject
    {
      get
      {
        return _choiceObject;
      }
    }


    public override bool Apply()
    {
      if(!base.Apply())
        return false;

      _choiceObject.Selection = base._choice;

      return true;
    }

  }
}
