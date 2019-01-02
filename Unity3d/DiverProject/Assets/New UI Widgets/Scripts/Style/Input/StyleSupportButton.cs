namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the button.
	/// </summary>
	public class StyleSupportButton : MonoBehaviour
	{
		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// The mask.
		/// </summary>
		[SerializeField]
		public Image Mask;

		/// <summary>
		/// The border.
		/// </summary>
		[SerializeField]
		public Image Border;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public GameObject Text;

		/// <summary>
		/// The image.
		/// </summary>
		[SerializeField]
		public Image Image;
	}
}