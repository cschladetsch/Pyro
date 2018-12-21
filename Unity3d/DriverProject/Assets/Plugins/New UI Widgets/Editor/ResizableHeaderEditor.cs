namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// ResizableHeader editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ResizableHeader), true)]
	public class ResizableHeaderEditor : CursorsEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResizableHeaderEditor"/> class.
		/// </summary>
		public ResizableHeaderEditor()
		{
			Cursors = new List<string>()
			{
				"CurrentCamera",
				"CursorTexture",
				"CursorHotSpot",
				"AllowDropCursor",
				"AllowDropCursorHotSpot",
				"DeniedDropCursor",
				"DeniedDropCursorHotSpot",
				"DefaultCursorTexture",
				"DefaultCursorHotSpot",
			};
		}
	}
}