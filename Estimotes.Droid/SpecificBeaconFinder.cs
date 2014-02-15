using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using EstimoteSdk;
using Android;
using System.Globalization;

namespace Estimotes.Droid
{

    class SpecificBeaconFinder: Java.Lang.Object, BeaconManager.IRangingListener, BeaconManager.IServiceReadyCallback
    {
        static readonly string Tag = typeof(SpecificBeaconFinder).Name;
        public EventHandler<BeaconFoundEventArgs> BeaconFound = delegate
        {

        };

        readonly Context _context;
        BeaconManager _beaconManager;
        Beacon _beacon;
        Region _region;

        public SpecificBeaconFinder(Context context)
        {
            _context = context;
            _beaconManager = new BeaconManager(context);
            _beaconManager.SetRangingListener(this);
        }

        public void LookForBeacon(Region region, Beacon beacon)
        {
            _beacon = beacon;
            _beaconManager.Connect(this);
        }

        public void OnBeaconsDiscovered(Region region, IList<Beacon> beacons)
        {
            _region = region;
            var foundBeacon = (from b in beacons
                                        where b.MacAddress.Equals(_beacon.MacAddress)
                                        select b).FirstOrDefault();
            if (foundBeacon != null)
            {
                BeaconFound(this, new BeaconFoundEventArgs(foundBeacon));
            }

            _beaconManager.StopRanging(_region);
            _beaconManager.Disconnect();
            _region = null;
            _beacon = null;
        }

        public void OnServiceReady()
        {
            if (_region == null)
            {
                return;
            }
            try 
            {
                _beaconManager.StartRanging(_region);
            }
            catch (RemoteException e)
            {
                Toast.MakeText(_context, "Cannot start ranging, something terrible happened!", ToastLength.Long).Show();
                Log.Error(Tag, "Cannot start ranging, {0}", e);
            }

        }
    }
    
}
