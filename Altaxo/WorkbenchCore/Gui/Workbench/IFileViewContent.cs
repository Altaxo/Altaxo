using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  public interface IFileViewContent : IViewContent
  {
    FileName PrimaryFileName { get; }

    OpenedFile PrimaryFile { get; }

    void Load(OpenedFile openedFile, Stream sourceStream);

    void Save(OpenedFile openedFile, Stream stream);

    bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent currentView);

    bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent newView);

    void SwitchFromThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent newView);

    void SwitchToThisWithoutSaveLoad(OpenedFile openedFile, IFileViewContent currentView);
  }
}
