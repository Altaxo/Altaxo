using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.BasicTypes
{
	public interface IDateTimeNakedControl
	{
		DateTime SelectedValue { get; set; }
	}

	[UserControllerForObject(typeof(DateTime), 100)]
	[ExpectedTypeOfView(typeof(IDateTimeNakedControl))]
	public class DateTimeController : MVCANControllerEditImmutableDocBase<DateTime, IDateTimeNakedControl>
	{
		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (null != _view)
			{
				_view.SelectedValue = _doc;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc = _view.SelectedValue;

			return ApplyEnd(true, disposeController);
		}

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}
	}
}