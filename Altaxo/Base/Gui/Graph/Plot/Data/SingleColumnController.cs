using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Plot.Data
{
  public interface ISingleColumnController
  {
    string GroupName { get; }
    string LabelText { get; }
    string ColumnText { get; }
    string ColumnToolTip { get; }
    int SeverityLevel { get; }
    IMVVMDropHandler ColumnDropHandler { get; }
    ICommand CmdColumnAddTo { get; }
    ICommand CmdColumnEdit { get; }
    ICommand CmdColumnErase { get; }

    string TransformationText { get; }
    string TransformationToolTip { get; }
    bool IsTransformationPopupOpen { get; set; }

    IMVVMDropHandler TransformationDropHandler { get; }
    ICommand CmdTransformationEdit { get; }
    ICommand CmdTransformationErase { get; }

    ICommand CmdTransformationAddAsSingle { get; }

    ICommand CmdTransformationAddAsPrepending { get; }

    ICommand CmdTransformationAddAsAppending { get; }

    ICommand CmdCloseTransformationPopup { get; }
  }

  /// <summary>
  /// Interface that the parent controller of <see cref="SingleColumnController"/> has to implement.
  /// </summary>
  public interface ISingleColumnControllerParent
  {
    /// <summary>Called when the 'Add' button is pressed.</summary>
    /// <param name="ctrl"></param>
    void EhPlotColumnAddTo(SingleColumnController ctrl);
    void EhPlotColumnEdit(SingleColumnController ctrl);
    void EhPlotColumnErase(SingleColumnController ctrl);
    void EhPlotColumnTransformationEdit(SingleColumnController ctrl);
    void EhPlotColumnTransformationErase(SingleColumnController ctrl);
    void EhPlotColumnTransformationAddAsSingle(SingleColumnController ctrl);
    void EhPlotColumnTransformationAddAsPrepending(SingleColumnController ctrl);
    void EhPlotColumnTransformationAddAsAppending(SingleColumnController ctrl);

    void EhPlotColumnDrop(SingleColumnController ctrl, object data);
    void EhPlotColumnTransformationDrop(SingleColumnController ctrl, object data);
  }

  public class SingleColumnController : INotifyPropertyChanged, ISingleColumnController
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    public SingleColumnController()
    {
      CmdColumnAddTo = new RelayCommand(EhColumnAddTo);
      CmdColumnEdit = new RelayCommand(EhColumnEdit);
      CmdColumnErase = new RelayCommand(EhColumnErase);
      CmdTransformationEdit = new RelayCommand(EhTransformationEdit);
      CmdTransformationErase = new RelayCommand(EhTransformationErase);
      CmdTransformationAddAsSingle = new RelayCommand(EhTransformationAddAsSingle);
      CmdTransformationAddAsPrepending = new RelayCommand(EhTransformationAddAsPrepending);
      CmdTransformationAddAsAppending = new RelayCommand(EhTransformationAddAsAppending);
      CmdCloseTransformationPopup = new RelayCommand(EhCloseTransformationPopup);

      ColumnDropHandler = new MyColumnDropHandler(this);
      TransformationDropHandler = new MyTransformationDropHandler(this);
    }



    #region Bindings

    ISingleColumnControllerParent _parent;

    public ICommand CmdColumnAddTo { get; }
    public ICommand CmdColumnEdit { get; }
    public ICommand CmdColumnErase { get; }

    public ICommand CmdTransformationEdit { get; }
    public ICommand CmdTransformationErase { get; }

    public ICommand CmdTransformationAddAsSingle { get; }

    public ICommand CmdTransformationAddAsPrepending { get; }

    public ICommand CmdTransformationAddAsAppending { get; }

    public ICommand CmdCloseTransformationPopup { get; }

    public PlotColumnTag Tag {get; set;}

    private string _groupName;

    public string GroupName
    {
      get => _groupName;
      set
      {
        if (!(_groupName == value))
        {
          _groupName = value;
          OnPropertyChanged(nameof(GroupName));
        }
      }
    }


    private string _labelText;

    public string LabelText
    {
      get => _labelText;
      set
      {
        if (!(_labelText == value))
        {
          _labelText = value;
          OnPropertyChanged(nameof(LabelText));
        }
      }
    }

    private string _columnText;

    

    public string ColumnText
    {
      get => _columnText;
      set
      {
        if (!(_columnText == value))
        {
          _columnText = value;
          OnPropertyChanged(nameof(ColumnText));
        }
      }
    }

    private string _columnToolTip;

    public string ColumnToolTip
    {
      get => _columnToolTip;
      set
      {
        if (!(_columnToolTip == value))
        {
          _columnToolTip = value;
          OnPropertyChanged(nameof(ColumnToolTip));
        }
      }
    }

    private int _severityLevel;

    public int SeverityLevel
    {
      get => _severityLevel;
      set
      {
        if (!(_severityLevel == value))
        {
          _severityLevel = value;
          OnPropertyChanged(nameof(SeverityLevel));
        }
      }
    }

    private string _transformationText;

    public string TransformationText
    {
      get => _transformationText;
      set
      {
        if (!(_transformationText == value))
        {
          _transformationText = value;
          OnPropertyChanged(nameof(TransformationText));
        }
      }
    }

    private string _transformationToolTip;

    public string TransformationToolTip
    {
      get => _transformationToolTip;
      set
      {
        if (!(_transformationToolTip == value))
        {
          _transformationToolTip = value;
          OnPropertyChanged(nameof(TransformationToolTip));
        }
      }
    }

    private bool _isTransformationPopupOpen;

    public bool IsTransformationPopupOpen
    {
      get => _isTransformationPopupOpen;
      set
      {
        if (!(_isTransformationPopupOpen == value))
        {
          _isTransformationPopupOpen = value;
          OnPropertyChanged(nameof(IsTransformationPopupOpen));
        }
      }
    }


    private IMVVMDropHandler _columnDropHandler;

    public IMVVMDropHandler ColumnDropHandler
    {
      get => _columnDropHandler;
      set
      {
        if (!(_columnDropHandler == value))
        {
          _columnDropHandler = value;
          OnPropertyChanged(nameof(ColumnDropHandler));
        }
      }
    }

    private IMVVMDropHandler _transformationDropHandler;

    public IMVVMDropHandler TransformationDropHandler
    {
      get => _transformationDropHandler;
      set
      {
        if (!(_transformationDropHandler == value))
        {
          _transformationDropHandler = value;
          OnPropertyChanged(nameof(TransformationDropHandler));
        }
      }
    }





    #endregion

    #region Command handlers

    private void EhColumnAddTo()
    {
      _parent?.EhPlotColumnAddTo(this);
    }

    private void EhColumnEdit()
    {
      _parent?.EhPlotColumnEdit(this);
    }

    private void EhColumnErase()
    {
      _parent?.EhPlotColumnErase(this);
    }

    private void EhTransformationEdit()
    {
      _parent?.EhPlotColumnTransformationEdit(this);
    }

    private void EhTransformationErase()
    {
      _parent?.EhPlotColumnTransformationErase(this);
    }

    private void EhTransformationAddAsSingle()
    {
      IsTransformationPopupOpen = false;
      _parent?.EhPlotColumnTransformationAddAsSingle(this);
    }

    private void EhTransformationAddAsPrepending()
    {
      IsTransformationPopupOpen = false;
      _parent?.EhPlotColumnTransformationAddAsPrepending(this);
    }

    private void EhTransformationAddAsAppending()
    {
      IsTransformationPopupOpen = false;
      _parent?.EhPlotColumnTransformationAddAsAppending(this);
    }


    private void EhCloseTransformationPopup()
    {
      IsTransformationPopupOpen = false;
    }

    private void EhPlotColumnDrop(object data)
    {
      _parent?.EhPlotColumnDrop(this, data);
    }

    #endregion

    #region Column Drop Handler

    public class MyColumnDropHandler : IMVVMDropHandler
    {
      SingleColumnController _parent;
      public MyColumnDropHandler(SingleColumnController parent)
      {
        _parent = parent;
      }

      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isCopy = false;
        isMove = false;

        if(targetItem is PlotColumnTag tag)
        {
          _parent.EhPlotColumnDrop(data);
          isCopy = true;
          isMove = false;
        }
      }

      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        canCopy = true;
        canMove = true;
        itemIsSwallowingData = false;
      }
    }

    #endregion

    #region Transformation Drop Handler

    public class MyTransformationDropHandler : IMVVMDropHandler
    {
      SingleColumnController _parent;
      public MyTransformationDropHandler(SingleColumnController parent)
      {
        _parent = parent;
      }

      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isCopy = false;
        isMove = false;

        if (targetItem is PlotColumnTag tag)
        {
          _parent.EhPlotColumnDrop(data);
          isCopy = true;
          isMove = false;
        }
      }

      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        canCopy = true;
        canMove = true;
        itemIsSwallowingData = false;
      }
    }

    #endregion

  }
}
