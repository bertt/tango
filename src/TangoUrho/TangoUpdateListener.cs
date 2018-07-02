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
            var baseFrameType = (TangoCoordinateFrameType)p0.BaseFrame;
            var targetFrameType = (TangoCoordinateFrameType)p0.TargetFrame;

            // 
            // Log.Debug("Tag", baseFrameType.ToString());
            // Log.Debug("Tag", targetFrameType.ToString());
            var translation = p0.GetTranslationAsFloats();
            var orientation = p0.GetRotationAsFloats();

            var position = new Vector3(translation[0], translation[1], translation[2]);
            var rotation = new Quaternion(orientation[0], orientation[1], orientation[2], orientation[3]);

            // Log.Debug(Tag, $"{position.X}, {position.Y},{position.Z}");
            // Log.Debug(Tag, $"{rotation.X}, {rotation.Y},{rotation.Z},{rotation.W}");
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

            // var f = new FloorFinder();
            // var floor = f.FindFloor(pointCloud,0);
            //Log.Debug(Tag, $"Floor: {floor}");
            Log.Debug(Tag, "Point Cloud Available! Points:" + pointCloud.NumPoints + ", z: " + z);

            _activity.PointCloudIsAvailable();

            // WritePointCloudData(pointCloud);

            // why do we need this?
            // _activity.UpdatePointCloud(pointCloud);

        }

        public void WritePointCloudData(TangoPointCloudData pointCloud)
        {
            var m_pointsCount = pointCloud.NumPoints;
            Log.Debug("Test", $"{m_pointsCount}");

            // Count each depth point into a bucket based on its world position y value.
            for (int i = 0; i < m_pointsCount; i++)
            {
                var x = pointCloud.Points.Get(i * 4);
                var y = pointCloud.Points.Get(i * 4 + 1);
                var z = pointCloud.Points.Get(i * 4 + 2);
                var c = pointCloud.Points.Get(i * 4 + 3);

                var point = new Vector3(x, y, z);

                Log.Debug("bertho", $"{x},{y},{z},{c}");
            }
            var e = false;
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