using Android.App;
using Android.Util;
using Com.Google.Atap.Tangoservice;

namespace App1
{
    public class TangoUpdateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {
        private readonly Activity _activity;
        private string Tag = "Ajax";

        public TangoUpdateListener(Activity activity)
        {
            _activity = activity;
        }

        public void OnPoseAvailable(TangoPoseData p0)
        {
            // this is being called
            // Log.Debug(Tag, "OnPoseAvailable");
        }

        public void OnTangoEvent(TangoEvent p0)
        {
            // this is being called
            // Log.Debug(Tag, "OntangoEvent:" + p0.EventKey);
        }

        public void OnPointCloudAvailable(TangoPointCloudData pointCloud)
        {
            // this is being called when depth mode is enabled in the config
            var z = calculateAveragedDepth(pointCloud);
            Log.Debug(Tag, "Point Cloud Available! Points:" + pointCloud.NumPoints + ", z: " + z);
        }

        public void OnFrameAvailable(int p0)
        {
            // this is being called after adding call to tango.ConnectTextureId
            // Todo: handle the frame
            Log.Debug(Tag, "OnFrameAvailable");
        }

        public void OnXyzIjAvailable(TangoXyzIjData xyzIj)
        {
            // when is this called?
            Log.Debug(Tag, "OnXyziEvent");
        }

        private float calculateAveragedDepth(TangoPointCloudData pointCloud)
        {
            float totalZ = 0;
            float averageZ = 0;
            var numPoints = pointCloud.NumPoints;
            if (numPoints != 0)
            {
                int numFloats = 4 * numPoints;
                for (int i = 2; i < numFloats; i = i + 4)
                {
                    totalZ = totalZ + pointCloud.Points.Get(i);
                }
                averageZ = totalZ / numPoints;
            }
            return averageZ;
        }



    }
}