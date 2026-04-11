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

#nullable enable
using System;
using Altaxo.Main.Services.Implementation;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Defines the severity of a message.
  /// </summary>
  public enum MessageLevel
  {
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Error message.
    /// </summary>
    Error
  }

  /// <summary>
  /// Provides methods for reporting messages, prompting the user, and displaying exceptions.
  /// </summary>
  [GlobalService("SD.MessageService", FallbackImplementation = typeof(FallbackMessageService))]
  public interface IMessageService
  {
    /// <summary>
    /// Shows an exception.
    /// </summary>
    /// <param name="ex">The exception to show.</param>
    /// <param name="message">An optional message to display together with the exception.</param>
    void ShowException(Exception ex, string? message = null);

    /// <summary>
    /// Shows an exception.
    /// </summary>
    /// <param name="ex">The handled exception to show.</param>
    /// <param name="message">An optional message to display together with the exception.</param>
    void ShowHandledException(Exception ex, string? message = null);

    /// <summary>
    /// Writes a message line.
    /// </summary>
    /// <param name="level">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="message">The message text.</param>
    void WriteLine(MessageLevel level, string source, string message);

    /// <summary>
    /// Writes a formatted message line.
    /// </summary>
    /// <param name="messageLevel">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args);

    /// <summary>
    /// Writes a formatted message line using the specified format provider.
    /// </summary>
    /// <param name="messageLevel">The message severity.</param>
    /// <param name="source">The message source.</param>
    /// <param name="provider">The format provider.</param>
    /// <param name="format">The composite format string.</param>
    /// <param name="args">The format arguments.</param>
    void WriteLine(MessageLevel messageLevel, string source, System.IFormatProvider provider, string format, params object[] args);

    /// <summary>
    /// Shows an error.
    /// </summary>
    /// <param name="message">The error message text.</param>
    void ShowError(string message);

    /// <summary>
    /// Shows an error using a message box.
    /// <paramref name="formatstring"/> is first passed through the
    /// <see cref="StringParser"/>,
    /// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
    /// </summary>
    /// <param name="formatstring">The resource or format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    void ShowErrorFormatted(string formatstring, params object[] formatitems);

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The warning message text.</param>
    void ShowWarning(string message);

    /// <summary>
    /// Shows a warning message.
    /// <paramref name="formatstring"/> is first passed through the
    /// <see cref="StringParser"/>,
    /// then through <see cref="string.Format(string, object)"/>, using the formatitems as arguments.
    /// </summary>
    /// <param name="formatstring">The resource or format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    void ShowWarningFormatted(string formatstring, params object[] formatitems);

    /// <summary>
    /// Shows a message.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="caption">The dialog caption.</param>
    void ShowMessage(string message, string? caption = null);

    /// <summary>
    /// Shows a formatted message.
    /// </summary>
    /// <param name="formatstring">The resource or format string.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="formatitems">The format arguments.</param>
    void ShowMessageFormatted(string formatstring, string? caption, params object[] formatitems);

    /// <summary>
    /// Asks the user a Yes/No question, using "Yes" as the default button.
    /// Returns <c>true</c> if yes was clicked, <c>false</c> if no was clicked.
    /// </summary>
    /// <param name="question">The question text.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <returns><see langword="true"/> if the user answered Yes; otherwise, <see langword="false"/>.</returns>
    bool AskQuestion(string question, string? caption = null);

    /// <summary>
    /// Shows a custom dialog.
    /// </summary>
    /// <param name="caption">The title of the dialog.</param>
    /// <param name="dialogText">The description shown in the dialog.</param>
    /// <param name="acceptButtonIndex">
    /// The number of the button that is the default accept button.
    /// Use -1 if you don't want to have an accept button.
    /// </param>
    /// <param name="cancelButtonIndex">
    /// The number of the button that is the cancel button.
    /// Use -1 if you don't want to have a cancel button.
    /// </param>
    /// <param name="buttontexts">The captions of the buttons.</param>
    /// <returns>The number of the button that was clicked, or -1 if the dialog was closed  without clicking a button.</returns>
    int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts);

    /// <summary>
    /// Shows an input box.
    /// </summary>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="dialogText">The text shown in the dialog.</param>
    /// <param name="defaultValue">The initial input value.</param>
    /// <returns>The entered string.</returns>
    string ShowInputBox(string caption, string dialogText, string defaultValue);

    /// <summary>
    /// Gets the default message box title.
    /// </summary>
    string DefaultMessageBoxTitle { get; }

    /// <summary>
    /// Gets the application product name.
    /// </summary>
    string ProductName { get; }

    /// <summary>
    /// Show a message informing the user about a save error.
    /// </summary>
    /// <param name="fileName">The file name involved in the save error.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="dialogName">The dialog title or resource name.</param>
    /// <param name="exceptionGot">The exception that caused the save error.</param>
    void InformSaveError(PathName fileName, string message, string dialogName, Exception exceptionGot);

    /// <summary>
    /// Show a message informing the user about a save error,
    /// and allow him to retry/save under alternative name.
    /// </summary>
    /// <param name="fileName">The file name involved in the save error.</param>
    /// <param name="message">The message to show.</param>
    /// <param name="dialogName">The dialog title or resource name.</param>
    /// <param name="exceptionGot">The exception that caused the save error.</param>
    /// <param name="chooseLocationEnabled"><see langword="true"/> to enable choosing an alternative save location; otherwise, <see langword="false"/>.</param>
    /// <returns>The action chosen by the user.</returns>
    ChooseSaveErrorResult ChooseSaveError(PathName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled);
  }

  /// <summary>
  /// Fallback implementation of <see cref="IMessageService"/> that writes to a text writer.
  /// </summary>
  internal sealed class FallbackMessageService : TextWriterMessageService
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FallbackMessageService"/> class.
    /// </summary>
    public FallbackMessageService() : base(Console.Out)
    {
    }
  }

  /// <summary>
  /// Represents a user's choice when handling a save error.
  /// </summary>
  public sealed class ChooseSaveErrorResult
  {
    /// <summary>
    /// Gets a value indicating whether the operation should be retried.
    /// </summary>
    public bool IsRetry { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the error should be ignored.
    /// </summary>
    public bool IsIgnore { get; private set; }

    /// <summary>
    /// Gets a value indicating whether an alternative file name was chosen.
    /// </summary>
    public bool IsSaveAlternative { get { return AlternativeFileName is not null; } }

    /// <summary>
    /// Gets the alternative file name chosen by the user.
    /// </summary>
    public PathName? AlternativeFileName { get; private set; }

    private ChooseSaveErrorResult()
    {
    }

    /// <summary>
    /// Gets the result that indicates a retry should be performed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ChooseSaveErrorResult is immutable")]
    public static readonly ChooseSaveErrorResult Retry = new ChooseSaveErrorResult { IsRetry = true };

    /// <summary>
    /// Gets the result that indicates the error should be ignored.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ChooseSaveErrorResult is immutable")]
    public static readonly ChooseSaveErrorResult Ignore = new ChooseSaveErrorResult { IsIgnore = true };

    /// <summary>
    /// Creates a result that indicates an alternative file name should be used.
    /// </summary>
    /// <param name="alternativeFileName">The alternative file name.</param>
    /// <returns>A save-error result for the alternative file name.</returns>
    public static ChooseSaveErrorResult SaveAlternative(PathName alternativeFileName)
    {
      if (alternativeFileName is null)
        throw new ArgumentNullException("alternativeFileName");
      return new ChooseSaveErrorResult { AlternativeFileName = alternativeFileName };
    }
  }
}
