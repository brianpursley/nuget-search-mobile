using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Dependency adapter.
	/// </summary>
	internal class DependencyAdapter : BaseAdapter<PackageDependency>
	{
		private Activity context;
		private IList<PackageDependency> dependencies;
        private RowSelectedDelegate rowSelectedCallback;

		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Android.DependencyAdapter"/> class.
		/// </summary>
		/// <param name="context">The parent context</param>
		/// <param name="dependencies">The dependencies to include in this adapter</param>
        /// <param name="rowSelectedCallback">Method to be called when a row is selected</param>
		public DependencyAdapter(
			Activity context, 
            IList<PackageDependency> dependencies,
            RowSelectedDelegate rowSelectedCallback) : base()
		{
			this.context = context;
            this.dependencies = dependencies;
            this.rowSelectedCallback = rowSelectedCallback;
		}

        public delegate void RowSelectedDelegate(string packageTitle);

		/// <summary>
		/// Gets the view type count.  For this adapter, it is just one.
		/// </summary>
		/// <value>The view type count.</value>
		public override int ViewTypeCount 
		{
			get { return 1; }
		}

		/// <summary>
		/// Gets the number of items contained in this adapter.
		/// </summary>
		/// <value>The number of items in the adapter</value>
		public override int Count 
		{
			get { return this.dependencies.Count; }
		}

		/// <summary>
		/// Gets the <see cref="PackageDependency"/> at the specified position.
		/// </summary>
		/// <returns>The PackageDependency at the specified position</returns>
		/// <param name="position"></param>
		public override PackageDependency this[int position] 
		{  
			get { return this.dependencies[position]; }
		}

		/// <summary>
		/// Gets the item ID at the specified position.  This adapter simply returns the position as the ID.
		/// </summary>
		/// <returns>The item ID</returns>
		/// <param name="position"></param>
		public override long GetItemId(int position)
		{
			return position;
		}

		/// <summary>
		/// Gets the view used to display the item at the specified position
		/// </summary>
		/// <returns>View</returns>
		/// <param name="position"></param>
		/// <param name="convertView"></param>
		/// <param name="parent"></param>
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			// Get the item at this position
			PackageDependency item = this.dependencies[position];
			
			// Re-use the view that was passed in the convertView argument, or inflate a new view if it was null
			View view = convertView;
			if (view == null)
			{
				view = this.context.LayoutInflater.Inflate(Resource.Layout.DependencyItem, parent, false);
				view.Click += this.View_Click;
			}
			
			// Set the title in the tag for retrieval in the click event handler
			view.Tag = item.Title;
			
			// Display the title and version range for this dependency
			view.FindViewById<TextView>(Resource.Id.dependencyTitle).Text = item.Title;
			view.FindViewById<TextView>(Resource.Id.dependencyVersionRange).Text = 
				string.IsNullOrWhiteSpace(item.VersionRange) ? string.Empty : string.Format("({0})", item.VersionRange);

			return view;
		}

		/// <summary>
		/// Click event handler for the view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void View_Click(object sender, EventArgs e)
		{
            if (this.rowSelectedCallback != null)
            {
                View view = sender as View;
                this.rowSelectedCallback(view.Tag.ToString());
            }
		}
	}
}
