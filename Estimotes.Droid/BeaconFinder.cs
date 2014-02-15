using System;
using System.Collections.Generic;
using Android.Content;
using EstimoteSdk;
using EstimoteSdk.Utils;

namespace Estimotes.Droid
{
    class BeaconFinder : Java.Lang.Object,  BeaconManager.IServiceReadyCallback
    {
        public static readonly int REQUEST_ENABLE_BLUETOOTH = 12344321;
        static readonly String ESTIMOTE_BEACON_PROXIMITY_UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D";
        static readonly String ESTIMOTE_IOS_PROXIMITY_UUID = "8492E75F-4FD6-469D-B132-043FE94921D8";
        static readonly Region ALL_ESTIMOTE_BEACONS_REGION = new Region("rid", ESTIMOTE_BEACON_PROXIMITY_UUID, null, null);
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
                throw new Exception("The device does not have have Bluetooth LE!");
            }

            _beaconManager.Ranging += HandleRanging;

        }

        void HandleRanging(object sender, BeaconManager.RangingEventArgs e)
        {
            var filteredBeacons = new List<Beacon>();
            foreach (var item in e.P1)
            {
                var uuid = item.ProximityUUID;
                if (uuid.Equals(ESTIMOTE_BEACON_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase) ||
                    uuid.Equals(ESTIMOTE_IOS_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase))
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

