using System;
using System.Collections.Generic;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

using EstimoteSdk;

namespace Estimotes.Droid
{
    [Activity(Label = "ListBeaconsActivity")]
    public class ListBeaconsActivity : Activity
    {
        static readonly String Tag = typeof(ListBeaconsActivity).FullName;
        static readonly String EXTRAS_TARGET_ACTIVITY = "extrasTargetActivity";
        static readonly String EXTRAS_BEACON = "extrasBeacon";
        LeDevicesListAdapter _adapter;
        BeaconFinder _beaconFinder;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _beaconFinder = new BeaconFinder(this);
            _beaconFinder.BeaconsFound += NewBeaconsFound;

            _adapter = new LeDevicesListAdapter(this);
            var list = FindViewById<ListView>(Resource.Id.device_list);
            list.Adapter = _adapter;
            list.ItemClick += (sender, e) => { var beacon = _adapter[e.Position]; };
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (!_beaconFinder.IsBluetoothEnabled)
            {
                var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, EstimoteValues.REQUEST_ENABLE_BLUETOOTH);
            }
            else
            {
                _beaconFinder.FindBeacons();
            }
        }

        void NewBeaconsFound(object sender, BeaconsFoundEventArgs e)
        {
            _adapter.Update(e.Beacons);
            ActionBar.Subtitle = string.Format("Found {0} beacons.", _adapter.Count);
            _beaconFinder.Stop();
        }

        protected override void OnStop()
        {
            try
            {
                _beaconFinder.Stop();
            }
            catch (RemoteException e)
            {
                Log.Debug(Tag, "Error while stopping ranging");
            }

            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == EstimoteValues.REQUEST_ENABLE_BLUETOOTH)
            {
                if (resultCode == Result.Ok)
                {
                    ActionBar.Subtitle = "Scanning...";
                    _adapter.Update(new List<Beacon>(0));
                    _beaconFinder.FindBeacons();
                }
                else
                {
                    Toast.MakeText(this, "Bluetooth not enabled", ToastLength.Long).Show();
                    ActionBar.Subtitle = "Bluetooth not enabled";
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}
