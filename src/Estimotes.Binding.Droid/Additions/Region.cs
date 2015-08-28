using System;
using Android.Runtime;

namespace EstimoteSdk
{
    public partial class Region
    {

        public Region(string identifier, string proximityUUID)
            : this(identifier, proximityUUID, null, null)
        {
            
        }

        public Region(string identifier, string proximityUUID, int major)
            : this(identifier, proximityUUID, new Java.Lang.Integer(major), null)
        {

        }

        public Region(string identifier, string proximityUUID, int major, int minor)
            : this(identifier, proximityUUID, new Java.Lang.Integer(major), new Java.Lang.Integer(minor))
        {

        }

        public int Major {
            get {
                return this._MajorInternal ().IntValue ();
            }
        }

        public int Minor {
            get {
                return this._MinorInternal ().IntValue ();
            }
        }
    }
}

