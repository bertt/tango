using UnityEngine;
using System.Collections;
namespace WhiteCat.Example
{
	public class WindowFactory : MonoBehaviour
	{

		[SerializeField]
		GameObject _prefab;


		static WindowFactory _instance;


		void Awake()
		{
			_instance = this;
		}


		public static GameObject Clone()
		{
			return Instantiate(_instance._prefab);
		}
	}
}