namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using EasyLayoutNS;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// Base class for ListView renderers.
		/// </summary>
		protected abstract class ListViewTypeBase
		{
			/// <summary>
			/// Owner.
			/// </summary>
			protected ListViewCustom<TComponent, TItem> Owner;

			/// <summary>
			/// The layout elements of the DefaultItem.
			/// </summary>
			protected List<ILayoutElement> layoutElements = new List<ILayoutElement>();

			/// <summary>
			/// The DefaultItem layout group.
			/// </summary>
			protected LayoutGroup DefaultItemLayoutGroup;

			/// <summary>
			/// The DefaultItem layout group.
			/// </summary>
			protected LayoutGroup DefaultItemLayout;

			/// <summary>
			/// The DefaultItem nested layout groups.
			/// </summary>
			protected LayoutGroup[] DefaultItemNestedLayouts;

			TComponent defaultItemCopy;

			RectTransform defaultItemCopyRect;

			/// <summary>
			/// Gets the default item copy.
			/// </summary>
			/// <value>The default item copy.</value>
			protected TComponent DefaultItemCopy
			{
				get
				{
					if (defaultItemCopy == null)
					{
						defaultItemCopy = Compatibility.Instantiate(Owner.DefaultItem);
						defaultItemCopy.transform.SetParent(Owner.DefaultItem.transform.parent, false);
						defaultItemCopy.gameObject.name = "DefaultItemCopy";
						defaultItemCopy.gameObject.SetActive(false);

						Utilites.FixInstantiated(Owner.DefaultItem, defaultItemCopy);
					}

					return defaultItemCopy;
				}
			}

			/// <summary>
			/// Gets the RectTransform of DefaultItemCopy.
			/// </summary>
			/// <value>RectTransform.</value>
			protected RectTransform DefaultItemCopyRect
			{
				get
				{
					if (defaultItemCopyRect == null)
					{
						defaultItemCopyRect = defaultItemCopy.transform as RectTransform;
					}

					return defaultItemCopyRect;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeBase"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			protected ListViewTypeBase(ListViewCustom<TComponent, TItem> owner)
			{
				Owner = owner;
			}

			/// <summary>
			/// Is looped list allowed?
			/// </summary>
			/// <returns>True if looped list allowed; otherwise false.</returns>
			public virtual bool IsTileView
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Sets the default item.
			/// </summary>
			public virtual void ResetDefaultItem()
			{
				if (defaultItemCopy != null)
				{
					Destroy(defaultItemCopy.gameObject);
					defaultItemCopy = null;
				}

				if (defaultItemCopyRect != null)
				{
					Destroy(defaultItemCopyRect.gameObject);
					defaultItemCopyRect = null;
				}
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			/// <returns>Maximum count of the visible items.</returns>
			public abstract int GetMaxVisibleItems();

			/// <summary>
			/// Calculates the size of the item.
			/// </summary>
			/// <param name="reset">Reset item size.</param>
			/// <returns>Item size.</returns>
			public virtual Vector2 GetItemSize(bool reset = false)
			{
				var rt = Owner.DefaultItem.transform as RectTransform;

				layoutElements.Clear();
				Compatibility.GetComponents<ILayoutElement>(Owner.DefaultItem.gameObject, layoutElements);

				var size = Owner.ItemSize;

				if ((size.y == 0) || reset)
				{
					var preffered_height = layoutElements.Max(x => Mathf.Max(x.minHeight, x.preferredHeight));
					size.y = Mathf.Max(preffered_height, rt.rect.height);
				}

				if ((size.x == 0) || reset)
				{
					var preffered_width = layoutElements.Max(x => Mathf.Max(x.minWidth, x.preferredWidth));
					size.x = Mathf.Max(preffered_width, rt.rect.width);
				}

				return size;
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public abstract float CalculateTopFillerSize();

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public abstract float CalculateBottomFillerSize();

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetFirstVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the last visible index.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetLastVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the item position.
			/// </summary>
			/// <returns>The item position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPosition(int index);

			/// <summary>
			/// Gets the item middle position by index.
			/// </summary>
			/// <returns>The item middle position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionMiddle(int index);

			/// <summary>
			/// Gets the item position bottom.
			/// </summary>
			/// <returns>The item position bottom.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionBottom(int index);

			/// <summary>
			/// Calculate and sets the sizes of the items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			public virtual void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
			}

			/// <summary>
			/// Calculates the size of the component for the specified item.
			/// </summary>
			/// <returns>The component size.</returns>
			/// <param name="item">Item.</param>
			protected virtual Vector2 CalculateComponentSize(TItem item)
			{
				if (DefaultItemLayout == null)
				{
					return Owner.ItemSize;
				}

				Owner.SetData(DefaultItemCopy, item);

				DefaultItemNestedLayouts.ForEach(LayoutUtilites.UpdateLayout);

				LayoutUtilites.UpdateLayout(DefaultItemLayout);

				var size = new Vector2(
					LayoutUtility.GetPreferredWidth(DefaultItemCopyRect),
					LayoutUtility.GetPreferredHeight(DefaultItemCopyRect));

				return size;
			}

			/// <summary>
			/// Calculates the size of the component for the item with the specified index.
			/// </summary>
			/// <returns>The component size.</returns>
			/// <param name="index">Index.</param>
			public virtual Vector2 CalculateSize(int index)
			{
				DefaultItemCopy.gameObject.SetActive(true);

				if (DefaultItemLayout == null)
				{
					DefaultItemLayout = DefaultItemCopy.GetComponent<LayoutGroup>();

					DefaultItemNestedLayouts = DefaultItemCopy.GetComponentsInChildren<LayoutGroup>();
					Array.Reverse(DefaultItemNestedLayouts);
				}

				var size = CalculateComponentSize(Owner.DataSource[index]);

				DefaultItemCopy.gameObject.SetActive(false);

				return size;
			}

			/// <summary>
			/// Adds the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void AddCallback(ListViewItem item)
			{
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void RemoveCallback(ListViewItem item)
			{
			}

			/// <summary>
			/// Calls specified function with each component.
			/// </summary>
			/// <param name="func">Func.</param>
			public virtual void ForEachComponent(Action<ListViewItem> func)
			{
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			public abstract int GetNearestIndex(Vector2 point);

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public abstract int GetNearestItemIndex();

			/// <summary>
			/// Get the size of the ListView.
			/// </summary>
			/// <returns>The size.</returns>
			public abstract float ListSize();

			/// <summary>
			/// Get block index by item index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <returns>Block index.</returns>
			public virtual int GetBlockIndex(int index)
			{
				return index;
			}

			/// <summary>
			/// Gets the items per block count.
			/// </summary>
			/// <returns>The items per block.</returns>
			public virtual int GetItemsPerBlock()
			{
				return 1;
			}

			/// <summary>
			/// Determines whether this instance can be virtualized.
			/// </summary>
			/// <returns><c>true</c> if this instance can virtualized; otherwise, <c>false</c>.</returns>
			public virtual bool CanVirtualize()
			{
				var scrollRectSpecified = Owner.scrollRect != null;
				var containerSpecified = Owner.Container != null;
				var currentLayout = containerSpecified ? ((Owner.layout != null) ? Owner.layout : Owner.Container.GetComponent<LayoutGroup>()) : null;
				var validLayout = currentLayout ? ((currentLayout is EasyLayout) || (currentLayout is HorizontalOrVerticalLayoutGroup)) : false;

				return scrollRectSpecified && validLayout;
			}

			/// <summary>
			/// Process the item move event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			/// <param name="item">Item.</param>
			public virtual void OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				if (!Owner.Navigation)
				{
					return;
				}

				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						break;
					case MoveDirection.Right:
						break;
					case MoveDirection.Up:
						if (item.Index > 0)
						{
							Owner.SelectComponentByIndex(item.Index - 1);
						}

						break;
					case MoveDirection.Down:
						if (Owner.IsValid(item.Index + 1))
						{
							Owner.SelectComponentByIndex(item.Index + 1);
						}

						break;
				}
			}

			/// <summary>
			/// Validates the content size and item size.
			/// </summary>
			public virtual void ValidateContentSize()
			{
			}
		}
	}
}