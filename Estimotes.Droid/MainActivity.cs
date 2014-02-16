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
using Java.Interop;

namespace Estimotes.Droid
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        const int QUICKACTION_DISTANCEDEMO = 1;
        const int QUICKACTION_NOTIFYDEMO = 2;

        static readonly String Tag = typeof(MainActivity).FullName;

        LeDevicesListAdapter _adapter;
        FindAllBeacons _findAllBeacons;
        QuickAction _quickAction;
        Beacon _selectedBeacon;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main);
            ActionBar.SetHomeButtonEnabled(true);

            _findAllBeacons = new FindAllBeacons(this);
            _findAllBeacons.BeaconsFound += NewBeaconsFound;

            InitializeQuickAction();

            InitializeListView();
        }

        void InitializeQuickAction()
        {
            var distanceItem = new ActionItem(QUICKACTION_DISTANCEDEMO, "Distance Demo", Resources.GetDrawable(Android.Resource.Drawable.StatSysDataBluetooth));
            var notifyItem = new ActionItem(QUICKACTION_NOTIFYDEMO, "Notify Demo", Resources.GetDrawable(Android.Resource.Drawable.StatNotifyError));
            _quickAction = new QuickAction(this, QuickActionLayout.Horizontal);
            _quickAction.AddActionItem(distanceItem);
            _quickAction.AddActionItem(notifyItem);
            _quickAction.ActionItemClicked += HandleActionItemClicked;
        }

        void InitializeListView()
        {
            _adapter = new LeDevicesListAdapter(this);
            var list = FindViewById<ListView>(Resource.Id.device_list);
            list.Adapter = _adapter;
            list.ItemClick += (sender, e) =>
            {
                _selectedBeacon= _adapter[e.Position];
                _quickAction.Show((View) sender);
            };
        }

        void HandleActionItemClicked (object sender, ActionItemClickEventArgs e)
        {
            switch (e.ActionItem.ActionId)
            {
                case QUICKACTION_DISTANCEDEMO:
                    this.StartActivityForBeacon<DistanceBeaconActivity>(_selectedBeacon);
                    break;
                case QUICKACTION_NOTIFYDEMO:
                    this.StartActivityForBeacon<NotifyDemoActivity>(_selectedBeacon);
                    break;
                default:
                    Log.Wtf(Tag, "Don't know how to handle the ActionItem {0}.", e.ActionItem.Title);
                    break;
            }
            _selectedBeacon = null;
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

        protected override void OnPause()
        {
            _selectedBeacon = null;
            base.OnPause();
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
