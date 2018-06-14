using System;
using System.Collections.Generic;
using Com.Google.Atap.Tangoservice;
using Urho;

namespace App1
{
    public class FloorFinder
    {
        /// Used for floor finding, container for the number of points that fall into a y bucket within a sensitivity range.
        private Dictionary<float, int> m_numPointsAtY;
        private List<float> m_nonNoiseBuckets;
        private const float SENSITIVITY = 0.02f;
        private const int NOISE_THRESHOLD = 500;
        private const int RECOGNITION_THRESHOLD = 1000;
        public bool m_floorFound = false;
        private bool m_findFloorWithDepth = true;
        public float m_floorPlaneY = 0.0f;

        public FloorFinder()
        {
            m_numPointsAtY = new Dictionary<float, int>();
            m_nonNoiseBuckets = new List<float>();
        }

        public float FindFloor(TangoPointCloudData pointCloud, float cameraY)
        {
            var m_points = pointCloud.Points;
            m_numPointsAtY.Clear();
            m_nonNoiseBuckets.Clear();
            var m_pointsCount = pointCloud.NumPoints;

            // Count each depth point into a bucket based on its world position y value.
            for (int i = 0; i < m_pointsCount; i++)
            {
                var x = pointCloud.Points.Get(i * 4);
                var y = pointCloud.Points.Get(i * 4 + 1);
                var z = pointCloud.Points.Get(i * 4 + 2);
                var point = new Vector3(x, y, z);

                if (!point.Equals(Vector3.Zero))
                {
                    // Group similar points into buckets based on sensitivity. 
                    float roundedY = (float)Math.Round(point.Y / SENSITIVITY) * SENSITIVITY;
                    if (!m_numPointsAtY.ContainsKey(roundedY))
                    {
                        m_numPointsAtY.Add(roundedY, 0);
                    }

                    m_numPointsAtY[roundedY]++;

                    // Check if the y plane is a non-noise plane.
                    if (m_numPointsAtY[roundedY] > NOISE_THRESHOLD && !m_nonNoiseBuckets.Contains(roundedY))
                    {
                        m_nonNoiseBuckets.Add(roundedY);
                    }
                }
            }

            // Find a plane at the y value. The y value must be below the camera y position.
            m_nonNoiseBuckets.Sort();
            for (int i = 0; i < m_nonNoiseBuckets.Count; i++)
            {
                float yBucket = m_nonNoiseBuckets[i];
                int numPoints = m_numPointsAtY[yBucket];
                if (numPoints > RECOGNITION_THRESHOLD && yBucket < cameraY)
                {
                    // Reject the plane if it is not the lowest.
                    if (yBucket > m_nonNoiseBuckets[0])
                    {
                        return float.MinValue;
                    }

                    m_floorFound = true;
                    m_findFloorWithDepth = false;
                    m_floorPlaneY = yBucket;
                    m_numPointsAtY.Clear();
                    m_nonNoiseBuckets.Clear();
                }
            }
            return m_floorPlaneY;
        }
    }
}