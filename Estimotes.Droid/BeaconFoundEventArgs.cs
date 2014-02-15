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
    class BeaconFoundEventArgs : EventArgs
    {
        public BeaconFoundEventArgs(Beacon beacon)
        {
            FoundBeacon = beacon;
        }

        public Beacon FoundBeacon { get; private set; }
    }
    
}
