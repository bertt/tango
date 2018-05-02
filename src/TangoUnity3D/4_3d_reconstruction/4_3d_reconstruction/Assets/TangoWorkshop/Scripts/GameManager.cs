using UnityEngine;
using Tango;

namespace TangoWorkshop
{
    [AddComponentMenu("Tango Workshop/Game Manager"), DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {

        // this is used twice: once in LateUpdate() to offset the marker from the reconstruction surface, and again in OnGUI to remove that offset when placing shapes
        private const float POS_OFFSET = 0.025f;

        [Tooltip("The prefab used to mark the location where shapes will be created.")]
        public GameObject markerPrefab;
        [Tooltip("Drag & drop shape prefabs here to use them in the game. If you make your own prefabs, make sure they have a Mesh Filter, Mesh Renderer, Collider, Rigid Body and Shape Controller attached.")]
        public GameObject[] shapePrefabs;

        private GameObject marker;

        void Start()
        {
            // make an instance of the marker prefab
            marker = Instantiate(markerPrefab);
        }

        void LateUpdate()
        {
            // place the marker by casting a ray from the center of the screen
            RaycastHit hit;
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenCenter), out hit))
            {
                // make the marker active if the raycast was successful
                marker.SetActive(true);
                // set the position (with slight offset) and rotation of the marker
                marker.transform.position = hit.point + hit.normal * POS_OFFSET;
                marker.transform.LookAt(hit.point + hit.normal);
            }
            else
            {
                // make the marker inactive if the raycast wasn't successful
                marker.SetActive(false);
            }
        }

        void OnGUI()
        {
            // set some initial variables
            GUI.color = Color.white;
            float height = Screen.height - 128f;

            // create buttons for each prefab shape
            foreach (GameObject shapePrefab in shapePrefabs)
            {

                // create the "throw" button and the code for when it's pressed
                if (GUI.Button(new Rect(32f, height, 256f, 96f), "<size=30>Throw a:\n" + shapePrefab.name + "</size>"))
                {
                    // create the new shape at the position with a default rotation (Quaternion.identity)
                    GameObject newShape = Instantiate(shapePrefab, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
                    // set the new shape's velocity
                    newShape.GetComponent<Rigidbody>().velocity = newShape.transform.forward * 3f;
                }

                // only create the "place" button if we have a place to put it (i.e. the marker object is active)
                if (marker.activeSelf)
                {
                    // create the "place" button and the code for when it's pressed
                    if (GUI.Button(new Rect(320f, height, 256f, 96f), "<size=30>Place a:\n" + shapePrefab.name + "</size>"))
                    {
                        // this position logic assumes that the new shape has a height of 1 (meter)
                        // marker.transform.forward is used as opposed to marker.transform.up because the
                        // marker is a quad primitive and Unity's quad primitives have to be rotated to lay flat
                        Vector3 position = marker.transform.position + marker.transform.forward * ((shapePrefab.transform.localScale.y / 2f) - POS_OFFSET);

                        // create the new shape at the position with a default rotation (Quaternion.identity)
                        Instantiate(shapePrefab, position, Quaternion.identity);
                    }
                }

                // move position up for the next row of buttons
                height -= 128f;
            }
        }
    }
}

