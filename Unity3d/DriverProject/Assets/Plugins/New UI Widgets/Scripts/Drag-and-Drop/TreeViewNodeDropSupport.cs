﻿namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TreeViewNode drop support.
	/// Receive drops from TreeView and ListViewIcons.
	/// </summary>
	[AddComponentMenu("UI/UIWidgets/TreeView Node Drop Support")]
	[RequireComponent(typeof(TreeViewComponent))]
	public class TreeViewNodeDropSupport : TreeViewCustomNodeDropSupport<TreeView, TreeViewComponent, TreeViewItem>, IDropSupport<ListViewIconsItemDescription>
	{
		#region IDropSupport<ListViewIconsItemDescription>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			var node = Source.Node;
			if (node.Nodes == null)
			{
				node.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			}

			var newItem = new TreeViewItem(data.Name)
			{
				LocalizedName = data.LocalizedName,
				Icon = data.Icon,
				Value = data.Value,
			};
			var newNode = new TreeNode<TreeViewItem>(newItem);
			node.Nodes.Add(newNode);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(ListViewIconsItemDescription data, PointerEventData eventData)
		{
		}
		#endregion
	}
}