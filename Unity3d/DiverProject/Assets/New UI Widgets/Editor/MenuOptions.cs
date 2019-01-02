#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class MenuOptions
	{
#region Collections

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/Combobox", false, 1000)]
		public static void CreateCombobox()
		{
			Utilites.CreateWidgetFromAsset("Combobox");
		}

		/// <summary>
		/// Create ComboboxIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIcons");
		}

		/// <summary>
		/// Create ComboboxIconsMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsMultiselect");
		}

		/// <summary>
		/// Create DirectoryTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Utilites.CreateWidgetFromAsset("DirectoryTreeView");
		}

		/// <summary>
		/// Create FileListView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Utilites.CreateWidgetFromAsset("FileListView");
		}

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Utilites.CreateWidgetFromAsset("ListView");
		}

		/// <summary>
		/// Create istViewColors.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListViewColors", false, 1055)]
		public static void CreateListViewColors()
		{
			Utilites.CreateWidgetFromAsset("ListViewColors");
		}

		/// <summary>
		/// Create ListViewInt.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Utilites.CreateWidgetFromAsset("ListViewInt");
		}

		/// <summary>
		/// Create ListViewHeight.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Utilites.CreateWidgetFromAsset("ListViewHeight");
		}

		/// <summary>
		/// Create ListViewIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Utilites.CreateWidgetFromAsset("ListViewIcons");
		}

		/// <summary>
		/// Create ListViewPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Utilites.CreateWidgetFromAsset("ListViewPaginator");
		}

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Utilites.CreateWidgetFromAsset("TreeView");
		}
#endregion

#region Containers

		/// <summary>
		/// Create Accordion.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Utilites.CreateWidgetFromAsset("Accordion");
		}

		/// <summary>
		/// Create Tabs.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Containers/Tabs", false, 2010)]
		public static void CreateTabs()
		{
			Utilites.CreateWidgetFromAsset("Tabs");
		}

		/// <summary>
		/// Create TabsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Containers/TabsLeft", false, 2020)]
		public static void CreateTabsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsLeft");
		}

		/// <summary>
		/// Create TabsIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Containers/TabsIcons", false, 2030)]
		public static void CreateTabsIcons()
		{
			Utilites.CreateWidgetFromAsset("TabsIcons");
		}

		/// <summary>
		/// Create TabsIconsLeft.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Containers/TabsIconsLeft", false, 2040)]
		public static void CreateTabsIconsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsLeft");
		}
#endregion

#region Dialogs

		/// <summary>
		/// Create DatePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/DatePicker", false, 3000)]
		public static void CreateDatePicker()
		{
			Utilites.CreateWidgetFromAsset("DatePicker");
		}

		/// <summary>
		/// Create DateTimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/DateTimePicker", false, 3005)]
		public static void CreateDateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("DateTimePicker");
		}

		/// <summary>
		/// Create Dialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/Dialog Template", false, 3010)]
		public static void CreateDialog()
		{
			Utilites.CreateWidgetFromAsset("DialogTemplate");
		}

		/// <summary>
		/// Create FileDialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Utilites.CreateWidgetFromAsset("FileDialog");
		}

		/// <summary>
		/// Create FolderDialog.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Utilites.CreateWidgetFromAsset("FolderDialog");
		}

		/// <summary>
		/// Create Notify.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/Notify Template", false, 3040)]
		public static void CreateNotify()
		{
			Utilites.CreateWidgetFromAsset("NotifyTemplate");
		}

		/// <summary>
		/// Create PickerBool.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Utilites.CreateWidgetFromAsset("PickerBool");
		}

		/// <summary>
		/// Create PickerIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/PickerIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Utilites.CreateWidgetFromAsset("PickerIcons");
		}

		/// <summary>
		/// Create PickerInt.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Utilites.CreateWidgetFromAsset("PickerInt");
		}

		/// <summary>
		/// Create PickerString.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Utilites.CreateWidgetFromAsset("PickerString");
		}

		/// <summary>
		/// Create Popup.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Utilites.CreateWidgetFromAsset("Popup");
		}

		/// <summary>
		/// Create TimePicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Dialogs/TimePicker", false, 3100)]
		public static void CreateTimePicker()
		{
			Utilites.CreateWidgetFromAsset("TimePicker");
		}
#endregion

#region Input

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Utilites.CreateWidgetFromAsset("Autocomplete");
		}

		/// <summary>
		/// Create AutocompleteIcons.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteIcons");
		}

		/// <summary>
		/// Create ButtonBig.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ButtonBig", false, 4000)]
		public static void CreateButtonBig()
		{
			Utilites.CreateWidgetFromAsset("ButtonBig");
		}

		/// <summary>
		/// Create ButtonSmall.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ButtonSmall", false, 4010)]
		public static void CreateButtonSmall()
		{
			Utilites.CreateWidgetFromAsset("ButtonSmall");
		}

		/// <summary>
		/// Create Calendar.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Utilites.CreateWidgetFromAsset("Calendar");
		}

		/// <summary>
		/// Create CenteredSlider.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Utilites.CreateWidgetFromAsset("CenteredSlider");
		}

		/// <summary>
		/// Create CenteredSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("CenteredSliderVertical");
		}

		/// <summary>
		/// Create ColorPicker.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Utilites.CreateWidgetFromAsset("ColorPicker");
		}

		/// <summary>
		/// Create ColorPickerRange.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRange");
		}

		/// <summary>
		/// Create ColorPickerRangeHSV.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ColorPickerRangeHSV", false, 4063)]
		public static void CreateColorPickerRangeHSV()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRangeHSV");
		}

		/// <summary>
		/// Create ColorsList.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/ColorsList", false, 4065)]
		public static void CreateColorsList()
		{
			Utilites.CreateWidgetFromAsset("ColorsList");
		}

		/// <summary>
		/// Create DateTime.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/DateTime", false, 4067)]
		public static void CreateDateTime()
		{
			Utilites.CreateWidgetFromAsset("DateTime");
		}

		/// <summary>
		/// Create RangeSlider.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/RangeSlider", false, 4070)]
		public static void CreateRangeSlider()
		{
			Utilites.CreateWidgetFromAsset("RangeSlider");
		}

		/// <summary>
		/// Create RangeSliderFloat.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloat");
		}

		/// <summary>
		/// Create RangeSliderVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/RangeSliderVertical", false, 4090)]
		public static void CreateRangeSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderVertical");
		}

		/// <summary>
		/// Create RangeSliderFloatVertical.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloatVertical");
		}

		/// <summary>
		/// Create Spinner.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Utilites.CreateWidgetFromAsset("Spinner");
		}

		/// <summary>
		/// Create SpinnerFloat.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloat");
		}

		/// <summary>
		/// Create Switch.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Utilites.CreateWidgetFromAsset("Switch");
		}

		/// <summary>
		/// Create Time12.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Time12", false, 4140)]
		public static void CreateTime12()
		{
			Utilites.CreateWidgetFromAsset("Time12");
		}

		/// <summary>
		/// Create Time24.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Input/Time24", false, 4150)]
		public static void CreateTime24()
		{
			Utilites.CreateWidgetFromAsset("Time24");
		}
#endregion

		/// <summary>
		/// Create AudioPlayer.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Utilites.CreateWidgetFromAsset("AudioPlayer");
		}

		/// <summary>
		/// Create Progressbar.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/(obsolete) Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Utilites.CreateWidgetFromAsset("Progressbar");
		}

		/// <summary>
		/// Create ProgressbarDeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/ProgressbarDeterminate", false, 5014)]
		public static void CreateProgressbarDeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarDeterminate");
		}

		/// <summary>
		/// Create ProgressbarIndeterminate.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/ProgressbarIndeterminate", false, 5017)]
		public static void CreateProgressbarIndeterminate()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarIndeterminate");
		}

		/// <summary>
		/// Create ScrollRectPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/ScrollRectPaginator", false, 5020)]
		public static void CreateScrollRectPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectPaginator");
		}

		/// <summary>
		/// Create ScrollRectNumericPaginator.
		/// </summary>
		[MenuItem("GameObject/UI/UIWidgets/Misc/ScrollRectNumericPaginator", false, 5030)]
		public static void CreateScrollRectNumericPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectNumericPaginator");
		}
	}
}
#endif