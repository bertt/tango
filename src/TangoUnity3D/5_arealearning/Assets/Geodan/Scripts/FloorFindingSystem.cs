using System.Collections;
using System.IO;
using Tango;
using UnityEngine;


public class FloorFindingSystem : MonoBehaviour 
{
    private TangoApplication _tangoApplication;
    private TangoPointCloud _pointCloud;
    private float _time = 0f;
    private bool _searchCompleted= false;
    public static bool floorFound = false;
    
    public void Start()
    {
        _pointCloud = FindObjectOfType<TangoPointCloud>();

        if (_pointCloud == null)
        {
            Debug.LogError("TangoPointCloud is NULL, required to find floor.");
            return;
        }

        _tangoApplication = FindObjectOfType<TangoApplication>();

        if (_tangoApplication == null)
        {
            Debug.LogError("TangoApplication is NULL, required to find floor.");
            return;
        }


        //initialize
        _pointCloud.FindFloor();
    }
    
    public void Update()
    {
        //update timer
        _time += Time.deltaTime;

        if (_time > RecordingSystem.FloorScanRate && _searchCompleted == true) //update and check for ongoing search
        {
            //reset timer
            _time = 0;

            //request new floor position from pointcloud
            _searchCompleted = false;
            floorFound = false;
            _tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.MAXIMUM);
            _pointCloud.FindFloor();
        }

        // If the point cloud has found the floor, adjust the position accordingly.
        if (_pointCloud.m_floorFound)
        {
            _searchCompleted = true;
            floorFound = true;
            if (transform.position.y != _pointCloud.m_floorPlaneY)
                transform.position = new Vector3(0.0f, _pointCloud.m_floorPlaneY, 0.0f);
        }
    }
}
