#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
  #region Interfaces

  public interface IAxisLinkView 
  {

   

    /// <summary>
    /// Initializes the type of the link.
    /// </summary>
    /// <param name="isStraight">If <c>true</c>, the linke is initialized as 1:1 link and all other fields are disabled.</param>
    void LinkType_Initialize(bool isStraight);

    /// <summary>
    /// Initializes the content of the OrgA edit box.
    /// </summary>
    void OrgA_Initialize(string text);

    /// <summary>
    /// Initializes the content of the OrgB edit box.
    /// </summary>
    void OrgB_Initialize(string text);

    /// <summary>
    /// Initializes the content of the EndA edit box.
    /// </summary>
    void EndA_Initialize(string text);

    /// <summary>
    /// Initializes the content of the EndB edit box.
    /// </summary>
    void EndB_Initialize(string text);


    /// <summary>
    /// Enables / Disables the edit boxes for the org and end values
    /// </summary>
    /// <param name="bEnable">True if the boxes are enabled for editing.</param>
    void Enable_OrgAndEnd_Boxes(bool bEnable);


		/// <summary>Called when the contents of OrgA is changed. The argument provides the string to validate and is used to give a feedback if the validation is not successful.</summary>
		event Action< ValidationEventArgs<string>> OrgA_Validating;
		/// <summary>Called when the contents of OrgB is changed. The argument provides the string to validate and is used to give a feedback if the validation is not successful.</summary>
		event Action<ValidationEventArgs<string>> OrgB_Validating;
		/// <summary>Called when the contents of EndA is changed. The argument provides the string to validate and is used to give a feedback if the validation is not successful.</summary>
		event Action<ValidationEventArgs<string>> EndA_Validating;
		/// <summary>Called when the contents of EndB is changed. The argument provides the string to validate and is used to give a feedback if the validation is not successful.</summary>
		event Action<ValidationEventArgs<string>> EndB_Validating;
		/// <summary>Called if the type of the link is changed. The argument is true, if the type of link is "straight".</summary>
		event Action<bool> LinkType_Changed;
  
  }
  #endregion

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
	[ExpectedTypeOfView(typeof(IAxisLinkView))]
	[UserControllerForObject(typeof(LinkedScaleParameters))]
  public class AxisLinkController : IMVCAController
  {
    IAxisLinkView _view;
		LinkedScaleParameters _doc;
		LinkedScaleParameters _tempDoc;

		bool m_LinkType;


    public AxisLinkController(LinkedScaleParameters doc)
    {
			_doc = doc;
			_tempDoc = (LinkedScaleParameters)_doc;
			m_LinkType = _tempDoc.IsStraightLink;
      SetElements(true);
    }


    void SetElements(bool bInit)
    {
      if(null!=_view)
      {
        _view.LinkType_Initialize(m_LinkType);
        _view.OrgA_Initialize(Serialization.GUIConversion.ToString(_tempDoc.OrgA));
				_view.OrgB_Initialize(Serialization.GUIConversion.ToString(_tempDoc.OrgB));
				_view.EndA_Initialize(Serialization.GUIConversion.ToString(_tempDoc.EndA));
				_view.EndB_Initialize(Serialization.GUIConversion.ToString(_tempDoc.EndB));
      }
    }
    #region ILinkAxisController Members
 
    public void EhView_LinkTypeChanged(bool isStraightLink)
    {
			m_LinkType = isStraightLink;

      if(null!=_view)
        _view.Enable_OrgAndEnd_Boxes(!isStraightLink);
    }

    public void EhView_OrgAValidating(ValidationEventArgs<string>e)
    {
			double val;
			if (!GUIConversion.IsDouble(e.ValueToValidate, out val))
				e.AddError("Provided text is not recognized as a number");
			else if (!Altaxo.Calc.RMath.IsFinite(val))
				e.AddError("Provided value is not a finite number");
			else
				_tempDoc.OrgA = val;
    }

		public void EhView_OrgBValidating(ValidationEventArgs<string> e)
    {
			double val;
			if (!GUIConversion.IsDouble(e.ValueToValidate, out val))
				e.AddError("Provided text is not recognized as a number");
			else if (!Altaxo.Calc.RMath.IsFinite(val))
				e.AddError("Provided value is not a finite number");
			else
				_tempDoc.OrgB = val;
		}

		public void EhView_EndAValidating(ValidationEventArgs<string> e)
    {
			double val;
			if (!GUIConversion.IsDouble(e.ValueToValidate, out val))
				e.AddError("Provided text is not recognized as a number");
			else if (!Altaxo.Calc.RMath.IsFinite(val))
				e.AddError("Provided value is not a finite number");
			else
				_tempDoc.EndA = val;
		}

		public void EhView_EndBValidating(ValidationEventArgs<string> e)
    {
			double val;
			if (!GUIConversion.IsDouble(e.ValueToValidate, out val))
				e.AddError("Provided text is not recognized as a number");
			else if (!Altaxo.Calc.RMath.IsFinite(val))
				e.AddError("Provided value is not a finite number");
			else
				_tempDoc.EndB = val;
		}

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
			if (m_LinkType)
				_doc.SetToStraightLink();
			else
				_doc.CopyFrom(_tempDoc);
		
      return true;
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
					_view.OrgA_Validating -= EhView_OrgAValidating;
					_view.OrgA_Validating -= EhView_OrgBValidating;
					_view.OrgA_Validating -= EhView_EndAValidating;
					_view.OrgA_Validating -= EhView_EndBValidating;
					_view.LinkType_Changed -= EhView_LinkTypeChanged;
				}

				_view = value as IAxisLinkView;

				if (null != _view)
				{
					SetElements(false); // set only the view elements, dont't initialize the variables

					_view.OrgA_Validating += EhView_OrgAValidating;
					_view.OrgA_Validating += EhView_OrgBValidating;
					_view.OrgA_Validating += EhView_EndAValidating;
					_view.OrgA_Validating += EhView_EndBValidating;
					_view.LinkType_Changed += EhView_LinkTypeChanged;

				}
			}
    }

    public object ModelObject
    {
      get { return this._doc; }
    }

    #endregion
  }
}
