using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  public abstract class ProjectBrowseControllerCommand : ICSharpCode.Core.AbstractCommand
  {
    protected abstract void Run(ProjectBrowseController ctrl);
    public override void Run()
    {
      Run((ProjectBrowseController)Owner);
    }
    protected ProjectBrowseController Ctrl
    {
      get
      {
        return (ProjectBrowseController)Owner;
      }
    }
  }

	public class CmdListItemShow : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.ShowSelectedListItem();
		}
	}

	public class CmdListItemHide : ProjectBrowseControllerCommand
	{
		protected override void Run(ProjectBrowseController ctrl)
		{
			ctrl.HideSelectedListItems();
		}
	}


  public class CmdListItemDelete : ProjectBrowseControllerCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.DeleteSelectedListItems();
    }
  }

  public class CmdViewOnSelectListNodeOff : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ViewOnSelectListNodeOn = false;
    }

    #region ICheckableMenuCommand Members

    public bool IsChecked
    {
      get
      {
        return !Ctrl.ViewOnSelectListNodeOn;
      }
      set
      {
      }
    }

    #endregion

    #region IMenuCommand Members

    public bool IsEnabled
    {
      get
      {
        return true;
      }
      set
      {
      }
    }

    #endregion
  }

  public class CmdViewOnSelectListNodeOn : ProjectBrowseControllerCommand, ICSharpCode.Core.ICheckableMenuCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.ViewOnSelectListNodeOn = true;
    }

    #region ICheckableMenuCommand Members

    public bool IsChecked
    {
      get
      {
        return Ctrl.ViewOnSelectListNodeOn;
      }
      set
      {
      }
    }

    #endregion

    #region IMenuCommand Members

    public bool IsEnabled
    {
      get
      {
        return true;
      }
      set
      {
      }
    }

    #endregion
  }


  public class CmdNewEmptyWorksheet : ProjectBrowseControllerCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewEmptyWorksheet();
    }
  }

  public class CmdNewStandardWorksheet : ProjectBrowseControllerCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewStandardWorksheet();
    }
  }

  public class CmdNewGraph : ProjectBrowseControllerCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.CreateNewGraph();
    }
  }


  public class CmdPlotCommonColumns : ProjectBrowseControllerCommand
  {
    protected override void Run(ProjectBrowseController ctrl)
    {
      ctrl.PlotCommonColumns();
    }
  }

}
