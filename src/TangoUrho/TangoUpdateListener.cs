using Android.App;
using Android.Util;
using Com.Google.Atap.Tangoservice;
using Urho;

namespace App1
{
    public class TangoUpdateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {
        private readonly CreateRouteActivity _activity;
        private string Tag = "Ajax";

        public TangoUpdateListener(CreateRouteActivity activity)
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

            _activity.PointCloudIsAvailable();

            WritePointCloudData(pointCloud);

            // why do we need this?
            // _activity.UpdatePointCloud(pointCloud);

        }

        public void WritePointCloudData(TangoPointCloudData pointCloud)
        {
            var m_pointsCount = pointCloud.NumPoints;

            // Count each depth point into a bucket based on its world position y value.
            for (int i = 0; i < m_pointsCount; i++)
            {
                var x = pointCloud.Points.Get(i * 4);
                var y = pointCloud.Points.Get(i * 4 + 1);
                var z = pointCloud.Points.Get(i * 4 + 2);
                var point = new Vector3(x, y, z);

                Log.Debug("Test", $"{x},{y},{z}");
            }
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