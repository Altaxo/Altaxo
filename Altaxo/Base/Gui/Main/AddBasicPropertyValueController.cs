using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Main
{
	public class AddBasicPropertyValueData : ICloneable
	{
		public string PropertyName { get; set; }

		public object PropertyValue { get; set; }

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public interface IAddBasicPropertyValueView
	{
		string PropertyName { get; set; }

		SelectableListNodeList PropertyTypes { set; }

		event Action PropertyTypeChanged;

		bool ShowValueAsString { set; }

		string ValueAsString { get; set; }

		bool ShowValueAsDouble { set; }

		double ValueAsDouble { get; set; }

		bool ShowValueAsInt { set; }

		int ValueAsInt { get; set; }

		bool ShowValueAsDateTime { set; }

		DateTime ValueAsDateTime { get; set; }
	}

	[UserControllerForObject(typeof(AddBasicPropertyValueData))]
	[ExpectedTypeOfView(typeof(IAddBasicPropertyValueView))]
	public class AddBasicPropertyValueController : MVCANControllerEditOriginalDocBase<AddBasicPropertyValueData, IAddBasicPropertyValueView>
	{
		SelectableListNodeList _propertyTypes;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				if (null == _propertyTypes)
					_propertyTypes = new SelectableListNodeList();
				else
					_propertyTypes.Clear();

				_propertyTypes.Add(new SelectableListNode("String", typeof(string), (_doc.PropertyValue is string) || (null == _doc.PropertyValue)));
				_propertyTypes.Add(new SelectableListNode("Double", typeof(double), _doc.PropertyValue is double));
				_propertyTypes.Add(new SelectableListNode("Integer", typeof(int), _doc.PropertyValue is int));
				_propertyTypes.Add(new SelectableListNode("DateTime", typeof(DateTime), _doc.PropertyValue is DateTime));
			}

			if (null != _view)
			{
				_view.PropertyName = _doc.PropertyName;
				_view.PropertyTypes = _propertyTypes;
				ShowPropertyValue();
			}
		}

		void ShowPropertyValue()
		{
			if ((_doc.PropertyValue is string) || (null == _doc.PropertyValue))
			{
				_view.ShowValueAsString = true;
				_view.ValueAsString = (string)_doc.PropertyValue;
			}
			else if (_doc.PropertyValue is double)
			{
				_view.ShowValueAsDouble = true;
				_view.ValueAsDouble = (double)_doc.PropertyValue;
			}
			else if (_doc.PropertyValue is int)
			{
				_view.ShowValueAsInt = true;
				_view.ValueAsInt = (int)_doc.PropertyValue;
			}
			else if (_doc.PropertyValue is DateTime)
			{
				_view.ShowValueAsDateTime = true;
				_view.ValueAsDateTime = (DateTime)_doc.PropertyValue;
			}
		}

		public override bool Apply(bool disposeController)
		{
			var name = _view.PropertyName;
			if (string.IsNullOrEmpty(name))
			{
				Current.Gui.ErrorMessageBox("The property name must not be null or empty. Please provide a valid property name");
				return ApplyEnd(false, disposeController);
			}
			_doc.PropertyName = name;

			var proptype = (Type)_propertyTypes.FirstSelectedNode.Tag;

			if (proptype == typeof(string))
			{
				_doc.PropertyValue = _view.ValueAsString;
			}
			else if (proptype == typeof(double))
			{
				_doc.PropertyValue = _view.ValueAsDouble;
			}
			else if (proptype == typeof(int))
			{
				_doc.PropertyValue = _view.ValueAsInt;
			}
			else if (proptype == typeof(DateTime))
			{
				_doc.PropertyValue = _view.ValueAsDateTime;
			}

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.PropertyTypeChanged += EhPropertyTypeChanged;
		}

		protected override void DetachView()
		{
			_view.PropertyTypeChanged -= EhPropertyTypeChanged;
			base.DetachView();
		}

		private void EhPropertyTypeChanged()
		{
			var selnode = _propertyTypes.FirstSelectedNode;
			if (null == selnode)
				return;

			var proptype = (Type)selnode.Tag;

			if (proptype == typeof(string))
			{
				_view.ShowValueAsString = true;
				if (_doc.PropertyValue is string)
					_view.ValueAsString = (string)_doc.PropertyValue;
			}
			else if (proptype == typeof(double))
			{
				_view.ShowValueAsDouble = true;
				if (_doc.PropertyValue is double)
					_view.ValueAsDouble = (double)_doc.PropertyValue;
			}
			else if (proptype == typeof(int))
			{
				_view.ShowValueAsInt = true;
				if (_doc.PropertyValue is int)
					_view.ValueAsInt = (int)_doc.PropertyValue;
			}
			else if (proptype == typeof(DateTime))
			{
				_view.ShowValueAsDateTime = true;
				if (_doc.PropertyValue is DateTime)
					_view.ValueAsDateTime = (DateTime)_doc.PropertyValue;
			}
		}
	}
}