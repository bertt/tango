using UnityEngine;
using WhiteCat.Paths;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WhiteCat.Tween
{
	/// <summary>
	/// 路径位置插值动画
	/// </summary>
	[AddComponentMenu("White Cat/Tween/Path/Tween Position Along Path")]
	[RequireComponent(typeof(MoveAlongPath))]
	public class TweenPositionAlongPath : TweenFloat
	{
		MoveAlongPath _moveAlongPath;
		public MoveAlongPath moveAlongPath
		{
			get
			{
				if (!_moveAlongPath)
				{
					_moveAlongPath = GetComponent<MoveAlongPath>();
				}
				return _moveAlongPath;
			}
		}


		public override float from
		{
			get { return _from; }
			set
			{
				if (moveAlongPath.path && !moveAlongPath.path.circular)
				{
					_from = Mathf.Clamp(value, 0f, moveAlongPath.path.length);
				}
				else _from = value;
			}
		}


		public override float to
		{
			get { return _to; }
			set
			{
				if (moveAlongPath.path && !moveAlongPath.path.circular)
				{
					_to = Mathf.Clamp(value, 0f, moveAlongPath.path.length);
				}
				else _to = value;
			}
		}


		public override float current
		{
			get
			{
				return moveAlongPath.distance;
            }
			set
			{
				moveAlongPath.distance = value;
            }
		}

#if UNITY_EDITOR

		const float _discSize = 0.2f;
		const float _arrowSize = 0.7f;
		static Color _beginColor = new Color(1, 0.5f, 0);
		static Color _endColor = new Color(0, 0.75f, 1);

		Location _fromLocation = new Location(-1, 0f);
		Location _toLocation = new Location(-1, 0f);


		protected override void Editor_OnSceneGUI()
		{
			var path = moveAlongPath.path;
            if (path)
			{
				_fromLocation = path.GetLocationByLength(_from, _fromLocation.index);
				var position = path.GetPoint(_fromLocation);
				var direction = path.GetTangent(_fromLocation);
				float handleSize = HandleUtility.GetHandleSize(position);

				Handles.color = _beginColor;
				Handles.DrawWireDisc(position, direction, handleSize * _discSize);
				float arrowLength = handleSize * _arrowSize;
				Handles.ArrowCap(0, position, Quaternion.LookRotation(_from > _to ? -direction : direction), arrowLength);

				_toLocation = path.GetLocationByLength(_to, _toLocation.index);
				position = path.GetPoint(_toLocation);
				direction = path.GetTangent(_toLocation);
				handleSize = HandleUtility.GetHandleSize(position);

				Handles.color = _endColor;
				Handles.DrawWireDisc(position, direction, handleSize * _discSize);
				arrowLength = handleSize * _arrowSize;
				if (_from > _to) direction  = - direction;
                Handles.ArrowCap(0, position - direction * arrowLength, Quaternion.LookRotation(direction), arrowLength);
			}
		}


		protected override void DrawExtraFields()
		{
			if (moveAlongPath.path && !moveAlongPath.path.circular)
			{
				DrawClampedFromToValues(0f, moveAlongPath.path.length);
			}
			else
			{
				base.DrawExtraFields();
			}
		}

#endif // UNITY_EDITOR

	} // class TweenPositionAlongPath

} // namespace WhiteCat.Tween