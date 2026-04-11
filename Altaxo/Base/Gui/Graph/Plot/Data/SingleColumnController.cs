using System.ComponentModel;
using System.Windows.Input;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Plot.Data
{
  /// <summary>
  /// Public interface for a controller that manages a single plot column.
  /// </summary>
  public interface ISingleColumnController
  {
    /// <summary>Gets the group name.</summary>
    string GroupName { get; }
    /// <summary>Gets the label text.</summary>
    string LabelText { get; }
    /// <summary>Gets the column text.</summary>
    string ColumnText { get; }
    /// <summary>Gets the column tooltip.</summary>
    string ColumnToolTip { get; }
    /// <summary>Gets the severity level.</summary>
    int SeverityLevel { get; }
    /// <summary>Gets the drop handler for the column.</summary>
    IMVVMDropHandler ColumnDropHandler { get; }
    /// <summary>Gets the command to add the column to another group.</summary>
    ICommand CmdColumnAddTo { get; }
    /// <summary>Gets the command to edit the column.</summary>
    ICommand CmdColumnEdit { get; }
    /// <summary>Gets the command to erase the column.</summary>
    ICommand CmdColumnErase { get; }

    /// <summary>Gets the transformation text.</summary>
    string TransformationText { get; }
    /// <summary>Gets the transformation tooltip.</summary>
    string TransformationToolTip { get; }
    /// <summary>Gets or sets a value indicating whether the transformation popup is open.</summary>
    bool IsTransformationPopupOpen { get; set; }

    /// <summary>Gets the drop handler for transformations.</summary>
    IMVVMDropHandler TransformationDropHandler { get; }
    /// <summary>Gets the command to edit the transformation.</summary>
    ICommand CmdTransformationEdit { get; }
    /// <summary>Gets the command to erase the transformation.</summary>
    ICommand CmdTransformationErase { get; }

    /// <summary>Gets the command to add the transformation as a single transformation.</summary>
    ICommand CmdTransformationAddAsSingle { get; }

    /// <summary>Gets the command to prepend the transformation.</summary>
    ICommand CmdTransformationAddAsPrepending { get; }

    /// <summary>Gets the command to append the transformation.</summary>
    ICommand CmdTransformationAddAsAppending { get; }

    /// <summary>Gets the command to close the transformation popup.</summary>
    ICommand CmdCloseTransformationPopup { get; }
  }

  /// <summary>
  /// Interface that the parent controller of <see cref="SingleColumnController"/> has to implement.
  /// </summary>
  public interface ISingleColumnControllerParent
  {
    /// <summary>Called when the 'Add' button is pressed.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnAddTo(SingleColumnController ctrl);
    /// <summary>Called when the edit button is pressed.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnEdit(SingleColumnController ctrl);
    /// <summary>Called when the erase button is pressed.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnErase(SingleColumnController ctrl);
    /// <summary>Called when the transformation edit button is pressed.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnTransformationEdit(SingleColumnController ctrl);
    /// <summary>Called when the transformation erase button is pressed.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnTransformationErase(SingleColumnController ctrl);
    /// <summary>Called when a transformation should be added as a single transformation.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnTransformationAddAsSingle(SingleColumnController ctrl);
    /// <summary>Called when a transformation should be prepended.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnTransformationAddAsPrepending(SingleColumnController ctrl);
    /// <summary>Called when a transformation should be appended.</summary>
    /// <param name="ctrl">The source controller.</param>
    void EhPlotColumnTransformationAddAsAppending(SingleColumnController ctrl);

    /// <summary>Called when data is dropped onto the column.</summary>
    /// <param name="ctrl">The source controller.</param>
    /// <param name="data">The dropped data.</param>
    void EhPlotColumnDrop(SingleColumnController ctrl, object data);
    /// <summary>Called when data is dropped onto the transformation area.</summary>
    /// <param name="ctrl">The source controller.</param>
    /// <param name="data">The dropped data.</param>
    void EhPlotColumnTransformationDrop(SingleColumnController ctrl, object data);
  }

  /// <summary>
  /// Controller for a single plot column.
  /// </summary>
  public class SingleColumnController : INotifyPropertyChanged, ISingleColumnController
  {
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    /// <summary>
    /// Initializes a new instance of the <see cref="SingleColumnController"/> class.
    /// </summary>
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

    private ISingleColumnControllerParent _parent;

    /// <summary>
    /// Gets or sets the parent controller.
    /// </summary>
    public ISingleColumnControllerParent Parent
    {
      get => _parent;
      set
      {
        if (!(_parent == value))
        {
          _parent = value;
          OnPropertyChanged(nameof(Parent));
        }
      }
    }


    /// <summary>
    /// Gets the command that assigns the currently selected source column to this plot column.
    /// </summary>
    public ICommand CmdColumnAddTo { get; }
    /// <summary>
    /// Gets the command that edits this plot column.
    /// </summary>
    public ICommand CmdColumnEdit { get; }
    /// <summary>
    /// Gets the command that clears this plot column.
    /// </summary>
    public ICommand CmdColumnErase { get; }

    /// <summary>
    /// Gets the command that edits the transformation assigned to this plot column.
    /// </summary>
    public ICommand CmdTransformationEdit { get; }
    /// <summary>
    /// Gets the command that removes the transformation assigned to this plot column.
    /// </summary>
    public ICommand CmdTransformationErase { get; }

    /// <summary>
    /// Gets the command that adds the dropped transformation as the only transformation.
    /// </summary>
    public ICommand CmdTransformationAddAsSingle { get; }

    /// <summary>
    /// Gets the command that prepends a dropped transformation.
    /// </summary>
    public ICommand CmdTransformationAddAsPrepending { get; }

    /// <summary>
    /// Gets the command that appends a dropped transformation.
    /// </summary>
    public ICommand CmdTransformationAddAsAppending { get; }

    /// <summary>
    /// Gets the command that closes the transformation popup.
    /// </summary>
    public ICommand CmdCloseTransformationPopup { get; }

    /// <summary>
    /// Gets or sets the plot-column tag.
    /// </summary>
    public PlotColumnTag Tag { get; set; }

    private string _groupName;

    /// <summary>
    /// Gets or sets the name of the column group.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the label text shown for the plot column.
    /// </summary>
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



    /// <summary>
    /// Gets or sets the text shown for the assigned column.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the tooltip text for the assigned column.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the severity level for the current column state.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the text shown for the assigned transformation.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the tooltip text for the assigned transformation.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the transformation popup is open.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the drop handler for column data.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the drop handler for transformation data.
    /// </summary>
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

    /// <summary>
    /// Drop handler for column data.
    /// </summary>
    public class MyColumnDropHandler : IMVVMDropHandler
    {
      SingleColumnController _parent;
      /// <summary>
      /// Initializes a new instance of the <see cref="MyColumnDropHandler"/> class.
      /// </summary>
      /// <param name="parent">The owning single-column controller.</param>
      public MyColumnDropHandler(SingleColumnController parent)
      {
        _parent = parent;
      }

      /// <inheritdoc/>
      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isCopy = false;
        isMove = false;

        _parent.EhPlotColumnDrop(data);
        isCopy = true;
        isMove = false;
      }

      /// <inheritdoc/>
      public void DropCanAcceptData(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool canCopy, out bool canMove, out bool itemIsSwallowingData)
      {
        canCopy = true;
        canMove = true;
        itemIsSwallowingData = false;
      }
    }

    #endregion

    #region Transformation Drop Handler

    /// <summary>
    /// Drop handler for transformation data.
    /// </summary>
    public class MyTransformationDropHandler : IMVVMDropHandler
    {
      SingleColumnController _parent;
      /// <summary>
      /// Initializes a new instance of the <see cref="MyTransformationDropHandler"/> class.
      /// </summary>
      /// <param name="parent">The owning single-column controller.</param>
      public MyTransformationDropHandler(SingleColumnController parent)
      {
        _parent = parent;
      }

      /// <inheritdoc/>
      public void Drop(object data, object targetItem, DragDropRelativeInsertPosition insertPosition, bool isCtrlKeyPressed, bool isShiftKeyPressed, out bool isCopy, out bool isMove)
      {
        isCopy = false;
        isMove = false;

        _parent.EhPlotColumnDrop(data);
        isCopy = true;
        isMove = false;
      }

      /// <inheritdoc/>
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
