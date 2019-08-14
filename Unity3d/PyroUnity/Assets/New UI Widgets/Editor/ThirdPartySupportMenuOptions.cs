namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Menu options to toggle third party packages support.
	/// </summary>
	public static class ThirdPartySupportMenuOptions
	{
#if UNITY_5_6_OR_NEWER
#region DataBindSupport
		const string DataBindSupport = "UIWIDGETS_DATABIND_SUPPORT";

		/// <summary>
		/// Enable DataBind support.
		/// </summary>
		[MenuItem("Edit/Project Settings/UIWidgets/Enable Data Bind support", false, 1000)]
		public static void EnableDataBindSupport()
		{
			if (CanEnableDataBindSupport())
			{
				var current_path = Utilites.GetAssetPath("DataBindFolder");
				var new_path = System.IO.Path.GetDirectoryName(Utilites.GetAssetPath("ScriptsFolder")) + "/" + System.IO.Path.GetFileName(current_path);
				if (current_path != new_path)
				{
					AssetDatabase.MoveAsset(current_path, new_path);
				}

				ScriptingDefineSymbols.Add(DataBindSupport);
				Compatibility.ForceRecompileByLabel("DataBindFolder");
			}
		}

		/// <summary>
		/// Can enable DataBind support?
		/// </summary>
		/// <returns>true if DataBind installed and support not enabled; otherwise false.</returns>
		[MenuItem("Edit/Project Settings/UIWidgets/Enable Data Bind support", true, 1000)]
		public static bool CanEnableDataBindSupport()
		{
			if (Utilites.GetType("Slash.Unity.DataBind.Core.Presentation.DataProvider") == null)
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(DataBindSupport);
		}

		/// <summary>
		/// Disable DataBind support.
		/// </summary>
		[MenuItem("Edit/Project Settings/UIWidgets/Disable Data Bind support", false, 1010)]
		public static void DisableDataBindSupport()
		{
			if (CanDisableDataBindSupport())
			{
				ScriptingDefineSymbols.Remove(DataBindSupport);
			}
		}

		/// <summary>
		/// Can disable DataBind support?
		/// </summary>
		/// <returns>true if DataBind support enabled; otherwise false.</returns>
		[MenuItem("Edit/Project Settings/UIWidgets/Disable Data Bind support", true, 1010)]
		public static bool CanDisableDataBindSupport()
		{
			return ScriptingDefineSymbols.Contains(DataBindSupport);
		}
#endregion
#endif

#region TMProSupport

		const string TMProSupport = "UIWIDGETS_TMPRO_SUPPORT";

		const string TMProAssemblies = "l:UiwidgetsTMProRequiredAssemblyDefinition";

		const string TMProPackage = "Unity.TextMeshPro";

		/// <summary>
		/// Enable DataBind Support.
		/// </summary>
		[MenuItem("Edit/Project Settings/UIWidgets/Enable TextMesh Pro support", false, 1020)]
		public static void EnableTMProSupport()
		{
			if (CanEnableTMProSupport())
			{
				ScriptingDefineSymbols.Add(TMProSupport);
				AssemblyDefinitionsEditor.Add(TMProAssemblies, TMProPackage);

				var version = Compatibility.GetTMProVersion();
				var path = Utilites.GetAssetPath("TMProSupport" + version + "Package");
				var interactive = IsTMProPrefabsInstalled();
				Compatibility.ForceRecompileByLabel("TMProFolder");
				Compatibility.ImportPackage(path, interactive);

				AssetDatabase.Refresh();
			}
		}

		static bool IsTMProPrefabsInstalled()
		{
			var key = "l:UiwidgetsAccordionTMProPrefab";
			var guids = AssetDatabase.FindAssets(key);

			return guids.Length > 0;
		}

		/// <summary>
		/// Can enable TMPro support?
		/// </summary>
		/// <returns>true if TMPro installed and support not enabled; otherwise false.</returns>
		[MenuItem("Edit/Project Settings/UIWidgets/Enable TextMesh Pro support", true, 1020)]
		public static bool CanEnableTMProSupport()
		{
			if (string.IsNullOrEmpty(Compatibility.GetTMProVersion()))
			{
				return false;
			}

			return !ScriptingDefineSymbols.Contains(TMProSupport);
		}

		/// <summary>
		/// Disable TMPro support.
		/// </summary>
		[MenuItem("Edit/Project Settings/UIWidgets/Disable TextMesh Pro support", false, 1030)]
		public static void DisableTMProSupport()
		{
			if (CanDisableTMProSupport())
			{
				ScriptingDefineSymbols.Remove(TMProSupport);
				AssemblyDefinitionsEditor.Remove(TMProAssemblies, TMProPackage);

				AssetDatabase.Refresh();
			}
		}

		/// <summary>
		/// Can disable TMPro support?
		/// </summary>
		/// <returns>true if TMPro suppport enabled; otherwise false.</returns>
		[MenuItem("Edit/Project Settings/UIWidgets/Disable TextMesh Pro support", true, 1030)]
		public static bool CanDisableTMProSupport()
		{
			return ScriptingDefineSymbols.Contains(TMProSupport);
		}
#endregion
	}
}