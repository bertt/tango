using UnityEngine;
using System.Collections;

namespace TangoWorkshop
{
    [AddComponentMenu("Tango Workshop/Shape Controller"), DisallowMultipleComponent]
    public class ShapeController : MonoBehaviour
    {

        private const float VELOCITY_THRESHOLD = 0.2f;

        new private Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (rigidbody.velocity.magnitude > VELOCITY_THRESHOLD) gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            else gameObject.layer = LayerMask.NameToLayer("Default");

            // basic clean up
            if (Vector3.Distance(transform.position, Camera.main.transform.position) > 20f) Destroy(gameObject);
        }
    }
}
