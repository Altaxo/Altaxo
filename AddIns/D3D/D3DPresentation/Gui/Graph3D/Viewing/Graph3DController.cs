using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	public class Graph3DController : IDisposable, IMVCANController
	{
		public event EventHandler TitleNameChanged;

		public object _view;

		public Altaxo.Graph3D.Graph3DDocument Doc { get; set; }

		public object ModelObject
		{
			get
			{
				return Doc;
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}

			set
			{
				_view = value;
			}
		}

		public UseDocument UseDocumentCopy
		{
			set
			{
			}
		}

		public IList<object> SelectedObjects { get; internal set; }

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is Altaxo.Graph3D.Graph3DDocument))
				return false;

			Doc = (Altaxo.Graph3D.Graph3DDocument)args[0];

			return true;
		}

		internal void CutSelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void CopySelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void PasteObjectsFromClipboard()
		{
			throw new NotImplementedException();
		}

		internal void RemoveSelectedObjects()
		{
			throw new NotImplementedException();
		}

		public bool Apply(bool disposeController)
		{
			throw new NotImplementedException();
		}

		public bool Revert(bool disposeController)
		{
			throw new NotImplementedException();
		}
	}
}