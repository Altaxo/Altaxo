using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Common.Drawing
{
	public interface IWpfColorView
	{
		Color SelectedColor { get; set; }
	}

	/// <summary>
	/// Dummy controller to enable the use of <see cref="ColorPickerControl"/> directly from here in dialogs.
	/// </summary>
	class ColorController : IMVCAController
	{
		Color _doc;
		IWpfColorView _view;

		public ColorController(Color c)
		{
			_doc = c;
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IWpfColorView;

				if (null != _view)
					_view.SelectedColor = _doc;
			}
		}

		public object ModelObject
		{
			get { return _doc;  }
		}

		public bool Apply()
		{
			if(null!=_view)
				_doc = _view.SelectedColor;
			return true;
		}
	}
}
