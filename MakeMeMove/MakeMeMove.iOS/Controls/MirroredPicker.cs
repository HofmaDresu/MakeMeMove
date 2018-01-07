using System;
using System.Linq;
using System.Collections.Generic;
using MakeMeMove.iOS.Models;
using UIKit;
using MakeMeMove.iOS.Helpers;

namespace MakeMeMove.iOS.Controls
{
	public static class MirroredPicker
	{
		public static UIPickerView Create(PickerModel model, UITextField fieldToMirror, Func<IList<string>, string> transformFunction = null, Action doneAction = null, Action changeAction = null)
		{
			var pickerView = new UIPickerView { Model = model };

			var toolbar = new UIToolbar { BarStyle = UIBarStyle.Default, BarTintColor = Colors.PrimaryColor, Translucent = true };
			toolbar.SizeToFit();

			model.PickerChanged += (sender, e) =>
			{
				var fieldVal = transformFunction == null ? e.SelectedValues[0].ToString() : transformFunction(e.SelectedValues.Select(o => o.ToString()).ToList());
				fieldToMirror.Text = fieldVal;
                changeAction?.Invoke();
            };

			var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				fieldToMirror.ResignFirstResponder();
				doneAction?.Invoke();
			});
			doneButton.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = UIColor.White
			}, UIControlState.Normal);
			toolbar.SetItems(new[] { doneButton }, true);

			fieldToMirror.InputView = pickerView;
			fieldToMirror.InputAccessoryView = toolbar;
			return pickerView;
		}
	}
}

