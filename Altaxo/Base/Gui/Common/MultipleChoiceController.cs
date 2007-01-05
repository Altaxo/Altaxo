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
  
  public interface IMultipleChoiceViewEventSink
  {
    void EhChoiceChanged(int choice, bool selected );
  }
  public interface IMultipleChoiceView
  {
    IMultipleChoiceViewEventSink Controller { set; }

    void InitializeDescription(string value);
    void InitializeChoices(string[] values, bool[] initialchoices);
  }
 

  #endregion

  /// <summary>
  /// Controller for a single value. This is a string here, but in derived classes, that can be anything that can be converted to and from a string.
  /// </summary>
  public class MultipleChoiceController : IMVCAController, IMultipleChoiceViewEventSink
  {
    protected IMultipleChoiceView _view;
    protected string[] _choices;
    protected bool[]   _selections;
    protected bool[]   _selectionsTemp;

    protected string _descriptionText = "Enter value:";

    public MultipleChoiceController(string[] values, bool[] selected)
    {
      _choices = values;
      _selections = selected;
      _selectionsTemp = (bool[])selected.Clone();
    }

    protected virtual void Initialize()
    {
      if(null!=_view)
      {
        _view.InitializeDescription(_descriptionText);
        _view.InitializeChoices(_choices,_selectionsTemp);
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

        _view = value as IMultipleChoiceView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _selections;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      for(int i=0;i<_selections.Length;i++)
        _selections[i] = _selectionsTemp[i];

      return true;
    }

    #endregion

    #region IMultipleChoiceViewEventSink Members

    public virtual void EhChoiceChanged(int choice, bool selection)
    {
      _selectionsTemp[choice] = selection;
    }

    #endregion
  }
}
