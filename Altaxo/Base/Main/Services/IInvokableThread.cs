using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Thread that can be invoked, i.e. code can be executed using <see cref="Invoke"/> or <see cref="InvokeAsync"/> always from this thread. This is especially important
  /// for objects which are thread sensitive. These objects must be created and it's functions must be called always from the same thread.
  /// </summary>
  public interface IInvokeableThread : IDisposable
  {
    /// <summary>
    /// Executes the provided action synchronously. This means that this function returns only after the provided action was executed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Invoke(Action action);

    /// <summary>
    /// Executes the provided action asynchronously. This means that this function immediately returns, without waiting for the action to be executed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void InvokeAsync(Action action);
  }
}
