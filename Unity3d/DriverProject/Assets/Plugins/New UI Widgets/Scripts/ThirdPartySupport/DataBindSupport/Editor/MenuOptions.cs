#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	class MenuOptions
	{
		static string GetPath()
		{
			return Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
		}

		[MenuItem("Assets/UIWidgets/Add Data Bind support", false)]
		static void AddDataBindUISupport()
		{
			var options = GetOptions();
			var path = GetPath();

			foreach (var option in options)
			{
				var gen = new DataBindGenerator(option, path);
				gen.Generate();
			}
		}

		static List<DataBindOption> GetOptions()
		{
			if (Selection.activeObject == null)
			{
				return null;
			}
			var type = Selection.activeObject as MonoScript;
			if (type == null)
			{
				return null;
			}

			return DataBindOption.GetOptions(type.GetClass());
		}

		[MenuItem("Assets/UIWidgets/Add Data Bind support", true)]
		static bool AddDataBindUISupportValidation()
		{
			var options = GetOptions();

			return options!=null && options.Count > 0;
		}
	}
}
#endif