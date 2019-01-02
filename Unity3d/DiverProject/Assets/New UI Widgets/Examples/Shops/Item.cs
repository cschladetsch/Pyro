namespace UIWidgets.Examples.Shops
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Item.
	/// </summary>
	[Serializable]
	public class Item : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		[SerializeField]
		int count;

		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		public event OnChange OnChange = () => { };

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

		/// <summary>
		/// Gets or sets the count. -1 for infinity count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return count;
			}

			set
			{
				if (count == -1)
				{
					Changed();
					return;
				}

				count = value;
				Changed();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.Item"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="count">Count.</param>
		public Item(string name, int count)
		{
			Name = name;
			Count = count;
		}

		void Changed()
		{
			PropertyChanged(this, null);

			OnChange();
		}
	}
}