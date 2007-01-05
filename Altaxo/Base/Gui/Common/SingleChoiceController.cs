#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Gui.Common
{

  #region Interfaces
  
  public interface ISingleChoiceViewEventSink
  {
    void EhChoiceChanged(int newchoice);
  }

  /// <summary>
  /// This interface must be implemented by controls that allow to choose a single
  /// selection out of multiple values.
  /// </summary>
  public interface ISingleChoiceView
  {
    /// <summary>
    /// Sets the controller.
    /// </summary>
    ISingleChoiceViewEventSink Controller { set; }

    /// <summary>
    /// Initializes a descriptive text.
    /// </summary>
    /// <param name="value">The descriptive text.</param>
    void InitializeDescription(string value);
    
    /// <summary>
    /// Initializes the choices and the initial selection.
    /// </summary>
    /// <param name="values">The choices.</param>
    /// <param name="initialchoice">The index of the initial selected choice.</param>
    void InitializeChoice(string[] values, int initialchoice);
  }
 

  public interface ISingleChoiceObject
  {
    string[] Choices   { get; }
    int      Selection { get; set; }
  }

  public class SingleChoiceObject : ISingleChoiceObject
  {
    protected string[] _choices;
    protected int      _selection;

    public SingleChoiceObject(string[] choices, int selection)
    {
      _choices = (string[])choices.Clone();
      _selection = selection;
    }

    public string[] Choices
    {
      get 
      {
        return (string[])_choices.Clone();
      }
    }

    public int Selection
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
    

  }

  #endregion

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  public class SingleChoiceController : IMVCAController, ISingleChoiceViewEventSink
  {
    protected ISingleChoiceView _view;
    protected string[]          _values;
    protected int               _choice;
    protected int               _choiceTemp;

    protected string _descriptionText = "Your choice:";

    public SingleChoiceController(string[] values, int selected)
    {
      Initialize(values,selected);
    }

    protected SingleChoiceController()
    {
    }

    protected void Initialize(string[] values, int selected)
    {
      _values = values;
      _choice = _choiceTemp = selected;
    }

    protected virtual void Initialize()
    {
      if(null!=_view)
      {
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChoice(_values,_choice);
      }
    }

    public string DescriptionText
    {
      get 
      {
        return _descriptionText; 
      }
      set
      {
        _descriptionText = value;
        if(null!=_view)
        {
          _view.InitializeDescription(_descriptionText);
        }
      }
    }
    #region IMVCController Members

    public virtual object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as ISingleChoiceView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _choice;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      _choice = _choiceTemp;
      return true;
    }

    #endregion

    #region ISingleChoiceViewEventSink Members

    public virtual void EhChoiceChanged(int val)
    {
      _choiceTemp = val;
    }

    #endregion
  }
}
