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

		void InitializeAvailableTextOnlyColorModels(SelectableListNodeList listOfTextOnlyColorModels);

		void InitializeColorModel(IColorModel colorModel, bool silentSet);

		void InitializeTextOnlyColorModel(ITextOnlyColorModel colorModel, bool silentSet);

		void InitializeCurrentColor(AxoColor color);

		event Action ColorModelSelectionChanged;

		event Action TextOnlyColorModelSelectionChanged;

		event Action<AxoColor> CurrentColorChanged;
	}

	[ExpectedTypeOfView(typeof(IColorModelView))]
	public class ColorModelController : MVCANDControllerEditImmutableDocBase<AxoColor, IColorModelView>
	{
		private SelectableListNodeList _availableColorModels;
		private SelectableListNodeList _availableTextOnlyColorModels;

		private IColorModel _currentColorModel;
		private ITextOnlyColorModel _currentTextOnlyColorModel;

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

				_currentColorModel = new ColorModelRGB();
				foreach (var modelType in models)
				{
					_availableColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentColorModel.GetType()));
				}

				// Text only color models
				_availableTextOnlyColorModels = new SelectableListNodeList();
				var textOnlyModels = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ITextOnlyColorModel));

				_currentTextOnlyColorModel = new TextOnlyColorModelRGB();
				foreach (var modelType in textOnlyModels)
				{
					_availableTextOnlyColorModels.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(modelType), modelType, modelType == _currentTextOnlyColorModel.GetType()));
				}
			}
			if (null != _view)
			{
				_view.InitializeAvailableColorModels(_availableColorModels);
				_view.InitializeAvailableTextOnlyColorModels(_availableTextOnlyColorModels);
				_view.InitializeColorModel(_currentColorModel, false);
				_view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);
				_view.InitializeCurrentColor(_doc);
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
			_view.TextOnlyColorModelSelectionChanged += EhTextOnlyColorModelSelectionChanged;
			_view.CurrentColorChanged += EhCurrentColorChanged;
		}

		protected override void DetachView()
		{
			_view.ColorModelSelectionChanged -= EhColorModelSelectionChanged;
			_view.TextOnlyColorModelSelectionChanged -= EhTextOnlyColorModelSelectionChanged;
			_view.CurrentColorChanged -= EhCurrentColorChanged;

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

		private void EhTextOnlyColorModelSelectionChanged()
		{
			var node = _availableTextOnlyColorModels.FirstSelectedNode;

			if (null != node && (Type)node.Tag != _currentTextOnlyColorModel.GetType())
			{
				var newTextOnlyColorModel = (ITextOnlyColorModel)Activator.CreateInstance((Type)node.Tag);
				_currentTextOnlyColorModel = newTextOnlyColorModel;
				_view.InitializeTextOnlyColorModel(_currentTextOnlyColorModel, false);
			}
		}

		private void EhCurrentColorChanged(AxoColor color)
		{
			_doc = color;
			OnMadeDirty();
		}
	}
}