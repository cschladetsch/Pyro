namespace UIWidgets.Styles
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class StyleMenuOptions
	{
		/// <summary>
		/// Creates the style.
		/// </summary>
		[MenuItem("Assets/Create/UIWidgets - Style", false)]
		public static void CreateStyle()
		{
			var path = AssetDatabase.GetAssetPath(Selection.activeObject) + "/UIWidgets Style.asset";
			var file = AssetDatabase.GenerateUniqueAssetPath(path);
			var style = ScriptableObject.CreateInstance<Style>();

			AssetDatabase.CreateAsset(style, file);
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();

			style.SetDefaultValues();
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Apply the default style.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Apply default style", false, 10)]
		public static void ApplyDefaultStyle()
		{
			var style = Style.DefaultStyle();
			if (style == null)
			{
				Debug.LogWarning("Default style not found.");
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(Selection.activeGameObject, "Apply style v2");
			style.ApplyTo(Selection.activeGameObject);
		}

		/// <summary>
		/// Check is the default style exists.
		/// </summary>
		/// <returns><c>true</c>, if default style is exists, <c>false</c> otherwise.</returns>
		[MenuItem("GameObject/UI/UIWidgets/Apply default style", true, 10)]
		public static bool DefaultStyleIsExists()
		{
			return Selection.activeGameObject != null;
		}
	}
}