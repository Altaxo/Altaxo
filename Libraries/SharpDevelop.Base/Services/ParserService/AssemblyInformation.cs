// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Xml;
using SharpDevelop.Internal.Parser;

//using ICSharpCode.SharpAssembly.Metadata.Rows;
//using ICSharpCode.SharpAssembly.Metadata;
//using ICSharpCode.SharpAssembly.PE;
//using ICSharpCode.SharpAssembly;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Services {
	
	/// <summary>
	/// This class loads an assembly and converts all types from this assembly
	/// to a parser layer Class Collection.
	/// </summary>
	[Serializable]
	public class AssemblyInformation : MarshalByRefObject
	{
		ClassCollection classes = new ClassCollection();
		
		/// <value>
		/// A <code>ClassColection</code> that contains all loaded classes.
		/// </value>
		public ClassCollection Classes {
			get {
				return classes;
			}
		}
		
		public AssemblyInformation()
		{
		}
		
		byte[] GetBytes(string fileName)
		{
			FileStream fs = System.IO.File.OpenRead(fileName);
			long size = fs.Length;
			byte[] outArray = new byte[size];
			fs.Read(outArray, 0, (int)size);
			fs.Close();
			return outArray;
		}
		
		public void Load(string fileName, bool nonLocking)
		{
//			AssemblyReader assembly = new AssemblyReader();
//			assembly.Load(fileName);
//			
//			TypeDef[] typeDefTable = (TypeDef[])assembly.MetadataTable.Tables[TypeDef.TABLE_ID];
//			
//			for (int i = 0; i < typeDefTable.Length; ++i) {
//				Console.WriteLine("ADD " + i);
//				classes.Add(new SharpAssemblyClass(assembly, typeDefTable, i));
//			}
			
			// read xml documentation for the assembly
			XmlDocument doc        = null;
			Hashtable   docuNodes  = new Hashtable();
			string      xmlDocFile = System.IO.Path.ChangeExtension(fileName, ".xml");
			
			string   localizedXmlDocFile = System.IO.Path.GetDirectoryName(fileName) + System.IO.Path.DirectorySeparatorChar +
			                               Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName + System.IO.Path.DirectorySeparatorChar +
								           System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(fileName), ".xml");
			if (System.IO.File.Exists(localizedXmlDocFile)) {
				xmlDocFile = localizedXmlDocFile;
			}
			
			if (System.IO.File.Exists(xmlDocFile)) {
				doc = new XmlDocument();
				doc.Load(xmlDocFile);
				
				// convert the XmlDocument into a hash table
				if (doc.DocumentElement != null && doc.DocumentElement["members"] != null) {
					foreach (XmlNode node in doc.DocumentElement["members"].ChildNodes) {
						if (node != null && node.Attributes != null && node.Attributes["name"] != null) {
							docuNodes[node.Attributes["name"].InnerText] = node;
						}
					}
				}
			}
			
			System.Reflection.Assembly asm = nonLocking ? Assembly.Load(GetBytes(fileName)) : Assembly.LoadFrom(fileName);
			foreach (Type type in asm.GetTypes()) {
				if (!type.FullName.StartsWith("<")) {
					classes.Add(new ReflectionClass(type, docuNodes));
				}
			}
		}
	}
}
