using UnityEngine;
using UnityEngine.UI;

public class RecordingSystem : MonoBehaviour {

    private Camera _camera;
    private GameObject _floor;
    private RouteLineRenderer _linerenderer;
    private Vector3 previousMarker = new Vector3(-9999, -9999, -9999);

    public bool isRecording = false;
    public static float MarkerDistance = 3;
    public GameObject Marker;
    public GameObject MarkerFolder;
    private int markerCounter = 0;
    public GameObject Obstacle;
    bool autoPlaceMarker = false;

    public static float FloorScanRate = 1;

    // Use this for initialization
    void Start() {
        _camera = Camera.main;
        _floor = GameObject.Find("Tango Floor");
    }

    public void SetMarkerDistance(float value)
    {
        MarkerDistance = value;
    }

    public void SetFloorScanRate(float value)
    {
        FloorScanRate = value;
    }

    public void SetAutoPlaceMarker(bool value)
    {
        autoPlaceMarker = value;
    }

    public void StartRecording() {
        
        //reset all markers
        GameManager.Markers.Clear();
        previousMarker = new Vector3(-9999, -9999, -9999);

        //clear all markers in markerfolder, maybe combine with markers.clear for efficiency as finding is expensive
        GameObject[] markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (var m in markers)
        {
            DestroyImmediate(m);
        }

        //clear all obstacles in markerfolder
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var o in obstacles)
        {
            DestroyImmediate(o);
        }

        //start!      
        isRecording = true;
        GameManager.Modus = GameManager.Modi.isRecording;
    }

    public void StopRecording() {
        GameManager.Modus = GameManager.Modi.isNotRecording;
        isRecording = false;
        //enable navigation if markercount >1
        //var UI = GameObject.FindObjectOfType<UIManager>();

        //if (GameManager.Markers.Count >= 1)
        //    UI.Navigate.GetComponent<Button>().interactable = true;
        //else
        //    UI.Navigate.GetComponent<Button>().interactable = false;
    }

    public bool ToggleOnOff() {
        isRecording = !isRecording;
        
        if (isRecording)
            StartRecording();   
        else
            StopRecording();

        return isRecording;  
    }

    public void AddMarker()
    {
        //set markerposition
        previousMarker = _camera.transform.position;

        CreateMarker("");
    }

    public void AddObstacle()
    {
        //set markerposition
        previousMarker = _camera.transform.position;

        CreateMarker("hier is een lift");
        //CreateObstacle();
    }

    // Update is called once per frame
    void Update ()
    {
        if (isRecording && autoPlaceMarker)
        {
            //calculate distance
            float dist = Vector3.Distance(previousMarker ,_camera.transform.position);

            //if distance larger than threshold, place marker
            if (dist > MarkerDistance && FloorFindingSystem.floorFound)
            {
                //set markerposition
                previousMarker = _camera.transform.position;

                //create marker
                CreateMarker("");
            }
        }
    }

    public void CreateMarker(string msg)
    {
        var marker = GameObject.Instantiate(Marker);
        marker.transform.parent = MarkerFolder.transform;
        marker.transform.position = new Vector3(_camera.transform.position.x, _floor.transform.position.y, _camera.transform.position.z);
        marker.transform.rotation = Quaternion.LookRotation(marker.transform.forward);

        if (msg != "")
            marker.name = msg;
        else
        {
            //marker.name = String.Format("Marker: {0}", (GameManager.Markers.Count - 1));
            marker.name = "Marker";
        }

        //add marker to collection
        GameManager.Markers.Add(marker);
        
        //change navarrow of previous marker to this marker, skip first marker
        if (GameManager.Markers.Count > 1)
        {
            var prevMarker = GameManager.Markers[GameManager.Markers.Count - 2];

            prevMarker.transform.rotation = Quaternion.LookRotation(marker.transform.position - prevMarker.transform.position);
        }
        //create screenshot for markersnapshot with camera position
        //marker.GetComponent<SimulationSnapshot>().Snapshot = ;
        //marker.GetComponent<SimulationSnapshot>().CameraTransform = gameObject.transform;
    }

    private void CreateObstacle()
    {
        var obstacle = GameObject.Instantiate(Obstacle);
        obstacle.transform.parent = MarkerFolder.transform;
        obstacle.transform.position = new Vector3(_camera.transform.position.x, _floor.transform.position.y, _camera.transform.position.z);
        obstacle.transform.rotation = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
    }
}