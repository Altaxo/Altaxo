#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Collections;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Manages the concurrent rendering of <see cref="GraphDocument"/>s.
  /// </summary>
  /// <remarks>
  /// The graph documents are rendered in separate, non-Gui threads. Since Altaxo is not thread safe,
  /// exceptions during rendering may happen because either the graph document or the underlying plot data were changed at the same time.
  ///  This can not be avoided for now. Thats why, if an exception during rendering occurs, we try it again and again.
  /// We give the rendering a maximum of 16 trials and 30 seconds time from the first unsuccessful trial to the last.
  /// </remarks>
  public class GraphDocumentRenderManager
  {
    #region Inner classes

    /// <summary>
    /// Render task item managed by the <see cref="GraphDocumentRenderManager"/>.
    /// </summary>
    private class GraphDocumentRenderTask
    {
      private GraphDocumentRenderManager _parent;

      internal object Owner { get; private set; }

      internal GraphDocument Document { get; private set; }

      private Action<GraphDocument, object> _rendering;

      /// <summary>
      /// The trial count down. Integer that is counted down with every trial to render the document. When the trial count down has
      /// reached zero, no more rendering trials are allowed.
      /// </summary>
      private int _trialCountDown = 16;

      /// <summary>
      /// The time when the first rendering exception occured. (null if no such exception occured).
      /// </summary>
      private TimeSpan? _timeOfFirstRenderingException;

      /// <summary>
      /// The maximum time span between now and the first rendering exception. If this time span exceeds the Time span designated here,
      /// no further rendering trials are allowed.
      /// </summary>
      private static readonly TimeSpan _maximumTrialTimeAllowed = TimeSpan.FromSeconds(30);

      /// <summary>
      /// True if the last rendering was successful.
      /// </summary>
      private bool _wasSuccessful;

      public override bool Equals(object? obj)
      {
        var from = obj as GraphDocumentRenderTask;
        if (from is not null)
          return Owner.Equals(from.Owner);
        else
          return false;
      }

      public override int GetHashCode()
      {
        return Owner.GetHashCode();
      }

      public GraphDocumentRenderTask(
        GraphDocumentRenderManager parent,
        object token,
        GraphDocument doc,
        Action<GraphDocument, object> renderingAction
        )
      {
        if (parent is null)
          throw new ArgumentNullException(nameof(parent));
        if (token is null)
          throw new ArgumentNullException(nameof(token));
        if (doc is null)
          throw new ArgumentNullException(nameof(doc));
        if (renderingAction is null)
          throw new ArgumentNullException(nameof(renderingAction));

        _parent = parent;
        Owner = token;
        Document = doc;
        _rendering = renderingAction;
      }

      /// <summary>
      /// Gets a value indicating whether the rendering was successfull.
      /// </summary>
      /// <value>
      ///   <c>true</c> if the rendering was successful; otherwise, <c>false</c>.
      /// </value>
      public bool WasSuccessful { get { return _wasSuccessful; } }

      /// <summary>
      /// Gets a value indicating whether more rendering trials are allowed.
      /// </summary>
      /// <value>
      ///   <c>true</c> if more rendering trials are allowed; otherwise, <c>false</c>. False is returned of either the rendering trial count down has reached zero
      /// or the maximum allowed time for trials has been reached.
      /// </value>
      public bool MoreTrialsAllowed
      {
        get
        {
          if (_timeOfFirstRenderingException is null)
            return true; // not even a rendering exception encountered
          return _trialCountDown > 0 && ((Current.HighResolutionClock.CurrentTime - _timeOfFirstRenderingException.Value) <= _maximumTrialTimeAllowed);
        }
      }

      /// <summary>
      /// Renders the task. This function is used by the render manager.
      /// </summary>
      public void RenderTask()
      {
        try
        {
          --_trialCountDown;

          _rendering(Document, Owner);
          _wasSuccessful = true;
        }
        catch (Exception ex)
        {
          if (_timeOfFirstRenderingException is null)
            _timeOfFirstRenderingException = Current.HighResolutionClock.CurrentTime;

          if (!Document.IsDisposeInProgress && !MoreTrialsAllowed)
          {
            Current.Console.WriteLine(
            "{0}: Error drawing graph {1} (file: {2})\r\n" +
            "Details: {3}",
            DateTime.Now,
            Document.Name,
            Current.Project.Name,
            ex
            );
          }
        }
        finally
        {
          _parent.EhRenderTaskFinished(this);
        }
      }
    }

    #endregion Inner classes

    private static GraphDocumentRenderManager _instance = new GraphDocumentRenderManager();

    public static GraphDocumentRenderManager Instance { get { return _instance; } }

    /// <summary>The list of rendering tasks waiting to be started.</summary>
    private ConcurrentTokenizedLinkedList<object, GraphDocumentRenderTask> _tasksWaiting;

    /// <summary>The rendering tasks currently in progress.</summary>
    private ConcurrentDictionary<GraphDocument, GraphDocumentRenderTask> _tasksRendering;

    /// <summary>The maximum number of tasks that should render concurrently.</summary>
    private int _maxTasksConcurrentlyRendering = 1;

    private volatile bool _isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphDocumentRenderManager"/> class.
    /// </summary>
    public GraphDocumentRenderManager()
    {
      _maxTasksConcurrentlyRendering = Math.Max(1, System.Environment.ProcessorCount - 2); // leave 1 processor for the Gui thread, and another for calculations
      _tasksWaiting = new ConcurrentTokenizedLinkedList<object, GraphDocumentRenderTask>();
      _tasksRendering = new ConcurrentDictionary<GraphDocument, GraphDocumentRenderTask>();

      var projService = Current.IProjectService;
      projService.ProjectClosed += EhProjectClosed;
      projService.ProjectOpened += EhProjectOpened;
      _isEnabled = Current.Project is not null;
    }

    private void EhProjectOpened(object sender, Main.ProjectEventArgs e)
    {
      _isEnabled = true;
      TryStartWaitingTasks();
    }

    private void EhProjectClosed(object? sender, Main.ProjectEventArgs e)
    {
      _isEnabled = false;
      _tasksWaiting.Clear();
    }

    /// <summary>
    /// Adds a new render task.
    /// </summary>
    /// <param name="owner">The owner. This is any object that is able to uniquely identify the render task.</param>
    /// <param name="doc">The graph document to render.</param>
    /// <param name="renderingAction">The rendering action. This action is called when the provided graph document should be rendered.</param>
    public void AddTask(object owner, GraphDocument doc, Action<GraphDocument, object> renderingAction)
    {
      var task = new GraphDocumentRenderTask(this, owner, doc, renderingAction);
      _tasksWaiting.TryAddLast(owner, task);
      TryStartWaitingTasks();
    }

    /// <summary>
    /// The render task calls back this function when it is finished.
    /// It removes the render task from the list of currently rendering tasks. If the task was not successfully finished and
    /// more trials are allowed, the render task is put back at the end of the list of waiting rendering tasks.
    /// </summary>
    /// <param name="rendering">The rendering task that was just finished.</param>
    private void EhRenderTaskFinished(GraphDocumentRenderTask rendering)
    {
      if (_tasksRendering.TryRemove(rendering.Document, out var renderTask))
      {
        if (!renderTask.WasSuccessful && renderTask.MoreTrialsAllowed)
          _tasksWaiting.TryAddLast(renderTask.Owner, renderTask);
      }

      TryStartWaitingTasks();
    }

    /// <summary>
    /// Starts as many waiting render tasks as possible.
    /// </summary>
    private void TryStartWaitingTasks()
    {
      if (!_isEnabled)
        return;

      for (int i = _tasksWaiting.Count - 1; i >= 0; --i)
      {
        if (_tasksRendering.Count >= _maxTasksConcurrentlyRendering)
          break;

        // we try to start the first document in queue, but if this fails,
        // we put it back at the end of the queue, and try to start the now-first document
        // we will do this maximal  _documentsWaiting.Count times, in order to avoid infinite loops
        if (_tasksWaiting.TryTakeFirst(out var token, out var rendering))
        {
          if (_tasksRendering.TryAdd(rendering.Document, rendering))
          {
            Task.Factory.StartNew(rendering.RenderTask);
            break;
          }
          else // unfortunately, it seems that this GraphDocument is already rendering, thus we put it back in the queue
          {
            _tasksWaiting.TryAddLast(token, rendering);
          }
        }
      }
    }
  }
}
