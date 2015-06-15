﻿using Altaxo.Collections;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.ProjectBrowser
{
	public class CopyItemsToMultipleFolderData : ICloneable
	{
		public bool RelocateReferences { get; set; }

		public bool OverwriteExistingItems { get; set; }

		protected List<string> _foldersToCopyTo = new List<string>();

		public List<string> FoldersToCopyTo { get { return _foldersToCopyTo; } }

		public object Clone()
		{
			var result = (CopyItemsToMultipleFolderData)this.MemberwiseClone();
			result._foldersToCopyTo = new List<string>(this.FoldersToCopyTo);
			return result;
		}
	}

	public interface ICopyItemsToMultipleFolderView
	{
		void InitializeFolderTree(NGTreeNode rootNode);

		bool RelocateReferences { get; set; }

		bool OverwriteExistingItems { get; set; }

		string AdditionalFoldersLineByLine { get; }

		event Action CopySelectedFolderNames;

		event Action UnselectAllFolders;
	}

	[ExpectedTypeOfView(typeof(ICopyItemsToMultipleFolderView))]
	[UserControllerForObject(typeof(CopyItemsToMultipleFolderData))]
	public class CopyItemsToMultipleFoldersController : MVCANControllerEditCopyOfDocBase<CopyItemsToMultipleFolderData, ICopyItemsToMultipleFolderView>
	{
		private NGTreeNode _projectFolders;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_projectFolders = new NGTreeNode("/") { Tag = ProjectFolder.RootFolderName, IsExpanded = true };
				CreateDirectoryNode(ProjectFolder.RootFolderName, _projectFolders);
			}

			if (null != _view)
			{
				_view.InitializeFolderTree(_projectFolders);
				_view.RelocateReferences = _doc.RelocateReferences;
				_view.OverwriteExistingItems = _doc.OverwriteExistingItems;
			}
		}

		public override bool Apply(bool disposeController)
		{
			var foldersToCopyTo = new HashSet<string>();

			foldersToCopyTo.AddRange(_projectFolders.TakeFromFirstLeavesToHere(true).Where(node => node.IsSelected).Select(node => (string)node.Tag));
			_doc.OverwriteExistingItems = _view.OverwriteExistingItems;
			_doc.RelocateReferences = _view.RelocateReferences;

			string additionalFoldersLineByLine = _view.AdditionalFoldersLineByLine;
			var additionalFolders = additionalFoldersLineByLine.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var folderRaw in additionalFolders)
			{
				string folderName = folderRaw.Trim();
				if (string.IsNullOrEmpty(folderName))
					continue;
				if (folderName.Last() != ProjectFolder.DirectorySeparatorChar)
					folderName = folderName + ProjectFolder.DirectorySeparatorChar;

				foldersToCopyTo.Add(folderName);
			}

			_doc.FoldersToCopyTo.AddRange(foldersToCopyTo);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.CopySelectedFolderNames += EhCopySelectedFolderNames;
			_view.UnselectAllFolders += EhUnselectAllFolders;
		}

		protected override void DetachView()
		{
			_view.CopySelectedFolderNames -= EhCopySelectedFolderNames;
			_view.UnselectAllFolders -= EhUnselectAllFolders;
			base.DetachView();
		}

		private void EhCopySelectedFolderNames()
		{
			var foldersToCopyTo = new List<string>();
			foldersToCopyTo.AddRange(_projectFolders.TakeFromFirstLeavesToHere(true).Where(node => node.IsSelected).Select(node => (string)node.Tag));
			var dao = Current.Gui.GetNewClipboardDataObject();
			dao.SetData("Text", string.Join("\r\n", foldersToCopyTo));
			Current.Gui.SetClipboardDataObject(dao);
		}

		private void EhUnselectAllFolders()
		{
			_projectFolders.TakeFromFirstLeavesToHere(true).ForEachDo(x => x.IsSelected = false);
		}

		private void CreateDirectoryNode(string dir, NGTreeNode node)
		{
			var subfolders = Current.Project.Folders.GetSubfoldersAsStringList(dir, false);
			foreach (var subfolder in subfolders)
			{
				var subnode = new NGTreeNode(ProjectFolder.ConvertFolderNameToDisplayFolderLastPart(subfolder))
				{
					Tag = subfolder,
					IsExpanded = true
				};

				node.Nodes.Add(subnode);
				CreateDirectoryNode(subfolder, subnode);
			}
		}
	}
}