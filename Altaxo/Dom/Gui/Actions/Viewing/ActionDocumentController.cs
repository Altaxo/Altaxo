#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable warnings
using System;
using System.Windows.Input;
using Altaxo.Gui.Workbench;
using Altaxo.Main;

namespace Altaxo.Gui.Actions.Viewing
{
  /// <summary>
  /// Defines the view used to display and edit an action document.
  /// </summary>
  public interface IActionDocumentView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls the view of an action document.
  /// </summary>
  [UserControllerForObject(typeof(ActionDocument))]
  [UserControllerForObject(typeof(ActionViewLayout))]
  [ExpectedTypeOfView(typeof(IActionDocumentView))]
  public class ActionDocumentController : AbstractViewContent, IDisposable, IMVCANController
  {
    /// <summary>
    /// The view associated with this controller.
    /// </summary>
    protected IActionDocumentView _view;

    /// <summary>
    /// The action document that this controller manages.
    /// </summary>
    protected ActionDocument _doc;

    #region Bindings


    /// <summary>
    /// Gets or sets the action controller associated with this document controller.
    /// </summary>
    public IMVCANDController ActionController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          if (field is { } oldController)
          {
            oldController.MadeDirty -= EhActionHasChanged;
            oldController.Dispose();
          }
          field = value;
          if (field is { } newController)
          {
            newController.MadeDirty += EhActionHasChanged;
          }
          OnPropertyChanged(nameof(ActionController));
        }
      }
    }

    /// <summary>
    /// Gets the command to execute the action associated with this document.
    /// </summary>
    public ICommand CmdExecute => field ??= new RelayCommand(EhCmdExecute);


    /// <summary>
    /// Gets or sets the description of the action associated with this document.
    /// </summary>
    public string ActionDescription
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(ActionDescription));
        }
      }
    }



    #endregion Bindings


    /// <summary>
    /// Weak event handler for tunneled document events.
    /// </summary>
    protected WeakActionHandler<object, object, TunnelingEventArgs> _weakEventHandlerForDoc_TunneledEvent;

    /// <summary>
    /// Gets the action document managed by this controller.
    /// </summary>
    public ActionDocument ActionDocument { get { return _doc; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDocumentController"/> class.
    /// </summary>
    public ActionDocumentController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDocumentController"/> class.
    /// </summary>
    /// <param name="doc">The document to control.</param>
    public ActionDocumentController(ActionDocument doc)
    {
      InitializeDocument(doc);
    }

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
      {
        return false;
      }

      if (args[0] is ActionDocument actionDoc)
      {
        _doc = actionDoc;
        InternalInitializeDocument(actionDoc);
        return true;
      }
      else if (args[0] is ActionViewLayout actionViewLayout)
      {
        InternalInitializeDocument(actionViewLayout.Document);
        return true;
      }
      else
      {
        return false;
      }
    }


    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    /// <summary>
    /// Initializes the controller with the provided view options.
    /// </summary>
    /// <param name="doc">The action document.</param>
    protected void InternalInitializeDocument(ActionDocument doc)
    {
      if (doc is null)
        throw new ArgumentNullException("No document provided");

      _doc = doc;

      Title = GetTitleFromDocumentName(ActionDocument);

      var actionDocument = ActionDocument;
      {
        // Attention: use LOCAL variables here in order to avoid references to the controller!
        _weakEventHandlerForDoc_TunneledEvent?.Remove();
        actionDocument.TunneledEvent += new WeakActionHandler<object, object, Altaxo.Main.TunnelingEventArgs>(EhDocumentTunneledEvent, actionDocument, nameof(actionDocument.TunneledEvent));
      }

      Initialize(true);
    }

    private void EhDocumentTunneledEvent(object sender, object originalSource, TunnelingEventArgs e)
    {
      if (e is Altaxo.Main.DocumentPathChangedEventArgs && _view is not null)
      {
        Title = GetTitleFromDocumentName(ActionDocument);
      }

      if (e is DisposeEventArgs && object.ReferenceEquals(originalSource, ActionDocument))
      {
        Current.Workbench.CloseContent(this);
      }
    }

    private static string GetTitleFromDocumentName(ActionDocument doc)
    {
      return doc.Name;
    }

    /// <summary>
    /// Initializes the view state.
    /// </summary>
    /// <param name="initData"><see langword="true"/> to initialize data; otherwise, <see langword="false"/>.</param>
    protected void Initialize(bool initData)
    {

      if (initData)
      {
        var ctrl = (IMVCANDController)Current.Gui.GetControllerAndControl(new object[] { _doc.Action }, typeof(IMVCANDController));

        ActionController = ctrl;
        ActionDescription = _doc.Action.GetType().Name;
      }
    }


    private void EhCmdExecute()
    {
      var exception = Current.Gui.ExecuteAsUserCancellable(1000, (reporter) =>
        _doc.Action.Execute(reporter)
        );

      if (exception is not null)
      {
        Current.Gui.ErrorMessageBox(exception.ToString(), "Exception during action execution");
      }
    }


    private void EhDocumentChanged(object sender, EventArgs e)
    {
    }

    private void EhActionHasChanged(IMVCANDController controller)
    {
      _doc.Action = (IAction)controller.ProvisionalModelObject;
    }

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      throw new NotImplementedException();
    }



    /// <inheritdoc/>
    public bool CanPaste()
    {
      return false;
    }

    /// <inheritdoc/>
    public bool Paste()
    {
      return false;
    }




    /// <inheritdoc/>
    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is { } oldView)
        {
          oldView.DataContext = null;
        }

        _view = value as IActionDocumentView;

        if (_view is { } newView)
        {
          newView.DataContext = this;
          Initialize(false);
        }
      }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
      ViewObject = null;
      _weakEventHandlerForDoc_TunneledEvent?.Remove();
      _doc = null;

      base.Dispose();
    }

    /// <inheritdoc />
    public override object ModelObject
    {
      get
      {
        return new ActionViewLayout(_doc);
      }
    }
  }
}
