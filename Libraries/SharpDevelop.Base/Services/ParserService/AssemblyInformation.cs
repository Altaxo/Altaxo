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
		
		// i really hate code duplication, see AbstractProject.cs
		// After .NET 2.0 we need a clean, application domain based assembly loading mechanism!!!
		byte[] GetBytes(string fileName)
		{
			FileStream fs = System.IO.File.OpenRead(fileName);
			long size = fs.Length;
			byte[] outArray = new byte[size];
			fs.Read(outArray, 0, (int)size);
			fs.Close();
			return outArray;
		}
		
		string loadingPath = String.Empty;
		Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			string file = args.Name;
			int idx = file.IndexOf(',');
			if (idx >= 0) {
				file = file.Substring(0, idx);
			}
			try {
				if (File.Exists(loadingPath + file + ".exe")) {
					return Assembly.Load(GetBytes(loadingPath + file + ".exe"));
				} 
				if (File.Exists(loadingPath + file + ".dll")) {
					return Assembly.Load(GetBytes(loadingPath + file + ".dll"));
				} 
			} catch (Exception ex) {
				Console.WriteLine("Can't load assembly : " + ex.ToString());
			}
			return null;
		}
		
		
		public void Load(string fileName, bool nonLocking)
		{
			try {
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
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
				loadingPath = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
#if !ModifiedForAltaxo			
				System.Reflection.Assembly asm = nonLocking ? Assembly.Load(GetBytes(fileName)) : Assembly.LoadFrom(fileName);
#else
      System.Reflection.Assembly asm=null;
			
      System.Reflection.Assembly[] alreadyLoadedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly alreadyLoadedAssembly in alreadyLoadedAssemblies)
      {
        // quest for AssemblyBuilder since call to Location with an AssemblyBuilder results in an exception
        if(!(alreadyLoadedAssembly is System.Reflection.Emit.AssemblyBuilder) && (alreadyLoadedAssembly.Location == fileName))
        {
          asm = alreadyLoadedAssembly;
          break;
        }
      }

      if(asm==null)
      {
        asm = nonLocking ? Assembly.Load(GetBytes(fileName)) : Assembly.LoadFrom(fileName);
      }
#endif
				foreach (Type type in asm.GetTypes()) {
					if (!type.FullName.StartsWith("<") && type.IsPublic) {
						classes.Add(new ReflectionClass(type, docuNodes));
					}
				}
			} finally {
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(MyResolveEventHandler);
			}
		}
	}
}
