using System;
using System.Reflection;
using System.ComponentModel;

namespace Altaxo.Serialization
{
	/// <summary>
	/// Summary description for EnumParsing.
	/// </summary>
	public class EnumParsing
	{
		public static string GetDescription(Enum value)
		{
			FieldInfo fi= value.GetType().GetField(value.ToString()); 
			DescriptionAttribute[] attributes = 
				(DescriptionAttribute[])fi.GetCustomAttributes(
				typeof(DescriptionAttribute), false);
			return (attributes.Length>0)?attributes[0].Description:value.ToString();
		}
	}
}
