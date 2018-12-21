﻿namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Used only to attach custom editor to DragSupport.
	/// </summary>
	public abstract class BaseDragSupport : MonoBehaviour
	{
		/// <summary>
		/// The drag points.
		/// </summary>
		protected static Dictionary<Transform, RectTransform> DragPoints = new Dictionary<Transform, RectTransform>();

		Transform canvasTransform;

		/// <summary>
		/// Gets a canvas transform of current gameobject.
		/// </summary>
		protected Transform CanvasTransform
		{
			get
			{
				if (canvasTransform == null)
				{
					canvasTransform = Utilites.FindTopmostCanvas(transform);
				}

				return canvasTransform;
			}
		}

		/// <summary>
		/// Gets the drag point.
		/// </summary>
		public RectTransform DragPoint
		{
			get
			{
				var contains_key = DragPoints.ContainsKey(CanvasTransform);
				if (!contains_key || (DragPoints[CanvasTransform] == null))
				{
					var go = new GameObject("DragPoint");
					var dragPoint = go.AddComponent<RectTransform>();
					dragPoint.SetParent(CanvasTransform, false);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
					dragPoint.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

					DragPoints[CanvasTransform] = dragPoint;
				}

				return DragPoints[CanvasTransform];
			}
		}
	}
}