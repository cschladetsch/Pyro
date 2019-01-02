namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// TreeViewSampleItem interace.
	/// </summary>
	public interface ITreeViewSampleItem : IObservable
	{
		/// <summary>
		/// Display item data using specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		void Display(TreeViewSampleComponent component);
	}
}