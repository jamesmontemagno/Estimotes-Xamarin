using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Estimotes.Droid
{
    [Activity(Label = "Estimotes.Droid", MainLauncher = true )]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.all_demos);
            ActionBar.SetDisplayHomeAsUpEnabled(true);


            var b1 = FindViewById<Button>(Resource.Id.distance_demo_button);
            b1.Click += (sender, e) => {
                var intent = new Intent(this, typeof(ListBeaconsActivity));
                StartActivity(intent);
            };

        }
    }
}


