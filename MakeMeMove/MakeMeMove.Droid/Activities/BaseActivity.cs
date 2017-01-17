using System.IO;
using Android.Support.V7.App;
using SQLite;
using Environment = System.Environment;
using Android.Content;
using Android.Widget;

namespace MakeMeMove.Droid.Activities
{
    public class BaseActivity : AppCompatActivity
    {
        protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));

        protected override void OnResume()
        {
            base.OnResume();

            if(Data.ShouldAskForRating())
            {
                
                new AlertDialog.Builder(this)
                    .SetTitle(Resource.String.rate_title)
                    .SetMessage(Resource.String.rate_message)
                    .SetCancelable(false)
                    .SetPositiveButton(Resource.String.sure, (sender, args) =>
                    {
                        var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=co.fudist.makememove"));

                        try
                        {
                            StartActivity(intent);
                        }
                        catch (System.Exception)
                        {
                            Toast.MakeText(this, Resource.String.MarketFailure, ToastLength.Long);
                            Data.ResetRatingCycle();
                            return;
                        }
                        Data.PreventRatingCheck();
                    })
                    .SetNegativeButton(Resource.String.never, (sender, args) =>
                    {
                        Data.PreventRatingCheck();
                    })
                    .SetNeutralButton(Resource.String.not_now, (sender, args) =>
                    {
                        Data.ResetRatingCycle();
                    })
                    .Show();
            }

        }
    }
}