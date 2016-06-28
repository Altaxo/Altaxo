#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Altaxo.Gui.Common
{
	public interface ISimpleImmutableObjectView
	{
		/// <summary>
		/// Initializes the view's values. Item1 is the Name of the value to set; Item2 is the value type, and Item3 is the value itself.
		/// </summary>
		/// <param name="values">The values.</param>
		void Values_Initialize(IEnumerable<Tuple<string, Type, object>> values);

		/// <summary>
		/// Gets the value at index idx.
		/// </summary>
		/// <param name="idx">The index.</param>
		/// <returns></returns>
		object Value_Get(int idx);
	}

	/// <summary>
	/// Controller for simple immutable objects which have simple properties (double, string) and setter methods
	/// following the naming scheme WithPropertyName(propertyName). It is essential that the parameter name of
	/// the setter method is the same (but the first letter can be lower case) as the property name.
	/// You must subclass this controller and set a attribute at which classes can be controlled.
	/// </summary>
	[ExpectedTypeOfView(typeof(ISimpleImmutableObjectView))]
	public class SimpleImmutableObjectController<TObject> : MVCANControllerEditImmutableDocBase<TObject, ISimpleImmutableObjectView> where TObject : Altaxo.Main.IImmutable
	{
		private class ValueInfo
		{
			public string Name;
			public object Value;
			public Type ValueType;
			public MethodInfo SetterMethod;
		}

		private List<ValueInfo> _valueInfo;

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_valueInfo = new List<ValueInfo>();
				var allMethods = _doc.GetType().GetMethods();

				foreach (var method in allMethods)
				{
					if (!(method.ReturnType == _doc.GetType()))
						continue;

					var methodParameters = method.GetParameters();
					if (methodParameters.Length != 1)
						continue;

					var parameterType = methodParameters[0].ParameterType;

					if (parameterType != typeof(double) && parameterType != typeof(DateTime) && parameterType != typeof(string))
						continue;

					var parameterName = methodParameters[0].Name;
					parameterName = parameterName.Substring(0, 1).ToUpperInvariant() + parameterName.Substring(1); // Capitalize parameter name

					object parameterValue = null;
					var getterProp = _doc.GetType().GetProperty(parameterName);
					if (null != getterProp && getterProp.PropertyType == parameterType)
						parameterValue = getterProp.GetValue(_doc, null);

					_valueInfo.Add(new ValueInfo() { Name = parameterName, ValueType = parameterType, SetterMethod = method, Value = parameterValue });
				}
			}
			if (null != _view)
			{
				_view.Values_Initialize(_valueInfo.Select(x => new Tuple<string, Type, object>(x.Name, x.ValueType, x.Value)));
			}
		}

		public override bool Apply(bool disposeController)
		{
			for (int i = 0; i < _valueInfo.Count; ++i)
			{
				var value = _view.Value_Get(i);
				_doc = (TObject)_valueInfo[i].SetterMethod.Invoke(_doc, new[] { value });
			}

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}
	}
}