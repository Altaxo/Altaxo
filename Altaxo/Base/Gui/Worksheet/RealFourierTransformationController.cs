using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;

using Altaxo.Data;

namespace Altaxo.Gui.Worksheet
{
	public interface IRealFourierTransformationView
	{
		void SetColumnToTransform(string val);
		void SetXIncrement(string val, bool bMarkAsWarning);
		void SetOutputQuantities(SelectableListNodeList list);
		void SetCreationOptions(SelectableListNodeList list);
	}


	[ExpectedTypeOfView(typeof(IRealFourierTransformationView))]
	[UserControllerForObject(typeof(AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))]
	public class RealFourierTransformationController : IMVCANController
	{
		IRealFourierTransformationView _view;
		AnalysisRealFourierTransformationCommands.RealFourierTransformOptions _doc;

		SelectableListNodeList _outputQuantities;
		SelectableListNodeList _creationOptions;


		void Initialize(bool bInitData)
		{
			if (bInitData)
			{
				_outputQuantities = new SelectableListNodeList();
				_creationOptions = new SelectableListNodeList();
			}

			if (_view != null)
			{
				var yColName = Main.DocumentPath.GetPathString(_doc.ColumnToTransform, int.MaxValue);
				_view.SetColumnToTransform(yColName);

				string xInc = _doc.XIncrementValue.ToString();
				if (_doc.XIncrementMessage != null)
					xInc += string.Format(" ({0})", _doc.XIncrementMessage);
				_view.SetXIncrement(xInc, _doc.XIncrementMessage!=null);

				_outputQuantities.FillWithFlagEnumeration(_doc.Output);
				_view.SetOutputQuantities(_outputQuantities);

				_creationOptions.FillWithEnumeration(_doc.OutputPlacement);
				_view.SetCreationOptions(_creationOptions);
			}
		}


		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0 || !(args[0] is AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))
				return false;
			_doc = args[0] as AnalysisRealFourierTransformationCommands.RealFourierTransformOptions;
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IRealFourierTransformationView;
				if (_view != null)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public bool Apply()
		{
			_doc.Output = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutput)_outputQuantities.GetFlagEnumValueAsInt32();
			_doc.OutputPlacement = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutputPlacement)_creationOptions.FirstSelectedNode.Item;
			return true;
		}
	}
}
