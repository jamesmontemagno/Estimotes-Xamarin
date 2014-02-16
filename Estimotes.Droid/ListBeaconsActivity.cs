using System;
using System.Collections.Generic;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

using EstimoteSdk;

namespace Estimotes.Droid
{
    [Activity(Label = "ListBeaconsActivity")]
    public class ListBeaconsActivity : Activity
    {
        public static readonly String EXTRAS_TARGET_ACTIVITY = "extrasTargetActivity";
        public static readonly String EXTRAS_BEACON = "extrasBeacon";
        static readonly String Tag = typeof(ListBeaconsActivity).FullName;
        LeDevicesListAdapter _adapter;
        FindAllBeacons _findAllBeacons;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _findAllBeacons = new FindAllBeacons(this);
            _findAllBeacons.BeaconsFound += NewBeaconsFound;

            _adapter = new LeDevicesListAdapter(this);
            var list = FindViewById<ListView>(Resource.Id.device_list);
            list.Adapter = _adapter;
            list.ItemClick += (sender, e) =>{
                                  var beacon = _adapter[e.Position];
                                  var intent = new Intent(this, typeof(DistanceBeaconActivity));
                                  intent.PutExtra(EXTRAS_BEACON, beacon);
                                  StartActivity(intent);
                              };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.scan_menu, menu);
            var refreshItem = menu.FindItem(Resource.Id.refresh);
            //            refreshItem.SetActionView(Resource.Layout.actionbar_indeterminate_progress);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (!_findAllBeacons.IsBluetoothEnabled)
            {
                var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, EstimoteValues.REQUEST_ENABLE_BLUETOOTH);
            }
            else
            {
                _findAllBeacons.FindBeacons();
            }
        }

        void NewBeaconsFound(object sender, BeaconsFoundEventArgs e)
        {
            _adapter.Update(e.Beacons);
            ActionBar.Subtitle = string.Format("Found {0} beacons.", _adapter.Count);
            _findAllBeacons.Stop();
        }

        protected override void OnStop()
        {
            try
            {
                _findAllBeacons.Stop();
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
                    _findAllBeacons.FindBeacons();
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
