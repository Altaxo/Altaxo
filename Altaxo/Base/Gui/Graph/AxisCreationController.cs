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

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Supports the creation of a new axis by the axis creation dialog.
	/// </summary>
	public class AxisCreationArguments : ICloneable
	{
		// Input arguments
		public List<CSAxisInformation> AxisStyles { get; set; }

		/// <summary>Before showing the axis creation dialog, this property should be set to
		/// the currently choosen axis style.
		/// At the end of the axis creation dialog, this contains the identifier of the axis
		/// that should be used as an template to copy the axis from.</summary>
		/// <value>The template style.</value>
		public CSLineID TemplateStyle { get; set; }

		/// <summary>Gets or sets the current style.  After successfully showing the dialog, this property contains the identifier of the axis that should be created.</summary>
		/// <value>The current style.</value>
		public CSLineID CurrentStyle { get; set; }

		/// <summary>Gets or sets a value indicating whether to move a axis style or to copy it..</summary>
		/// <value>
		/// 	If <see langword="true"/>, the new axis should be created from the template axis, and then the template axis should be removed.
		/// 	If <see langword="false"/>, the new axis should be created as a copy of the template axis, but the template axis must not be removed.
		/// </value>
		public bool MoveAxis { get; set; }

		public void InitializeAxisInformationList(G2DCoordinateSystem cs, AxisStyleCollection currentAxisStyles)
		{
			var dict = new Dictionary<CSLineID, CSAxisInformation>();
			AxisStyles = new List<CSAxisInformation>();

			foreach (var style in cs.AxisStyles)
			{
				if (!dict.ContainsKey(style.Identifier))
				{
					dict.Add(style.Identifier, style);
					AxisStyles.Add(style);
				}
			}

			foreach (var axstyle in currentAxisStyles)
			{
				if (null != axstyle.CachedAxisInformation && !AxisStyles.Contains(axstyle.CachedAxisInformation))
				{
					if (!dict.ContainsKey(axstyle.CachedAxisInformation.Identifier))
					{
						dict.Add(axstyle.CachedAxisInformation.Identifier, axstyle.CachedAxisInformation);
						AxisStyles.Add(axstyle.CachedAxisInformation);
					}
				}
			}

			if (null != CurrentStyle && !dict.ContainsKey(CurrentStyle))
			{
				var info = cs.GetAxisStyleInformation(CurrentStyle);
				AxisStyles.Add(info);
			}
		}

		public static void AddAxis(AxisStyleCollection collection, AxisCreationArguments creationArgs)
		{
			var context = collection.GetPropertyContext();
			AxisStyle axstyle = new AxisStyle(creationArgs.CurrentStyle, false, false, false, null, context);
			if (creationArgs.TemplateStyle != null && collection.Contains(creationArgs.TemplateStyle))
			{
				axstyle.CopyWithoutIdFrom(collection[creationArgs.TemplateStyle]);
				if (creationArgs.MoveAxis)
					collection.Remove(creationArgs.TemplateStyle);
			}
			collection.Add(axstyle);
		}

		public object Clone()
		{
			var result = (AxisCreationArguments)this.MemberwiseClone();
			result.AxisStyles = new List<CSAxisInformation>(this.AxisStyles);
			return result;
		}
	}

	public interface IAxisCreationView
	{
		bool UsePhysicalValue { get; set; }

		double AxisPositionLogicalValue { get; set; }

		Altaxo.Data.AltaxoVariant AxisPositionPhysicalValue { get; set; }

		bool MoveAxis { get; set; }

		void InitializeAxisTemplates(SelectableListNodeList list);

		event Action SelectedAxisTemplateChanged;
	}

	[ExpectedTypeOfView(typeof(IAxisCreationView))]
	[UserControllerForObject(typeof(AxisCreationArguments))]
	public class AxisCreationController : MVCANControllerEditOriginalDocBase<AxisCreationArguments, IAxisCreationView>
	{
		private SelectableListNodeList _axisTemplates;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_axisTemplates = new SelectableListNodeList();
				foreach (var style in _doc.AxisStyles)
				{
					var node = new SelectableListNode(style.NameOfAxisStyle, style, style.Identifier == _doc.TemplateStyle);
					_axisTemplates.Add(node);
				}
				var selNode = _axisTemplates.FirstSelectedNode;
				if (null == selNode && 0 != _axisTemplates.Count)
				{
					selNode = _axisTemplates[0];
					selNode.IsSelected = true;
				}
				if (null != selNode)
				{
					_doc.TemplateStyle = (selNode.Tag as CSAxisInformation).Identifier;
				}
			}
			if (null != _view)
			{
				_view.MoveAxis = _doc.MoveAxis;
				_view.InitializeAxisTemplates(_axisTemplates);
				SetViewAccordingToAxisIdentifier();
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.MoveAxis = _view.MoveAxis;

			if (_view.UsePhysicalValue)
			{
				_doc.CurrentStyle = CSLineID.FromPhysicalVariant(_doc.TemplateStyle.ParallelAxisNumber, _view.AxisPositionPhysicalValue);
			}
			else
			{
				_doc.CurrentStyle = new CSLineID(_doc.TemplateStyle.ParallelAxisNumber, _view.AxisPositionLogicalValue);
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			_view.SelectedAxisTemplateChanged += EhSelectedAxisTemplateChanged;
		}

		protected override void DetachView()
		{
			_view.SelectedAxisTemplateChanged -= EhSelectedAxisTemplateChanged;
		}

		private void EhSelectedAxisTemplateChanged()
		{
			_doc.TemplateStyle = (_axisTemplates.FirstSelectedNode.Tag as CSAxisInformation).Identifier;
			SetViewAccordingToAxisIdentifier();
		}

		private void SetViewAccordingToAxisIdentifier()
		{
			_view.UsePhysicalValue = _doc.TemplateStyle.UsePhysicalValueOtherFirst;
			if (_doc.TemplateStyle.UsePhysicalValueOtherFirst)
			{
				_view.AxisPositionPhysicalValue = _doc.TemplateStyle.PhysicalValueOtherFirst;
			}
			else
			{
				_view.AxisPositionLogicalValue = _doc.TemplateStyle.LogicalValueOtherFirst;
			}
		}
	}
}