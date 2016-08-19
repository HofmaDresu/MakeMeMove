using System;
using System.Linq;
using System.Collections.Generic;
using UIKit;

namespace MakeMeMove.iOS.Models
{
	public class PickerModel : UIPickerViewModel
	{
		public List<List<object>> values;

		public event EventHandler<PickerChangedEventArgs> PickerChanged;

		public PickerModel(List<string> values)
		{
			var convertedStrings = new List<object>();
			foreach (var item in values)
			{
				convertedStrings.Add(item);
			}

			this.values = new List<List<object>>();
			this.values.Add(convertedStrings);
		}

		public PickerModel(List<object> values)
		{
			this.values = new List<List<object>> { values };
		}

		public PickerModel(List<List<object>> values)
		{
			this.values = values;
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return values.Count();
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return values[(int)component].Count;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			return values[(int)component][(int)row].ToString();
		}

		public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
		{
			return 40f;
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			var SelectedValues = new List<object>();

			for (var i = 0; i < values.Count(); i++)
			{
				SelectedValues.Add(values[i][(int)pickerView.SelectedRowInComponent(i)]);
			}

			if (PickerChanged != null)
			{
				PickerChanged(pickerView, new PickerChangedEventArgs { SelectedValues = SelectedValues.ToArray() });
			}
		}
	}

	public class PickerChangedEventArgs : EventArgs
	{
		public object[] SelectedValues { get; set; }
	}
}

