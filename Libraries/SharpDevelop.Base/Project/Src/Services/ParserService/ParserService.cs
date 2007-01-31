// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2151 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public static class ParserService
	{
		static IList<ParserDescriptor> parser;
		static IList<ProjectContentRegistryDescriptor> registries;
#if ModifiedForAltaxo
    // neccessary to support scripts, which are usually displayed in dialog windows
    static object _activeModalContent;

    /// <summary>
    /// Registers a parseable content contained into a modal dialog box. This function must be
    /// called immediately before the call to form.ShowDialog(..).
    /// </summary>
    /// <param name="content">The content of the dialog box. Must be IParseable and IEditable.</param>
    public static void RegisterModalContent(object content)
    {
      _activeModalContent = content;
    }

    /// <summary>
    /// Unregisters the parseable content of a modal dialog box. Must be immediatly called after return
    /// from form.ShowDialog().
    /// </summary>
    public static void UnregisterModalContent()
    {
      _activeModalContent = null;
    }
#endif
		
		static Dictionary<IProject, IProjectContent> projectContents = new Dictionary<IProject, IProjectContent>();
		static Dictionary<string, ParseInformation> parsings = new Dictionary<string, ParseInformation>();
		static ProjectContentRegistry defaultProjectContentRegistry = new ProjectContentRegistry();
		
		static string domPersistencePath;
		
		internal static void InitializeParserService()
		{
			if (parser == null) {
				parser = AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false);
				registries = AddInTree.BuildItems<ProjectContentRegistryDescriptor>("/Workspace/ProjectContentRegistry", null, false);
				
				domPersistencePath = Path.Combine(Path.GetTempPath(), "SharpDevelop" + RevisionClass.MainVersion);
				#if DEBUG
				domPersistencePath = Path.Combine(domPersistencePath, "Debug");
				#endif
				Directory.CreateDirectory(domPersistencePath);
				defaultProjectContentRegistry.ActivatePersistence(domPersistencePath);
				ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			}
		}
		
		/// <summary>
		/// Gets the cache directory used for DOM persistence.
		/// </summary>
		public static string DomPersistencePath {
			get {
				return domPersistencePath;
			}
		}
		
		public static ProjectContentRegistry DefaultProjectContentRegistry {
			get {
				return defaultProjectContentRegistry;
			}
		}
		
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				if (ProjectService.CurrentProject == null || !projectContents.ContainsKey(ProjectService.CurrentProject)) {
					return DefaultProjectContent;
				}
				return projectContents[ProjectService.CurrentProject];
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				return projectContents.Values;
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects plus the referenced project contents.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContentsWithReferences {
			get {
				return Linq.Distinct(Linq.Concat(AllProjectContents, defaultProjectContentRegistry.GetLoadedProjectContents()));
			}
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			abortLoadSolutionProjectsThread = true;
			
			lock (reParse1) { // clear queue of reparse thread
				reParse1.Clear();
				reParse2.Clear();
			}
			lock (projectContents) {
				foreach (IProjectContent content in projectContents.Values) {
					content.Dispose();
				}
				projectContents.Clear();
				parsings.Clear();
			}
			lock (parseQueue) {
				parseQueue.Clear();
			}
			lastUpdateHash.Clear();
		}
		
		static Thread loadSolutionProjectsThread;
		static bool   abortLoadSolutionProjectsThread;
		
		// do not use an event for this because a solution might be loaded before ParserService
		// is initialized
		internal static void OnSolutionLoaded()
		{
			if (loadSolutionProjectsThread != null) {
				if (!abortLoadSolutionProjectsThread)
					throw new InvalidOperationException("Cannot open new combine without closing old combine!");
				if (!loadSolutionProjectsThread.Join(50)) {
					// loadSolutionProjects might be waiting for main thread, so give it
					// a chance to complete safethread calls by putting this method at
					// the end of the invoking queue
					WorkbenchSingleton.SafeThreadAsyncCall(OnSolutionLoaded);
					return;
				}
			}
			loadSolutionProjectsThread = new Thread(new ThreadStart(LoadSolutionProjects));
			loadSolutionProjectsThread.Name = "loadSolutionProjects";
			loadSolutionProjectsThread.Priority = ThreadPriority.BelowNormal;
			loadSolutionProjectsThread.IsBackground = true;
			loadSolutionProjectsThread.Start();
		}
		
		public static bool LoadSolutionProjectsThreadRunning {
			get {
				return loadSolutionProjectsThread != null;
			}
		}
		
		static void LoadSolutionProjects()
		{
			try {
				abortLoadSolutionProjectsThread = false;
				LoggingService.Info("Start LoadSolutionProjects thread");
				LoadSolutionProjectsInternal();
			} finally {
				LoggingService.Info("LoadSolutionProjects thread ended");
				loadSolutionProjectsThread = null;
				OnLoadSolutionProjectsThreadEnded(EventArgs.Empty);
			}
		}
		
		static void LoadSolutionProjectsInternal()
		{
			List<ParseProjectContent> createdContents = new List<ParseProjectContent>();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				try {
					ParseProjectContent newContent = project.CreateProjectContent();
					if (newContent != null) {
						lock (projectContents) {
							projectContents[project] = newContent;
						}
						createdContents.Add(newContent);
					}
				} catch (Exception e) {
					MessageService.ShowError(e, "Error while retrieving project contents from " + project);
				}
			}
			WorkbenchSingleton.SafeThreadAsyncCall(ProjectService.ParserServiceCreatedProjectContents);
			int workAmount = 0;
			foreach (ParseProjectContent newContent in createdContents) {
				if (abortLoadSolutionProjectsThread) return;
				try {
					newContent.Initialize1();
					workAmount += newContent.GetInitializationWorkAmount();
				} catch (Exception e) {
					MessageService.ShowError(e, "Error while initializing project references:" + newContent);
				}
			}
			StatusBarService.ProgressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", workAmount, false);
			foreach (ParseProjectContent newContent in createdContents) {
				if (abortLoadSolutionProjectsThread) break;
				try {
					newContent.Initialize2();
				} catch (Exception e) {
					MessageService.ShowError(e, "Error while initializing project contents:" + newContent);
				}
			}
			StatusBarService.ProgressMonitor.Done();
		}
		
		static void InitAddedProject(object state)
		{
			ParseProjectContent newContent = (ParseProjectContent)state;
			newContent.Initialize1();
			StatusBarService.ProgressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", newContent.GetInitializationWorkAmount(), false);
			newContent.Initialize2();
			StatusBarService.ProgressMonitor.Done();
		}
		
		#region Reparse projects
		// queue of projects waiting to reparse references
		static Queue<ParseProjectContent> reParse1 = new Queue<ParseProjectContent>();
		
		// queue of projects waiting to reparse code
		static Queue<ParseProjectContent> reParse2 = new Queue<ParseProjectContent>();
		static Thread reParseThread;
		
		static void ReparseProjects()
		{
			LoggingService.Info("reParse thread started");
			Thread.Sleep(100); // enable main thread to fill the queues completely
			try {
				ReparseProjectsInternal();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		static void ReparseProjectsInternal()
		{
			bool parsing = false;
			ParseProjectContent job;
			
			while (true) {
				// get next job
				lock (reParse1) {
					if (reParse1.Count > 0) {
						if (parsing) {
							StatusBarService.ProgressMonitor.Done();
						}
						parsing = false;
						job = reParse1.Dequeue();
					} else if (reParse2.Count > 0) {
						if (!parsing) {
							int workAmount = 0;
							foreach (ParseProjectContent ppc in reParse2) {
								workAmount += ppc.GetInitializationWorkAmount();
							}
							StatusBarService.ProgressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", workAmount, false);
						}
						parsing = true;
						job = reParse2.Dequeue();
					} else {
						// all jobs done
						reParseThread = null;
						if (parsing) {
							StatusBarService.ProgressMonitor.Done();
						}
						LoggingService.Info("reParse thread finished all jobs");
						return;
					}
				}
				
				// execute job
				if (parsing) {
					LoggingService.Info("reparsing code for " + job.Project);
					job.ReInitialize2();
				} else {
					LoggingService.Debug("reloading references for " + job.Project);
					job.ReInitialize1();
				}
			}
		}
		
		public static void Reparse(IProject project, bool initReferences, bool parseCode)
		{
			ParseProjectContent pc = GetProjectContent(project) as ParseProjectContent;
			if (pc != null) {
				lock (reParse1) {
					if (initReferences && !reParse1.Contains(pc)) {
						LoggingService.Debug("Enqueue for reinitializing references: " + project);
						reParse1.Enqueue(pc);
					}
					if (parseCode && !reParse2.Contains(pc)) {
						LoggingService.Debug("Enqueue for reparsing code: " + project);
						reParse2.Enqueue(pc);
					}
					if (reParseThread == null) {
						LoggingService.Info("Starting reParse thread");
						reParseThread = new Thread(new ThreadStart(ReparseProjects));
						reParseThread.Name = "reParse";
						reParseThread.Priority = ThreadPriority.BelowNormal;
						reParseThread.IsBackground = true;
						reParseThread.Start();
					}
				}
			}
		}
		#endregion
		
		/// <remarks>Can return null.</remarks>
		internal static IProjectContent CreateProjectContentForAddedProject(IProject project)
		{
			lock (projectContents) {
				ParseProjectContent newContent = project.CreateProjectContent();
				if (newContent != null) {
					projectContents[project] = newContent;
					ThreadPool.QueueUserWorkItem(InitAddedProject, newContent);
				}
				return newContent;
			}
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			lock (projectContents) {
				if (projectContents.ContainsKey(project)) {
					return projectContents[project];
				}
			}
			return null;
		}
		
		static Queue<KeyValuePair<string, string>> parseQueue = new Queue<KeyValuePair<string, string>>();
		
		static void ParseQueue()
		{
			while (true) {
				KeyValuePair<string, string> entry;
				lock (parseQueue) {
					if (parseQueue.Count == 0)
						return;
					entry = parseQueue.Dequeue();
				}
				ParseFile(entry.Key, entry.Value);
			}
		}
		
		public static void EnqueueForParsing(string fileName)
		{
			EnqueueForParsing(fileName, GetParseableFileContent(fileName));
		}
		
		public static void EnqueueForParsing(string fileName, string fileContent)
		{
			lock (parseQueue) {
				parseQueue.Enqueue(new KeyValuePair<string, string>(fileName, fileContent));
			}
		}
		
		public static void StartParserThread()
		{
			abortParserUpdateThread = false;
			Thread parserThread = new Thread(new ThreadStart(ParserUpdateThread));
			parserThread.Name = "parser";
			parserThread.Priority = ThreadPriority.BelowNormal;
			parserThread.IsBackground  = true;
			parserThread.Start();
		}
		
		public static void StopParserThread()
		{
			abortParserUpdateThread = true;
		}
		
		static volatile bool abortParserUpdateThread = false;
		
		static Dictionary<string, int> lastUpdateHash = new Dictionary<string, int>();
		
		static void ParserUpdateThread()
		{
			LoggingService.Info("ParserUpdateThread started");
			Thread.Sleep(750);
			
			// preload mscorlib, we're going to need it probably
			IProjectContent dummyVar = defaultProjectContentRegistry.Mscorlib;
			
			while (!abortParserUpdateThread) {
				try {
					ParseQueue();
					ParserUpdateStep();
				} catch (Exception e) {
					ICSharpCode.Core.MessageService.ShowError(e);
					
					// don't fire an exception every 2 seconds at the user, give him at least
					// time to read the first :-)
					Thread.Sleep(10000);
				}
				Thread.Sleep(2000);
			}
			LoggingService.Info("ParserUpdateThread stopped");
		}
		
		static object[] GetWorkbench()
		{
			IWorkbenchWindow activeWorkbenchWindow = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (activeWorkbenchWindow == null)
				return null;
			IBaseViewContent activeViewContent = activeWorkbenchWindow.ActiveViewContent;
			if (activeViewContent == null)
				return null;
			return new object[] { activeViewContent, activeWorkbenchWindow.ViewContent };
		}
		
		public static void ParseCurrentViewContent()
		{
			ParserUpdateStep();
		}
		
		static void ParserUpdateStep()
		{
			object[] workbench;
			try {
				workbench = WorkbenchSingleton.SafeThreadFunction<object[]>(GetWorkbench);
			} catch (InvalidOperationException) { // includes ObjectDisposedException
				// maybe workbench has been disposed while waiting for the SafeThreadCall
				// can occur after workbench unload or after aborting SharpDevelop with
				// Application.Exit()
				LoggingService.Warn("InvalidOperationException while trying to invoke GetWorkbench()");
				return; // abort this thread
			}
#if ModifiedForAltaxo
      if (_activeModalContent != null)
      {
        IEditable editable = _activeModalContent as IEditable;
        if (editable != null)
        {
          string fileName = null;

          IParseableContent parseableContent = _activeModalContent as IParseableContent;

          //ivoko: Pls, do not throw text = parseableContent.ParseableText away. I NEED it.
          string text = null;
          if (parseableContent != null)
          {
            fileName = parseableContent.ParseableContentName;
            text = parseableContent.ParseableText;
          }
          else if (_activeModalContent is ICSharpCode.SharpDevelop.Gui.IViewContent)
          {
            fileName = ((ICSharpCode.SharpDevelop.Gui.IViewContent)_activeModalContent).FileName;
          }
          if (!(fileName == null || fileName.Length == 0))
          {
            ParseInformation parseInformation = null;
            bool updated = false;
            if (text == null)
            {
              text = editable.Text;
              if (text == null) return;
            }
            int hash = text.GetHashCode();
            if (!lastUpdateHash.ContainsKey(fileName) || lastUpdateHash[fileName] != hash)
            {
              parseInformation = ParseFile(fileName, text, true);
              lastUpdateHash[fileName] = hash;
              updated = true;
            }
            if (updated)
            {
              if (parseInformation != null && editable is IParseInformationListener)
              {
                ((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
              }
            }
            OnParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, text, updated, parseInformation));
          }
        }

        return;
      }
#endif
			if (workbench != null) {
				IEditable editable = workbench[0] as IEditable;
				if (editable != null) {
					string fileName = null;
					
					IViewContent viewContent = (IViewContent)workbench[1];
					IParseableContent parseableContent = workbench[0] as IParseableContent;
					
					//ivoko: Pls, do not throw text = parseableContent.ParseableText away. I NEED it.
					string text = null;
					if (parseableContent != null) {
						fileName = parseableContent.ParseableContentName;
						text = parseableContent.ParseableText;
					} else {
						fileName = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
					}
					
					if (!(fileName == null || fileName.Length == 0)) {
						ParseInformation parseInformation = null;
						bool updated = false;
						if (text == null) {
							text = editable.Text;
							if (text == null) return;
						}
						int hash = text.GetHashCode();
						if (!lastUpdateHash.ContainsKey(fileName) || lastUpdateHash[fileName] != hash) {
							parseInformation = ParseFile(fileName, text, !viewContent.IsUntitled);
							lastUpdateHash[fileName] = hash;
							updated = true;
						}
						if (updated) {
							if (parseInformation != null && editable is IParseInformationListener) {
								((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
							}
						}
						OnParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, text, updated, parseInformation));
					}
				}
			}
		}
		
		public static void ParseViewContent(IViewContent viewContent)
		{
			string text = ((IEditable)viewContent).Text;
			ParseInformation parseInformation = ParseFile(viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName,
			                                              text, !viewContent.IsUntitled);
			if (parseInformation != null && viewContent is IParseInformationListener) {
				((IParseInformationListener)viewContent).ParseInformationUpdated(parseInformation);
			}
		}
		
		/// <summary>
		/// <para>This event is called every two seconds. It is called directly after the parser has updated the
		/// project content and it is called after the parser noticed that there is nothing to update.</para>
		/// <para><b>WARNING: This event is called on the parser thread - You need to use Invoke if you do
		/// anything in your event handler that could touch the GUI.</b></para>
		/// </summary>
		public static event ParserUpdateStepEventHandler ParserUpdateStepFinished;
		
		static void OnParserUpdateStepFinished(ParserUpdateStepEventArgs e)
		{
			if (ParserUpdateStepFinished != null) {
				ParserUpdateStepFinished(typeof(ParserService), e);
			}
		}
		
		public static ParseInformation ParseFile(string fileName)
		{
			return ParseFile(fileName, null);
		}
		
		public static ParseInformation ParseFile(string fileName, string fileContent)
		{
			return ParseFile(fileName, fileContent, true);
		}
		
		static IProjectContent GetProjectContent(string fileName)
		{
			lock (projectContents) {
				foreach (KeyValuePair<IProject, IProjectContent> projectContent in projectContents) {
					if (projectContent.Key.IsFileInProject(fileName)) {
						return projectContent.Value;
					}
				}
			}
			return null;
		}
		
		static DefaultProjectContent defaultProjectContent;
		
		public static IProjectContent DefaultProjectContent {
			get {
				if (defaultProjectContent == null) {
					lock (projectContents) {
						if (defaultProjectContent == null) {
							CreateDefaultProjectContent();
						}
					}
				}
				return defaultProjectContent;
			}
		}
		
		static void CreateDefaultProjectContent()
		{
			LoggingService.Info("Creating default project content");
			//LoggingService.Debug("Stacktrace is:\n" + Environment.StackTrace);
			defaultProjectContent = new DefaultProjectContent();
			defaultProjectContent.AddReferencedContent(defaultProjectContentRegistry.Mscorlib);
			Thread t = new Thread(new ThreadStart(CreateDefaultProjectContentReferences));
			t.IsBackground = true;
			t.Priority = ThreadPriority.BelowNormal;
			t.Name = "CreateDefaultPC";
			t.Start();
		}
		
		static void CreateDefaultProjectContentReferences()
		{
			IList<string> defaultReferences = AddInTree.BuildItems<string>("/SharpDevelop/Services/ParserService/SingleFileGacReferences", null, false);
			foreach (string defaultReference in defaultReferences) {
				ReferenceProjectItem item = new ReferenceProjectItem(null, defaultReference);
				defaultProjectContent.AddReferencedContent(ParserService.GetProjectContentForReference(item));
			}
			if (WorkbenchSingleton.Workbench != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += delegate {
					if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
						string file = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName
							?? WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.UntitledName;
						if (file != null) {
							IParser parser = GetParser(file);
							if (parser != null && parser.Language != null) {
								defaultProjectContent.Language = parser.Language;
								defaultProjectContent.DefaultImports = parser.Language.CreateDefaultImports(defaultProjectContent);
							}
						}
					}
				};
			}
		}
		
		public static ParseInformation ParseFile(string fileName, string fileContent, bool updateCommentTags)
		{
			return ParseFile(null, fileName, fileContent, updateCommentTags);
		}
		
		public static ParseInformation ParseFile(IProjectContent fileProjectContent, string fileName, string fileContent, bool updateCommentTags)
		{
			if (fileName == null) throw new ArgumentNullException("fileName");
			
			IParser parser = GetParser(fileName);
			if (parser == null) {
				return null;
			}
			
			ICompilationUnit parserOutput = null;
			
			try {
				if (fileProjectContent == null) {
					// GetProjectContent is expensive because it compares all file names, so
					// we accept the project content as optional parameter.
					fileProjectContent = GetProjectContent(fileName);
					if (fileProjectContent == null) {
						fileProjectContent = DefaultProjectContent;
					}
				}
				
				if (fileContent == null) {
					if (!File.Exists(fileName)) {
						return null;
					}
					fileContent = GetParseableFileContent(fileName);
				}
				parserOutput = parser.Parse(fileProjectContent, fileName, fileContent);
				
				if (parsings.ContainsKey(fileName)) {
					ParseInformation parseInformation = parsings[fileName];
					fileProjectContent.UpdateCompilationUnit(parseInformation.MostRecentCompilationUnit, parserOutput, fileName);
				} else {
					fileProjectContent.UpdateCompilationUnit(null, parserOutput, fileName);
				}
				if (updateCommentTags) {
					TaskService.UpdateCommentTags(fileName, parserOutput.TagComments);
				}
				return UpdateParseInformation(parserOutput, fileName, updateCommentTags);
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			return null;
		}
		
		public static ParseInformation UpdateParseInformation(ICompilationUnit parserOutput, string fileName, bool updateCommentTags)
		{
			if (!parsings.ContainsKey(fileName)) {
				parsings[fileName] = new ParseInformation();
			}
			
			ParseInformation parseInformation = parsings[fileName];
			
			try {
				OnParseInformationUpdated(new ParseInformationEventArgs(fileName, parseInformation, parserOutput));
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			return parseInformation;
		}
		
		public static string GetParseableFileContent(string fileName)
		{
			IWorkbenchWindow window = FileService.GetOpenFile(fileName);
			if (window != null) {
				IViewContent viewContent = window.ViewContent;
				IEditable editable = viewContent as IEditable;
				if (editable != null) {
					return editable.Text;
				}
			}
			//string res = project.GetParseableFileContent(fileName);
			//if (res != null)
			//	return res;
			
			// load file
			Encoding tmp = DefaultFileEncoding;
			return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(fileName, ref tmp, tmp);
		}
		
		public static Encoding DefaultFileEncoding {
			get {
				Properties textEditorProperties = PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties());
				return Encoding.GetEncoding(textEditorProperties.Get("Encoding", 1252));
			}
		}
		
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			if (!parsings.ContainsKey(fileName)) {
				return ParseFile(fileName);
			}
			return parsings[fileName];
		}
		
		public static void ClearParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return;
			}
			LoggingService.Info("ClearParseInformation: " + fileName);
			if (parsings.ContainsKey(fileName)) {
				ParseInformation parseInfo = parsings[fileName];
				if (parseInfo != null && parseInfo.MostRecentCompilationUnit != null) {
					parseInfo.MostRecentCompilationUnit.ProjectContent.RemoveCompilationUnit(parseInfo.MostRecentCompilationUnit);
				}
				parsings.Remove(fileName);
				OnParseInformationUpdated(new ParseInformationEventArgs(fileName, parseInfo, null));
			}
		}
		
		public static IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CreateExpressionFinder(fileName);
			}
			return null;
		}
		
		public static readonly string[] DefaultTaskListTokens = {"HACK", "TODO", "UNDONE", "FIXME"};
		
		public static IParser GetParser(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			IParser curParser = null;
			foreach (ParserDescriptor descriptor in parser) {
				if (descriptor.CanParse(fileName)) {
					curParser = descriptor.Parser;
					break;
				}
			}
			
			if (curParser != null) {
				curParser.LexerTags = PropertyService.Get("SharpDevelop.TaskListTokens", DefaultTaskListTokens);
			}
			
			return curParser;
		}
		
		////////////////////////////////////
		
		public static ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context)
		{
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				return resolver.CtrlSpace(caretLine, caretColumn, fileName, fileContent, context);
			}
			return null;
		}
		
		public static IResolver CreateResolver(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CreateResolver();
			}
			return null;
		}
		
		public static ResolveResult Resolve(ExpressionResult expressionResult,
		                                    int caretLineNumber,
		                                    int caretColumn,
		                                    string fileName,
		                                    string fileContent)
		{
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				return resolver.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
			}
			return null;
		}

		static void OnParseInformationUpdated(ParseInformationEventArgs e)
		{
			if (ParseInformationUpdated != null) {
				ParseInformationUpdated(null, e);
			}
		}
		
		static void OnLoadSolutionProjectsThreadEnded(EventArgs e)
		{
			if (LoadSolutionProjectsThreadEnded != null) {
				LoadSolutionProjectsThreadEnded(null, e);
			}
		}
		
		public static event ParseInformationEventHandler ParseInformationUpdated;
		public static event EventHandler LoadSolutionProjectsThreadEnded;
		
		public static ProjectContentRegistry GetRegistryForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem || item.Project == null) {
				return defaultProjectContentRegistry;
			}
			foreach (ProjectContentRegistryDescriptor registry in registries) {
				if (registry.UseRegistryForProject(item.Project)) {
					ProjectContentRegistry r = registry.Registry;
					if (r != null) {
						return r;
					} else {
						return defaultProjectContentRegistry; // fallback when class not found
					}
				}
			}
			return defaultProjectContentRegistry;
		}
		
		public static IProjectContent GetExistingProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetExistingProjectContent(item.Include, item.FileName);
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetProjectContentForReference(item.Include, item.FileName);
		}
	}
}
