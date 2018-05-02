using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat
{
	/// <summary>
	/// 游戏对象池
	/// </summary>
	[AddComponentMenu("White Cat/Common/Game Object Pool")]
	public class GameObjectPool : ScriptableComponentWithEditor
	{
		[SerializeField] [Editable(true, false)]
		GameObject _originalObject;

		[SerializeField] [Editable(true, false)]
		int _startQuantity = 0;

		[SerializeField]
		float _autoRecycleCountdown = 10f;

		LinkedList<RecyclableObject> _storedObjects;
		LinkedList<RecyclableObject> _activeObjects;


		// 可回收对象组件
		class RecyclableObject : ScriptableComponent
		{
			// 所属对象池
			[HideInInspector]
			public GameObjectPool pool;

			// 链表节点
			[System.NonSerialized]
			public LinkedListNode<RecyclableObject> node;

			// 物体开始激活的时间, -1 表示未激活
			[System.NonSerialized]
			public float activeTime;


			public void Recycle()
			{
				pool.Awake();

				if (node == null)
				{
					node = new LinkedListNode<RecyclableObject>(this);
				}
				else
				{
					node.List.Remove(node);
				}

				pool._storedObjects.AddFirst(node);
				activeTime = -1f;

				gameObject.SetActive(false);
				transform.SetParent(pool.transform, false);
			}
		}


		/// <summary>
		/// 是否自动回收
		/// </summary>
		public bool autoRecycle
		{
			get { return enabled; }
			set { enabled = value; }
		}


		/// <summary>
		/// 自动回收倒计时. 仅当 autoRecycle 为 true 时才会自动回收
		/// </summary>
		public float autoRecycleCountdown
		{
			get { return _autoRecycleCountdown; }
			set { _autoRecycleCountdown = value; }
		}


		/// <summary>
		/// 添加对象
		/// </summary>
		/// <param name="number"> 添加的对象数量 </param>
		public void AddObjects(int number = 1)
		{
			Awake();

			while (number > 0)
			{
				GameObject obj = Instantiate(_originalObject);
				obj.transform.SetParent(transform, false);
				obj.SetActive(false);

				var recyclable = obj.AddComponent<RecyclableObject>();
				recyclable.pool = this;
				recyclable.node = new LinkedListNode<RecyclableObject>(recyclable);
				recyclable.activeTime = -1f;

				_storedObjects.AddFirst(recyclable.node);

				number--;
			}
		}


		/// <summary>
		/// 取出一个游戏对象
		/// </summary>
		/// <returns> 取出的对象 </returns>
		public GameObject TakeOut()
		{
			Awake();

			RecyclableObject recyclable = null;

			while (_storedObjects.Count > 0)
			{
				recyclable = _storedObjects.First.Value;
				_storedObjects.RemoveFirst();
				if (recyclable) break;
			}

			if (!recyclable)
			{
				AddObjects();
				recyclable = _storedObjects.First.Value;
				_storedObjects.RemoveFirst();
			}

			_activeObjects.AddLast(recyclable.node);

			recyclable.transform.SetParent(null, false);
			recyclable.gameObject.SetActive(true);
			recyclable.activeTime = Time.time;

			return recyclable.gameObject;
		}


		/// <summary>
		/// 回收游戏对象
		/// </summary>
		/// <param name="target"> 被回收的对象 </param>
		public static void Recycle(GameObject target)
		{
			target.GetComponent<RecyclableObject>().Recycle();
		}


		/// <summary>
		/// 回收所有取出的对象
		/// </summary>
		public void RecycleAll()
		{
			Awake();
			while (_activeObjects.Count > 0)
			{
				var recyclable = _activeObjects.First.Value;

				if (recyclable) recyclable.Recycle();
				else _activeObjects.RemoveFirst();
			}
		}


		// 初始化
		void Awake()
		{
			if (_storedObjects == null)
			{
				_storedObjects = new LinkedList<RecyclableObject>();
				_activeObjects = new LinkedList<RecyclableObject>();
				AddObjects(_startQuantity);
			}
		}


		// 自动回收对象
		void Update()
		{
			while (_activeObjects.Count > 0)
			{
				var recyclable = _activeObjects.First.Value;

				if (!recyclable)
				{
					_activeObjects.RemoveFirst();
				}
				else if (recyclable.activeTime + _autoRecycleCountdown > Time.time)
				{
					return;
				}
				else
				{
					recyclable.Recycle();
				}
			}
		}


#if UNITY_EDITOR

		SerializedProperty _originalObjectProperty;
		SerializedProperty _startQuantityProperty;
		SerializedProperty _autoRecycleCountdownProperty;


		protected override void Editor_OnEnable()
		{
			_originalObjectProperty = editor.serializedObject.FindProperty("_originalObject");
			_startQuantityProperty = editor.serializedObject.FindProperty("_startQuantity");
			_autoRecycleCountdownProperty = editor.serializedObject.FindProperty("_autoRecycleCountdown");
		}


		protected override void Editor_OnDisable()
		{
			_originalObjectProperty = null;
			_startQuantityProperty = null;
			_autoRecycleCountdownProperty = null;
		}


		protected override void Editor_OnInspectorGUI()
		{
			editor.serializedObject.Update();

			EditorGUILayout.PropertyField(_originalObjectProperty);
			EditorGUILayout.PropertyField(_startQuantityProperty);

			editor.DrawToggleLayout(enabled, value => enabled = value, "Auto-Recycle");
			if (enabled)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_autoRecycleCountdownProperty, EditorKit.GlobalContent("Countdown"));
				EditorGUI.indentLevel--;
			}

			editor.serializedObject.ApplyModifiedProperties();
		}

#endif

	} // class GameObjectPool

} // namespace WhiteCat