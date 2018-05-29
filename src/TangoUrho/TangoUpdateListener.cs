using Android.Util;
using Com.Google.Atap.Tangoservice;

namespace App1
{
    public class TangoUpdateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {
        private readonly MainActivity _activity;
        private string Tag = "Ajax";


        public TangoUpdateListener(MainActivity activity)
        {
            _activity = activity;
        }

        public void OnFrameAvailable(int p0)
        {
            Log.Debug(Tag, "OnFrameAvailable");
        }

        public void OnPointCloudAvailable(TangoPointCloudData pointCloud)
        {
            Log.Debug(Tag, "OnPoinctCloudAvailable");

        }

        public void OnPoseAvailable(TangoPoseData p0)
        {
            Log.Debug(Tag, "OnPoseAvailable");
        }

        public void OnTangoEvent(TangoEvent p0)
        {
            Log.Debug(Tag, "OntangoEvent");
        }

        public void OnXyzIjAvailable(TangoXyzIjData xyzIj)
        {
            Log.Debug(Tag, "OnXyziEvent");
        }
    }
}