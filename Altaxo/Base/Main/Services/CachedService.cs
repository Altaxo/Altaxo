using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Structure that caches a service provided by <see cref="Altaxo.Current"/>. It monitors if the services in <see cref="Altaxo.Current"/> change,
	/// and will then update the cached service.
	/// </summary>
	/// <typeparam name="T">Primary type under which to search for the service in <see cref="Altaxo.Current"/>.</typeparam>
	/// <typeparam name="U">Secondary type (usually an interface type) under which to search for the service in <see cref="Altaxo.Current"/>.</typeparam>
	public struct CachedService<T, U> where T : U
	{
		private Action<T> _serviceAttached;
		private Action<T> _serviceDetached;
		private bool _isRequiredService;

		// operational variables
		private T _instance;

		/// <summary>
		/// True if it was tried to retrive the instance
		/// </summary>
		private bool _instanceRetrievalTried;

		/// <summary>
		/// Initializes a new instance of the <see cref="CachedService{T, U}"/> struct.
		/// </summary>
		/// <param name="isRequiredService">If set to <c>true</c>, it is threated as a required service, and thus, an exception will be thrown if the service is not found.</param>
		/// <param name="serviceAttached">Action that will be executed if a new service instance is cached here.</param>
		/// <param name="serviceDetached">Action that is executed if an old service instance is released from this cache.</param>
		public CachedService(bool isRequiredService, Action<T> serviceAttached, Action<T> serviceDetached)
		{
			_serviceAttached = serviceAttached;
			_serviceDetached = serviceDetached;
			_isRequiredService = isRequiredService;

			_instance = default(T);
			_instanceRetrievalTried = false;

			Current.ServiceChanged += new WeakActionHandler(EhServiceChanged, handler => Current.ServiceChanged -= handler);
			// EhServiceChanged(); do not call EhServiceChanged here: for services how have a static intance of CachedService to cache their own service, we would create a circular dependence
		}

		private void EhServiceChanged()
		{
			if (null != _instance)
			{
				_serviceDetached?.Invoke(_instance);
			}

			if (_isRequiredService)
				_instance = Current.GetRequiredService<T, U>();
			else
				_instance = Current.GetService<T, U>();

			if (null != _instance)
			{
				_serviceAttached?.Invoke(_instance);
			}

			_instanceRetrievalTried = true;
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="CachedService{T, U}"/> to the service <see cref="T"/>.
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator T(CachedService<T, U> s)
		{
			return s.Instance;
		}

		/// <summary>
		/// Gets the service that is cached here.
		/// </summary>
		/// <value>
		/// The service.
		/// </value>
		public T Instance
		{
			get
			{
				if (null == _instance && !_instanceRetrievalTried)
				{
					EhServiceChanged();
				}

				return _instance;
			}
		}
	}
}