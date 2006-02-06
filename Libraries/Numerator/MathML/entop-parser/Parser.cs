using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace entop_parser
{
	/// <summary>
	/// Summary description for Parser.
	/// </summary>
	public class Parser
	{
		[STAThread]
		static void Main(string[] args)
		{
			args = new string[2];
			args[0] = "xhtml-math11-f.dtd";
			args[1] = "operator-dictionary.txt";
			ArrayList entities = new ArrayList();
			ArrayList operators = new ArrayList();
			StreamWriter writer = null;

			if(args.Length != 2)
			{
				Console.WriteLine("usage: entop_parser mathml.dtd.name operator.txt.name");
				return;
			}

			try
			{
				string line;
				StreamReader dtdReader = new StreamReader(args[0]);
				Regex regex = new Regex(
				"<!ENTITY\\s+(?<name>\\w+)\\s+\\\"\\s*(?<value>.+)\\s*\\\"\\s*>\\s*<!--\\s*(?<comment>.+)\\s*-->|" +
				"<!ENTITY\\s+(?<name>\\w+)\\s+\\\"\\s*(?<value>.+)\\s*\\\"\\s*>");

				foreach(Match m in regex.Matches(dtdReader.ReadToEnd()))
				{
					string name = m.Groups.Count > 1 ? m.Groups[1].ToString() : "no name";
					string val = m.Groups.Count > 2 ? m.Groups[2].ToString() : "no value";
					string comment = m.Groups.Count > 3 ? m.Groups[3].ToString() : "no comment";
					if(FindEntity(entities, name) == null)
					{
						entities.Add(new Entity(name, val, comment));
					}
				}

				entities.Sort();

				StreamReader opReader = new StreamReader(args[1]);

				while((line = opReader.ReadLine()) != null)
				{
					line = line.TrimStart(new char[] {' ','\t'});
					if(line.Length > 0)
					{
						operators.Add(new Operator(line));
					}
				}

				opReader.Close();

				foreach(Operator o in operators)
				{
					Entity e = FindEntity(entities, o.Comment);
					if(e != null)
					{
						if(e.code1 != 0 && e.code2 != 0)
						{
							o.Name = "\"\\x" + e.code1.ToString("x") + "\\x" + e.code2.ToString("x") + "\"";
						}
						else
						{
							o.Name = "\"\\x" + e.code1.ToString("x") + "\"";
						}
                        
					}
					else
					{
						o.Name = "\"" + o.Comment + "\"";
					}
				}

				writer = new StreamWriter("entities.cs");
				foreach(Entity e in entities)
				{
					writer.WriteLine(e.ToString());
				}
				writer.Close();

				writer = new StreamWriter("operators.cs");
				foreach(Operator o in operators)
				{
					writer.WriteLine(o.ToString());
				}
				writer.Close();





			}
			catch(Exception e)
			{
				Console.WriteLine("Error: {0}, {1}", e.Message, e.StackTrace);
			}
		}

		static Entity FindEntity(ArrayList entities, string name)
		{
			foreach(Entity e in entities)
			{
				if (e.name == name) return e;
			}
			return null;
		}
	}
}
