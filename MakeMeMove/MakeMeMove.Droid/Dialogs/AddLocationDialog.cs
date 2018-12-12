using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace MakeMeMove.Droid.Dialogs
{
    public interface IAddLocationDialogListener
    {
        void OnSaveClick(string locationName);
    }

    public class AddLocationDialog : DialogFragment
    {
        private IAddLocationDialogListener _listener;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder
                .SetView(Activity.LayoutInflater.Inflate(Resource.Layout.LocationDialog, null))
                .SetPositiveButton(Resource.String.Save, (dialog, args) =>
                {
                    var newLocationNameView = (dialog as AlertDialog).FindViewById<TextView>(Resource.Id.NewLocationName);

                    if (!string.IsNullOrWhiteSpace(newLocationNameView.Text))
                    {
                        _listener.OnSaveClick(newLocationNameView.Text);
                    }
                })
                .SetNegativeButton(Resource.String.Cancel, (s, args) => { });

            return builder.Create();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context is IAddLocationDialogListener listener)
            {
                _listener = listener;
            }
        }
    }
}