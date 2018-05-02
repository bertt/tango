using UnityEngine;
using WhiteCat;
using WhiteCat.FSM;

namespace WhiteCat.Example
{
	public class Gun : BaseStateMachine
	{
		public Transform muzzle;
		public float bulletSpeed = 100f;
		public int bulletGroupCount = 6;
		public float normalIntervalTime = 0.1f;
		public float groupIntervalTime = 0.3f;

		GameObjectPool bulletPool;


		void Start()
		{
			bulletPool = GetComponent<GameObjectPool>();

			var repeatingShoot = new SerializableState();
			float shootIntervalTime = 0f;
			int bulletCount = 0;

			repeatingShoot.onEnter += () =>
			{
				Shoot();
				bulletCount++;
				if (bulletCount == bulletGroupCount)
				{
					shootIntervalTime = groupIntervalTime;
					bulletCount = 0;
				}
				else
				{
					shootIntervalTime = normalIntervalTime;
				}
			};

			repeatingShoot.onUpdate += delta =>
			{
				if (currentStateTime > shootIntervalTime)
				{
					currentState = repeatingShoot;
				}
			};

			currentState = repeatingShoot;
		}


		public void Shoot()
		{
			var bullet = bulletPool.TakeOut().transform;
			bullet.position = muzzle.position;
			bullet.rotation = muzzle.rotation;
			bullet.GetComponent<Rigidbody>().velocity = muzzle.forward * bulletSpeed;
		}


		void FixedUpdate()
		{
			OnUpdate(Time.deltaTime);
		}
	}
}