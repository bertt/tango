using UnityEngine;

public class NavigatorSystem : MonoBehaviour
{

    private Camera _camera;
    public static GameObject TargetMarker;
    public static int curMarkerId = 0;

    public static bool useTactileFeedback = true;
    public static bool useObstacleDetection = true;
    public static bool useVisualFeedback = true;
    public static bool useAcousticFeedback = true;
    public static bool useSpeechSynthesis = false;
    public static bool isFinished = true;
    public static bool isNavigating = false;
    public static float Dot = 0;
    private float speechTimer = 1;
    public static float TactileSensitivity = 0.05f;
    GameObject c;
    void Start()
    {
        _camera = Camera.main;

        c = new GameObject("test");     
    }

    public void SetTactileSensitivity(float value)
    {
        TactileSensitivity = value;
    }

    public void ToggleTactileFeedback()
    {
        useTactileFeedback = !useTactileFeedback;
    }

    public void ToggleSpeechSynthesis()
    {
        useSpeechSynthesis = !useSpeechSynthesis;
    }

    public void ToggleObstacleDetection()
    {
        useObstacleDetection = !useObstacleDetection;
    }

    public void ToggleAcousticFeedback()
    {
        useAcousticFeedback = !useAcousticFeedback;

        var listener = GameObject.FindObjectOfType<AudioListener>();
        listener.enabled = useAcousticFeedback;
    }

    public void ToggleVisualFeedback()
    {
        useVisualFeedback = !useVisualFeedback;

        foreach (var m in GameManager.Markers)
        {
            var renderers = m.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = useVisualFeedback;
            }
        }

        var route = gameObject.GetComponent<RecordingSystem>().MarkerFolder.GetComponent<RouteLineRenderer>();
        route.enabled = useVisualFeedback;
    }

    public bool ToggleOnOff()
    {
        isNavigating = !isNavigating;

        if (isNavigating)
            StartNavigation();
        else
            StopNavigation();

        return isNavigating;
    }

    public void StopNavigation()
    {
        GameManager.Modus = GameManager.Modi.isNotNavigating;
        isNavigating = false;

        //show all markers in route
        for (int i = 0; i < GameManager.Markers.Count; i++)
        {
            GameManager.Markers[i].GetComponent<AcousticMarker>().Stop();
            GameManager.Markers[i].SetActive(true);
        }
    }

    public void StartNavigation()
    {
        GameManager.Modus = GameManager.Modi.isNavigating;
        isFinished = false;
        isNavigating = true;

        //disable all markers and show first
        for (int i = 0; i < GameManager.Markers.Count; i++)
        {
            GameManager.Markers[i].GetComponent<AcousticMarker>().Stop();
            GameManager.Markers[i].SetActive(false);
        }

        if (GameManager.Markers.Count > 0)
        {

            TargetMarker = GameManager.Markers[0];
            curMarkerId = TargetMarker.GetComponent<AcousticMarker>()._id;
            GameManager.Markers[0].GetComponent<AcousticMarker>().PlayBeacon();
            GameManager.Markers[0].SetActive(true);
        }
    }

    void Update()
    {
        if (isNavigating && TargetMarker)
        {
            //calculate normalized 2D projected vectors
            //first created projected points

            //get current rotation and reset after projection
          
            c.transform.position = _camera.transform.position;
            c.transform.rotation = Quaternion.Euler(new Vector3(0, _camera.transform.rotation.eulerAngles.y, _camera.transform.rotation.eulerAngles.z));

            Vector3 v1a = new Vector3(c.transform.position.x, 0, c.transform.position.z);
            Vector3 v1b = new Vector3(c.transform.forward.x, 0, c.transform.forward.z);

            Vector3 v2b = new Vector3(TargetMarker.transform.position.x, 0, TargetMarker.transform.position.z);

            //then create vectors
            Vector3 v2 = v2b - v1a;

            //draw vectors using line
            Debug.DrawLine(v1a, v1a + v1b.normalized, Color.red, 0.1f);
            Debug.DrawLine(v1a, v1a + v2.normalized, Color.green, 0.1f);

            //calculate dot product in 2D, discard Z
            Dot = Vector3.Dot(v1b, v2.normalized);

            
            if (useTactileFeedback)
            {
                if (!isFinished)
                {
                    //if dot product larger then threshold, vibrate
                    if (Dot > (1 - TactileSensitivity))
                        Handheld.Vibrate();
                }
            }

            //do obstacle checks here
            if (useObstacleDetection)
            {
                //var fwd = _camera.transform.TransformDirection(Vector3.forward);
                //if (Physics.Raycast(transform.position, fwd, 2))
                //{
                //    Handheld.Vibrate();
                //}
            }

            if (useSpeechSynthesis)
            {
                //calculate distance

                if (!isFinished)
                {
                    speechTimer -= Time.deltaTime;

                    if (speechTimer <0) //only calculate every second
                    {
                        speechTimer = 3; //reset timer

                        if (curMarkerId+1 < GameManager.Markers.Count)
                        {
                            var marker = GameManager.Markers[curMarkerId+1];
                            Vector3 camPos = new Vector3(_camera.transform.position.x, 0, _camera.transform.position.z);
                            var dist = Vector3.Distance(new Vector3(GameManager.Markers[curMarkerId].transform.position.x, 0, GameManager.Markers[curMarkerId].transform.position.z), camPos);

                            var angle = Vector3.Angle(_camera.transform.forward, marker.transform.position - _camera.transform.position);
                            var cross = Vector3.Cross(_camera.transform.forward, marker.transform.position - _camera.transform.position);
                            if (cross.y < 0)
                                angle = -angle;

                            //Vector3 v1a = new Vector3(_camera.transform.position.x, 0, _camera.transform.position.z);
                            //Vector3 v1b = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z);

                            //Vector3 v2b = new Vector3(TargetMarker.transform.position.x, 0, TargetMarker.transform.position.z);

                            ////then create vectors
                            //Vector3 v2 = v2b - v1;

                            //Debug.DrawLine(v1a, v1a + v1b.normalized, Color.blue, 0.1f);
                            //Debug.DrawLine(v1a, v1a + v2.normalized, Color.yellow, 0.1f);

                            //float angle = Vector3.Dot(v1b, v2.normalized);

                            Debug.Log("curId: " + curMarkerId + " targetmarker: " + TargetMarker.GetComponent<AcousticMarker>()._id + " angle: " + angle + "dist: " + dist);

                            if (dist <= 3)
                            {
                                string dir = "";

                                if (angle < -25)
                                    dir = "iets linksaf";
                                if (angle < -40)
                                    dir = "flink linksaf";
                                if (angle < -80)
                                    dir = "scherp linksaf";
                                if (angle > 25)
                                    dir = "iets rechtsaf";
                                if (angle > 40)
                                    dir = "flink rechtssaf";
                                if (angle > 80)
                                    dir = "scherp rechtssaf";
                                if ((angle >= (-15)) && (angle <= (15)))
                                    dir = "rechtdoor";

                                if (dir != "" && GameManager.Markers[curMarkerId + 1].name == "Marker")
                                {
                                    var msg = "zometeen" + dir;
                                    SpeechSynthesisManager.Speak(msg);
                                    Debug.Log(msg);
                                }                                
                            }
                        }
                        else
                        {
                            var marker = TargetMarker;
                            Vector3 camPos = new Vector3(_camera.transform.position.x, 0, _camera.transform.position.z);
                            var dist = Vector3.Distance(marker.transform.position, camPos);

                            var angle = Vector3.Angle(_camera.transform.forward, marker.transform.position - _camera.transform.position);
                            var cross = Vector3.Cross(_camera.transform.forward, marker.transform.position - _camera.transform.position);
                            if (cross.y < 0)
                                angle = -angle;

                            bool msgFound = false;
                            string msg="";

                            if (dist < 5)
                            {
                                msg = "vijf";
                                msgFound = true;
                             
                            }

                            if (dist < 4)
                            {
                                msg = "vier";
                                msgFound = true;
                                
                            }

                            if (dist < 3)
                            {
                                msg = "drie";
                                msgFound = true;
                               
                            }

                            if (dist < 2)
                            {
                                msg = "twee";
                                msgFound = true;                                
                            }

                            if (dist < 1)
                            {
                                msg = "een";
                                msgFound = true;
                            }
                            
                            if (msgFound)
                            {
                                SpeechSynthesisManager.Speak(msg);
                              
                                //Debug.Log(msg);
                            }
                        }
                    }
                }                
            }
        }
    }
}