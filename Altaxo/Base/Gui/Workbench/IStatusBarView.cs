using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  public interface IStatusBarView
  {
    bool IsStatusBarVisible { set; }
    object CursorStatusBarPanelContent { set; }
    object SelectionStatusBarPanelContent { set; }
    object ModeStatusBarPanelContent { set; }

    void SetMessage(string message, bool highlighted, object icon);

    void HideProgress();

    void DisplayProgress(string taskName, double progress, OperationStatus status);
  }
}
