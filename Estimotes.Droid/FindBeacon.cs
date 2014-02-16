using System;

using Android.Content;

using EstimoteSdk;
using EstimoteSdk.Utility;

using JavaObject = Java.Lang.Object;

namespace Estimotes.Droid
{

    abstract class FindBeacon : JavaObject, BeaconManager.IServiceReadyCallback
    {
        protected FindBeacon(Context context)
        {
            #if DEBUG
            L.EnableDebugLogging(true);
            #endif

            Context = context;
            BeaconManager = new BeaconManager(context);
            if (!BeaconManager.HasBluetooth)
            {
                throw new Exception("The device does not have have Bluetooth!");
            }

        }
        protected Context Context { get; private set; }
        protected BeaconManager BeaconManager { get; private set;}
        public bool IsBluetoothEnabled { get { return BeaconManager.IsBluetoothEnabled; } }

        public abstract void OnServiceReady();

        public virtual void Stop()
        {
            BeaconManager.Disconnect();
        }

    }
    
}
