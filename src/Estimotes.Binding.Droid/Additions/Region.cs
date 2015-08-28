using System;
using Android.Runtime;

namespace EstimoteSdk
{
    public partial class Region
    {
        static IntPtr id_getMajor;
        public virtual unsafe int Major {
            // Metadata.xml XPath method reference: path="/api/package[@name='com.estimote.sdk']/class[@name='Region']/method[@name='getMajor' and count(parameter)=0]"
            [Register ("getMajor", "()Ljava/lang/Integer;", "GetGetMajorHandler")]
            get {
                if (id_getMajor == IntPtr.Zero)
                    id_getMajor = JNIEnv.GetMethodID (class_ref, "getMajor", "()Ljava/lang/Integer;");
                try {

                    if (GetType () == ThresholdType)
                        return global::Java.Lang.Object.GetObject<global::Java.Lang.Integer> (JNIEnv.CallObjectMethod  (Handle, id_getMajor), JniHandleOwnership.TransferLocalRef).IntValue ();
                    else
                        return global::Java.Lang.Object.GetObject<global::Java.Lang.Integer> (JNIEnv.CallNonvirtualObjectMethod  (Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getMajor", "()Ljava/lang/Integer;")), JniHandleOwnership.TransferLocalRef).IntValue ();
                } finally {
                }
            }
        }


        static IntPtr id_getMinor;
        public virtual unsafe int Minor {
            // Metadata.xml XPath method reference: path="/api/package[@name='com.estimote.sdk']/class[@name='Region']/method[@name='getMinor' and count(parameter)=0]"
            [Register ("getMinor", "()Ljava/lang/Integer;", "GetGetMinorHandler")]
            get {
                if (id_getMinor == IntPtr.Zero)
                    id_getMinor = JNIEnv.GetMethodID (class_ref, "getMinor", "()Ljava/lang/Integer;");
                try {

                    if (GetType () == ThresholdType)
                        return global::Java.Lang.Object.GetObject<global::Java.Lang.Integer> (JNIEnv.CallObjectMethod  (Handle, id_getMinor), JniHandleOwnership.TransferLocalRef).IntValue ();
                    else
                        return global::Java.Lang.Object.GetObject<global::Java.Lang.Integer> (JNIEnv.CallNonvirtualObjectMethod  (Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "getMinor", "()Ljava/lang/Integer;")), JniHandleOwnership.TransferLocalRef).IntValue ();
                } finally {
                }
            }
        }
    }
}

