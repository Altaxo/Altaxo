using System;
using System.Collections;
using System.Text;

namespace Altaxo.Gui.Common
{
  public class ControlViewElement : ICloneable
  {
    public string Title;
    public Main.GUI.IApplyController Controller;
    public object View;

    public ControlViewElement(ControlViewElement from)
    {
      this.Title = from.Title;
      this.Controller = from.Controller;
      this.View = from.View;
    }

    public ControlViewElement(string title, Main.GUI.IApplyController controller, object view)
    {
      this.Title = title;
      this.Controller = controller;
      this.View = view;
    }

    public ControlViewElement(string title, Main.GUI.IMVCAController controller)
    {
      this.Title = title;
      this.Controller = controller;
      this.View = controller.ViewObject;
    }

    public ControlViewElement Clone()
    {
      return new ControlViewElement(this);
    }

    #region ICloneable Members


    object ICloneable.Clone()
    {
      return new ControlViewElement(this);
    }

    #endregion
}
}
