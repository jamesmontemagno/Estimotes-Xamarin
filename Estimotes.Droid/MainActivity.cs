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
        IMenuItem _refreshItem;

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
            _quickAction = new QuickAction(this, QuickActionLayout.Horizontal);
            var builder = _quickAction.GetBuilder();

            builder.SetItemId(QUICKACTION_NOTIFYDEMO)
                .SetTitle("Distance Demo")
                .SetIcon(Android.Resource.Drawable.StatSysDataBluetooth)
                .AddToParent();

            builder.SetItemId(QUICKACTION_NOTIFYDEMO)
                .SetTitle("Notify Demo")
                .SetIcon(Android.Resource.Drawable.StatNotifyError)
                .AddToParent();
            
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
                var imgView = e.View.FindViewWithTag("beacon_image") ;
                _quickAction.Show(imgView);
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
            _refreshItem = menu.FindItem(Resource.Id.refresh);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }
            if (item.ItemId == Resource.Id.refresh)
            {
                LookForBeacons();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnStart()
        {
            base.OnStart();
            LookForBeacons();
        }

        protected override void OnPause()
        {
            _selectedBeacon = null;
            base.OnPause();
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
                    _adapter.Update(new Beacon[0]);
                    LookForBeacons();
                }
                else
                {
                    Toast.MakeText(this, "Bluetooth not enabled.", ToastLength.Long).Show();
                    ActionBar.Subtitle = "Bluetooth not enabled.";
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        void LookForBeacons()
        {
            if (!_findAllBeacons.IsBluetoothEnabled)
            {
                var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, EstimoteValues.REQUEST_ENABLE_BLUETOOTH);
            }
            else
            {
                _refreshItem.SetActionView(Resource.Layout.actionbar_indeterminate_progress);
                _findAllBeacons.FindBeacons();
            }
        }

        void NewBeaconsFound(object sender, BeaconsFoundEventArgs e)
        {
            _adapter.Update(e.Beacons);
            ActionBar.Subtitle = string.Format("Found {0} beacons.", _adapter.Count);
            _findAllBeacons.Stop();

            // TODO - change back to the refresh icon.
        }

    }
}
