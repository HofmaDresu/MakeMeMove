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
                .SetMessage("TEST MESSAGE")
                .SetPositiveButton(Resource.String.Save, (s, args) => _listener.OnSaveClick("TEST"))
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