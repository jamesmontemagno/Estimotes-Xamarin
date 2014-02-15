using System;
using System.Linq;

using Android.Content;

using EstimoteSdk;

using Object = Java.Lang.Object;

namespace Estimotes.Droid
{
    class BeaconFinder : Object, BeaconManager.IServiceReadyCallback
    {
        public static readonly Region ALL_ESTIMOTE_BEACONS_REGION = new Region("rid", EstimoteValues.ESTIMOTE_BEACON_PROXIMITY_UUID, null, null);
        readonly BeaconManager _beaconManager;
        public EventHandler<BeaconsFoundEventArgs> BeaconsFound = delegate { };
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

        public bool IsBluetoothEnabled { get { return _beaconManager.IsBluetoothEnabled; } }

        public void OnServiceReady()
        {
            _beaconManager.StartRanging(ALL_ESTIMOTE_BEACONS_REGION);
        }

        void HandleRanging(object sender, BeaconManager.RangingEventArgs e)
        {
            var filteredBeacons = (from item in e.Beacons
                                   let uuid = item.ProximityUUID
                                   where uuid.Equals(EstimoteValues.ESTIMOTE_BEACON_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase) ||
                                         uuid.Equals(EstimoteValues.ESTIMOTE_IOS_PROXIMITY_UUID, StringComparison.OrdinalIgnoreCase)
                                   select item).ToList();

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

        public void Stop()
        {
            _beaconManager.StopRanging(ALL_ESTIMOTE_BEACONS_REGION);
            _beaconManager.Disconnect();
        }
    }
}
