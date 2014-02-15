using System;
using System.Collections.Generic;
using Android.Content;
using EstimoteSdk;
using EstimoteSdk.Utils;

namespace Estimotes.Droid
{
    class BeaconFinder : Java.Lang.Object,  BeaconManager.IServiceReadyCallback
    {
        public static readonly Region ALL_ESTIMOTE_BEACONS_REGION = new Region("rid", EstimoteValues.ESTIMOTE_BEACON_PROXIMITY_UUID, null, null);
        readonly BeaconManager _beaconManager;
        public EventHandler<BeaconsFoundEventArgs> BeaconsFound = delegate
        {
        };
        // Analysis disable once FieldCanBeMadeReadOnly.Local
        object _locker = new object();

        public BeaconFinder(Context context)
        {
            #if DEBUG
            L.EnableDebugLogging(true);
            #endif

            _beaconManager = new BeaconManager(context);
            if (!_beaconManager.HasBluetooth)
            {
                throw new Exception("The device does not have have Bluetooth!");
            }

            _beaconManager.Ranging += HandleRanging;
        }

        void HandleRanging(object sender, BeaconManager.RangingEventArgs e)
        {
            var filteredBeacons = new List<Beacon>();
            foreach (var item in e.Beacons)
            {
                var uuid = item.ProximityUUID;
                if (uuid.Equals(EstimoteValues.ESTIMOTE_BEACON_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase) ||
                    uuid.Equals(EstimoteValues.ESTIMOTE_IOS_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase))
                {
                    filteredBeacons.Add(item);
                }
            }

            BeaconsFound(this, new BeaconsFoundEventArgs(filteredBeacons));
        }

        public void FindBeacons()
        {
            if (!_beaconManager.IsBluetoothEnabled)
            {
                throw new Exception("Bluetooth is not enabled.");
            }
            _beaconManager.Connect(this);
        }

        public void OnServiceReady()
        {
            _beaconManager.StartRanging(ALL_ESTIMOTE_BEACONS_REGION);
        }

        public void Stop()
        {
            _beaconManager.StopRanging(ALL_ESTIMOTE_BEACONS_REGION);
            _beaconManager.Disconnect();
        }

        public bool IsBluetoothEnabled
        {
            get { return _beaconManager.IsBluetoothEnabled; }
        }
    }
}

