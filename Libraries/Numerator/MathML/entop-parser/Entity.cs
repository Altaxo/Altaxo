using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace entop_parser
{
	/// <summary>
	/// Summary description for Entity.
	/// </summary>
	public class Entity : IComparable
	{
		public string name;
		public short code1, code2;
		private string val;
		private string comment;
		private bool validNum = false;
		

		//private static Regex regex = new Regex(

		public Entity(string name, string val, string comment)
		{
            string[] values = GetValues(val);

			try
			{
				if(values[0][0] == 'x' || values[0][0] == 'X')
				{
					val = values[0].Substring(1);
					code1 = Int16.Parse(val, NumberStyles.HexNumber);
					validNum = true;
				}
				else
				{
					code1 = Int16.Parse(values[0]);
					validNum = true;
				}

				if(values.Length == 2)
				{
					if(values[1][0] == 'x' || values[1][0] == 'X')
					{
						val = values[1].Substring(1);
						code2 = Int16.Parse(val, NumberStyles.HexNumber);
						validNum = true;
					}
					else
					{
						code2 = Int16.Parse(values[0]);
						validNum = true;
					}
				}
				else
				{
					code2 = 0;
				}
			}
			catch(Exception)
			{
				code1 = 0;
				code2 = 0;
				validNum = false;
			}




			this.name = name;
			this.val = val;
			this.comment = comment;

		}

		private static string[] GetValues(string val)
		{
			int len = 0;
			string[] values = val.Split(new char[] {';', '#', '&'});

			foreach(string s in values)
			{
				if(s.Length > 0) len++;
			}

			string[] result = new string[len];

			len = 0;

			foreach(string s in values)
			{
				if(s.Length > 0) result[len++] = s;
			}

			return result;
		}

		public override string ToString()
		{
			int s = 20 - name.Length;
			if(s < 0) s = 0;

			string spaces = new string(' ', s);

			if(validNum)
			{
				return String.Format("new Entity(\"{0}\",{1} \'\\x{2:x}\', \'\\x{3:x}\'), // {4}", name, spaces, code1, code2, comment);
			}
			else
			{
				return String.Format("// new Entity(\"{0}\",{1} \"{2}\"), {3}", name, spaces, val, comment);
			}
		}

		public int CompareTo(object obj)
		{
			Entity e = (Entity)obj;
			return name.CompareTo(e.name);
		}
	}
}
