using System;
using UnityEngine;

namespace WhiteCat.Paths
{
	public partial class BezierPath
	{
		/// <summary>
		/// 贝塞尔路径节点
		/// </summary>
		[Serializable]
		class Node
		{
			[SerializeField] Vector3 _middleControlPoint = Vector3.zero;
			[SerializeField] Vector3 _forwardTangent = Vector3.forward;
			[SerializeField] Vector3 _backTangent = Vector3.back;
			[SerializeField] bool _broken = false;


			public Vector3 middleControlPoint
			{
				get { return _middleControlPoint; }
				set { _middleControlPoint = value; }
			}


			public Vector3 forwardTangent
			{
				get { return _forwardTangent; }
				set
				{
					_forwardTangent = value;
					if (!_broken)
					{
						float length = value.magnitude;
						if (length > Kit.OneMillionth)
						{
							_backTangent = -_backTangent.magnitude / length * value;
						}
					}
				}
			}


			public Vector3 backTangent
			{
				get { return _backTangent; }
				set
				{
					_backTangent = value;
					if (!_broken)
					{
						float length = value.magnitude;
						if (length > Kit.OneMillionth)
						{
							_forwardTangent = -_forwardTangent.magnitude / length * value;
						}
					}
				}
			}


			public Vector3 forwardControlPoint
			{
				get { return _middleControlPoint + _forwardTangent; }
				set { forwardTangent = value - _middleControlPoint; }
			}


			public Vector3 backControlPoint
			{
				get { return _middleControlPoint + _backTangent; }
				set { backTangent = value - _middleControlPoint; }
			}


			public bool broken
			{
				get { return _broken; }
				set
				{
					if (_broken != value)
					{
						_broken = value;
						if (!value)
						{
							float forwardLength = _forwardTangent.magnitude;
							float backLength = _backTangent.magnitude;
							if (forwardLength > Kit.OneMillionth && backLength > Kit.OneMillionth)
							{
								_forwardTangent = Vector3.Slerp(_forwardTangent,
									-forwardLength / backLength * _backTangent,
									backLength / (forwardLength + backLength));

								_backTangent = -backLength / forwardLength * _forwardTangent;
							}
						}
					}
				}
			}

		} // class Node

	} // class BezierPath

} // namespace WhiteCat.Paths