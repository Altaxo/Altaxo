#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Altaxo.Chat
{
  public partial class AltaxoScriptingToolProvider
  {
    /// <summary>Provides values and output handling exposed to a running script.</summary>
    public sealed class ScriptGlobals
    {
      /// <summary>Sets the maximum number of characters retained from script output.</summary>
      private const int MaxOutputChars = 20_000;

      /// <summary>Stores text written by the script.</summary>
      private readonly StringBuilder _out = new();

      /// <summary>Protects the captured output buffer from concurrent script writes.</summary>
      private readonly object _outputLock = new();

      /// <summary>Indicates that the captured output has reached its size limit.</summary>
      private bool _outputTruncated;

      /// <summary>Gets the cancellation token scripts can observe to stop cooperative work.</summary>
      public CancellationToken Cancel { get; init; }

      /// <summary>Gets or initializes an optional callback for mirroring printed lines.</summary>
      internal Action<string>? MirrorOutput { get; init; }

      /// <summary>Writes a value to the script's captured output.</summary>
      /// <param name="o">The value to append; null is written as an empty line.</param>
      public void Print(object? o)
      {
        var text = o?.ToString() ?? string.Empty;
        lock (_outputLock)
        {
          if (!_outputTruncated)
          {
            var line = text + Environment.NewLine;
            var available = MaxOutputChars - _out.Length;
            if (line.Length <= available)
            {
              _out.Append(line);
            }
            else
            {
              _out.Append(line, 0, available);
              _out.Append("\n… [output truncated]");
              _outputTruncated = true;
            }
          }
        }

        MirrorOutput?.Invoke(text);
      }

      // Expose whatever the scripts should reach, e.g. the current Altaxo project.
      // Replace with the real entry point of your API:
      // public Altaxo.Main.Project Project => Altaxo.Current.Project;

      /// <summary>Gets the output accumulated by calls to <see cref="Print(object?)"/>.</summary>
      /// <returns>The captured output text.</returns>
      internal string GetOutput()
      {
        lock (_outputLock)
          return _out.ToString();
      }
    }

    /// <summary>Describes a diagnostic produced while compiling a script.</summary>
    /// <param name="Severity">The diagnostic severity.</param>
    /// <param name="Id">The diagnostic identifier.</param>
    /// <param name="Message">The diagnostic message.</param>
    /// <param name="Line">The one-based source line number.</param>
    /// <param name="Column">The one-based source column number.</param>
    public sealed record ScriptDiagnostic(string Severity, string Id, string Message, int Line, int Column);

    /// <summary>Contains the execution outcome, output, diagnostics, and any exception from a script run.</summary>
    /// <param name="Success">Whether the script completed successfully.</param>
    /// <param name="ReturnValue">The string representation of the script's return value, if any.</param>
    /// <param name="ReturnType">The full name of the return value's runtime type, if any.</param>
    /// <param name="Output">The text captured from calls to the script output function.</param>
    /// <param name="Diagnostics">The diagnostics reported during compilation or execution.</param>
    /// <param name="Exception">The exception details when execution failed with an exception.</param>
    /// <param name="Duration">The elapsed time measured for the run.</param>
    public sealed record ScriptResult(
        bool Success,
        string? ReturnValue,
        string? ReturnType,
        string Output,
        IReadOnlyList<ScriptDiagnostic> Diagnostics,
        string? Exception,
        TimeSpan Duration);

    /// <summary>
    /// Compiles and executes C# scripts with configured assembly references and namespace imports, returning
    /// diagnostics, captured output, execution metadata, and result information.
    /// </summary>
    /// <remarks>Scripts are compiled before execution so diagnostics can be returned without running code
    /// when compilation fails. Execution timeout is enforced for the caller, but running script code can continue in
    /// the background unless it observes cancellation through the provided token.</remarks>
    public sealed class ScriptRunner
    {
      /// <summary>Gets the Roslyn options used to compile and execute scripts.</summary>
      private readonly ScriptOptions _options;

      /// <summary>Creates a script runner using loaded assemblies and standard namespace imports.</summary>
      public ScriptRunner()
        : this(GetDefaultReferences(), new[] { "System", "System.Linq", "Altaxo" })
      {
      }

      /// <summary>Gets loaded assemblies with file-backed metadata for use as script references.</summary>
      /// <returns>The non-dynamic loaded assemblies that have a non-empty location.</returns>
      private static IEnumerable<Assembly> GetDefaultReferences() => AppDomain.CurrentDomain.GetAssemblies()
        .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location));

      /// <summary>Creates a script runner with the specified assembly references and namespace imports.</summary>
      /// <param name="references">The assemblies available to scripts.</param>
      /// <param name="imports">The namespaces imported into scripts.</param>
      public ScriptRunner(IEnumerable<Assembly> references, IEnumerable<string> imports)
      {
        _options = ScriptOptions.Default
            .WithReferences(references)          // e.g. typeof(Altaxo.Data.DataTable).Assembly
            .WithImports(imports)                // e.g. "System", "System.Linq", "Altaxo.Data"
            .WithEmitDebugInformation(true);
      }

      /// <summary>Compiles and executes a script, capturing its output, diagnostics, result, and failures.</summary>
      /// <param name="code">The C# script source to execute.</param>
      /// <param name="timeout">The maximum allowed execution time.</param>
      /// <param name="runOnGuiThread">Whether to start and resume the script on Altaxo's GUI thread.</param>
      /// <param name="mirrorToConsole">Whether calls to <see cref="ScriptGlobals.Print(object?)"/> are also written to the Altaxo console.</param>
      /// <param name="ct">A token that can cancel compilation or execution waiting.</param>
      /// <returns>The script's execution result, including diagnostics and captured output.</returns>
      /// <remarks>A timeout or cancellation cannot forcibly stop arbitrary in-process script code. Such code may continue
      /// running in the background, and code running on the GUI thread can block the application.</remarks>
      public async Task<ScriptResult> RunAsync(
        string code,
        TimeSpan timeout,
        bool runOnGuiThread = true,
        bool mirrorToConsole = false,
        CancellationToken ct = default)
      {
        ArgumentNullException.ThrowIfNull(code);
        if (timeout <= TimeSpan.Zero || timeout == Timeout.InfiniteTimeSpan)
          throw new ArgumentOutOfRangeException(nameof(timeout), "The timeout must be greater than zero and finite.");

        var sw = Stopwatch.StartNew();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);
        var cancellationTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

        var globals = new ScriptGlobals
        {
          Cancel = cts.Token,
          MirrorOutput = mirrorToConsole ? text => Current.Console.WriteLine(text) : null
        };
        var script = CSharpScript.Create(code, _options, typeof(ScriptGlobals));

        // 1) Compile only -> diagnostics without executing anything
        ImmutableArray<Diagnostic> diags;
        try
        {
          diags = script.Compile(cts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
          return CreateTimeoutResult(timeout, globals, Array.Empty<ScriptDiagnostic>(), sw);
        }

        ct.ThrowIfCancellationRequested();
        if (cts.IsCancellationRequested)
          return CreateTimeoutResult(timeout, globals, Array.Empty<ScriptDiagnostic>(), sw);

        var diagList = diags.Select(CreateScriptDiagnostic).ToList();

        if (diags.Any(d => d.Severity == DiagnosticSeverity.Error))
          return new(false, null, null, "", diagList, null, sw.Elapsed);

        // 2) Run. Cancellation is cooperative; a timed-out script may continue in the background.
        var runTask = runOnGuiThread
          ? StartOnGuiThread(script, globals, cts.Token)
          : Task.Run(() => script.RunAsync(globals, cts.Token), CancellationToken.None);
        var finished = await Task.WhenAny(runTask, cancellationTask);
        ct.ThrowIfCancellationRequested();
        if (finished != runTask)
          return CreateTimeoutResult(timeout, globals, diagList, sw);

        try
        {
          var state = await runTask;
          return new(true, state.ReturnValue?.ToString(), state.ReturnValue?.GetType().FullName,
                     globals.GetOutput(), diagList, null, sw.Elapsed);
        }
        catch (CompilationErrorException ex)
        {
          return new(false, null, null, globals.GetOutput(), ex.Diagnostics.Select(CreateScriptDiagnostic).ToList(), null, sw.Elapsed);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
          throw;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
          return CreateTimeoutResult(timeout, globals, diagList, sw);
        }
        catch (Exception ex)
        {
          return new(false, null, null, globals.GetOutput(), diagList, $"{ex.GetType().FullName}: {ex.Message}", sw.Elapsed);
        }
      }

      /// <summary>Starts script execution on Altaxo's GUI synchronization context.</summary>
      /// <param name="script">The compiled script to execute.</param>
      /// <param name="globals">The globals supplied to the script.</param>
      /// <param name="cancellationToken">The token used to request cooperative cancellation.</param>
      /// <returns>A task that completes with the script state or its execution error.</returns>
      private static Task<ScriptState<object?>> StartOnGuiThread(
        Microsoft.CodeAnalysis.Scripting.Script<object?> script,
        ScriptGlobals globals,
        CancellationToken cancellationToken)
      {
        var completion = new TaskCompletionSource<ScriptState<object?>>(TaskCreationOptions.RunContinuationsAsynchronously);
        Current.Dispatcher.SynchronizationContext.Post(async _ =>
        {
          try
          {
            completion.TrySetResult(await script.RunAsync(globals, cancellationToken));
          }
          catch (OperationCanceledException ex)
          {
            completion.TrySetCanceled(ex.CancellationToken);
          }
          catch (Exception ex)
          {
            completion.TrySetException(ex);
          }
        }, null);
        return completion.Task;
      }

      /// <summary>Converts a Roslyn diagnostic to the script runner's serializable diagnostic model.</summary>
      /// <param name="diagnostic">The Roslyn diagnostic to convert.</param>
      /// <returns>The converted diagnostic with one-based source coordinates.</returns>
      private static ScriptDiagnostic CreateScriptDiagnostic(Diagnostic diagnostic)
      {
        var position = diagnostic.Location.GetLineSpan().StartLinePosition;
        return new ScriptDiagnostic(diagnostic.Severity.ToString(), diagnostic.Id, diagnostic.GetMessage(), position.Line + 1, position.Character + 1);
      }

      /// <summary>Creates a failed result describing a timeout and any output captured before it occurred.</summary>
      /// <param name="timeout">The configured time limit.</param>
      /// <param name="globals">The script globals containing captured output.</param>
      /// <param name="diagnostics">The diagnostics collected before execution timed out.</param>
      /// <param name="sw">The stopwatch measuring elapsed execution time.</param>
      /// <returns>A timeout result including the captured output and elapsed duration.</returns>
      private static ScriptResult CreateTimeoutResult(TimeSpan timeout, ScriptGlobals globals, IReadOnlyList<ScriptDiagnostic> diagnostics, Stopwatch sw) =>
        new(false, null, null, globals.GetOutput(), diagnostics,
          $"Timeout after {timeout}. The script may still be running in the background unless it observes cancellation.", sw.Elapsed);
    }
  }
}
