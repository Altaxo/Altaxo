#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Serialization.Ascii;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Serialization.Ascii
{
	public interface IAsciiImportOptionsView
	{
		event Action DoAnalyze;

		event Action SeparationStrategyChanged;

		int? NumberOfMainHeaderLines { get; set; }

		int? IndexOfCaptionLine { get; set; }

		void SetGuiSeparationStrategy(SelectableListNodeList list);

		bool GuiSeparationStrategyIsKnown { get; set; }

		void SetNumberFormatCulture(SelectableListNodeList list);

		bool NumberFormatCultureIsKnowm { get; set; }

		void SetDateTimeFormatCulture(SelectableListNodeList list);

		bool DateTimeFormatCultureIsKnown { get; set; }

		bool TableStructureIsKnown { get; set; }

		System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>> TableStructure { set; }

		bool RenameColumnsWithHeaderNames { get; set; }

		bool RenameWorksheetWithFileName { get; set; }

		SelectableListNodeList HeaderLinesDestination { set; }

		object AsciiSeparationStrategyDetailView { set; }

		object AsciiDocumentAnalysisOptionsView { get; }

		bool ImportMultipleAsciiVertically { get; set; }
	}

	[ExpectedTypeOfView(typeof(IAsciiImportOptionsView))]
	[UserControllerForObject(typeof(AsciiImportOptions))]
	public class AsciiImportOptionsController : MVCANControllerBase<AsciiImportOptions, IAsciiImportOptionsView>
	{
		private System.IO.Stream _asciiStreamData;

		private SelectableListNodeList _separationStrategyList;
		private SelectableListNodeList _numberFormatList;
		private SelectableListNodeList _dateTimeFormatList;
		private SelectableListNodeList _headerLinesDestination;
		private System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>> _tableStructure;

		private IMVCANController _separationStrategyInstanceController;
		private IMVCANController _asciiDocumentAnalysisOptionsController;

		private Dictionary<Type, IAsciiSeparationStrategy> _separationStrategyInstances = new Dictionary<Type, IAsciiSeparationStrategy>();

		private AsciiDocumentAnalysisOptions _analysisOptions;

		public override bool InitializeDocument(params object[] args)
		{
			if (args != null && args.Length >= 2 && args[1] is System.IO.Stream)
				_asciiStreamData = args[1] as System.IO.Stream;

			if (args != null && args.Length >= 3 && args[2] is AsciiDocumentAnalysisOptions)
				_analysisOptions = (AsciiDocumentAnalysisOptions)args[2];
			else
				_analysisOptions = Current.PropertyService.GetValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);

			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_asciiDocumentAnalysisOptionsController = (IMVCANController)Current.Gui.GetController(new object[] { _analysisOptions }, typeof(IMVCANController), UseDocument.Directly);

				_separationStrategyInstances.Clear();

				if (null != _doc.SeparationStrategy)
					_separationStrategyInstances.Add(_doc.SeparationStrategy.GetType(), _doc.SeparationStrategy);

				_separationStrategyList = new SelectableListNodeList();
				var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Serialization.Ascii.IAsciiSeparationStrategy));

				foreach (var t in types)
					_separationStrategyList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, _doc.SeparationStrategy == null ? false : t == _doc.SeparationStrategy.GetType()));

				GetAvailableCultures(ref _numberFormatList, _doc.NumberFormatCulture);
				GetAvailableCultures(ref _dateTimeFormatList, _doc.DateTimeFormatCulture);

				_headerLinesDestination = new SelectableListNodeList(_doc.HeaderLinesDestination);

				if (_doc.RecognizedStructure != null)
				{
					_tableStructure = new System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>>(Boxed<AsciiColumnType>.ToBoxedItems(_doc.RecognizedStructure.ColumnTypes));
				}
				else
				{
					_tableStructure = new System.Collections.ObjectModel.ObservableCollection<Boxed<AsciiColumnType>>();
				}
			}

			if (null != _view)
			{
				_asciiDocumentAnalysisOptionsController.ViewObject = _view.AsciiDocumentAnalysisOptionsView;

				_view.NumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines;
				_view.IndexOfCaptionLine = _doc.IndexOfCaptionLine;

				_view.RenameColumnsWithHeaderNames = _doc.RenameColumns;
				_view.RenameWorksheetWithFileName = _doc.RenameWorksheet;

				_view.GuiSeparationStrategyIsKnown = _doc.SeparationStrategy != null;
				_view.SetGuiSeparationStrategy(_separationStrategyList);
				EhSeparationStrategyChanged();

				_view.NumberFormatCultureIsKnowm = _doc.NumberFormatCulture != null;
				_view.DateTimeFormatCultureIsKnown = _doc.DateTimeFormatCulture != null;
				_view.TableStructureIsKnown = _doc.RecognizedStructure != null;

				_view.SetNumberFormatCulture(_numberFormatList);
				_view.SetDateTimeFormatCulture(_dateTimeFormatList);

				_view.TableStructure = _tableStructure;

				_view.HeaderLinesDestination = _headerLinesDestination;

				_view.ImportMultipleAsciiVertically = _doc.ImportMultipleStreamsVertically;
			}
		}

		private int CompareCultures(CultureInfo x, CultureInfo y)
		{
			return string.Compare(x.DisplayName, y.DisplayName);
		}

		private void GetAvailableCultures(ref SelectableListNodeList list, CultureInfo currentlySelectedCulture)
		{
			list = new SelectableListNodeList();
			var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			Array.Sort(cultures, CompareCultures);

			var invCult = System.Globalization.CultureInfo.InvariantCulture;
			AddCulture(list, invCult, null != currentlySelectedCulture && invCult.ThreeLetterISOLanguageName == currentlySelectedCulture.ThreeLetterISOLanguageName);

			foreach (var cult in cultures)
				AddCulture(list, cult, null != currentlySelectedCulture && cult.Name == currentlySelectedCulture.Name);

			if (null == list.FirstSelectedNode)
				list[0].IsSelected = true;
		}

		private void AddCulture(SelectableListNodeList cultureList, CultureInfo cult, bool isSelected)
		{
			cultureList.Add(new SelectableListNode(cult.DisplayName, cult, isSelected));
		}

		private void EhDoAsciiAnalysis()
		{
			ApplyWithoutClosing(); // getting _doc filled with user choices

			if (_doc.IsFullySpecified)
			{
				Current.Gui.InfoMessageBox("The import options are fully specified. There is nothing left for analysis.");
				return;
			}

			ReadAnalysisOptionsAndAnalyze();
		}

		private void ReadAnalysisOptionsAndAnalyze()
		{
			if (!_asciiDocumentAnalysisOptionsController.Apply())
				return;

			_analysisOptions = (AsciiDocumentAnalysisOptions)_asciiDocumentAnalysisOptionsController.ModelObject;

			if (_asciiStreamData != null)
			{
				_asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
				_doc = AsciiDocumentAnalysis.Analyze(_doc, _asciiStreamData, _analysisOptions);
				_asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
				Initialize(true); // getting Gui elements filled with the result of the analysis
			}
		}

		private void EhSeparationStrategyChanged()
		{
			var selNode = _separationStrategyList.FirstSelectedNode;
			if (null == selNode)
				return;

			var sepType = (Type)selNode.Tag;
			if (null != _doc.SeparationStrategy && _doc.SeparationStrategy.GetType() == sepType && null != _separationStrategyInstanceController)
				return;

			if (_separationStrategyInstanceController != null)
			{
				if (_separationStrategyInstanceController.Apply())
				{
					var oldSep = (IAsciiSeparationStrategy)_separationStrategyInstanceController.ModelObject;
					_separationStrategyInstances[oldSep.GetType()] = oldSep;
				}
			}

			IAsciiSeparationStrategy sep;
			if (_separationStrategyInstances.ContainsKey(sepType))
			{
				sep = _separationStrategyInstances[sepType];
			}
			else
			{
				sep = (IAsciiSeparationStrategy)System.Activator.CreateInstance((Type)selNode.Tag);
				_separationStrategyInstances.Add(sep.GetType(), sep);
			}

			_doc.SeparationStrategy = sep;

			_separationStrategyInstanceController = (IMVCANController)Current.Gui.GetController(new object[] { sep }, typeof(IMVCANController));
			object view = null;
			if (null != _separationStrategyInstanceController)
			{
				Current.Gui.FindAndAttachControlTo(_separationStrategyInstanceController);
				view = _separationStrategyInstanceController.ViewObject;
			}
			if (null != _view)
				_view.AsciiSeparationStrategyDetailView = view;
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.DoAnalyze += EhDoAsciiAnalysis;
			_view.SeparationStrategyChanged += EhSeparationStrategyChanged;
		}

		protected override void DetachView()
		{
			_view.DoAnalyze -= EhDoAsciiAnalysis;
			_view.SeparationStrategyChanged -= EhSeparationStrategyChanged;

			base.DetachView();
		}

		private bool ApplyWithoutClosing()
		{
			if (null != _separationStrategyInstanceController)
				if (_separationStrategyInstanceController.Apply())
					_doc.SeparationStrategy = (IAsciiSeparationStrategy)_separationStrategyInstanceController.ModelObject;
				else
					return false;

			_doc.NumberOfMainHeaderLines = _view.NumberOfMainHeaderLines;
			_doc.IndexOfCaptionLine = _view.IndexOfCaptionLine;

			_doc.RenameColumns = _view.RenameColumnsWithHeaderNames;
			_doc.RenameWorksheet = _view.RenameWorksheetWithFileName;
			_doc.ImportMultipleStreamsVertically = _view.ImportMultipleAsciiVertically;

			if (_view.NumberFormatCultureIsKnowm)
			{
				_doc.NumberFormatCulture = (CultureInfo)_numberFormatList.FirstSelectedNode.Tag;
			}
			else
			{
				_doc.NumberFormatCulture = null;
			}

			if (_view.DateTimeFormatCultureIsKnown)
			{
				_doc.DateTimeFormatCulture = (CultureInfo)_dateTimeFormatList.FirstSelectedNode.Tag;
			}
			else
			{
				_doc.DateTimeFormatCulture = null;
			}

			if (_view.GuiSeparationStrategyIsKnown)
			{
				// this case was already handled above
			}
			else
			{
				_doc.SeparationStrategy = null;
			}

			if (_view.TableStructureIsKnown)
			{
				_doc.RecognizedStructure.Clear();
				Boxed<AsciiColumnType>.AddRange(_doc.RecognizedStructure.ColumnTypes, _tableStructure);
				if (_doc.RecognizedStructure.Count == 0)
					_doc.RecognizedStructure = null;
			}
			else
			{
				_doc.RecognizedStructure = null;
			}

			_doc.HeaderLinesDestination = (AsciiHeaderLinesDestination)_headerLinesDestination.FirstSelectedNode.Tag;

			return true;
		}

		public override bool Apply()
		{
			if (!ApplyWithoutClosing())
				return false;

			if (!_doc.IsFullySpecified)
			{
				ReadAnalysisOptionsAndAnalyze();
			}

			if (!_doc.IsFullySpecified)
			{
				Current.Gui.InfoMessageBox("The analysis of the document was unable to determine some of the import options. You have to specify them manually.", "Attention");
				return false;
			}

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}