using UnityEngine;
using WhiteCat;

namespace WhiteCat.Example
{
	public class UseAttributes : MonoBehaviour
	{
		[Max(0f)]
		public float smallerThan0;

		[Min(0f)]
		public float biggerThan0;

		[Clamp(-1f, 1f)]
		public float clampedValue;

		[Editable(false, true)]
		public float nonEditable = 1f;

		[Label("Custom Name")]
		public float originalName = 1f;

		[Layer]
		public int layer;

		[EulerAngles]
		public Quaternion eulerAngles = Quaternion.identity;

		[Direction]
		public Vector3 direction = Vector3.forward;

		[SerializeField]
		[GetSet("getSetValue")]
		float m_getSetValue;


		public float getSetValue
		{
			get { return m_getSetValue; }
			set { m_getSetValue = Mathf.Round(value); }
		}
	}
}