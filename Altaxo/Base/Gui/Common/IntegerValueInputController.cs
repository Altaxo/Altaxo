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
using System.ComponentModel;

namespace Altaxo.Gui.Common
{
  [ExpectedTypeOfView(typeof(ISingleValueView))]
  public class IntegerValueInputController : IMVCAController
  {
    ISingleValueView m_View;

    int m_InitialContents;

    int m_EnteredContents;

    string _description;

    private IIntegerValidator m_Validator;

    public IntegerValueInputController(int initialcontents, string description)
    {
      m_InitialContents = initialcontents;
      m_EnteredContents = initialcontents;
      _description = description;

    }

    void Initialize()
    {
      m_View.DescriptionText=_description;
    }

    ISingleValueView View
    {
      get { return m_View; }
      set
      {
        if (m_View != null)
          m_View.ValueText_Validating -= this.EhView_ValidatingValue1;
        
        m_View = value;
        Initialize();

        if (m_View != null)
          m_View.ValueText_Validating += this.EhView_ValidatingValue1;
      }
    }

    public int EnteredContents
    {
      get { return m_EnteredContents; }
    }

    public IIntegerValidator Validator
    {
      set { m_Validator = value; }
    }

    public bool Validate()
    {
      string value = m_View.ValueText;
      string err = null;
      if (Altaxo.Serialization.GUIConversion.IsInteger(value, out m_EnteredContents))
      {
        if (null != this.m_Validator)
          err = m_Validator.Validate(m_EnteredContents);
      }
      else
      {
        err = "You must enter a integer value!";
      }

      if (null != err)
        Current.Gui.ErrorMessageBox(err);

      return null == err;
    }


    #region ISingleValueFormController Members

    public void EhView_ValidatingValue1(object sender, CancelEventArgs e)
    {
      // we validate only during apply
    }
    #endregion


    /// <summary>
    /// Provides an interface to a validator to validates the user input
    /// </summary>
    public interface IIntegerValidator
    {
      /// <summary>
      /// Validates if the user input number i is valid user input.
      /// </summary>
      /// <param name="i">The number entered by the user.</param>
      /// <returns>Null if this input is valid, error message else.</returns>
      string Validate(int i);
    }

    public class ZeroOrPositiveIntegerValidator : IIntegerValidator
    {
      public string Validate(int i)
      {
        if (i < 0)
          return "The provided number must be zero or positive!";
        else
          return null;
      }
    }

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return m_View;
      }
      set
      {
        if (m_View != null)
          m_View.ValueText_Validating -= this.EhView_ValidatingValue1;
        
        m_View = value as ISingleValueView;
        if(m_View!=null)
        {
          Initialize();
          m_View.ValueText_Validating += this.EhView_ValidatingValue1;
        }
      }
    }

    public object ModelObject
    {
      get { return m_InitialContents; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      if (!Validate())
        return false;

      m_InitialContents = m_EnteredContents;
      return true;
    }

    #endregion
  } // end of class IntegerValueInputController
}
