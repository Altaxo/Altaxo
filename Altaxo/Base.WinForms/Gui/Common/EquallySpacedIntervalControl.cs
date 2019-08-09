using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
	public partial class EquallySpacedIntervalControl : UserControl, IEquallySpacedIntervalView
	{
		public EquallySpacedIntervalControl()
		{
			InitializeComponent();
		}

		#region IEquallySpacedIntervalView Members

		public event Action<EquallySpacedIntervalSpecificationMethod> MethodChanged;

		public event Action<string> StartChanged;

		public event Action<string> EndChanged;

		public event Action<string> CountChanged;

		public event Action<string> IntervalChanged;

		public event Action<CancelEventArgs> CountValidating;

		public event Action<CancelEventArgs> IntervalValidating;

		public void EnableEditBoxes(bool start, bool end, bool count, bool interval)
		{
			_edStart.Enabled = start;
			_edEnd.Enabled = end;
			_edCount.Enabled = count;
			_edIntv.Enabled = interval;
		}

		public void InitializeMethod(EquallySpacedIntervalSpecificationMethod method)
		{
			switch (method)
			{
				case EquallySpacedIntervalSpecificationMethod.StartEndCount:
					_rbStartEndCount.Checked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.StartCountInterval:
					_rbStartCountInterval.Checked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.EndCountInterval:
					_rbEndCountIntv.Checked = true;
					break;
				case EquallySpacedIntervalSpecificationMethod.StartEndInterval:
					_rbStartEndIntv.Checked = true;
					break;
				default:
					throw new ArgumentException("method unknown");
			}
		}

		public void InitializeStart(string text)
		{
			_edStart.Text = text;
		}

		public void InitializeEnd(string text)
		{
			_edEnd.Text = text;
		}

		public void InitializeCount(string text)
		{
			_edCount.Text = text;
		}

		public void InitializeInterval(string text)
		{
			_edIntv.Text = text;
		}

		#endregion

		private void _edStart_TextChanged(object sender, EventArgs e)
		{
			if (null != StartChanged)
				StartChanged(_edStart.Text);
		}

		private void _edEnd_TextChanged(object sender, EventArgs e)
		{
			if (null != EndChanged)
				EndChanged(_edEnd.Text);
		}

		private void _edCount_TextChanged(object sender, EventArgs e)
		{
			if (null != CountChanged)
				CountChanged(_edCount.Text);
		}

		private void _edIntv_TextChanged(object sender, EventArgs e)
		{
			if (null != IntervalChanged)
				IntervalChanged(_edIntv.Text);
		}

		private void _rbStartEndCount_CheckedChanged(object sender, EventArgs e)
		{
			EquallySpacedIntervalSpecificationMethod method = EquallySpacedIntervalSpecificationMethod.StartEndCount;
			if (_rbEndCountIntv.Checked)
				method = EquallySpacedIntervalSpecificationMethod.EndCountInterval;
			else if (_rbStartEndCount.Checked)
				method = EquallySpacedIntervalSpecificationMethod.StartEndCount;
			else if (_rbStartCountInterval.Checked)
				method = EquallySpacedIntervalSpecificationMethod.StartCountInterval;
			else if (_rbStartEndIntv.Checked)
				method = EquallySpacedIntervalSpecificationMethod.StartEndInterval;

			if (null != MethodChanged)
				MethodChanged(method);
		}

		private void _edIntv_Validating(object sender, CancelEventArgs e)
		{
			if (null != IntervalValidating)
				IntervalValidating(e);
		}

		private void _edCount_Validating(object sender, CancelEventArgs e)
		{
			if (null != CountValidating)
				CountValidating(e);
		}

	}
}
