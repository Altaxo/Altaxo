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
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  #region Interfaces
  public interface IIntegerAndComboBoxController : IMVCAController
  {
    /// <summary>
    /// Get/sets the view this controller controls.
    /// </summary>
    IIntegerAndComboBoxView View { get; set; }

    void EhView_IntegerChanged(int val, ref bool bCancel);

    void EhView_ComboBoxSelectionChanged(SelectableListNode selectedItem);

  }

  public interface IIntegerAndComboBoxView : IMVCView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IIntegerAndComboBoxController Controller { get; set; }
    
    void ComboBox_Initialize(SelectableListNodeList items, SelectableListNode defaultItem);
  
    void ComboBoxLabel_Initialize(string text);
    
    void IntegerEdit_Initialize(int min, int max, int val);

    void IntegerLabel_Initialize(string text);

    
  
  }
  #endregion

  /// <summary>
  /// Summary description for IntegerAndComboBoxController.
  /// </summary>
  public class IntegerAndComboBoxController : IIntegerAndComboBoxController
  {
    protected IIntegerAndComboBoxView m_View;
    protected string m_IntegerLabelText;
    protected string m_ComboBoxLabelText;
    protected int m_IntegerMinimum;
    protected int m_IntegerMaximum;
    protected int m_IntegerValue;
    protected SelectableListNodeList m_ComboBoxItems;
    protected SelectableListNode m_SelectedItem;

    
    public IntegerAndComboBoxController(string integerLabel,
      int intMin, int intMax, 
      int intVal,
      string comboBoxLabel,
      SelectableListNodeList items,
      int defaultItem)
    {
      m_IntegerLabelText = integerLabel;
      m_IntegerMinimum = intMin;
      m_IntegerMaximum = intMax;
      m_IntegerValue    = intVal;
      m_ComboBoxLabelText = comboBoxLabel;
      m_ComboBoxItems = items;
      m_SelectedItem = items[defaultItem];

      SetElements(true);
    }


    public void SetElements(bool bInit)
    {
      if(bInit)
      {
      }

      if(View!=null)
      {
        View.ComboBoxLabel_Initialize(m_ComboBoxLabelText);
        View.ComboBox_Initialize(m_ComboBoxItems,m_SelectedItem);
        View.IntegerLabel_Initialize(m_IntegerLabelText);
        View.IntegerEdit_Initialize(m_IntegerMinimum,m_IntegerMaximum,m_IntegerValue);
      }
    }

    public IIntegerAndComboBoxView View
    {
      get { return m_View; }
      set
      {
        if(null!=m_View)
          m_View.Controller = null;

        m_View = value;

        if(null!=m_View)
        {
          m_View.Controller = this;
          SetElements(false);
        }
      }
    }
  
    public bool Apply()
    {
      return true; // all is done on the fly, we don't need actions here
    }

    public SelectableListNode SelectedItem
    {
      get { return m_SelectedItem; }
    }

    public int IntegerValue
    {
      get { return this.m_IntegerValue; }
    }

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        
        return View;
      }
      set
      {
        View = value as IIntegerAndComboBoxView;
      }
    }

    public object ModelObject
    {
      get { return this.m_IntegerValue; }
    }

    #endregion

    #region IIntegerAndComboBoxController Members

    public void EhView_IntegerChanged(int val, ref bool bCancel)
    {
      m_IntegerValue = val;
      bCancel = false;
    }

    public void EhView_ComboBoxSelectionChanged(SelectableListNode selectedItem)
    {
      m_SelectedItem = selectedItem;
    }

    #endregion
  }



}
