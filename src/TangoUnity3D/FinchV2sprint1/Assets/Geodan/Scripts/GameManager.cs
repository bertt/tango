using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public enum Modi {
        idle,
        isRecording,
        isNotRecording,
        isNavigating,
        isNotNavigating
    }

    public static List<GameObject> Markers = new List<GameObject>();

    public static Modi Modus = Modi.idle;


    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            // This is a fix for a lifecycle issue where calling
            // Application.Quit() here, and restarting the application
            // immediately results in a deadlocked app.
            AndroidHelper.AndroidQuit();
        }
    }

    /// <summary>
    /// Application onPause / onResume callback.
    /// </summary>
    /// <param name="pauseStatus"><c>true</c> if the application about to pause, otherwise <c>false</c>.</param>
    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // When application is backgrounded, we reload the level because the Tango Service is disconected. All
            // learned area and placed marker should be discarded as they are not saved.
#pragma warning disable 618
            Application.LoadLevel(Application.loadedLevel);
#pragma warning restore 618
        }
    }

}

