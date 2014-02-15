using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using EstimoteSdk;

namespace Estimotes.Droid
{
    [Activity(Label = "DistanceBeaconActivity")]			
    public class DistanceBeaconActivity : Activity, ViewTreeObserver.IOnGlobalLayoutListener
    {
        static readonly string Tag = typeof(DistanceBeaconActivity).FullName;
        static readonly double RELATIVE_START_POS = 320.0 / 1110.0;
        static readonly double RELATIVE_STOP_POS = 885.0 / 1110.0;
        SpecificBeaconFinder _beaconFinder;
        Beacon _beacon;
        Region _region;
        View _dotView;
        View _sonar;
        int _startY = -1;
        int _segmentLength = -1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.distance_view);
            _dotView = FindViewById(Resource.Id.dot);
            _beacon = Intent.GetParcelableExtra(ListBeaconsActivity.EXTRAS_BEACON) as Beacon;
            _beaconFinder = new SpecificBeaconFinder(this);

            _beaconFinder.BeaconFound += (sender, e) =>  _dotView.Animate().TranslationY(ComputeDotPosY(e.FoundBeacon)).Start();


            _sonar = FindViewById(Resource.Id.sonar);
            _sonar.ViewTreeObserver.AddOnGlobalLayoutListener(this);

            if (_beacon == null)
            {
                Toast.MakeText(this, "Beacon not found in intent extras.", ToastLength.Long).Show();
                Finish();
            }
            _region = new Region("regionid", _beacon.ProximityUUID, new Java.Lang.Integer(_beacon.Major), new Java.Lang.Integer(_beacon.Minor));
        }

        public void OnGlobalLayout()
        {
            _sonar.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
            _startY = (int)(RELATIVE_START_POS * _sonar.MeasuredHeight);
            var stopY = (int)(RELATIVE_STOP_POS * _sonar.MeasuredHeight);
            _segmentLength = stopY - _startY;
            _dotView.Visibility = ViewStates.Visible;
            _dotView.TranslationY = ComputeDotPosY(_beacon);
        }

        float ComputeDotPosY(Beacon foundBeacon)
        {
            // Put the dot at the end of the scale when it's further than 6m.
            var distance = Math.Min(EstimoteSdk.Utils.ComputeAccuracy(foundBeacon), 6.0);
            return _startY + (int)(_segmentLength * (distance / 6.0));
        }
    }
}

