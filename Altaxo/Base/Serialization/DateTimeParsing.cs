using System;

namespace Altaxo.Serialization
{
	/// <summary>
	/// This class is only intended to group some static functions for parsing of strings.
	/// </summary>
	public class DateTimeParsing
	{

		/// <summary>
		/// Is the provided string a date/time?
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>True if the string can successfully parsed to a DateTime object.</returns>
		public static bool IsDateTime(string s)
		{
			bool bRet=false;
			try
			{
				System.Convert.ToDateTime(s);
				bRet=true;
			}
			catch(Exception)
			{
			}
			return bRet;
		}

	
	}



}
