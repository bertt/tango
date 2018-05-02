using UnityEngine;

public class TangoFollower : MonoBehaviour {

    public GameObject target;

	// Update is called once per frame
	void Update () {
        gameObject.transform.position = target.transform.position;
	}
}
