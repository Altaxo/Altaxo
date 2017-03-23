#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	public interface IColorModelView
	{
		void InitializeAvailableColorModels(SelectableListNodeList listOfColorModels);

		void InitializeColorModel(IColorModel colorModel, bool silentSet);

		void InitializeCurrentColor(AxoColor color, string colorName);

		event Action ColorModelSelectionChanged;
	}

	[ExpectedTypeOfView(typeof(IColorModelView))]
	public class ColorModelController : MVCANControllerEditImmutableDocBase<NamedColor, IColorModelView>
	{
		private SelectableListNodeList _availableColorModels;
		private IColorModel _currentColorModel;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_availableColorModels = new SelectableListNodeList();

				var models = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorModel));

				foreach (var modelType in models)
				{
					_availableColorModels.Add(new SelectableListNode(modelType.Name, modelType, modelType == typeof(ColorModelRGB)));
				}
				_currentColorModel = new ColorModelRGB();
			}
			if (null != _view)
			{
				_view.InitializeAvailableColorModels(_availableColorModels);
				_view.InitializeColorModel(_currentColorModel, true);
				_view.InitializeCurrentColor(_doc.Color, _doc.Name);
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.ColorModelSelectionChanged += EhColorModelSelectionChanged;
		}

		protected override void DetachView()
		{
			_view.ColorModelSelectionChanged -= EhColorModelSelectionChanged;

			base.DetachView();
		}

		private void EhColorModelSelectionChanged()
		{
			var node = _availableColorModels.FirstSelectedNode;

			if (null != node && (Type)node.Tag != _currentColorModel.GetType())
			{
				var newColorModel = (IColorModel)Activator.CreateInstance((Type)node.Tag);
				_currentColorModel = newColorModel;
				_view.InitializeColorModel(_currentColorModel, false);
			}
		}
	}
}