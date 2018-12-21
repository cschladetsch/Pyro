namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// TreeViewNode drag support.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/TreeView Node Drag Support")]
	[RequireComponent(typeof(TreeViewComponent))]
	public class TreeViewNodeDragSupport : TreeViewCustomNodeDragSupport<TreeViewComponent, ListViewIconsItemComponent, TreeViewItem>
	{
	}
}