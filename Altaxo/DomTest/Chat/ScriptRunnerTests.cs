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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Chat
{
  public class ScriptRunnerTests
  {
    [Fact]
    public async Task RunAsyncCapturesOutputAndReturnsLastValue()
    {
      var runner = new AltaxoScriptingToolProvider.ScriptRunner();

      var result = await runner.RunAsync("Print(1 + 1); 40 + 2", TimeSpan.FromSeconds(10), runOnGuiThread: false);

      Assert.True(result.Success, result.Exception);
      Assert.Equal("42", result.ReturnValue);
      Assert.Equal($"2{Environment.NewLine}", result.Output);
    }

    [Fact]
    public async Task RunAsyncBoundsCapturedOutput()
    {
      var runner = new AltaxoScriptingToolProvider.ScriptRunner();

      var result = await runner.RunAsync("Print(new string('x', 30000))", TimeSpan.FromSeconds(10), runOnGuiThread: false);

      Assert.True(result.Success, result.Exception);
      Assert.Contains("[output truncated]", result.Output);
      Assert.True(result.Output.Length < 20_100);
    }

    [Fact]
    public async Task RunAsyncPropagatesCallerCancellation()
    {
      var runner = new AltaxoScriptingToolProvider.ScriptRunner();
      using var cancellation = new CancellationTokenSource();

      var runTask = runner.RunAsync(
        "await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, Cancel)",
        TimeSpan.FromSeconds(30),
        runOnGuiThread: false,
        ct: cancellation.Token);
      cancellation.CancelAfter(TimeSpan.FromMilliseconds(100));

      await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);
    }

    [Fact]
    public async Task RunAsyncReportsTimeoutSeparatelyFromCallerCancellation()
    {
      var runner = new AltaxoScriptingToolProvider.ScriptRunner();

      var result = await runner.RunAsync(
        "await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, Cancel)",
        TimeSpan.FromMilliseconds(100),
        runOnGuiThread: false);

      Assert.False(result.Success);
      Assert.Contains("Timeout after", result.Exception);
    }
  }
}
