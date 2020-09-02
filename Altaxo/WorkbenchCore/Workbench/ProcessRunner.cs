// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Main.Services;
using Microsoft.Win32.SafeHandles;

namespace Altaxo.Workbench
{
  [Flags]
  public enum ProcessCreationFlags
  {
    None = 0,

    /// <summary>
    /// Creates a new console instead of inheriting the parent console.
    /// </summary>
    CreateNewConsole = 0x00000010,

    /// <summary>
    /// Launches a console application without a console window.
    /// </summary>
    CreateNoWindow = 0x08000000
  }

  public interface IProcessRunner : IDisposable
  {
    Task<int> RunInOutputPadAsync(IOutputCategory outputCategory, string program, params string[] arguments);

    string? WorkingDirectory { get; set; }
    ProcessCreationFlags CreationFlags { get; set; }
    IDictionary<string, string?> EnvironmentVariables { get; }
    bool RedirectStandardOutput { get; set; }
    bool RedirectStandardError { get; set; }
    bool RedirectStandardOutputAndErrorToSingleStream { get; set; }

    void Start(string program, params string[] arguments);

    void StartCommandLine(string commandLine);

    void Kill();

    Task WaitForExitAsync();

    Stream StandardOutput { get; }
    Stream StandardError { get; }

    StreamReader OpenStandardOutputReader();

    StreamReader OpenStandardErrorReader();
  }

  /// <summary>
  /// Class for starting external processes.
  /// Very similar to System.Diagnostics.Process, but supports binary stdout/stderr (not only text),
  /// and allows using the same pipe for both stdout and stderr.
  ///
  /// Also, implements an interface to support mocking in unit tests.
  /// </summary>
  public class ProcessRunner : IProcessRunner, IDisposable
  {
    public static Encoding OemEncoding
    {
      get
      {
        return NativeMethods.OemEncoding;
      }
    }

    #region SafeProcessHandle

    [SecurityCritical]
    private sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      // this private ctor is required for SafeHandle implementations
      private SafeProcessHandle() : base(true)
      {
      }

      internal SafeProcessHandle(IntPtr handle) : base(true)
      {
        base.SetHandle(handle);
      }

      [SecurityCritical]
      protected override bool ReleaseHandle()
      {
        return NativeMethods.CloseHandle(handle);
      }
    }

    #endregion SafeProcessHandle

    #region Native structures

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    protected struct STARTUPINFO
    {
      public uint cb;
      public string lpReserved;
      public string lpDesktop;
      public string lpTitle;
      public uint dwX;
      public uint dwY;
      public uint dwXSize;
      public uint dwYSize;
      public uint dwXCountChars;
      public uint dwYCountChars;
      public uint dwFillAttribute;
      public uint dwFlags;
      public short wShowWindow;
      public short cbReserved2;
      public IntPtr lpReserved2;
      public SafePipeHandle hStdInput;
      public SafePipeHandle hStdOutput;
      public SafePipeHandle hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    protected struct PROCESS_INFORMATION
    {
      public IntPtr hProcess;
      public IntPtr hThread;
      public int dwProcessId;
      public int dwThreadId;
    }

    #endregion Native structures

    #region Native methods

    [DllImport("kernel32.dll", EntryPoint = "CreateProcess", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeCreateProcess(
      string lpApplicationName,
      StringBuilder lpCommandLine,
      IntPtr lpProcessAttributes,
      IntPtr lpThreadAttributes,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
      uint dwCreationFlags,
      string lpEnvironment,
      string lpCurrentDirectory,
      [In] ref STARTUPINFO lpStartupInfo,
      out PROCESS_INFORMATION lpProcessInformation
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TerminateProcess(SafeProcessHandle processHandle, int exitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetExitCodeProcess(SafeProcessHandle processHandle, out int exitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr hMem);

    #endregion Native methods

    #region CommandLine <-> Argument Array

    private static readonly char[] charsNeedingQuoting = { ' ', '\t', '\n', '\v', '"' };

    /// <summary>
    /// Escapes a set of arguments according to the CommandLineToArgvW rules.
    /// </summary>
    /// <remarks>
    /// Command line parsing rules:
    /// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
    /// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
    /// - n backslashes not followed by a quotation mark simply produce n backslashes.
    /// </remarks>
    public static string? ArgumentArrayToCommandLine(params string[] arguments)
    {
      if (arguments is null)
        return null;
      var b = new StringBuilder();
      for (int i = 0; i < arguments.Length; i++)
      {
        if (i > 0)
          b.Append(' ');
        AppendArgument(b, arguments[i]);
      }
      return b.ToString();
    }

    private static void AppendArgument(StringBuilder b, string arg)
    {
      if (arg.Length > 0 && arg.IndexOfAny(charsNeedingQuoting) < 0)
      {
        b.Append(arg);
      }
      else
      {
        b.Append('"');
        for (int j = 0; ; j++)
        {
          int backslashCount = 0;
          while (j < arg.Length && arg[j] == '\\')
          {
            backslashCount++;
            j++;
          }
          if (j == arg.Length)
          {
            b.Append('\\', backslashCount * 2);
            break;
          }
          else if (arg[j] == '"')
          {
            b.Append('\\', backslashCount * 2 + 1);
            b.Append('"');
          }
          else
          {
            b.Append('\\', backslashCount);
            b.Append(arg[j]);
          }
        }
        b.Append('"');
      }
    }

    #endregion CommandLine <-> Argument Array

    #region RunInOutputPad

    public Task<int> RunInOutputPadAsync(IOutputCategory outputCategory, string program, params string[] arguments)
    {
      RedirectStandardOutputAndErrorToSingleStream = true;
      Start(program, arguments);
      var printedCommandLine = new StringBuilder();
      if (WorkingDirectory is not null)
      {
        printedCommandLine.Append(WorkingDirectory);
        printedCommandLine.Append("> ");
      }
      printedCommandLine.Append(CommandLine);
      outputCategory.AppendLine(printedCommandLine.ToString());

      using (TextReader reader = OpenStandardOutputReader())
      {
        throw new NotImplementedException("Needs investigation");
        //await reader.CopyToAsync(new MessageViewCategoryTextWriter(outputCategory));
      }
      // await WaitForExitAsync();
      // outputCategory.AppendLine(StringParser.Parse("${res:XML.MainMenu.ToolMenu.ExternalTools.ExitedWithCode} " + ExitCode));
      // return ExitCode;
    }

    #endregion RunInOutputPad

    #region Start Info Properties

    /// <summary>
    /// Gets or sets the process's working directory.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    private ProcessCreationFlags creationFlags = ProcessCreationFlags.CreateNoWindow;

    public ProcessCreationFlags CreationFlags
    {
      get { return creationFlags; }
      set { creationFlags = value; }
    }

    private IDictionary<string, string?>? _environmentVariables;

    public IDictionary<string, string?> EnvironmentVariables
    {
      get
      {
        if (_environmentVariables is null)
        {
          _environmentVariables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
          foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
          {
            _environmentVariables.Add((string)e.Key, (string?)e.Value);
          }
        }
        return _environmentVariables;
      }
    }

    public string? CommandLine { get; private set; }

    public bool RedirectStandardOutput { get; set; }
    public bool RedirectStandardError { get; set; }

    /// <summary>
    /// Gets whether to use a single stream for both stdout and stderr.
    /// </summary>
    public bool RedirectStandardOutputAndErrorToSingleStream { get; set; }

    #endregion Start Info Properties

    #region Start

    private bool _wasStarted;
    private SafeProcessHandle? _safeProcessHandle;

    public void Start(string program, params string[] arguments)
    {
      var commandLine = new StringBuilder();
      AppendArgument(commandLine, program);
      if (arguments is not null)
      {
        for (int i = 0; i < arguments.Length; i++)
        {
          commandLine.Append(' ');
          AppendArgument(commandLine, arguments[i]);
        }
      }
      StartCommandLine(commandLine.ToString());
    }

    public void StartCommandLine(string commandLine)
    {
      lock (_lockObj)
      {
        if (_wasStarted)
          throw new InvalidOperationException();
        DoStart(commandLine);
      }
    }

    protected virtual void DoStart(string commandLine)
    {
      CommandLine = commandLine;

      const uint STARTF_USESTDHANDLES = 0x00000100;

      const int STD_INPUT_HANDLE = -10;
      const int STD_OUTPUT_HANDLE = -11;
      const int STD_ERROR_HANDLE = -12;

      const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

      var startupInfo = new STARTUPINFO
      {
        cb = (uint)Marshal.SizeOf(typeof(STARTUPINFO)),
        dwFlags = STARTF_USESTDHANDLES,

        // Create pipes
        hStdInput = new SafePipeHandle(GetStdHandle(STD_INPUT_HANDLE), ownsHandle: false)
      };
      if (RedirectStandardOutput || RedirectStandardOutputAndErrorToSingleStream)
      {
        _standardOutput = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        startupInfo.hStdOutput = _standardOutput.ClientSafePipeHandle;
      }
      else
      {
        startupInfo.hStdOutput = new SafePipeHandle(GetStdHandle(STD_OUTPUT_HANDLE), ownsHandle: false);
      }
      if (RedirectStandardOutputAndErrorToSingleStream)
      {
        _standardError = _standardOutput;
        if (_standardError is not null)
          startupInfo.hStdError = _standardError.ClientSafePipeHandle;
      }
      else if (RedirectStandardError)
      {
        _standardError = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        startupInfo.hStdError = _standardError.ClientSafePipeHandle;
      }
      else
      {
        startupInfo.hStdError = new SafePipeHandle(GetStdHandle(STD_ERROR_HANDLE), ownsHandle: false);
      }

      uint flags = (uint)CreationFlags;

      string? environmentBlock = null;
      if (_environmentVariables is not null)
      {
        environmentBlock = BuildEnvironmentBlock(_environmentVariables);
        flags |= CREATE_UNICODE_ENVIRONMENT;
      }

      var processInfo = new PROCESS_INFORMATION();
      try
      {
        CreateProcess(string.Empty, new StringBuilder(commandLine), IntPtr.Zero, IntPtr.Zero, true, flags, environmentBlock!, WorkingDirectory!, ref startupInfo, out processInfo);
        _wasStarted = true;
      }
      finally
      {
        if (processInfo.hProcess != IntPtr.Zero && processInfo.hProcess != new IntPtr(-1))
        {
          _safeProcessHandle = new SafeProcessHandle(processInfo.hProcess);
        }
        if (processInfo.hThread != IntPtr.Zero && processInfo.hThread != new IntPtr(-1))
        {
          NativeMethods.CloseHandle(processInfo.hThread);
        }
        // Dispose the client side handles of the pipe.
        // They got copied into the new process, we don't need our local copies anymore.
        startupInfo.hStdInput.Dispose();
        startupInfo.hStdOutput.Dispose();
        startupInfo.hStdError?.Dispose();
        if (!_wasStarted)
        {
          // In case of error, dispose the server side of the pipes as well
          if (_standardOutput is not null)
          {
            _standardOutput.Dispose();
            _standardOutput = null;
          }
          if (_standardError is not null)
          {
            _standardError.Dispose();
            _standardError = null;
          }
        }
      }
      //StartStreamCopyAfterProcessCreation();
    }

    private static string BuildEnvironmentBlock(IEnumerable<KeyValuePair<string, string?>> environment)
    {
      var b = new StringBuilder();
      foreach (var pair in environment.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
      {
        b.Append(pair.Key);
        b.Append('=');
        b.Append(pair.Value);
        b.Append('\0');
      }
      b.Append('\0');
      return b.ToString();
    }

    protected virtual void CreateProcess(
      string lpApplicationName,
      StringBuilder lpCommandLine,
      IntPtr lpProcessAttributes,
      IntPtr lpThreadAttributes,
      bool bInheritHandles,
      uint dwCreationFlags,
      string lpEnvironment,
      string lpCurrentDirectory,
      ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation)
    {
      if (!NativeCreateProcess(lpApplicationName, lpCommandLine, lpProcessAttributes, lpThreadAttributes, bInheritHandles, dwCreationFlags,
                               lpEnvironment, lpCurrentDirectory, ref lpStartupInfo, out lpProcessInformation))
      {
        throw new Win32Exception();
      }
    }

    #endregion Start

    public void Dispose()
    {
      if (_safeProcessHandle is not null)
        _safeProcessHandle.Dispose();
      if (_standardOutput is not null)
        _standardOutput.Dispose();
      if (_standardError is not null)
        _standardError.Dispose();
    }

    #region HasExited / ExitCode / Kill

    public bool HasExited
    {
      get { return WaitForExit(0); }
    }

    /// <summary>
    /// Gets the process exit code.
    /// </summary>
    public int ExitCode
    {
      get
      {
        if (!WaitForExit(0))
          throw new InvalidOperationException("Process has not yet exited");
        return exitCode; // WaitForExit has the side effect of setting exitCode
      }
    }

    /// <summary>
    /// Sends the kill signal to the process.
    /// Does not wait for the process to complete to exit after being killed.
    /// </summary>
    public void Kill()
    {
      if (!_wasStarted)
        throw new InvalidOperationException("Process was not started");
      if (!TerminateProcess(_safeProcessHandle!, -1))
      {
        int err = Marshal.GetLastWin32Error();
        // If TerminateProcess fails, maybe it's because the process has already exited.
        if (!WaitForExit(0))
          throw new Win32Exception(err);
      }
    }

    #endregion HasExited / ExitCode / Kill

    #region WaitForExit

    private sealed class ProcessWaitHandle : WaitHandle
    {
      public ProcessWaitHandle(SafeProcessHandle processHandle)
      {
        var currentProcess = new HandleRef(this, NativeMethods.GetCurrentProcess());
        if (!NativeMethods.DuplicateHandle(currentProcess, processHandle, currentProcess, out var safeWaitHandle, 0, false, NativeMethods.DUPLICATE_SAME_ACCESS))
        {
          throw new Win32Exception();
        }
        base.SafeWaitHandle = safeWaitHandle;
      }
    }

    private bool hasExited;
    private int exitCode;

    public void WaitForExit()
    {
      WaitForExit(Timeout.Infinite);
    }

    public bool WaitForExit(int millisecondsTimeout)
    {
      if (hasExited)
        return true;
      if (!_wasStarted)
        throw new InvalidOperationException("Process was not yet started");
      if (_safeProcessHandle!.IsClosed)
        throw new ObjectDisposedException("ProcessRunner");
      using (var waitHandle = new ProcessWaitHandle(_safeProcessHandle))
      {
        if (waitHandle.WaitOne(millisecondsTimeout, false))
        {
          if (!GetExitCodeProcess(_safeProcessHandle, out exitCode))
            throw new Win32Exception();
          // Wait until the output is processed
          //					if (standardOutputTask != null)
          //						standardOutputTask.Wait();
          //					if (standardErrorTask != null)
          //						standardErrorTask.Wait();
          hasExited = true;
        }
      }
      return hasExited;
    }

    private readonly object _lockObj = new object();
    private TaskCompletionSource<object?>? _waitForExitTCS;
    private ProcessWaitHandle? _waitForExitAsyncWaitHandle;
    private RegisteredWaitHandle? _waitForExitAsyncRegisteredWaitHandle;

    /// <summary>
    /// Asynchronously waits for the process to exit.
    /// </summary>
    public Task WaitForExitAsync()
    {
      if (hasExited)
        return Task.FromResult(true);
      if (!_wasStarted)
        throw new InvalidOperationException("Process was not yet started");
      if (_safeProcessHandle!.IsClosed)
        throw new ObjectDisposedException("ProcessRunner");
      lock (_lockObj)
      {
        if (_waitForExitTCS is null)
        {
          _waitForExitTCS = new TaskCompletionSource<object?>();
          _waitForExitAsyncWaitHandle = new ProcessWaitHandle(_safeProcessHandle);
          _waitForExitAsyncRegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitForExitAsyncWaitHandle, WaitForExitAsyncCallback, null, -1, true);
        }
        return _waitForExitTCS.Task;
      }
    }

    private void WaitForExitAsyncCallback(object? context, bool wasSignaled)
    {
      _waitForExitAsyncRegisteredWaitHandle?.Unregister(null);
      _waitForExitAsyncRegisteredWaitHandle = null;
      _waitForExitAsyncWaitHandle?.Close();
      _waitForExitAsyncWaitHandle = null;
      // Wait until the output is processed
      //			if (standardOutputTask != null)
      //				await standardOutputTask;
      //			if (standardErrorTask != null)
      //				await standardErrorTask;
      _waitForExitTCS?.SetResult((object?)null);
    }

    #endregion WaitForExit

    #region StandardOutput/StandardError

    private AnonymousPipeServerStream? _standardOutput;
    private AnonymousPipeServerStream? _standardError;

    public Stream StandardOutput
    {
      get
      {
        if (_standardOutput is null)
          throw new InvalidOperationException(_wasStarted ? "stdout was not redirected" : "Process not yet started");
        return _standardOutput;
      }
    }

    public Stream StandardError
    {
      get
      {
        if (_standardError is null)
          throw new InvalidOperationException(_wasStarted ? "stderr was not redirected" : "Process not yet started");
        return _standardError;
      }
    }

    /// <summary>
    /// Opens a text reader around the standard output.
    /// </summary>
    public StreamReader OpenStandardOutputReader()
    {
      return new StreamReader(StandardOutput, OemEncoding);
    }

    /// <summary>
    /// Opens a text reader around the standard error.
    /// </summary>
    public StreamReader OpenStandardErrorReader()
    {
      return new StreamReader(StandardError, OemEncoding);
    }

    #endregion StandardOutput/StandardError
  }
}
