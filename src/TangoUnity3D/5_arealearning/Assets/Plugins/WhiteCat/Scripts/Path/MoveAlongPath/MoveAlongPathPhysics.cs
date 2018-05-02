using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Paths
{
	/// <summary>
	/// 在路径上通过物理的方式移动
	/// </summary>
	[AddComponentMenu("White Cat/Path/Move Along Path/Move Along Path Physics")]
	public class MoveAlongPathPhysics : MoveAlongPath
	{
		/// <summary>
		/// 速度
		/// </summary>
		public float speed = 0f;


		[Min(0)] [SerializeField]
		float _maxSpeed = 100f;


		[Min(0)] [SerializeField]
		float _drag = 0f;


		[Min(0)] [SerializeField]
		float _frictionCoefficient = 0.1f;


		bool _needInit = true;
		Vector3 _lastDirection;


		/// <summary>
		/// 力
		/// </summary>
		public Vector3 force;


		/// <summary>
		/// 最大速度
		/// </summary>
		public float maxSpeed
		{
			get { return _maxSpeed; }
			set { _maxSpeed = Mathf.Max(value, 0f); }
		}


		/// <summary>
		/// 阻力
		/// </summary>
		public float drag
		{
			get { return _drag; }
			set { _drag = Mathf.Max(value, 0f); }
		}


		/// <summary>
		/// 摩擦系数
		/// </summary>
		public float frictionCoefficient
		{
			get { return _frictionCoefficient; }
			set { _frictionCoefficient = Mathf.Max(value, 0f); }
		}


		void OnDisable()
		{
			_needInit = true;
			speed = 0f;
		}


		void FixedUpdate()
		{
			if (path)
			{
				// 初始化
				if (_needInit)
				{
					_needInit = false;
					Sync();
					_lastDirection = path.GetTangent(location);
				}

				// 方向
				Vector3 direction = path.GetTangent(location);

				// 分解速度
				float speed1 = Vector3.Dot(_lastDirection * speed, direction);
				float speed2 = Mathf.Sqrt(Mathf.Max(speed * speed - speed1 * speed1, 0f));
				_lastDirection = direction;

				// 去掉因撞击失去的速度
				if (speed1 < 0) speed = Mathf.Min(speed1 + _frictionCoefficient * speed2, 0f);
				else speed = Mathf.Max(speed1 - _frictionCoefficient * speed2, 0f);

				// 推力
				float thrust = Vector3.Dot(force, direction);

				// 压力
				float pressure = force.sqrMagnitude - thrust * thrust;
				pressure = pressure > 0 ? Mathf.Sqrt(pressure) : 0f;

				// 阻力合力
				float finalDrag = pressure * _frictionCoefficient + _drag;

				// 应用推力
				speed += thrust * Time.fixedDeltaTime;

				// 应用阻力
				if (speed < 0) speed = Mathf.Min(speed + finalDrag * Time.fixedDeltaTime, 0f);
				else speed = Mathf.Max(speed - finalDrag * Time.fixedDeltaTime, 0f);

				// 应用速度
				speed = Mathf.Clamp(speed, -_maxSpeed, _maxSpeed);
				distance += speed * Time.fixedDeltaTime;
			}
			else
			{
				_needInit = true;
			}
		}


#if UNITY_EDITOR

		SerializedProperty _speedProperty;
		SerializedProperty _maxSpeedProperty;
		SerializedProperty _dragProperty;
		SerializedProperty _frictionCoefficientProperty;
		SerializedProperty _forceProperty;


		protected override void Editor_OnEnable()
		{
			_speedProperty = editor.serializedObject.FindProperty("speed");
			_maxSpeedProperty = editor.serializedObject.FindProperty("_maxSpeed");
			_dragProperty = editor.serializedObject.FindProperty("_drag");
			_frictionCoefficientProperty = editor.serializedObject.FindProperty("_frictionCoefficient");
			_forceProperty = editor.serializedObject.FindProperty("force");
		}


		protected override void Editor_OnDisable()
		{
			_speedProperty = null;
			_maxSpeedProperty = null;
			_dragProperty = null;
			_frictionCoefficientProperty = null;
			_forceProperty = null;
		}


		protected override void Editor_OnExtraInspectorGUI()
		{
			editor.serializedObject.Update();
			EditorGUILayout.PropertyField(_speedProperty);
			EditorGUILayout.PropertyField(_maxSpeedProperty);
			EditorGUILayout.PropertyField(_dragProperty);
			EditorGUILayout.PropertyField(_frictionCoefficientProperty);
			EditorGUILayout.PropertyField(_forceProperty);
			editor.serializedObject.ApplyModifiedProperties();
		}

#endif

	} // class MoveAlongPathPhysics

} // namespace WhiteCat.Paths