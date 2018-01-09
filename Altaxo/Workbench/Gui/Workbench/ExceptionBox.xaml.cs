using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Interaction logic for SaveErrorChooseDialog.xaml
	/// </summary>
	public partial class ExceptionBox : Window
	{
		private Exception exceptionThrown;
		private string message;

		public ExceptionBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Creates a new ExceptionBox instance.
		/// </summary>
		/// <param name="exception">The exception to display</param>
		/// <param name="message">An additional message to display</param>
		/// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the
		/// continue button is not available.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public ExceptionBox(Exception exception, string message, bool mustTerminate)
		{
			this.exceptionThrown = exception;
			this.message = message;
			InitializeComponent();
			if (mustTerminate)
			{
				continueButton.Visibility = Visibility.Hidden;
			}

			exceptionTextBox.Text = getClipboardString();

			try
			{
				InitializeText();
			}
			catch
			{
			}
		}

		public static void RegisterExceptionBoxForUnhandledExceptions()
		{
			System.Windows.Forms.Application.ThreadException += ShowErrorBox;
			AppDomain.CurrentDomain.UnhandledException += ShowErrorBox;
			System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += Dispatcher_UnhandledException;
		}

		private static void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Current.Log.Error("Unhandled WPF exception", e.Exception);
			ShowErrorBox(e.Exception, "Unhandled WPF exception", false);
			e.Handled = true;
		}

		private static void ShowErrorBox(object sender, ThreadExceptionEventArgs e)
		{
			Current.Log.Error("ThreadException caught", e.Exception);
			ShowErrorBox(e.Exception, null);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
		private static void ShowErrorBox(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			Current.Log.Fatal("UnhandledException caught", ex);
			if (e.IsTerminating)
				Current.Log.Fatal("Runtime is terminating because of unhandled exception.");
			ShowErrorBox(ex, "Unhandled exception", e.IsTerminating);
		}

		/// <summary>
		/// Displays the exception box.
		/// </summary>
		public static void ShowErrorBox(Exception exception, string message)
		{
			ShowErrorBox(exception, message, false);
		}

		[ThreadStatic]
		private static bool showingBox;

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
		{
			// ignore reentrant calls (e.g. when there's an exception in OnRender)
			if (showingBox)
				return;
			showingBox = true;
			try
			{
				if (exception != null)
				{
					try
					{
					}
					catch (Exception ex)
					{
						Current.Log.Warn("Error tracking exception", ex);
					}
				}
				ExceptionBox box = new ExceptionBox(exception, message, mustTerminate);
				{
					box.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				Current.Log.Warn("Error showing ExceptionBox", ex);
				MessageBox.Show(
						exception != null ? exception.ToString() : "Error",
						message,
						MessageBoxButton.OK,
						MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
			}
			finally
			{
				showingBox = false;
			}
		}

		public void InitializeText()
		{
			this.Title = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.Title}");
			closeButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ExitAltaxo}");
			label3.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ThankYouMsg}");
			label2.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.HelpText2}");
			label.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.HelpText1}");
			continueButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.Continue}");
			reportButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ReportError}");
			copyErrorCheckBox.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.CopyToClipboard}");
			if (PresentationResourceService.InstanceAvailable)
				this._guiImage.Source = PresentationResourceService.GetBitmapSource("Altaxo.Gui.ExceptionBox.Image");
		}

		private void EhCloseButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBoxResult.Yes == MessageBox.Show(
					StringParser.Parse("${res:ICSharpCode.SharpDevelop.ExceptionBox.QuitWarning}"),
					MessageService.ProductName,
					MessageBoxButton.YesNo,
					MessageBoxImage.Question,
					MessageBoxResult.No,
					MessageBoxOptions.DefaultDesktopOnly))

			{
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}

		private void EhContinueButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void EhReportButton_Click(object sender, RoutedEventArgs e)
		{
			CopyInfoToClipboard();

			StartUrl("https://github.com/Altaxo/altaxo/issues");
		}

		private void CopyInfoToClipboard()
		{
			if (true == copyErrorCheckBox.IsChecked)
			{
				string exceptionText = exceptionTextBox.Text;
				if (System.Windows.Forms.Application.OleRequired() == ApartmentState.STA)
				{
					Clipboard.SetText(exceptionText);
				}
				else
				{
					Thread th = new Thread((ThreadStart)delegate
					{
						Clipboard.SetText(exceptionText);
					});
					th.Name = "CopyInfoToClipboard";
					th.SetApartmentState(ApartmentState.STA);
					th.Start();
				}
			}
		}

		private string getClipboardString()
		{
			StringBuilder sb = new StringBuilder();

			Version version = Assembly.GetEntryAssembly().GetName().Version;
			string versionText = string.Format("Altaxo {0}.{1} build {2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

			sb.Append(versionText);

			sb.AppendLine();

			if (message != null)
			{
				sb.AppendLine(message);
			}
			if (exceptionThrown != null)
			{
				sb.AppendLine("Exception thrown:");
				sb.AppendLine(exceptionThrown.ToString());
			}
			sb.AppendLine();
			sb.AppendLine("---- Recent log messages:");
			try
			{
				//LogMessageRecorder.AppendRecentLogMessages(sb, log4net.LogManager.GetLogger(typeof(log4netLoggingService)));
			}
			catch (Exception ex)
			{
				sb.AppendLine("Failed to append recent log messages.");
				sb.AppendLine(ex.ToString());
			}
			sb.AppendLine();
			sb.AppendLine("---- Post-error application state information:");
			try
			{
				Altaxo.Current.GetRequiredService<ApplicationStateInfoService>().AppendFormatted(sb);
			}
			catch (Exception ex)
			{
				sb.AppendLine("Failed to append application state information.");
				sb.AppendLine(ex.ToString());
			}
			return sb.ToString();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static void StartUrl(string url)
		{
			try
			{
				System.Diagnostics.Process.Start(url);
			}
			catch (Exception e)
			{
				Current.Log.Warn("Cannot start " + url, e);
			}
		}
	}
}