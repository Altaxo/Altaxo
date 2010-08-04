using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Document class that bundles a text question with a predefined set of choices.
	/// </summary>
  public class TextChoice 
  {
    protected string[] _choices;
    protected int _selection;
    bool _allowFreeText;
    string _choosenText;
    string _description = string.Empty;
		Func<string, string> _textValidationFunction;

  

    public TextChoice(string[] choices, int selection, bool allowFreeText)
    {
      _choices = (string[])choices.Clone();
      _selection = selection;
      _allowFreeText = allowFreeText;
    }

    public string[] Choices
    {
      get
      {
        return (string[])_choices.Clone();
      }
    }

		/// <summary>
		/// If a predefined choice is selected, this gets the index of the choice. If free text is
		/// given, the value is -1.
		/// </summary>
    public int SelectedIndex
    {
      get
      {
        return _selection;
      }
      set
      {
        _selection = value;
      }
    }

		/// <summary>
		/// Gets/sets whether free text choice is allowed or not allowed.
		/// </summary>
    public bool AllowFreeText
    {
      get
      {
        return _allowFreeText;
      }
      set
      {
        _allowFreeText = value;
      }
    }

		/// <summary>
		/// Get/sets the Text that is choosen by the user or entered as free text.
		/// </summary>
    public string Text
    {
      get { return _choosenText; }
      set { _choosenText = value; }
    }

		/// <summary>
		/// Description string that is shown close to the gui element where you enter the text.
		/// </summary>
    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

		public Func<string, string> TextValidationFunction
		{
			get
			{
				return _textValidationFunction;
			}
			set
			{
				_textValidationFunction = value;
			}
		}

  }


  public interface IFreeTextChoiceView
{
    /// <summary>
    /// Fired if the user has changed the selection from the selection list.
    /// </summary>
  event Action<int> SelectionChangeCommitted;

    /// <summary>Fired if free text was entered and needs to be validated. Make sure that this event is fired only,
    /// if free text was entered, and is not fired if the user has choosen from the selection list.</summary>
  event Action<string, CancelEventArgs> TextValidating;
  void SetDescription(string value);
  void SetChoices(string[] values, int initialselection, bool allowFreeText);
}

  [UserControllerForObject(typeof(TextChoice))]
  [ExpectedTypeOfView(typeof(IFreeTextChoiceView))]
  public class TextChoiceController : IMVCANController
  {
    IFreeTextChoiceView _view;
    TextChoice _doc;

    void Initialize(bool initData)
    {
      if (null != _view)
      {
        _view.SetDescription(_doc.Description);
        _view.SetChoices(_doc.Choices, _doc.SelectedIndex, _doc.AllowFreeText);
      }
    }

    void EhSelectionChangeCommitted(int selIndex)
    {
      _doc.SelectedIndex = selIndex;

      if(selIndex>=0)
        _doc.Text = _doc.Choices[selIndex];
    }

    void EhTextValidating(string text, CancelEventArgs e)
    {
			string validationResult = null;
			if (null != _doc.TextValidationFunction)
			{
				validationResult = _doc.TextValidationFunction(text);
			}
			if (null == validationResult)
			{

				_doc.Text = text;
				_doc.SelectedIndex = -1;
			}
			else
			{
				e.Cancel = true;
			}
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (null == args || 0 == args.Length || !(args[0] is TextChoice))
        return false;

      _doc = (TextChoice)args[0];
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set {  }
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (null != _view)
        {
          _view.SelectionChangeCommitted -= EhSelectionChangeCommitted;
          _view.TextValidating -= EhTextValidating;
        }

        _view = value as IFreeTextChoiceView;

        if (null != _view)
        {
          Initialize(false);
          _view.SelectionChangeCommitted += EhSelectionChangeCommitted;
          _view.TextValidating += EhTextValidating;
        }
      }
    }

    public object ModelObject
    {
			get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
			return true;
    }

    #endregion
  }
}
