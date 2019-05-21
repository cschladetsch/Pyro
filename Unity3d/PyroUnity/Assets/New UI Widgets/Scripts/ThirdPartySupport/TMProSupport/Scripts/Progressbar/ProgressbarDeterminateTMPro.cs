#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// ProgressbarDeterminateTMPro.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ProgressbarDeterminate.png
	/// <example>
	/// // Example of using Progressbar for show instantiating progress.
	/// <code>
	/// var bar = GetComponent&lt;Progressbar&gt;();
	/// bar.Max = 10000;
	/// for (int i = 0; i &lt; 10000; i++)
	/// {
	/// 	Instantiate(prefab);
	/// 	if (i % 100)
	/// 	{
	/// 		bar.Value = i;
	/// 		yield return null;//wait 1 frame
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public class ProgressbarDeterminateTMPro : ProgressbarDeterminateBase
	{
		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI EmptyBarText;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI FullBarText;

		/// <summary>
		/// Updates the text.
		/// </summary>
		protected override void UpdateText()
		{
			var text = TextFunc(this);

			if (FullBarText != null)
			{
				FullBarText.text = text;
			}

			if (EmptyBarText != null)
			{
				EmptyBarText.text = text;
			}
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			if (FullBarText != null)
			{
				style.ProgressbarDeterminate.FullBarText.ApplyTo(FullBarText.gameObject);
			}

			if (EmptyBarText != null)
			{
				style.ProgressbarDeterminate.EmptyBarText.ApplyTo(EmptyBarText.gameObject);
			}

			return true;
		}
		#endregion
	}
}
#endif