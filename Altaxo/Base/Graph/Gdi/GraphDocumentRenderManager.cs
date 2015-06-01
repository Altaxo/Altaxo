using Altaxo.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Gdi
{
	public class GraphDocumentRenderManager
	{
		#region Inner classes

		public class GraphDocumentRenderTask
		{
			private GraphDocumentRenderManager _parent;

			public object Owner { get; private set; }

			public GraphDocument Document { get; private set; }

			private Func<System.Drawing.Graphics> _beforeRendering;
			private Action<System.Drawing.Graphics> _afterRendering;

			public override bool Equals(object obj)
			{
				var from = obj as GraphDocumentRenderTask;
				if (null != from)
					return this.Owner.Equals(from.Owner);
				else
					return false;
			}

			public override int GetHashCode()
			{
				return Owner.GetHashCode();
			}

			public GraphDocumentRenderTask(GraphDocumentRenderManager parent, object token, GraphDocument doc, Func<System.Drawing.Graphics> beforeRendering, Action<System.Drawing.Graphics> afterRendering)
			{
				if (null == parent)
					throw new ArgumentNullException("parent");
				if (null == token)
					throw new ArgumentNullException("token");
				if (null == doc)
					throw new ArgumentNullException("doc");
				if (null == beforeRendering)
					throw new ArgumentNullException("beforeRendering");

				_parent = parent;
				Owner = token;
				Document = doc;
				_beforeRendering = beforeRendering;
				_afterRendering = afterRendering;
			}

			public void RenderTask()
			{
				try
				{
					var grfx = _beforeRendering();

					Document.DoPaint(grfx, false);

					if (null != _afterRendering)
						_afterRendering(grfx);
				}
				catch (Exception ex)
				{
					Current.Console.WriteLine(
					"{0}: Error drawing graph {1}\r\n" +
					"Details: {2}",
					DateTime.Now,
					Document.Name,
					ex
					);
				}
				finally
				{
					_parent.RenderTaskFinished(this);
				}
			}
		}

		#endregion Inner classes

		private static GraphDocumentRenderManager _instance = new GraphDocumentRenderManager();

		public static GraphDocumentRenderManager Instance { get { return _instance; } }

		private ConcurrentTokenizedPriorityQueue<long, GraphDocumentRenderTask, object> _documentsWaiting = new ConcurrentTokenizedPriorityQueue<long, GraphDocumentRenderTask, object>();

		private ConcurrentDictionary<GraphDocument, GraphDocumentRenderTask> _documentsRendering = new ConcurrentDictionary<GraphDocument, GraphDocumentRenderTask>();

		private int maxDocumentsConcurrentlyRendering = 4;

		private long _priority;

		public void AddTask(object owner, GraphDocument doc, Func<System.Drawing.Graphics> actionBefore, Action<System.Drawing.Graphics> actionAfter)
		{
			var task = new GraphDocumentRenderTask(this, owner, doc, actionBefore, actionAfter);

			var newprio = System.Threading.Interlocked.Increment(ref _priority);

			_documentsWaiting.TryAdd(owner, newprio, task);

			TryStartDocument();
		}

		private void RenderTaskFinished(GraphDocumentRenderTask rendering)
		{
			GraphDocumentRenderTask renderTask;
			_documentsRendering.TryRemove(rendering.Document, out renderTask);

			TryStartDocument();
		}

		private void TryStartDocument()
		{
			if (_documentsRendering.Count >= maxDocumentsConcurrentlyRendering)
				return;

			long prio;
			GraphDocumentRenderTask rendering;
			object token;

			if (_documentsWaiting.TryDequeue(out prio, out rendering, out token))
			{
				if (_documentsRendering.TryAdd(rendering.Document, rendering))
				{
					Task.Factory.StartNew(rendering.RenderTask);
				}
				else // unfortunately, it seems that this GraphDocument is already rendering, thus we put it back in the queue
				{
					var newprio = System.Threading.Interlocked.Increment(ref _priority);
					_documentsWaiting.TryAdd(token, newprio, rendering);
					// unfortunately, we can not use another member of the waiting queue cause this could lead to infinite loops (consider we have requests for only this one graphdocument from several requesters).
				}
			}
		}
	}
}