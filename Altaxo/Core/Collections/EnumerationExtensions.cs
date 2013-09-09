using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	/// <summary>
	/// Extends the operations for Flag Enumerations with setting and clearing of individual flags.
	/// </summary>
	public static class EnumerationExtensions
	{
		public static bool Is<T>(this System.Enum type, T value)
		{
			try
			{
				return (int)(object)type == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

		public static T WithSetFlag<T>(this System.Enum type, T value)
		{
			try
			{
				return (T)(object)(((int)(object)type | (int)(object)value));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
						string.Format(
								"Could not append value from enumerated type '{0}'.",
								typeof(T).Name
								), ex);
			}
		}

		public static T WithClearedFlag<T>(this System.Enum type, T value)
		{
			try
			{
				return (T)(object)(((int)(object)type & ~(int)(object)value));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
						string.Format(
								"Could not remove value from enumerated type '{0}'.",
								typeof(T).Name
								), ex);
			}
		}
	}
}