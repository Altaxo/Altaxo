// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.Core.AddIns
{
	/// <summary>
	/// Here is the ONLY point to get an <see cref="IAddInTree"/> object.
	/// </summary>
	public class AddInTreeSingleton : DefaultAddInTree
	{
		static IAddInTree addInTree = null;
		readonly static string defaultCoreDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + // DON'T REPLACE
		Path.DirectorySeparatorChar + ".." +
		Path.DirectorySeparatorChar + "AddIns";
		
		static bool ignoreDefaultCoreDirectory = false;
		static string[] addInDirectories       = null;
		
		/// <summary>
		/// Returns an <see cref="IAddInTree"/> object.
		/// </summary>
		
		public static IAddInTree AddInTree {
			get {
				if (addInTree == null) {
					CreateAddInTree();
				}
				return addInTree;
			}
		}
		
		public static bool SetAddInDirectories(string[] addInDirectories, bool ignoreDefaultCoreDirectory)
		{
			if (addInDirectories == null || addInDirectories.Length < 1) {
				// something went wrong
				return false;
			}
			AddInTreeSingleton.addInDirectories = addInDirectories;
			AddInTreeSingleton.ignoreDefaultCoreDirectory = ignoreDefaultCoreDirectory;
			return true;
		}
		
		static StringCollection InsertAddIns(StringCollection addInFiles)
		{
			StringCollection retryList  = new StringCollection();
			
			foreach (string addInFile in addInFiles) {
				AddIn addIn = new AddIn();
				try {
					addIn.Initialize(addInFile);
					addInTree.InsertAddIn(addIn);
				} catch (CodonNotFoundException) {
					retryList.Add(addInFile);
				} catch (ConditionNotFoundException) {
					retryList.Add(addInFile);
				} catch (Exception e) {
					throw new AddInInitializeException(addInFile, e);
				} 
			}
			
			return retryList;
		}
		
		static void CreateAddInTree()
		{
			addInTree = new DefaultAddInTree();
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			StringCollection addInFiles = null;
			StringCollection retryList  = null;
			
			if (ignoreDefaultCoreDirectory == false) {
				addInFiles = fileUtilityService.SearchDirectory(defaultCoreDirectory, "*.addin");
				retryList  = InsertAddIns(addInFiles);
			}
			else
				retryList = new StringCollection();
			
			if (addInDirectories != null) {
				foreach(string path in addInDirectories) {
					addInFiles = fileUtilityService.SearchDirectory(path, "*.addin");
					StringCollection partialRetryList  = InsertAddIns(addInFiles);
					if (partialRetryList.Count != 0) {
						string [] retryListArray = new string[partialRetryList.Count];
						partialRetryList.CopyTo(retryListArray, 0);
						retryList.AddRange(retryListArray);
					}
				}
			}
			
			while (retryList.Count > 0) {
				StringCollection newRetryList = InsertAddIns(retryList);
				
				// break if no add-in could be inserted.
				if (newRetryList.Count == retryList.Count) {
					break;
				}
				
				retryList = newRetryList;
			}
			
			if (retryList.Count > 0) {
				throw new ApplicationException("At least one AddIn uses an undefined codon or condition: " + retryList[0]);
			}
			//			tree.ShowCodonTree();
		}
	}
}
