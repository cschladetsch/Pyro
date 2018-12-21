namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// ListView components pool.
	/// </summary>
	/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewComponentPool<TComponent, TItem>
		where TComponent : ListViewItem
	{
		/// <summary>
		/// The components gameobjects container.
		/// </summary>
		public Transform Container;

		/// <summary>
		/// The owner.
		/// </summary>
		public ListViewBase Owner;

		/// <summary>
		/// The template.
		/// </summary>
		protected TComponent template;

		/// <summary>
		/// The template.
		/// </summary>
		public TComponent Template
		{
			get
			{
				return template;
			}

			set
			{
				SetTemplate(value);
			}
		}

		/// <summary>
		/// The components list.
		/// </summary>
		protected List<TComponent> Components = new List<TComponent>();

		/// <summary>
		/// The components cache list.
		/// </summary>
		protected List<TComponent> ComponentsCache = new List<TComponent>();

		/// <summary>
		/// The function to add callbacks.
		/// </summary>
		public Action<ListViewItem> CallbackAdd;

		/// <summary>
		/// The function to remove callbacks.
		/// </summary>
		public Action<ListViewItem> CallbackRemove;

		/// <summary>
		/// The displayed indices.
		/// </summary>
		protected List<int> DisplayedIndices = new List<int>();

		/// <summary>
		/// Find component with the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Component with the specified index.</returns>
		public TComponent Find(int index)
		{
			return Components.Find(x => x.Index == index);
		}

		/// <summary>
		/// Set the DisplayedIndices.
		/// </summary>
		/// <param name="newIndices">New indices.</param>
		/// <param name="action">Action.</param>
		public void DisplayedIndicesSet(List<int> newIndices, Action<TComponent> action)
		{
			SetCount(newIndices.Count);

			Components.ForEach((x, i) =>
			{
				x.Index = newIndices[i];
				action(x);
				LayoutUtilites.UpdateLayoutsRecursive(x);
			});

			DisplayedIndices.Clear();
			DisplayedIndices.AddRange(newIndices);

			Components.Sort(ComponentsComparer);
			Components.ForEach(SetComponentAsLastSibling);
		}

		/// <summary>
		/// Update the DisplayedIndices.
		/// </summary>
		/// <param name="newIndices">New indices.</param>
		/// <param name="action">Action.</param>
		public void DisplayedIndicesUpdate(List<int> newIndices, Action<TComponent> action)
		{
			if (DisplayedIndices.SequenceEqual(newIndices))
			{
				return;
			}

			var new_indices_to_change = newIndices.Except(DisplayedIndices).ToArray();

			if (new_indices_to_change.Length > 0)
			{
				var components_to_change = new Stack<TComponent>(Components.Where(x => !newIndices.Contains(x.Index)));

				var delta = new_indices_to_change.Length - components_to_change.Count;

				if (delta > 0)
				{
					for (int i = 0; i < delta; i++)
					{
						var component = CreateComponent();
						components_to_change.Push(component);
						Components.Add(component);
					}
				}
				else if (delta < 0)
				{
					for (int i = delta; i < 0; i++)
					{
						var component = components_to_change.Pop();
						DeactivateComponent(component);
						Components.Remove(component);
						ComponentsCache.Add(component);
					}
				}

				Owner.Items = Components.Convert(x => x as ListViewItem);

				new_indices_to_change.ForEach(index =>
				{
					var component = components_to_change.Pop();

					component.Index = index;
					action(component);
					LayoutUtilites.UpdateLayoutsRecursive(component);
				});
			}

			DisplayedIndices.Clear();
			DisplayedIndices.AddRange(newIndices);

			Components.Sort(ComponentsComparer);
			Components.ForEach(SetComponentAsLastSibling);
		}

		/// <summary>
		/// Sets the required components count.
		/// </summary>
		/// <param name="count">Count.</param>
		public void SetCount(int count)
		{
			Components.RemoveAll(IsNullComponent);

			if (Components.Count == count)
			{
				return;
			}

			if (Components.Count < count)
			{
				ComponentsCache.RemoveAll(IsNullComponent);

				for (int i = Components.Count; i < count; i++)
				{
					Components.Add(CreateComponent());
				}
			}
			else
			{
				for (int i = count; i < Components.Count; i++)
				{
					DeactivateComponent(Components[i]);
					ComponentsCache.Add(Components[i]);
				}

				Components.RemoveRange(count, Components.Count - count);
			}

			Owner.Items = Components.Convert(x => x as ListViewItem);
		}

		/// <summary>
		/// Sets the template.
		/// </summary>
		/// <param name="newTemplate">New template.</param>
		protected virtual void SetTemplate(TComponent newTemplate)
		{
			// clear previous DefaultItem data
			if (template != null)
			{
				template.gameObject.SetActive(false);
			}

			CallbacksRemove();

			Components.ForEach(DeactivateComponent);
			Components.Clear();

			ComponentsCache.ForEach(DestroyComponent);
			ComponentsCache.Clear();

			// set new DefaultItem data
			template = newTemplate;
			if (newTemplate != null)
			{
				template.Owner = Owner;
				template.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		protected void CallbacksRemove()
		{
			Components.ForEach(x => CallbackRemove(x));
		}

		/// <summary>
		/// Is component is null?
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if component is null; otherwise, false.</returns>
		protected bool IsNullComponent(TComponent component)
		{
			return component == null;
		}

		/// <summary>
		/// Create component instance.
		/// </summary>
		/// <returns>Component instance.</returns>
		protected TComponent CreateComponent()
		{
			TComponent component;
			if (ComponentsCache.Count > 0)
			{
				component = ComponentsCache[ComponentsCache.Count - 1];
				ComponentsCache.RemoveAt(ComponentsCache.Count - 1);
			}
			else
			{
				component = Compatibility.Instantiate(template);
				component.transform.SetParent(Container, false);
				Utilites.FixInstantiated(template, component);
				component.Owner = Owner;
			}

			component.Index = -2;
			component.transform.SetAsLastSibling();
			component.gameObject.SetActive(true);

			CallbackAdd(component);

			return component;
		}

		/// <summary>
		/// Deactivates the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected void DeactivateComponent(TComponent component)
		{
			if (component != null)
			{
				CallbackRemove(component);
				component.MovedToCache();
				component.Index = -1;
				component.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Destroy the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected void DestroyComponent(TComponent component)
		{
			UnityEngine.Object.Destroy(component.gameObject);
		}

		/// <summary>
		/// Compare components by component index.
		/// </summary>
		/// <returns>A signed integer that indicates the relative values of x and y.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		protected int ComponentsComparer(TComponent x, TComponent y)
		{
			return DisplayedIndices.IndexOf(x.Index).CompareTo(DisplayedIndices.IndexOf(y.Index));
		}

		/// <summary>
		/// Move the component transform to the end of the local transform list.
		/// </summary>
		/// <param name="item">Item.</param>
		protected void SetComponentAsLastSibling(Component item)
		{
			item.transform.SetAsLastSibling();
		}

		/// <summary>
		/// Apply function for each component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEach(Action<TComponent> action)
		{
			Components.ForEach(action);
		}

		/// <summary>
		/// Apply function for each component and cached components.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachAll(Action<TComponent> action)
		{
			Components.ForEach(action);
			ComponentsCache.ForEach(action);
		}

		/// <summary>
		/// Apply function for each cached component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachCache(Action<TComponent> action)
		{
			ComponentsCache.ForEach(action);
		}

		/// <summary>
		/// Get the copy of components list.
		/// </summary>
		/// <returns>Components list.</returns>
		public List<TComponent> List()
		{
			return Components.ToList();
		}
	}
}