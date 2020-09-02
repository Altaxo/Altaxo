using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

using WPG.Data;

namespace WPG
{
	public class PropertyTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			Property property = item as Property;
			if (property is null)
			{
				throw new ArgumentException("item must be of type Property");
			}
			FrameworkElement element = container as FrameworkElement;
			if (element is null)
			{
				return base.SelectTemplate(property.Value, container);
			}
			DataTemplate template = FindDataTemplate(property, element);
			return template;
		}		

		private DataTemplate FindDataTemplate(Property property, FrameworkElement element)
		{
			Type propertyType = property.PropertyType;


            if (!(property.PropertyType is String) && property.PropertyType is IEnumerable)
                propertyType = typeof(List<object>);
            
			DataTemplate template = TryFindDataTemplate(element, propertyType);

    		while (template is null && propertyType.BaseType is not null)
			{
				propertyType = propertyType.BaseType;
				template = TryFindDataTemplate(element, propertyType);
			}
			if (template is null)
			{
				template = TryFindDataTemplate(element, "default");
			}
			return template;
		}

		private static DataTemplate TryFindDataTemplate(FrameworkElement element, object dataTemplateKey)
		{
			object dataTemplate = element.TryFindResource(dataTemplateKey);
			if (dataTemplate is null)
			{
				dataTemplateKey = new ComponentResourceKey(typeof(PropertyGrid), dataTemplateKey);
				dataTemplate = element.TryFindResource(dataTemplateKey);
			}
			return dataTemplate as DataTemplate;
		}
	}
}
