
using Android.Util;
using Com.Google.Atap.Tangoservice;

namespace App1
{
    public class TangoNavigateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {

        private readonly NavigateRouteActivity _activity;
        private string Tag = "Navigate Listener";

        public TangoNavigateListener(NavigateRouteActivity activity)
        {
            _activity = activity;
        }

        public void OnFrameAvailable(int p0)
        {
            Log.Debug(Tag, $"Navigate OnFrameAvaiable");
        }

        public void OnPointCloudAvailable(TangoPointCloudData p0)
        {
            Log.Debug(Tag, $"Navigate OnPointCloudAvaiable");
        }

        public void OnPoseAvailable(TangoPoseData p0)
        {
            Log.Debug(Tag, $"Navigate OnPoseAvailable");
        }

        public void OnTangoEvent(TangoEvent p0)
        {
            // Log.Debug(Tag, $"Navigate OntangoEvent");
        }

        public void OnXyzIjAvailable(TangoXyzIjData p0)
        {
            Log.Debug(Tag, $"Navigate OnXijziavailable");
        }
    }
}