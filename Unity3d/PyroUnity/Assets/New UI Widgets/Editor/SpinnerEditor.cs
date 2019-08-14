namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.UI;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Spinner editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Spinner), true)]
	public class SpinnerEditor : SelectableEditor
	{
		#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		const string ValueChanged = "m_OnValueChanged";
		const string EndEdit = "m_OnEndEdit";
		#else
		const string ValueChanged = "m_OnValueChange";
		const string EndEdit = "m_EndEdit";
		#endif

		Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Properties.
		/// </summary>
		protected string[] properties = new string[]
		{
			// InputField
			"m_TextComponent",
			"m_CaretBlinkRate",
			"m_SelectionColor",
			"m_HideMobileInput",
			"m_Placeholder",
			ValueChanged,
			EndEdit,

			// Spinner
			"ValueMin",
			"ValueMax",
			"ValueStep",
			"SpinnerValue",
			"Validation",
			"plusButton",
			"minusButton",
			"AllowHold",
			"HoldStartDelay",
			"HoldChangeDelay",
			"onValueChangeInt",
			"onPlusClick",
			"onMinusClick",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			Array.ForEach(properties, x =>
			{
				var p = serializedObject.FindProperty(x);
				serializedProperties.Add(x, p);
			});
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			base.OnInspectorGUI();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedProperties["ValueMin"], true);
			EditorGUILayout.PropertyField(serializedProperties["ValueMax"], true);
			EditorGUILayout.PropertyField(serializedProperties["ValueStep"], true);
			EditorGUILayout.PropertyField(serializedProperties["SpinnerValue"], true);
			EditorGUILayout.PropertyField(serializedProperties["Validation"], true);
			EditorGUILayout.PropertyField(serializedProperties["AllowHold"], true);
			EditorGUILayout.PropertyField(serializedProperties["HoldStartDelay"], true);
			EditorGUILayout.PropertyField(serializedProperties["HoldChangeDelay"], true);
			EditorGUILayout.PropertyField(serializedProperties["plusButton"], true);
			EditorGUILayout.PropertyField(serializedProperties["minusButton"], true);

			EditorGUILayout.PropertyField(serializedProperties["m_TextComponent"]);

			if (serializedProperties["m_TextComponent"] != null && serializedProperties["m_TextComponent"].objectReferenceValue != null)
			{
				var text = serializedProperties["m_TextComponent"].objectReferenceValue as UnityEngine.UI.Text;
				if (text.supportRichText)
				{
					EditorGUILayout.HelpBox("Using Rich Text with input is unsupported.", MessageType.Warning);
				}

				if (text.alignment != TextAnchor.UpperLeft &&
					text.alignment != TextAnchor.UpperCenter &&
					text.alignment != TextAnchor.UpperRight)
				{
					EditorGUILayout.HelpBox("Using a non upper alignment with input is unsupported.", MessageType.Warning);
				}
			}

			EditorGUI.BeginDisabledGroup(serializedProperties["m_TextComponent"] == null || serializedProperties["m_TextComponent"].objectReferenceValue == null);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedProperties["m_Placeholder"]);
			EditorGUILayout.PropertyField(serializedProperties["m_CaretBlinkRate"]);
			EditorGUILayout.PropertyField(serializedProperties["m_SelectionColor"]);
			EditorGUILayout.PropertyField(serializedProperties["m_HideMobileInput"]);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedProperties[ValueChanged]);
			EditorGUILayout.PropertyField(serializedProperties[EndEdit]);

			EditorGUILayout.PropertyField(serializedProperties["onValueChangeInt"]);
			EditorGUILayout.PropertyField(serializedProperties["onPlusClick"]);
			EditorGUILayout.PropertyField(serializedProperties["onMinusClick"]);

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}
	}
}