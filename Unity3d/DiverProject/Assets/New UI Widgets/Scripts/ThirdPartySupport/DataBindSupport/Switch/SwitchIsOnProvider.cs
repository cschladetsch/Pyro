#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;
	
	/// <summary>
	/// Provides the IsOn of an Switch.
	/// </summary>
	[AddComponentMenu("Data Bind/UIWidgets/Getters/[DB] Switch IsOn Provider")]
	public class SwitchIsOnProvider : ComponentDataProvider<UIWidgets.Switch, System.Boolean>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Switch target)
		{
			target.OnValueChanged.AddListener(OnValueChangedSwitch);
		}

		/// <inheritdoc />
		protected override System.Boolean GetValue(UIWidgets.Switch target)
		{
			return target.IsOn;
		}
		
		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Switch target)
		{
			target.OnValueChanged.RemoveListener(OnValueChangedSwitch);
		}
		
		void OnValueChangedSwitch(System.Boolean arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif