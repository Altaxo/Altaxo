// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Utility;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelop.Internal.Parser;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultParserService : AbstractService, IParserService
	{
		Hashtable classes                = new Hashtable();
		Hashtable caseInsensitiveClasses = new Hashtable();
		
		// used to map 'real' namespace hashtable inside case insensitive hashtable
		const string CaseInsensitiveKey = "__CASE_INSENSITIVE_HASH";
		Hashtable namespaces = new Hashtable();
		Hashtable caseInsensitiveNamespaces = new Hashtable();
		
		Hashtable parsings   = new Hashtable();
		
		ParseInformation addedParseInformation = new ParseInformation();
		ParseInformation removedParseInformation = new ParseInformation();

//// Alex: this one keeps requests for parsing and is used to start parser (pulsed)
//// otherwise continuous reparsing of files is causing leaks
		public static Queue ParserPulse=new Queue();	// required for monitoring when to restart thread		
//// Alex: end of mod

		/// <remarks>
		/// The keys are the assemblies loaded. This hash table ensures that no
		/// assembly is loaded twice. I know that strong naming might be better but
		/// the use case isn't there. No one references 2 differnt files if he references
		/// the same assembly.
		/// </remarks>
		Hashtable loadedAssemblies = new Hashtable();
		
		ClassProxyCollection classProxies = new ClassProxyCollection();
		IParser[] parser;
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic",
			"Microsoft.JScript",
			"mscorlib",
			"System.Data",
			"System.Design",
			"System.DirectoryServices",
			"System.Drawing.Design",
			"System.Drawing",
			"System.EnterpriseServices",
			"System.Management",
			"System.Messaging",
			"System.Runtime.Remoting",
			"System.Runtime.Serialization.Formatters.Soap",

			"System.Security",
			"System.ServiceProcess",
			"System.Web.Services",
			"System.Web",
			"System.Windows.Forms",
			"System",
			"System.XML"
		};
		
		public DefaultParserService()
		{
			addedParseInformation.DirtyCompilationUnit = new DummyCompilationUnit();
			removedParseInformation.DirtyCompilationUnit = new DummyCompilationUnit();
		}
		
		public static string[] AssemblyList {
			get {
				return assemblyList;
			}
		}

		/// <remarks>
		/// The initialize method writes the location of the code completion proxy
		/// file to this string.
		/// </remarks>
		string codeCompletionProxyFile;
		string codeCompletionMainFile;

		class ClasstableEntry
		{
			IClass           myClass;
			ICompilationUnit myCompilationUnit;
			string           myFileName;

			public IClass Class {
				get {
					return myClass;
				}
			}

			public ICompilationUnit CompilationUnit {
				get {
					return myCompilationUnit;
				}
			}

			public string FileName {
				get {
					return myFileName;
				}
			}

			public ClasstableEntry(string fileName, ICompilationUnit compilationUnit, IClass c)
			{
				this.myCompilationUnit = compilationUnit;
				this.myFileName        = fileName;
				this.myClass           = c;
			}
		}
		
		public void GenerateCodeCompletionDatabaseFast(string createPath, IProgressMonitor progressMonitor)
		{
			SetCodeCompletionFileLocation(createPath);

			// write all classes and proxies to the disc
			BinaryWriter classWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			BinaryWriter proxyWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			if (progressMonitor != null) {
				progressMonitor.BeginTask("generate code completion database", assemblyList.Length);
			}
			
			// convert all assemblies
			for (int i = 0; i < assemblyList.Length; ++i) {
				try {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					string path = fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());

					AssemblyInformation frameworkAssemblyInformation = new AssemblyInformation();
					frameworkAssemblyInformation.Load(String.Concat(path, assemblyList[i], ".dll"), false);
					// create all class proxies
					foreach (IClass newClass in frameworkAssemblyInformation.Classes) {
						ClassProxy newProxy = new ClassProxy(newClass);
						classProxies.Add(newProxy);
						AddClassToNamespaceList(newProxy);

						PersistentClass pc = new PersistentClass(classProxies, newClass);
						newProxy.Offset = (uint)classWriter.BaseStream.Position;
						newProxy.WriteTo(proxyWriter);
						pc.WriteTo(classWriter);
					}
					
					if (progressMonitor != null) {
						progressMonitor.Worked(i);
					}
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
				System.GC.Collect();
			}

			classWriter.Close();
			proxyWriter.Close();
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}

		public void GenerateEfficientCodeCompletionDatabase(string createPath, IProgressMonitor progressMonitor)
		{
			SetCodeCompletionFileLocation(createPath);
			AssemblyInformation frameworkAssemblyInformation = new AssemblyInformation();

			if (progressMonitor != null) {
				progressMonitor.BeginTask("generate code completion database", assemblyList.Length * 3);
			}

			// convert all assemblies
			for (int i = 0; i < assemblyList.Length; ++i) {
				try {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					string path = fileUtilityService.GetDirectoryNameWithSeparator(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
					frameworkAssemblyInformation.Load(String.Concat(path, assemblyList[i], ".dll"), false);
					
					if (progressMonitor != null) {
						progressMonitor.Worked(i);
					}
				} catch (Exception e) {
					Console.WriteLine(e.ToString());
				}
				System.GC.Collect();
			}
					
			// create all class proxies
			for (int i = 0; i < frameworkAssemblyInformation.Classes.Count; ++i) {
				ClassProxy newProxy = new ClassProxy(frameworkAssemblyInformation.Classes[i]);
				classProxies.Add(newProxy);
				AddClassToNamespaceList(newProxy);

				if (progressMonitor != null) {
					progressMonitor.Worked(assemblyList.Length + (i * assemblyList.Length) / frameworkAssemblyInformation.Classes.Count);
				}
			}

			// write all classes and proxies to the disc
			BinaryWriter classWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			BinaryWriter proxyWriter = new BinaryWriter(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Create, FileAccess.Write, FileShare.None)));
			
			for (int i  = 0; i < frameworkAssemblyInformation.Classes.Count; ++i) {
				PersistentClass pc = new PersistentClass(classProxies, frameworkAssemblyInformation.Classes[i]);
				ClassProxy proxy = classProxies[i];
				proxy.Offset = (uint)classWriter.BaseStream.Position;
				proxy.WriteTo(proxyWriter);
				pc.WriteTo(classWriter);
				if (progressMonitor != null) {
					progressMonitor.Worked(2 * assemblyList.Length + (i * assemblyList.Length) / frameworkAssemblyInformation.Classes.Count);
				}
			}

			classWriter.Close();
			proxyWriter.Close();
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}

		void SetCodeCompletionFileLocation(string path)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string codeCompletionTemp = fileUtilityService.GetDirectoryNameWithSeparator(path);

			codeCompletionProxyFile = codeCompletionTemp + "CodeCompletionProxyDataV02.bin";
			codeCompletionMainFile  = codeCompletionTemp + "CodeCompletionMainDataV02.bin";
		}

		void SetDefaultCompletionFileLocation()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			SetCodeCompletionFileLocation(propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty).ToString());
		}

		public void LoadProxyDataFile()
		{
			if (!File.Exists(codeCompletionProxyFile)) {
				return;
			}
			BinaryReader reader = new BinaryReader(new BufferedStream(new FileStream(codeCompletionProxyFile, FileMode.Open, FileAccess.Read, FileShare.Read)));
			while (true) {
				try {
					ClassProxy newProxy = new ClassProxy(reader);
					classProxies.Add(newProxy);
					AddClassToNamespaceList(newProxy);
				} catch (Exception) {
					break;
				}
			}
			reader.Close();
		}
		
		void LoadThread()
		{
			SetDefaultCompletionFileLocation();
			
			BinaryFormatter formatter = new BinaryFormatter();
			
			if (File.Exists(codeCompletionProxyFile)) {
				LoadProxyDataFile();
			}
		}
		
		public override void InitializeService()
		{
			parser = (IParser[])(AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Parser").BuildChildItems(this)).ToArray(typeof(IParser));
			
			Thread myThread = new Thread(new ThreadStart(LoadThread));
			myThread.IsBackground = true;
			myThread.Priority = ThreadPriority.Lowest;
			myThread.Start();
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.CombineOpened += new CombineEventHandler(OpenCombine);
		}
		
		public void AddReferenceToCompletionLookup(IProject project, ProjectReference reference)
		{
			if (reference.ReferenceType != ReferenceType.Project) {
				string fileName = reference.GetReferencedFileName(project);
				if (fileName == null || fileName.Length == 0) {
					return;
				}
				foreach (string assemblyName in assemblyList) {
					if (Path.GetFileNameWithoutExtension(fileName).ToUpper() == assemblyName.ToUpper()) {
						return;
					}
				}
				// HACK : Don't load references for non C# projects
				if (project.ProjectType != "C#") {
					return;
				}
				if (File.Exists(fileName)) {
					Thread t = new Thread(new ThreadStart(new AssemblyLoader(this, fileName).LoadAssemblyParseInformations));
					t.Start();
				}
			}
		}
		
		class AssemblyLoader
		{
			DefaultParserService parserService;
			string assemblyFileName;
			
			public AssemblyLoader(DefaultParserService parserService, string assemblyFileName)
			{
				this.parserService    = parserService;
				this.assemblyFileName = assemblyFileName;
			}
			
			public void LoadAssemblyParseInformations()
			{
				if (parserService.loadedAssemblies[assemblyFileName] != null) {
					return;
				}
				parserService.loadedAssemblies[assemblyFileName] = true;
				try {
					AssemblyInformation assemblyInformation = new AssemblyInformation();
					assemblyInformation.Load(assemblyFileName, true);
					foreach (IClass newClass in assemblyInformation.Classes) {
						parserService.AddClassToNamespaceList(newClass);
						lock (parserService.classes) {
							parserService.caseInsensitiveClasses[newClass.FullyQualifiedName.ToLower()] = parserService.classes[newClass.FullyQualifiedName] = new ClasstableEntry(null, null, newClass);
						}
					}
				} catch (Exception e) {
					Console.WriteLine("Can't add reference : " + e.ToString());
				}
			}
		
		}

		
		public void OpenCombine(object sender, CombineEventArgs e)
		{
			ArrayList projects =  Combine.GetAllProjects(e.Combine);
			foreach (ProjectCombineEntry entry in projects) {
				foreach (ProjectReference r in entry.Project.ProjectReferences) {
					AddReferenceToCompletionLookup(entry.Project, r);
				}
			}
		}
		
		public void StartParserThread()
		{
			Thread t = new Thread(new ThreadStart(ParserUpdateThread));
			t.IsBackground  = true;
			t.Priority  = ThreadPriority.Lowest;
			t.Start();
		}
#if ModifiedForAltaxo
    object _activeModalContent;

    /// <summary>
    /// Registers a parseable content contained into a modal dialog box. This function must be
    /// called immediately before the call to form.ShowDialog(..).
    /// </summary>
    /// <param name="content">The content of the dialog box. Must be IParseable and IEditable.</param>
    public void RegisterModalContent(object content)
    {
      _activeModalContent = content; 
    }

    /// <summary>
    /// Unregisters the parseable content of a modal dialog box. Must be immediatly called after return
    /// from form.ShowDialog().
    /// </summary>
    public void UnregisterModalContent()
    {
      _activeModalContent = null; 
    }
#endif

		void ParserUpdateThread()
		{
//// Alex: file name to parse holder field
			string fn=null;
			while (true) {
				Thread.Sleep(1000); // not required
//// Alex: if some file was pulsed - during editor load and after - get file to reparse
				fn = null; // set to null for each repetition

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//	Mike: Doesn't work with folding marker update --> look at the folding markers
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

//				lock(DefaultParserService.ParserPulse) {
//					Monitor.Wait(DefaultParserService.ParserPulse);
//					if (DefaultParserService.ParserPulse.Count>0) {
//						fn = (string)DefaultParserService.ParserPulse.Dequeue();
//					}
//				}
////// Alex: end of mod
				try {
#if ModifiedForAltaxo
          if(_activeModalContent!=null)
          {
            IEditable editable = _activeModalContent as IEditable;
            if (editable != null) 
            {
              string fileName = null;
							
              IParseableContent parseableContent = _activeModalContent as IParseableContent;
              if (parseableContent != null) 
              {
                fileName = parseableContent.ParseableContentName;
              } 
              else if(_activeModalContent is ICSharpCode.SharpDevelop.Gui.IViewContent)
              {
                fileName = ((ICSharpCode.SharpDevelop.Gui.IViewContent)_activeModalContent).ContentName;
              }
              if (!(fileName == null || fileName.Length == 0)) 
              {
                Thread.Sleep(300);
                IParseInformation parseInformation = null;
                lock (parsings) 
                {
                  parseInformation = ParseFile(fileName, editable.Text);
                }
                if (parseInformation != null && editable is IParseInformationListener) 
                {
                  ((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
                }
              }
            }

            continue;
          }
#endif
          if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null && WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent != null) {
						IEditable editable = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as IEditable;
						if (editable != null) {
							string fileName = null;
							
							IParseableContent parseableContent = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as IParseableContent;
							
							if (parseableContent != null) {
								fileName = parseableContent.ParseableContentName;
							} else {
								fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
							}
							if (!(fileName == null || fileName.Length == 0)) {
								Thread.Sleep(300); // not required 
								IParseInformation parseInformation = null;
								lock (parsings) {
									parseInformation = ParseFile(fileName, editable.Text);
								}
								if (parseInformation != null && editable is IParseInformationListener) {
									((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
								}
//// Alex: reparsing was requested for file in fn
								if (fn != null) {
									ParseFile(fn); // TODO: this one should update file parsings requested through queue
								}
							}
						}
					}
				} catch (Exception e) {
					try {
						Console.WriteLine(e.ToString());
					} catch {}
				}
				Thread.Sleep(500); // not required
			}
		}
		
		Hashtable AddClassToNamespaceList(IClass addClass)
		{
			string nSpace = addClass.Namespace;
			if (nSpace == null) {
				nSpace = String.Empty;
			}
			
			string[] path = nSpace.Split('.');
			
			lock (namespaces) {
				Hashtable cur                = namespaces;
				Hashtable caseInsensitiveCur = caseInsensitiveNamespaces;
				
				for (int i = 0; i < path.Length; ++i) {
					if (cur[path[i]] == null) {
						Hashtable hashTable                = new Hashtable();
						Hashtable caseInsensitivehashTable = new Hashtable();
						cur[path[i]] = hashTable;
						caseInsensitiveCur[path[i].ToLower()] = caseInsensitivehashTable;
						caseInsensitivehashTable[CaseInsensitiveKey] = hashTable;
					} else {
						if (!(cur[path[i]] is Hashtable)) {
							return null;
						}
					}
					cur = (Hashtable)cur[path[i]];
					caseInsensitiveCur = (Hashtable)caseInsensitiveCur[path[i].ToLower()];
				}
				caseInsensitiveCur[addClass.Name.ToLower()] = cur[addClass.Name] = new ClassProxy(addClass);
				return cur;
			}
		}
		
#region Default Parser Layer dependent functions
		public IClass GetClass(string typeName)
		{
			return GetClass(typeName, true);
		}
		public IClass GetClass(string typeName, bool caseSensitive)
		{
//			Console.WriteLine("Get class >{0}<", typeName);
			if (!caseSensitive) {
				typeName = typeName.ToLower();
			}
			
			ClasstableEntry entry = (caseSensitive ? classes[typeName] : caseInsensitiveClasses[typeName]) as ClasstableEntry;
			if (entry != null) {
				return entry.Class;
			}
			
			// try to load the class from our data file
			int idx = classProxies.IndexOf(typeName, caseSensitive);
			if (idx > 0) {
				BinaryReader reader = new BinaryReader(new BufferedStream(new FileStream(codeCompletionMainFile, FileMode.Open, FileAccess.Read, FileShare.Read)));
				reader.BaseStream.Seek(classProxies[idx].Offset, SeekOrigin.Begin);
				IClass c = new PersistentClass(reader, classProxies);
				reader.Close();
				lock (classes) {
					caseInsensitiveClasses[typeName.ToLower()] = classes[typeName] = new ClasstableEntry(null, null, c);
				}
				return c;
			}
			return null;
		}
		
		public string[] GetNamespaceList(string subNameSpace)
		{
			return GetNamespaceList(subNameSpace, true);
		}
		public string[] GetNamespaceList(string subNameSpace, bool caseSensitive)
		{
//			Console.WriteLine("GetNamespaceList >{0}<", subNameSpace);
			
			Debug.Assert(subNameSpace != null);
			if (!caseSensitive) {
				subNameSpace = subNameSpace.ToLower();
			}
			
			string[] path = subNameSpace.Split('.');
			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
			
			if (subNameSpace.Length > 0) {
				for (int i = 0; i < path.Length; ++i) {
					if (!(cur[path[i]] is Hashtable)) {
						return null;
					}
					cur = (Hashtable)cur[path[i]];
				}
			}
			
			if (!caseSensitive) {
				cur = (Hashtable)cur[CaseInsensitiveKey];
			}
			
			ArrayList namespaceList = new ArrayList();
			foreach (DictionaryEntry entry in cur) {
				if (entry.Value is Hashtable && entry.Key.ToString().Length > 0) {
					namespaceList.Add(entry.Key);
				}
			}
			
			return (string[])namespaceList.ToArray(typeof(string));
		}
		
		public ArrayList GetNamespaceContents(string subNameSpace)
		{
			return GetNamespaceContents(subNameSpace, true);
		}
		public ArrayList GetNamespaceContents(string subNameSpace, bool caseSensitive)
		{
//			Console.WriteLine("GetNamespaceContents >{0}<", subNameSpace);
			
			ArrayList namespaceList = new ArrayList();
			if (subNameSpace == null) {
				return namespaceList;
			}
			if (!caseSensitive) {
				subNameSpace = subNameSpace.ToLower();
			}
			
			string[] path = subNameSpace.Split('.');
			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					return namespaceList;
				}
				cur = (Hashtable)cur[path[i]];
			}
			
			if (!caseSensitive) {
				cur = (Hashtable)cur[CaseInsensitiveKey];
			}
			
			foreach (DictionaryEntry entry in cur)  {
				if (entry.Value is Hashtable) {
					namespaceList.Add(entry.Key);
				} else {
					namespaceList.Add(entry.Value);
				}
			}
			return namespaceList;
		}
		
		public bool NamespaceExists(string name)
		{
			return NamespaceExists(name, true);
		}
		public bool NamespaceExists(string name, bool caseSensitive)
		{
//			Console.WriteLine("NamespaceExists >{0}<", name);
			if (name == null) {
				return false;
			}
			if (!caseSensitive) {
				name = name.ToLower();
			}
			string[] path = name.Split('.');
			Hashtable cur = caseSensitive ? namespaces : caseInsensitiveNamespaces;
			
			for (int i = 0; i < path.Length; ++i) {
				if (!(cur[path[i]] is Hashtable)) {
					return false;
				}
				cur = (Hashtable)cur[path[i]];
			}
			return true;
		}
#endregion
		
		public IParseInformation ParseFile(string fileName)
		{
			return ParseFile(fileName, null);
		}

		public IParseInformation ParseFile(string fileName, string fileContent)
		{
			IParser parser = GetParser(fileName);
			
			if (parser == null) {
				return null;
			}
			
			parser.LexerTags = new string[] { "HACK", "TODO", "UNDONE", "FIXME" };
			
			ICompilationUnitBase parserOutput = null;
			
			if (fileContent == null) {
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				if (projectService.CurrentOpenCombine != null) {
					ArrayList projects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
					foreach (ProjectCombineEntry entry in projects) {
						if (entry.Project.IsFileInProject(fileName)) {
							fileContent = entry.Project.GetParseableFileContent(fileName);
						}
					}
				}
			}
			
			if (fileContent != null) {
				parserOutput = parser.Parse(fileName, fileContent);
			} else {
				parserOutput = parser.Parse(fileName);
			}
			
			ParseInformation parseInformation = parsings[fileName] as ParseInformation;
			
			int itemsAdded = 0;
			int itemsRemoved = 0;
			
			if (parseInformation == null) {
				parseInformation = new ParseInformation();
			} else {
				itemsAdded = GetAddedItems(
				                           (ICompilationUnit)parseInformation.MostRecentCompilationUnit,
				                           (ICompilationUnit)parserOutput,
				                           (ICompilationUnit)addedParseInformation.DirtyCompilationUnit
				                           );
				
				itemsRemoved = GetRemovedItems(
				                               (ICompilationUnit)parseInformation.MostRecentCompilationUnit,
				                               (ICompilationUnit)parserOutput,
				                               (ICompilationUnit)removedParseInformation.DirtyCompilationUnit
				                               );
			}
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			parsings[fileName] = parseInformation;
									
			if (parseInformation.BestCompilationUnit is ICompilationUnit) {
				ICompilationUnit cu = (ICompilationUnit)parseInformation.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					AddClassToNamespaceList(c);
					lock (classes) {
						caseInsensitiveClasses[c.FullyQualifiedName.ToLower()] = classes[c.FullyQualifiedName] = new ClasstableEntry(fileName, cu, c);
					}
				}
			} else {
				Console.WriteLine("SKIP!");
			}
			
			if(itemsAdded > 0) {
				OnParseInformationAdded(new ParseInformationEventArgs(fileName, addedParseInformation));
			}
			if(itemsRemoved > 0) {
				OnParseInformationRemoved(new ParseInformationEventArgs(fileName, removedParseInformation));
			}
			return parseInformation;
		}
		
		void RemoveClasses(ICompilationUnit cu)
		{
			if (cu != null) {
				foreach (IClass c in cu.Classes) {
					lock (classes) {
						classes.Remove(c.FullyQualifiedName);
						caseInsensitiveClasses.Remove(c.FullyQualifiedName.ToLower());
					}
				}
			}
		}

		public IParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			object cu = parsings[fileName];
			if (cu == null) {
				return ParseFile(fileName);
			}
			return (IParseInformation)cu;
		}
		
		public virtual IParser GetParser(string fileName)
		{
			// HACK: I'm too lazy to do it 'right'
			if (Path.GetExtension(fileName).ToUpper() == ".CS") {
				return parser[0];
			}
			if (Path.GetExtension(fileName).ToUpper() == ".VB") {
				return parser[1];
			}
			return null;
		}
		
		int GetAddedItems(ICompilationUnit original, ICompilationUnit changed, ICompilationUnit result)
		{
			int count = 0;
			//result.LookUpTable.Clear();
			//result.Usings.Clear();
			//result.Attributes.Clear();
			result.Classes.Clear();
			//result.MiscComments.Clear();
			//result.DokuComments.Clear();
			//result.TagComments.Clear();
			
			//count += DiffUtility.GetAddedItems(original.LookUpTable,  changed.LookUpTable,  result.LookUpTable);
			//count += DiffUtility.GetAddedItems(original.Usings,       changed.Usings,       result.Usings);
			//count += DiffUtility.GetAddedItems(original.Attributes,   changed.Attributes,   result.Attributes);
			count += DiffUtility.GetAddedItems(original.Classes,      changed.Classes,      result.Classes);
			//count += DiffUtility.GetAddedItems(original.MiscComments, changed.MiscComments, result.MiscComments);
			//count += DiffUtility.GetAddedItems(original.DokuComments, changed.DokuComments, result.DokuComments);
			//count += DiffUtility.GetAddedItems(original.TagComments,  changed.TagComments,  result.TagComments);
			return count;
		}
		
		int GetRemovedItems(ICompilationUnit original, ICompilationUnit changed, ICompilationUnit result) {
			return GetAddedItems(changed, original, result);
		}
		
		////////////////////////////////////
		
		public ResolveResult Resolve(string expression,
		                             int caretLineNumber,
		                             int caretColumn,
		                             string fileName,
		                             string fileContent)
		{
			// added exception handling here to prevent silly parser exceptions from
			// being thrown and corrupting the textarea control
			//try {
				IParser parser = GetParser(fileName);
				Console.WriteLine("Parse info : " + GetParseInformation(fileName).MostRecentCompilationUnit.Tag);
				if (parser != null) {
					return parser.Resolve(this, expression, caretLineNumber, caretColumn, fileName, fileContent);
				}
				return null;
			//} catch {
//				return null;
			//}
		}

		protected void OnParseInformationAdded(ParseInformationEventArgs e)
		{
			if (ParseInformationAdded != null) {
				ParseInformationAdded(this, e);
			}
		}

		protected void OnParseInformationRemoved(ParseInformationEventArgs e)
		{
			if (ParseInformationRemoved != null) {
				ParseInformationRemoved(this, e);
			}
		}
		protected virtual void OnParseInformationChanged(ParseInformationEventArgs e)
		{
			if (ParseInformationChanged != null) {
				ParseInformationChanged(this, e);
			}
		}
		
		public event ParseInformationEventHandler ParseInformationAdded;
		public event ParseInformationEventHandler ParseInformationRemoved;
		public event ParseInformationEventHandler ParseInformationChanged;
	}
	
	[Serializable]
	public class DummyCompilationUnit : AbstractCompilationUnit
	{
		CommentCollection miscComments = new CommentCollection();
		CommentCollection dokuComments = new CommentCollection();
		TagCollection     tagComments  = new TagCollection();
		
		public override CommentCollection MiscComments {
			get {
				return miscComments;
			}
		}
		
		public override CommentCollection DokuComments {
			get {
				return dokuComments;
			}
		}
		
		public override TagCollection TagComments {
			get {
				return tagComments;
			}
		}
	}
}
