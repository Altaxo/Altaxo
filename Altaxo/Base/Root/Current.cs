using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Main.Services;
using Altaxo.Gui.Common;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Altaxo.Gui.Workbench;

namespace Altaxo
{
	public partial class Current
	{
		/// <summary>
		/// Avoids instantiation of this class from the outside, but allows other Current classes inherit from this class and add new static methods.
		/// </summary>
		protected Current()
		{
		}

		/// <summary>
		/// Occurs when a service was added or removed. Static classes that cache a service should invalidate their cached service member in response to this event.
		/// Attention: Only static classes should subscribe to this event! (Or use a weak event handler in order to avoid memory leaks).
		/// </summary>
		public static event Action ServiceChanged;

		// Cached services (if you add something, be sure to add it to InvalidateCachedServices too)
		private static AddInItems.IAddInTree _addInTree;

		private static Main.IComManager _comManager;
		private static IGuiFactoryService _guiFactoryService;
		private static IGuiTimerService _guiTimerService;
		private static IInfoWarningErrorTextMessageService _infoTextMessageService;
		private static ILoggingService _loggingService;
		private static IMessageService _messageService;
		private static ITextOutputService _outputService;
		private static Main.IProjectService _projectService;
		private static Main.Services.IPropertyService _propertyService;
		private static Main.Services.IResourceService _resourceService;
		private static IStatusBarService _statusBarService;
		private static IDispatcherMessageLoop _dispatcher;
		private static IWorkbench _workbench;

		private static void InvalidateCachedServices()
		{
			_addInTree = null;
			_comManager = null;
			_guiFactoryService = null;
			_guiTimerService = null;
			_infoTextMessageService = null;
			_loggingService = null;
			_outputService = null;
			_messageService = null;
			_outputService = null;
			_projectService = null;
			_propertyService = null;
			_resourceService = null;
			_statusBarService = null;
			_dispatcher = null;
			_workbench = null;
		}

		#region Unspecified services

		/// <summary>The service provider that is used when the "real" service provider is not already set.</summary>
		public static readonly IServiceProvider fallbackServiceProvider = new FallbackServiceProvider();

		/// <summary>The service provider that is used. Initially set to the fallback service provider.</summary>
		private static volatile IServiceProvider instance = fallbackServiceProvider;

		public static IServiceProvider FallbackServiceProvider { get { return fallbackServiceProvider; } }

		/// <summary>
		/// Gets the main service container.
		/// </summary>
		public static IServiceContainer Services
		{
			protected get { return GetRequiredService<IServiceContainer>(); }
			set
			{
				instance = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		public static void AddService<T>(T service)
		{
			if (instance is IServiceContainer container)
			{
				container.AddService(typeof(T), service);
			}

			InvalidateCachedServices(); // Invalidate our own cached services
			ServiceChanged?.Invoke();
		}

		public static void AddService<T, U>(T service)
		{
			if (instance is IServiceContainer container)
			{
				container.AddService(typeof(T), service);
				container.AddService(typeof(U), service);
			}

			InvalidateCachedServices(); // Invalidate our own cached services
			ServiceChanged?.Invoke();
		}

		public static void AddService<T, U, V>(T service)
		{
			if (instance is IServiceContainer container)
			{
				container.AddService(typeof(T), service);
				container.AddService(typeof(U), service);
				container.AddService(typeof(V), service);
			}

			InvalidateCachedServices(); // Invalidate our own cached services
			ServiceChanged?.Invoke();
		}

		public static void RemoveService<T>()
		{
			Services.RemoveService(typeof(T));
			InvalidateCachedServices(); // Invalidate our own cached services
			ServiceChanged?.Invoke();
		}

		public static void DisposeServicesAll()
		{
			(instance as IDisposable)?.Dispose();
		}

		/// <summary>
		/// Retrieves the service of type <typeparamref name="T"/> from the provider.
		/// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
		/// </summary>
		public static T GetRequiredService<T>()
		{
			object service = instance.GetService(typeof(T));
			if (service == null)
				throw new ServiceNotFoundException(typeof(T));
			return (T)service;
		}

		/// <summary>
		/// Gets the required service. The service is primarily being searched with key <typeparamref name="T"/>. If it is not found with this key,
		/// it is search with key <typeparamref name="U"/>, thus <typeparamref name="U"/> has to be a base class or an interface of type <typeparamref name="T"/>.
		/// If the service is found with this second key <typeparamref name="U"/> and is of type <typeparamref name="T"/>,
		/// the service will be register with the key <typeparamref name="T"/> in order to avoid further searching.
		/// </summary>
		/// <typeparam name="T">Type of the service to be searched.</typeparam>
		/// <typeparam name="U">Base class type or interface type of type <typeparamref name="T"/>.</typeparam>
		/// <returns>Service that is of type <typeparamref name="T"/>. If no such service is found, an exception is thrown.</returns>
		/// <exception cref="ServiceNotFoundException">If a service of type <typeparamref name="T"/> was not found.</exception>
		public static T GetRequiredService<T, U>() where T : U
		{
			object serviceObj = instance.GetService(typeof(T));
			if (serviceObj is T serviceT)
				return serviceT;
			serviceObj = instance.GetService(typeof(U));
			if (serviceObj is T serviceU)
			{
				AddService<T>(serviceU);
				return serviceU;
			}

			throw new ServiceNotFoundException(typeof(T));
		}

		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetService<T>()
		{
			object service = instance.GetService(typeof(T));
			return (T)service;
		}

		public static T GetService<T, U>() where T : U
		{
			object serviceObj = instance.GetService(typeof(T));
			if (serviceObj is T serviceT)
				return serviceT;
			serviceObj = instance.GetService(typeof(U));
			if (serviceObj is T serviceU)
			{
				AddService<T>(serviceU);
				return serviceU;
			}

			return default(T);
		}

		/// <summary>
		/// Returns a task that gets completed when the service is initialized.
		///
		/// This method does not try to initialize the service -- if no other code forces the service
		/// to be initialized, the task will never complete.
		/// </summary>
		/// <remarks>
		/// This method can be used to solve cyclic dependencies in service initialization.
		/// </remarks>
		public static Task<T> GetFutureService<T>() where T : class
		{
			return GetRequiredService<AltaxoServiceContainer>().GetFutureService<T>();
		}

		/// <summary>
		/// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService&lt;T&gt;()</code>,
		/// but does not throw a NullReferenceException when ActiveViewContent is null.
		/// (instead, null is returned).
		/// </summary>
		public static T GetActiveViewContentService<T>() where T : class
		{
			return (T)GetActiveViewContentService(typeof(T));
		}

		/// <summary>
		/// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService(type)</code>,
		/// but does not throw a NullReferenceException when ActiveViewContent is null.
		/// (instead, null is returned).
		/// </summary>
		public static object GetActiveViewContentService(Type type)
		{
			var workbench = instance.GetService(typeof(IWorkbench)) as IWorkbench;
			if (workbench != null)
			{
				var activeViewContent = workbench.ActiveViewContent;
				if (activeViewContent != null)
				{
					throw new NotImplementedException();
					//return activeViewContent.GetService(type);
				}
			}
			return null;
		}

		/// <inheritdoc see="IMessageService"/>
		public static IMessageService MessageService
		{
			get
			{
				return _messageService ?? (_messageService = GetRequiredService<IMessageService>());
			}
		}

		/// <inheritdoc see="IInfoWarningErrorTextMessageService"/>
		public static IInfoWarningErrorTextMessageService InfoTextMessageService
		{
			get
			{
				return _infoTextMessageService ?? (_infoTextMessageService = GetRequiredService<IInfoWarningErrorTextMessageService>());
			}
		}

		/// <inheritdoc see="IAnalyticsMonitor"/>
		public static IAnalyticsMonitor AnalyticsMonitor
		{
			get { return GetRequiredService<IAnalyticsMonitor>(); }
		}

		/// <inheritdoc see="IAddInTree"/>
		public static AddInItems.IAddInTree AddInTree
		{
			get { return _addInTree ?? (_addInTree = GetRequiredService<AddInItems.IAddInTree>()); }
		}

		#endregion Unspecified services

		#region Gui factory service

		public static IGuiFactoryService Gui
		{
			get
			{
				return _guiFactoryService ?? (_guiFactoryService = GetService<IGuiFactoryService>());
			}
		}

		#endregion Gui factory service

		#region SynchronizeInvoke

		/// <summary>
		/// Used to invoke calls in the context of the Gui thread.
		/// </summary>
		/// <value>
		/// Object that can be used to invoke calls in the context of the Gui thread.
		/// </value>
		public static IDispatcherMessageLoop Dispatcher
		{
			get
			{
				return _dispatcher ?? (_dispatcher = GetRequiredService<IDispatcherMessageLoop>());
			}
		}

		#endregion SynchronizeInvoke

		#region ResourceService

		public static Altaxo.Main.Services.IResourceService ResourceService
		{
			get
			{
				return _resourceService ?? (_resourceService = GetRequiredService<Altaxo.Main.Services.IResourceService>());
			}
		}

		#endregion ResourceService

		#region Property service

		/// <summary>
		/// Returns the property service, which is used to obtain application settings.
		/// </summary>
		public static IPropertyService PropertyService
		{
			get
			{
				return _propertyService ?? (_propertyService = GetRequiredService<IPropertyService>());
			}
		}

		#endregion Property service

		/// <summary>
		/// Returns the console window, which can be used by your scripts for textual output.
		/// </summary>
		public static ITextOutputService Console
		{
			get
			{
				return _outputService ?? (_outputService = GetRequiredService<ITextOutputService>());
			}
		}

		public static ILoggingService Log
		{
			get
			{
				return _loggingService ?? (_loggingService = GetRequiredService<ILoggingService>());
			}
		}

		public static IStatusBarService StatusBar
		{
			get
			{
				return _statusBarService ?? (_statusBarService = GetRequiredService<IStatusBarService>());
			}
		}

		public static IGuiTimerService GuiTimer
		{
			get
			{
				return _guiTimerService ?? (_guiTimerService = GetRequiredService<IGuiTimerService>());
			}
		}

		#region Project service

		/// <summary>
		/// Returns the project service, which provides methods to add worksheet and graphs, or open and close the document.
		/// </summary>
		public static Main.IProjectService IProjectService
		{
			get
			{
				return _projectService ?? (_projectService = GetRequiredService<Main.IProjectService>());
			}
		}

		#endregion Project service

		#region Com Manager

		public static Main.IComManager ComManager
		{
			get
			{
				return _comManager ?? (_comManager = GetService<Main.IComManager>());
			}
		}

		#endregion Com Manager

		#region Workbench

		/// <summary>
		/// Gets the main workbench.
		/// </summary>
		public static IWorkbench Workbench
		{
			get
			{
				return _workbench ?? (_workbench = GetRequiredService<IWorkbench>());
			}
		}

		#endregion Workbench

		#region ApplicationInstanceGuid

		private static Guid _applicationInstanceGuid = Guid.NewGuid();

		/// <summary>
		/// Gets a Guid that uniquely identifies the current application instance.
		/// Needed for drag/drop operations to decide if a drag source is coming from the own instance or another one.
		/// </summary>
		/// <value>
		/// The application instance unique identifier.
		/// </value>
		public static Guid ApplicationInstanceGuid
		{
			get
			{
				return _applicationInstanceGuid;
			}
		}

		#endregion ApplicationInstanceGuid
	}
}