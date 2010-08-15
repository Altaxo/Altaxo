// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 6196 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class RefactoringService
	{
		#region FindDerivedClasses
		
		/// <summary>
		/// Finds all classes deriving from baseClass.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		/// <param name="directDerivationOnly">If true, gets only the classes that derive directly from <paramref name="baseClass"/>.</param>
		public static IEnumerable<IClass> FindDerivedClasses(IClass baseClass, bool directDerivationOnly)
		{
			return FindDerivedClasses(baseClass, ParserService.AllProjectContents, directDerivationOnly);
		}
		
		/// <summary>
		/// Finds all classes deriving from baseClass.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		/// <param name="projectContents">The project contents in which derived classes should be searched.</param>
		/// <param name="directDerivationOnly">If true, gets only the classes that derive directly from <paramref name="baseClass"/>.</param>
		public static IEnumerable<IClass> FindDerivedClasses(IClass baseClass, IEnumerable<IProjectContent> projectContents, bool directDerivationOnly)
		{
			HashSet<IClass> resultList = new HashSet<IClass>();
			FindDerivedClasses(resultList, baseClass, projectContents, directDerivationOnly);
			return resultList.OrderBy(c => c.FullyQualifiedName);
		}
		
		/// <summary>
		/// Returns all classes deriving from baseClass as inheritance tree.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		public static IEnumerable<ITreeNode<IClass>> FindDerivedClassesTree(IClass baseClass)
		{
			return FindDerivedClassesTree(baseClass, ParserService.AllProjectContents);
		}
		
		/// <summary>
		/// Returns all classes deriving from baseClass as inheritance tree.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		/// <param name="projectContents">The project contents in which derived classes should be searched.</param>
		public static IEnumerable<ITreeNode<IClass>> FindDerivedClassesTree(IClass baseClass, IEnumerable<IProjectContent> projectContents)
		{
			return FindDerivedClassesTree(baseClass, projectContents, new HashSet<IClass>());
		}
		
		static IEnumerable<ITreeNode<IClass>> FindDerivedClassesTree(IClass baseClass, IEnumerable<IProjectContent> projectContents, HashSet<IClass> seenClasses)
		{
			baseClass = baseClass.GetCompoundClass();
			LoggingService.Debug("FindDerivedClasses tree for " + baseClass.FullyQualifiedName);
			
			var result = new List<TreeNode<IClass>>();
			
			foreach (IProjectContent pc in GetSuitableProjectContents(projectContents, baseClass)) {
				foreach (var directlyDerived in GetDerivedClasses(pc, baseClass, pc.Classes).Select(c => c.GetCompoundClass())) {
					
					if (!seenClasses.Contains(directlyDerived)) {
						seenClasses.Add(directlyDerived);
						
						var derivedChild = new TreeNode<IClass>(directlyDerived);
						result.Add(derivedChild);
						derivedChild.Children = FindDerivedClassesTree(directlyDerived, projectContents, seenClasses);
					}
				}
			}
			
			result.OrderBy(node => node.Content.FullyQualifiedName);
			return result;
		}
		
		static void FindDerivedClasses(HashSet<IClass> resultList, IClass baseClass, IEnumerable<IProjectContent> projectContents, bool directDerivationOnly)
		{
			baseClass = baseClass.GetCompoundClass();
			LoggingService.Debug("FindDerivedClasses for " + baseClass.FullyQualifiedName);
			List<IClass> list = new List<IClass>();
			foreach (IProjectContent pc in GetSuitableProjectContents(projectContents, baseClass)) {
				AddDerivedClasses(pc, baseClass, pc.Classes, list);
			}
			if (directDerivationOnly) {
				resultList.AddRange(list);
			} else {
				foreach (IClass c in list) {
					if (resultList.Add(c)) {
						FindDerivedClasses(resultList, c, projectContents, directDerivationOnly);
					}
				}
			}
		}
		
		static IEnumerable<IProjectContent> GetSuitableProjectContents(IEnumerable<IProjectContent> projectContents, IClass baseClass)
		{
			// only project contents referencing the content of the base class
			// can derive from the class
			return projectContents.Where(pc => (pc == baseClass.ProjectContent) || pc.ReferencedContents.Contains(baseClass.ProjectContent));
		}
		
		static void AddDerivedClasses(IProjectContent pc, IClass baseClass, IEnumerable<IClass> classList, IList<IClass> resultList)
		{
			string baseClassName = baseClass.Name;
			string baseClassFullName = baseClass.FullyQualifiedName;
			foreach (IClass c in classList) {
				AddDerivedClasses(pc, baseClass, c.InnerClasses, resultList);
				foreach (var baseType in c.BaseTypes) {
					string baseTypeName = baseType.Name;
					// If this type has our type as base, add it to derived
					if (pc.Language.NameComparer.Equals(baseType.Name, baseClassName) ||
					    pc.Language.NameComparer.Equals(baseType.Name, baseClassFullName)) {
						if (baseType.FullyQualifiedName == baseClassFullName &&
						    baseType.TypeArgumentCount == baseClass.TypeParameters.Count) {
							resultList.Add(c);
						}
					}
				}
			}
		}
		
		static IList<IClass> GetDerivedClasses(IProjectContent pc, IClass baseClass, IEnumerable<IClass> classList)
		{
			var result = new List<IClass>();
			AddDerivedClasses(pc, baseClass, classList, result);
			return result;
		}
		#endregion
		
		#region FindReferences
		/// <summary>
		/// Find all references to the specified member.
		/// </summary>
		public static List<Reference> FindReferences(IMember member, IProgressMonitor progressMonitor)
		{
			return FindReferences(member, null, progressMonitor);
		}
		
		static List<Reference> FindReferences(IMember member, string fileName, IProgressMonitor progressMonitor)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			return RunFindReferences(member.DeclaringType, member, fileName, progressMonitor);
		}
		
		/// <summary>
		/// Find all references to the specified class.
		/// </summary>
		public static List<Reference> FindReferences(IClass @class, IProgressMonitor progressMonitor)
		{
			return FindReferences(@class, null, progressMonitor);
		}
		
		static List<Reference> FindReferences(IClass @class, string fileName, IProgressMonitor progressMonitor)
		{
			if (@class == null)
				throw new ArgumentNullException("class");
			return RunFindReferences(@class, null, fileName, progressMonitor);
		}
		
		/// <summary>
		/// Find all references to the resolved entity.
		/// </summary>
		public static List<Reference> FindReferences(ResolveResult entity, IProgressMonitor progressMonitor)
		{
			return FindReferences(entity, null, progressMonitor);
		}
		
		/// <summary>
		/// Finds all references to the resolved entity, only in the file where the entity was resolved.
		/// </summary>
		public static List<Reference> FindReferencesLocal(ResolveResult entity, string fileName, IProgressMonitor progressMonitor)
		{
			return FindReferences(entity, fileName, progressMonitor);
		}
		
		static List<Reference> FindReferences(ResolveResult entity, string fileName, IProgressMonitor progressMonitor)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (entity is LocalResolveResult) {
				return RunFindReferences(entity.CallingClass, (entity as LocalResolveResult).Field,
				                         entity.CallingClass.CompilationUnit.FileName, progressMonitor);
			} else if (entity is TypeResolveResult) {
				TypeResolveResult trr = (TypeResolveResult)entity;
				if (trr.ResolvedClass != null) {
					return FindReferences(trr.ResolvedClass, fileName, progressMonitor);
				}
			} else if (entity is MemberResolveResult) {
				return FindReferences((entity as MemberResolveResult).ResolvedMember, fileName, progressMonitor);
			} else if (entity is MethodGroupResolveResult) {
				IMethod method = (entity as MethodGroupResolveResult).GetMethodIfSingleOverload();
				if (method != null) {
					return FindReferences(method, fileName, progressMonitor);
				}
			} else if (entity is MixedResolveResult) {
				return FindReferences((entity as MixedResolveResult).PrimaryResult, fileName, progressMonitor);
			}
			return null;
		}
		
		/// <summary>
		/// This method can be used in three modes:
		/// 1. Find references to classes (parentClass = targetClass, member = null, fileName = null)
		/// 2. Find references to members (parentClass = parent, member = member, fileName = null)
		/// 3. Find references to local variables (parentClass = parent, member = local var as field, fileName = parent.CompilationUnit.FileName)
		/// </summary>
		static List<Reference> RunFindReferences(IClass ownerClass, IMember member,
		                                         string fileName,
		                                         IProgressMonitor progressMonitor)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				if (progressMonitor != null) progressMonitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				if (progressMonitor != null) progressMonitor.ShowingDialog = false;
				return null;
			}
			List<ProjectItem> files;
			if (!string.IsNullOrEmpty(fileName)) {
				// search just in given file
				files = new List<ProjectItem>();
				files.Add(FindItem(fileName));
			} else {
				// search in all possible files
				ownerClass = ownerClass.GetCompoundClass();
				files = GetPossibleFiles(ownerClass, member);
			}
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			List<Reference> references = new List<Reference>();
			
			if (progressMonitor != null)
				progressMonitor.TaskName = StringParser.Parse("${res:SharpDevelop.Refactoring.FindingReferences}");
			
			foreach (ProjectItem item in files) {
				var entry = finder.Create(FileName.Create(item.FileName));
				
				if (progressMonitor != null) {
					progressMonitor.Progress += 1.0 / files.Count;
					if (progressMonitor.CancellationToken.IsCancellationRequested)
						return null;
				}
				
				ITextBuffer content = entry.GetContent();
				if (content != null) {
					AddReferences(references, ownerClass, member, entry.FileName, content.Text);
				}
			}
			
			return references;
		}
		
		/// <summary>
		/// This method can be used in three modes (like RunFindReferences)
		/// </summary>
		static void AddReferences(List<Reference> list,
		                          IClass parentClass, IMember member,
		                          string fileName, string fileContent)
		{
			TextFinder textFinder; // the class used to find the position to resolve
			if (member == null) {
				textFinder = parentClass.ProjectContent.Language.GetFindClassReferencesTextFinder(parentClass);
			} else {
				Debug.Assert(member.DeclaringType.GetCompoundClass() == parentClass.GetCompoundClass());
				textFinder = parentClass.ProjectContent.Language.GetFindMemberReferencesTextFinder(member);
			}
			
			// It is possible that a class or member does not have a name (when parsing incomplete class definitions)
			// - in that case, we cannot find references.
			if (textFinder == null) {
				return;
			}
			
			string fileContentForFinder = textFinder.PrepareInputText(fileContent);
			
			IExpressionFinder expressionFinder = null;
			TextFinderMatch match = new TextFinderMatch(-1, 0);
			
			while (true) {
				match = textFinder.Find(fileContentForFinder, match.Position + 1);
				if (match.Position < 0)
					break;
				
				if (expressionFinder == null) {
					expressionFinder = ParserService.GetExpressionFinder(fileName);
					if (expressionFinder == null) {
						// ignore file if we cannot get an expression finder
						return;
					}
				}
				ExpressionResult expr = expressionFinder.FindFullExpression(fileContent, match.ResolvePosition);
				if (expr.Expression != null) {
					Point position = GetPosition(fileContent, match.ResolvePosition);
				repeatResolve:
					// TODO: Optimize by re-using the same resolver if multiple expressions were
					// found in this file (the resolver should parse all methods at once)
					ResolveResult rr = ParserService.Resolve(expr, position.Y, position.X, fileName, fileContent);
					MemberResolveResult mrr = rr as MemberResolveResult;
					if (member != null) {
						// find reference to member
						if (rr != null && rr.IsReferenceTo(member)) {
							list.Add(new Reference(fileName, match.Position, match.Length, expr.Expression, rr));
						} else if (FixIndexerExpression(expressionFinder, ref expr, mrr)) {
							goto repeatResolve;
						}
					} else {
						// find reference to class
						if (rr != null && rr.IsReferenceTo(parentClass)) {
							list.Add(new Reference(fileName, match.Position, match.Length, expr.Expression, rr));
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Makes the given ExpressionResult point to the underlying expression if
		/// the expression is an indexer expression.
		/// </summary>
		/// <returns><c>true</c>, if the expression was an indexer expression and has been changed, <c>false</c> otherwise.</returns>
		public static bool FixIndexerExpression(IExpressionFinder expressionFinder, ref ExpressionResult expr, MemberResolveResult mrr)
		{
			if (mrr != null && mrr.ResolvedMember is IProperty && ((IProperty)mrr.ResolvedMember).IsIndexer) {
				// we got an indexer call as expression ("objectList[0].ToString()[2]")
				// strip the index from the expression to resolve the underlying expression
				string newExpr = expressionFinder.RemoveLastPart(expr.Expression);
				if (newExpr.Length >= expr.Expression.Length) {
					throw new ApplicationException("new expression must be shorter than old expression");
				}
				expr.Expression = newExpr;
				return true;
			}
			return false;
		}
		
		static Point GetPosition(string fileContent, int pos)
		{
			int line = 1;
			int column = 0;
			for (int i = 0; i < pos; ++i) {
				if (fileContent[i] == '\n') {
					++line;
					column = 0;
				} else {
					++column;
				}
			}
			return new Point(column, line);
		}
		
		static List<string> GetFileNames(IClass c)
		{
			List<string> list = new List<string>();
			CompoundClass cc = c as CompoundClass;
			if (cc != null) {
				foreach (IClass part in cc.Parts) {
					string fileName = part.CompilationUnit.FileName;
					if (fileName != null)
						list.Add(fileName);
				}
			} else {
				string fileName = c.CompilationUnit.FileName;
				if (fileName != null)
					list.Add(fileName);
			}
			return list;
		}
		
		/// <summary>
		/// Gets the files of files that could have a reference to the <paramref name="member"/>
		/// int the <paramref name="ownerClass"/>.
		/// </summary>
		static List<ProjectItem> GetPossibleFiles(IClass ownerClass, IEntity member)
		{
			List<ProjectItem> resultList = new List<ProjectItem>();
			if (ProjectService.OpenSolution == null) {
				foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string name = vc.PrimaryFileName;
					if (!string.IsNullOrEmpty(name) && ParserService.CreateParser(name) != null) {
						FileProjectItem tempItem = new FileProjectItem(null, ItemType.Compile);
						tempItem.Include = name;
						resultList.Add(tempItem);
					}
				}
				return resultList;
			}
			
			if (member == null) {
				// get files possibly referencing ownerClass
				while (ownerClass.DeclaringType != null) {
					// for nested classes, treat class as member
					member = ownerClass;
					ownerClass = ownerClass.DeclaringType;
				}
				if (member == null) {
					GetPossibleFilesInternal(resultList, ownerClass.ProjectContent, ownerClass.IsInternal);
					return resultList;
				}
			}
			if (member.IsPrivate) {
				List<string> fileNames = GetFileNames(ownerClass);
				foreach (string fileName in fileNames) {
					ProjectItem item = FindItem(fileName);
					if (item != null) resultList.Add(item);
				}
				return resultList;
			}
			
			if (member.IsProtected) {
				// TODO: Optimize when member is protected
			}
			
			GetPossibleFilesInternal(resultList, ownerClass.ProjectContent, ownerClass.IsInternal || member.IsInternal && !member.IsProtected);
			return resultList;
		}
		
		static ProjectItem FindItem(string fileName)
		{
			if (ProjectService.OpenSolution != null) {
				foreach (IProject p in ProjectService.OpenSolution.Projects) {
					foreach (ProjectItem item in p.Items) {
						if (FileUtility.IsEqualFileName(fileName, item.FileName)) {
							return item;
						}
					}
				}
			}
			FileProjectItem tempItem = new FileProjectItem(null, ItemType.Compile);
			tempItem.Include = fileName;
			return tempItem;
		}
		
		static void GetPossibleFilesInternal(List<ProjectItem> resultList, IProjectContent ownerProjectContent, bool internalOnly)
		{
			if (ProjectService.OpenSolution == null) {
				return;
			}
			foreach (IProject p in ProjectService.OpenSolution.Projects) {
				IProjectContent pc = ParserService.GetProjectContent(p);
				if (pc == null) continue;
				if (pc != ownerProjectContent) {
					if (internalOnly) {
						// internal = can be only referenced from same project content
						continue;
					}
					if (!pc.ReferencedContents.Contains(ownerProjectContent)) {
						// project contents that do not reference the owner's content cannot reference the member
						continue;
					}
				}
				foreach (ProjectItem item in p.Items) {
					if (item is FileProjectItem) {
						resultList.Add(item);
					}
				}
			}
		}
		#endregion
		
		#region Interface / abstract class implementation
		/// <summary>
		/// Gets actions which can add implementation of interface to given class.
		/// </summary>
		public static IEnumerable<ImplementInterfaceAction> GetImplementInterfaceActions(IClass c, bool isExplicitImpl, bool returnMissingInterfacesOnly = true)
		{
			foreach (IReturnType rt in c.BaseTypes) {
				IClass interf = rt.GetUnderlyingClass();
				if (interf != null && interf.ClassType == ClassType.Interface) {
					if (returnMissingInterfacesOnly && c.ImplementsInterface(interf)) {
						// this interface is already implemented
						continue;
					}
					IReturnType rtCopy = rt;
					yield return new ImplementInterfaceAction(rtCopy, c, isExplicitImpl);
				}
			}
		}
		/// <summary>
		/// Gets actions which can add implementation of abstract class to given class.
		/// </summary>
		public static IEnumerable<ImplementAbstractClassAction> GetImplementAbstractClassActions(IClass c, bool returnMissingClassesOnly = true)
		{
			foreach (IReturnType rt in c.BaseTypes) {
				IClass abstractClass = rt.GetUnderlyingClass();
				if (abstractClass != null && abstractClass.ClassType == ClassType.Class && abstractClass.IsAbstract) {
					if (returnMissingClassesOnly && c.ImplementsAbstractClass(abstractClass)) {
						// this interface is already implemented
						continue;
					}
					IReturnType rtCopy = rt;
					yield return new ImplementAbstractClassAction(rtCopy, c);
				}
			}
		}
		
		/// <summary>
		/// Action describing how to add implementation of an abstract class to a class.
		/// </summary>
		public class ImplementAbstractClassAction
		{
			public IReturnType ClassToImplement { get; private set; }
			public IClass TargetClass { get; private set; }
			
			public virtual void Execute()
			{
				var d = FindReferencesAndRenameHelper.GetDocument(TargetClass);
				if (d == null)
					return;
				CodeGenerator.ImplementAbstractClass(new RefactoringDocumentAdapter(d), TargetClass, ClassToImplement);
				ParserService.ParseCurrentViewContent();
			}
			
			public ImplementAbstractClassAction(IReturnType classToImplement, IClass targetClass)
			{
				if (targetClass == null)
					throw new ArgumentNullException("targetClass");
				if (classToImplement == null)
					throw new ArgumentNullException("interfaceToImplement");
				this.ClassToImplement = classToImplement;
				this.TargetClass = targetClass;
			}
		}
		
		/// <summary>
		/// Action describing how to add implementation of an interface to a class.
		/// </summary>
		public class ImplementInterfaceAction : ImplementAbstractClassAction
		{
			public bool IsExplicitImpl { get; private set; }
			
			public override void Execute()
			{
				var codeGen = TargetClass.ProjectContent.Language.CodeGenerator;
				var d = FindReferencesAndRenameHelper.GetDocument(TargetClass);
				if (d == null)
					return;
				codeGen.ImplementInterface(this.ClassToImplement, new RefactoringDocumentAdapter(d), this.IsExplicitImpl, this.TargetClass);
				ParserService.ParseCurrentViewContent();
			}
			
			public ImplementInterfaceAction(IReturnType interfaceToImplement, IClass targetClass, bool isExplicitImpl)
				:base(interfaceToImplement, targetClass)
			{
				this.IsExplicitImpl = isExplicitImpl;
			}
		}
		#endregion
		
		#region Add using
		public static IEnumerable<AddUsingAction> GetAddUsingActions(UnknownIdentifierResolveResult rr, ITextEditor textArea)
		{
			return GetAddUsingActionsForUnknownClass(rr.CallingClass, rr.Identifier, textArea);
		}
		
		public static IEnumerable<AddUsingAction> GetAddUsingActions(UnknownConstructorCallResolveResult rr, ITextEditor textArea)
		{
			return GetAddUsingActionsForUnknownClass(rr.CallingClass, rr.TypeName, textArea);
		}
		
		static IEnumerable<AddUsingAction> GetAddUsingActionsForUnknownClass(IClass callingClass, string unknownClassName, ITextEditor editor)
		{
			if (callingClass == null)
				yield break;
			IProjectContent pc = callingClass.ProjectContent;
			if (!pc.Language.RefactoringProvider.IsEnabledForFile(callingClass.CompilationUnit.FileName))
				yield break;
			List<IClass> searchResults = new List<IClass>();
			SearchAllClassesWithName(searchResults, pc, unknownClassName, pc.Language);
			foreach (IProjectContent rpc in pc.ReferencedContents) {
				SearchAllClassesWithName(searchResults, rpc, unknownClassName, pc.Language);
			}
			foreach (IClass c in searchResults) {
				string newNamespace = c.Namespace;
				yield return new AddUsingAction(callingClass.CompilationUnit, editor, newNamespace);
			}
		}
		
		static void SearchAllClassesWithName(List<IClass> searchResults, IProjectContent pc, string name, LanguageProperties language)
		{
			foreach (string ns in pc.NamespaceNames) {
				IClass c = pc.GetClass(ns + "." + name, 0, language, GetClassOptions.None);
				if (c != null)
					searchResults.Add(c);
			}
		}
		
		public class AddUsingAction
		{
			public ICompilationUnit CompilationUnit { get; private set; }
			public ITextEditor Editor { get; private set; }
			public string NewNamespace { get; private set; }
			
			public AddUsingAction(ICompilationUnit compilationUnit, ITextEditor editor, string newNamespace)
			{
				if (compilationUnit == null)
					throw new ArgumentNullException("compilationUnit");
				if (editor == null)
					throw new ArgumentNullException("editor");
				if (newNamespace == null)
					throw new ArgumentNullException("newNamespace");
				this.CompilationUnit = compilationUnit;
				this.Editor = editor;
				this.NewNamespace = newNamespace;
			}
			
			public void Execute()
			{
				NamespaceRefactoringService.AddUsingDeclaration(CompilationUnit, Editor.Document, NewNamespace, true);
				ParserService.BeginParse(Editor.FileName, Editor.Document);
			}
		}
		#endregion
	}
}
