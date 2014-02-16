using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

using EstimoteSdk;

using Object = Java.Lang.Object;

namespace Estimotes.Droid
{
    class FindSpecificBeacon : Object, BeaconManager.IRangingListener, BeaconManager.IServiceReadyCallback
    {
        static readonly string Tag = typeof(FindSpecificBeacon).FullName;
        readonly BeaconManager _beaconManager;
        readonly Context _context;
        public EventHandler<BeaconFoundEventArgs> BeaconFound = delegate { };
        Beacon _beacon;
        bool _isSearching = false;
        Region _region;

        public FindSpecificBeacon(Context context)
        {
            _context = context;
            _beaconManager = new BeaconManager(context);
            _beaconManager.SetRangingListener(this);
        }

        public void OnBeaconsDiscovered(Region region, IList<Beacon> beacons)
        {
            Log.Debug(Tag, "Found {0} beacons", beacons.Count);
            var foundBeacon = (from b in beacons
                               where b.MacAddress.Equals(_beacon.MacAddress)
                               select b).FirstOrDefault();
            if (foundBeacon != null)
            {
                BeaconFound(this, new BeaconFoundEventArgs(foundBeacon));
            }
        }

        public void OnServiceReady()
        {
            if (_region == null)
            {
                throw new Exception("What happened to the _region?");
            }
            try
            {
                _beaconManager.StartRanging(_region);
                Log.Debug(Tag, "Looking for beacons in the region.");
                _isSearching = true;
            }
            catch (RemoteException e)
            {
                Toast.MakeText(_context, "Cannot start ranging, something terrible happened!", ToastLength.Long).Show();
                Log.Error(Tag, "Cannot start ranging, {0}", e);
            }
        }

        public void LookForBeacon(Region region, Beacon beacon)
        {
            _beacon = beacon;
            _region = region;
            _beaconManager.Connect(this);
        }

        public void Stop()
        {
            if (_isSearching)
            {
                _beaconManager.StopRanging(_region);
                _beaconManager.Disconnect();
                _region = null;
                _beacon = null;
                _isSearching = false;
            }
        }
    }
}
