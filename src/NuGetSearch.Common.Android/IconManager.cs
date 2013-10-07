using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NuGetSearch.Common
{
	/// <summary>
	/// Icon Manager provides asynchronous icon loading with throttling
	/// </summary>
	/// <typeparam name="T">The platform-specific icon type</typeparam>
	public abstract class IconManager<T> : IIconManager<T> where T : class
	{
		// As requests come in, they are queued up in this queue
		private Queue<IconLoadRequest> requests = new Queue<IconLoadRequest>();
		
		// After an icon has been loaded, the results are stored in this dictionary for retrieval
		private Dictionary<string, T> loadedIcons = new Dictionary<string, T>();
		
		// Worker thread responsible for loading icons
		private Thread worker;
		
		// Semaphore used to "throttle" icon loading so that it does not consume all of the resources
		private SemaphoreSlim sem;
		private int semaphoreWaitTimeout;
		
		// Manual Reset Event used to signal the worker thread that a new request has arrived in the queue
		private ManualResetEventSlim mres = new ManualResetEventSlim(false);
		private int manualResetWaitTimeout = 10000;
		
		// Flag used to terminate the worker thread
		private bool running = true;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Common.IconManager{T}"/> class.
		/// </summary>
		/// <param name="semaphoreCount">Count used to initialize the semaphore that provides throttling</param>
		/// <param name="semaphoreWaitTimeout">Wait timeout, in milliseconds, used by the semaphore</param>
		protected IconManager(int semaphoreCount, int semaphoreWaitTimeout)
		{
			// Initialize the semaphore with the specified count
			this.sem = new SemaphoreSlim(semaphoreCount);
			this.semaphoreWaitTimeout = semaphoreWaitTimeout;
				
			// Create a worker thread that will be responsible for loading icons
			this.worker = new Thread(new ThreadStart(this.WorkerProc));
			this.worker.IsBackground = true;
			this.worker.Start();
		}

		/// <summary>
		/// Adds the specified URLs to the queue and registers the specified callback for each of them
		/// </summary>
		/// <param name="urlsToLoad">URLs to load</param>
		/// <param name="callback">Callback to be invoked after each URL has been loaded</param>
		public void Load(IEnumerable<string> urlsToLoad, Action<string> callback)
		{
			var alreadyLoadedUrls = new List<string>();
		
			// Add all of the specified URLs to the queue with the specified callback
			foreach (var url in urlsToLoad)
			{
				if (this.GetIcon(url) != null)
				{
					// If the URL is already loaded, then add it to a list that will be used later to invoke the callback methods
					alreadyLoadedUrls.Add(url);
				}
				else 
				{	
					// If the URL has not yet been loaded, then queue a new request to load it
					this.requests.Enqueue(new IconLoadRequest { Url = url, Callback = callback });
				}
			}
			
			// Set the Manual Reset Event to signal the worker thread that something is in the queue
			this.mres.Set();
			
			// Invoke the callback for all of the URLs that were already loaded
			if (callback != null) 
			{ 
				foreach (var url in alreadyLoadedUrls)
				{
					callback(url);
				}
			}
		}
		
		/// <summary>
		/// Adds the specified URL to the queue and registers the specified callback for it
		/// </summary>
		/// <param name="urlToLoad">URL to load</param>
		/// <param name="callback">Callback to be invoked after the URL has been loaded</param>
		public void Load(string urlToLoad, Action<string> callback)
		{
			this.Load(new string[] { urlToLoad }, callback);
		}

		/// <summary>
		/// Determines whether the icon with the specified URL has been loaded or not
		/// </summary>
		/// <returns><c>true</c> if the icon has been loaded; otherwise, <c>false</c>.</returns>
		/// <param name="url">URL to check</param>
		public bool IsLoaded(string url)
		{
			return this.loadedIcons.ContainsKey(url);
		}
		
		/// <summary>
		/// Gets the specified icon
		/// </summary>
		/// <returns></returns>
		/// <param name="url">URL to retrieve the bytes for</param>
		public T GetIcon(string url)
		{
			return this.loadedIcons.ContainsKey(url) ? this.loadedIcons[url] : null;
		}
		
		/// <summary>
		/// Releases all resource used by the <see cref="NuGetSearch.Common.IconManager"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="NuGetSearch.Common.IconManager"/>.
		/// The <see cref="Dispose"/> method leaves the <see cref="NuGetSearch.Common.IconManager"/> in an unusable
		/// state. After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="NuGetSearch.Common.IconManager"/> so the garbage collector can reclaim the memory that the
		/// <see cref="NuGetSearch.Common.IconManager"/> was occupying.</remarks>
		public void Dispose()
	    {
	        this.Dispose(true);
	        GC.SuppressFinalize(this);
	    }

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
	    {
	        if (disposing) 
	        {
	        	this.running = false;
	        	if (this.mres != null)
	        	{
	        		this.mres.Set();	        		
	        	}
	        	
	        	if (this.sem != null)
	        	{
	        		this.sem.Dispose();
	        		this.sem = null;
	        	}
	        	
	        	if (this.mres != null)
	        	{
		        	this.mres.Dispose();
		        	this.mres = null;
	        	}
	        	
	        	if (typeof(T) is IDisposable)
	        	{
	        		foreach (string key in this.loadedIcons.Keys)
	        		{
	        			try
	        			{
	        				if (this.loadedIcons[key] != null)
	        				{
	        					(this.loadedIcons[key] as IDisposable).Dispose();
        					}
	        			}
	        			catch
	        			{
	        			}
	        		}
	        	}
	        }
	    }
	    
		/// <summary>
		/// Loads the icon from the specified url
		/// </summary>
		/// <returns></returns>
		/// <param name="url"></param>
		protected abstract T LoadIcon(string url);
			   
	    /// <summary>
	    /// Worker thread responsible for loading icons
	    /// </summary>
	    private void WorkerProc()
		{
			// Keep running as long as the running flag is set
			while (this.running)
			{
				try
				{
					// Wait for the Manual Reset Event to be set (or timeout after 10 seconds)
					this.mres.Wait(this.manualResetWaitTimeout);				
					
					// Keep running as long as the running flag is set and there is something in the requests queue
					while (this.running && this.requests.Any())
					{
						// Pull a request out of the requests queue
						IconLoadRequest request = this.requests.Dequeue();
						
						// Initiate the asynchronous loading of the icon in this request
						if (request != null)
						{
							this.ProcessIconLoadRequestAsync(request);
						}
					}
				}
				catch (Exception ex)
				{
					// Exceptions that occur in this thread are ignored, so that it can continue processing other requests
					System.Diagnostics.Trace.WriteLine("WorkerProc Error: " + ex.Message);
				}
				finally
				{
					// Reset the Manual Reset Event, so that it can be signaled again
					try 
					{ 
						this.mres.Reset(); 
					} 
					catch 
					{ 
					}
				}
			}
		}
		
		/// <summary>
		/// Loads an icon and stores the resulting bytes in the loadedIconBytes dictionary
		/// </summary>
		/// <returns>The request async.</returns>
		/// <param name="request">Request.</param>
		private Task ProcessIconLoadRequestAsync(IconLoadRequest request)
		{
			// Wait for the semaphore (or timeout after 3 seconds) in order to throttle the loading of icon images 
			// and avoid consuming too much of the device's processing resources.
			this.sem.Wait(this.semaphoreWaitTimeout);
			
			// Start a task to load the icon bytes
			return Task.Run(() => 
			{ 
				try 
				{
					// Get the icon from the URL and store the result in the dictionary 
					if (this.GetIcon(request.Url) == null)
					{
						this.AddIcon(request.Url, this.LoadIcon(request.Url));
					}
				} 
				catch (Exception ex)
				{ 
					// Exceptions that occur in this thread are ignored, so that it can continue processing other requests
					System.Diagnostics.Trace.WriteLine("ProcessIconLoadRequestAsync Error: " + ex.Message);
					this.AddIcon(request.Url, null);
				}
				finally 
				{ 
					// Release the semaphore
					try 
					{ 
						this.sem.Release(); 
					} 
					catch 
					{ 
					}
				}
				
				// After the icon has been loaded, invoke the callback function
				if (request.Callback != null) 
				{ 
					request.Callback(request.Url); 
				}
			});
		}
		
		/// <summary>
		/// Adds the specified icon bytes to the dictionary with the URL as the key
		/// </summary>
		/// <param name="url"></param>
		/// <param name="icon"></param>
		private void AddIcon(string url, T icon)
		{
			lock (this.loadedIcons)
			{
				if (this.GetIcon(url) == null) 
				{ 
					this.loadedIcons.Add(url, icon);
					System.Diagnostics.Debug.WriteLine(string.Concat("Icon Manager added ", url));
				}
			}
		}
			      
		/// <summary>
		/// Internal class used to represent a request to load an icon
		/// </summary>
		private class IconLoadRequest
		{
			public string Url { get; set; }
			
			public Action<string> Callback { get; set; }
		}
	}
}
