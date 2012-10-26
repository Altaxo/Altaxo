using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Main.Commands;

namespace Altaxo.Gui.Common.Tools
{
  
  public interface ITestAllProjectsInFolderView
  {
   string FolderPaths { get; set; }
    bool TestSavingAndReopening { get; set; }
  }

  [UserControllerForObject(typeof(TestAllProjectsInFolderOptions))]
  [ExpectedTypeOfView(typeof(ITestAllProjectsInFolderView))]
  public class TestAllProjectsInFolderController : MVCANControllerBase<TestAllProjectsInFolderOptions, ITestAllProjectsInFolderView>
  {

    protected override void Initialize(bool initData)
    {
      if (null != _view)
      { 
        _view.FolderPaths = _doc.FolderPaths;
        _view.TestSavingAndReopening = _doc.TestSavingAndReopening;
      }
    }

    public override bool Apply()
    {
      _doc.TestSavingAndReopening = _view.TestSavingAndReopening;
      _doc.FolderPaths = _view.FolderPaths;

      if (_useDocumentCopy)
        CopyHelper.Copy(ref _originalDoc, _doc);

      return !string.IsNullOrEmpty(_doc.FolderPaths);

    }
  }
}
