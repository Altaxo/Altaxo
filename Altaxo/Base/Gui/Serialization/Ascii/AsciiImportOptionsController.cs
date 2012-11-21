using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
	public interface IAsciiImportOptionsView
	{
		event Action DoAnalyze;

		int NumberOfLinesToAnalyze { get; set; }

		int NumberOfMainHeaderLines { get; set; }
		int IndexOfCaptionLine { get; set; }

		void SetGuiSeparationStrategy(SelectableListNodeList list);

		int NumberOfDecimalSeparatorDots { set; }
		int NumberOfDecimalSeparatorCommas { set; }

		bool RenameColumnsWithHeaderNames { get; set; }
		bool RenameWorksheetWithFileName { get; set; }
	}

	[ExpectedTypeOfView(typeof(IAsciiImportOptionsView))]
	[UserControllerForObject(typeof(AsciiImportOptions))]
	public class AsciiImportOptionsController : MVCANControllerBase<AsciiImportOptions, IAsciiImportOptionsView>
	{
		System.IO.Stream _asciiStreamData;

		SelectableListNodeList _separationStrategyList;

		public override bool InitializeDocument(params object[] args)
		{
			if (args != null && args.Length >= 2 && args[1] is System.IO.Stream)
				_asciiStreamData = args[1] as System.IO.Stream;

			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_separationStrategyList = new SelectableListNodeList();
				var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Serialization.Ascii.IAsciiSeparationStrategy));

				foreach (var t in types)
					_separationStrategyList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, _doc.SeparationStrategy == null ? false : t == _doc.SeparationStrategy.GetType()));
			}

			if (null != _view)
			{
				_view.NumberOfLinesToAnalyze = _doc.NumberOfLinesToAnalyze;

				_view.NumberOfMainHeaderLines = _doc.NumberOfMainHeaderLines;
				_view.IndexOfCaptionLine = _doc.IndexOfCaptionLine;
				_view.NumberOfDecimalSeparatorDots = _doc.DecimalSeparatorDotCount;
				_view.NumberOfDecimalSeparatorCommas = _doc.DecimalSeparatorCommaCount;

				_view.RenameColumnsWithHeaderNames = _doc.RenameColumns;
				_view.RenameWorksheetWithFileName = _doc.RenameWorksheet;

				_view.SetGuiSeparationStrategy(_separationStrategyList);
			}
		}


		void EhDoAsciiAnalysis()
		{
			ApplyWithoutClosing(); // getting _doc filled with user choices

			if (_asciiStreamData != null)
			{
				_asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
				_doc = AsciiDocumentAnalysis.Analyze(_asciiStreamData, _doc);
				_asciiStreamData.Seek(0, System.IO.SeekOrigin.Begin);
				Initialize(true); // getting Gui elements filled with the result of the analysis
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.DoAnalyze += EhDoAsciiAnalysis;
		}

		protected override void DetachView()
		{
			_view.DoAnalyze -= EhDoAsciiAnalysis;

			base.DetachView();
		}

		private bool ApplyWithoutClosing()
		{

			_doc.NumberOfLinesToAnalyze = _view.NumberOfLinesToAnalyze;
			_doc.NumberOfMainHeaderLines = _view.NumberOfMainHeaderLines;
			_doc.IndexOfCaptionLine = _view.IndexOfCaptionLine;

			_doc.RenameColumns = _view.RenameColumnsWithHeaderNames;
			_doc.RenameWorksheet = _view.RenameWorksheetWithFileName;

			return true;
		}

		public override bool Apply()
		{
			return ApplyWithoutClosing();
		}
	}
}
