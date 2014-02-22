using System;
using System.Linq;

using Android.Content;

using EstimoteSdk;

namespace Estimotes.Droid
{

    class FindAllBeacons : FindBeacon
    {
        public static readonly Region ALL_ESTIMOTE_BEACONS_REGION = new Region("rid", EstimoteValues.ESTIMOTE_BEACON_PROXIMITY_UUID, null, null);
        public EventHandler<BeaconsFoundEventArgs> BeaconsFound = delegate { };

        public FindAllBeacons(Context context): base(context)
        {
            BeaconManager.Ranging += HandleRanging;
        }


        public  override void OnServiceReady()
        {
            BeaconManager.StartRanging(ALL_ESTIMOTE_BEACONS_REGION);
        }

        protected virtual void HandleRanging(object sender, BeaconManager.RangingEventArgs e)
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
            if (!BeaconManager.IsBluetoothEnabled)
            {
                throw new Exception("Bluetooth is not enabled.");
            }
            BeaconManager.Connect(this);
        }

        public override void Stop()
        {
            BeaconManager.StopRanging(ALL_ESTIMOTE_BEACONS_REGION);
            base.Stop();
        }

    }
}
