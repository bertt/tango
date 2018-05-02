using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using WhiteCatEditor;
#endif

namespace WhiteCat.Paths
{
	/// <summary>
	/// 旋转空间
	/// </summary>
	public enum RotationSpace
	{
		Path,
		World
	}


	/// <summary>
	/// 旋转朝向
	/// </summary>
	public enum RotationOrientation
	{
		Free,
		ForwardTangent,
		BackTangent,
	}


	/// <summary>
	/// RotationKeyframeList
	/// </summary>
	[AddComponentMenu("White Cat/Path/Keyframe List/Rotation Keyframe List")]
	public class RotationKeyframeList : Path.KeyframeList<Quaternion, Path.QuaternionKeyframe, Transform>
	{
		[SerializeField] RotationSpace _space = RotationSpace.Path;
		[SerializeField] RotationOrientation _orientation = RotationOrientation.ForwardTangent;


		public override string targetPropertyName
		{
			get { return "Rotation"; }
		}


		protected override Quaternion defaultKeyValue
		{
			get { return Quaternion.identity; }
		}


		/// <summary>
		/// 旋转空间
		/// </summary>
		public RotationSpace space
		{
			get { return _space; }
			set { _space = value; }
		}


		/// <summary>
		/// 旋转朝向
		/// </summary>
		public RotationOrientation orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}


		protected override Quaternion Lerp(Quaternion from, Quaternion to, float t)
		{
			return Quaternion.Slerp(from, to, t);
		}


		protected override void Apply(Transform target, Quaternion value, MoveAlongPath movingObject)
		{
			if (_space == RotationSpace.Path)
			{
				if (_orientation == RotationOrientation.ForwardTangent)
				{
					value = Quaternion.LookRotation(
						path.GetTangent(movingObject.location, Space.Self),
						value * Vector3.up);
				}
				else if (_orientation == RotationOrientation.BackTangent)
				{
					value = Quaternion.LookRotation(
						-path.GetTangent(movingObject.location, Space.Self),
						value * Vector3.up);
				}

				value = path.TransformRotation(value);
			}
			else
			{
				if (_orientation == RotationOrientation.ForwardTangent)
				{
					value = Quaternion.LookRotation(
						path.GetTangent(movingObject.location),
						value * Vector3.up);
				}
				else if (_orientation == RotationOrientation.BackTangent)
				{
					value = Quaternion.LookRotation(
						-path.GetTangent(movingObject.location),
						value * Vector3.up);
				}
			}

			target.rotation = value;
		}

#if UNITY_EDITOR

		protected override void DrawExtraInspector()
		{
			editor.DrawEnumPopupLayout(_space, value => space = (RotationSpace)value, "Space");
			editor.DrawEnumPopupLayout(_orientation, value => orientation = (RotationOrientation)value, "Orientation");

			if (_orientation != RotationOrientation.Free)
			{
				if (EditorKit.IndentedButton("Correct Orientations"))
				{
					Undo.RecordObject(this, editor.undoString);

					for (int i=0; i<count; i++)
					{
						var value = GetValue(i);

						if (_space == RotationSpace.Path)
						{
							if (_orientation == RotationOrientation.ForwardTangent)
							{
								value = Quaternion.LookRotation(
									path.GetTangent(path.GetLocationByLength(GetPosition(i)), Space.Self),
									value * Vector3.up);
							}
							else if (_orientation == RotationOrientation.BackTangent)
							{
								value = Quaternion.LookRotation(
									-path.GetTangent(path.GetLocationByLength(GetPosition(i)), Space.Self),
									value * Vector3.up);
							}
						}
						else
						{
							if (_orientation == RotationOrientation.ForwardTangent)
							{
								value = Quaternion.LookRotation(
									path.GetTangent(path.GetLocationByLength(GetPosition(i))),
									value * Vector3.up);
							}
							else if (_orientation == RotationOrientation.BackTangent)
							{
								value = Quaternion.LookRotation(
									-path.GetTangent(path.GetLocationByLength(GetPosition(i))),
									value * Vector3.up);
							}
						}

						SetValue(i, value);
					}
					
					EditorUtility.SetDirty(this);
				}
			}
		}


		protected override float keyValueWidth
		{
			get { return 177f; }
		}


		protected override Quaternion DrawKeyValue(Quaternion value, Rect rect)
		{
			EditorGUI.BeginChangeCheck();
			var angles = EditorGUI.Vector3Field(rect, GUIContent.none, value.eulerAngles);
			if (EditorGUI.EndChangeCheck())
			{
				value.eulerAngles = angles;
			}
			return value;
		}


		protected override void DrawKeyValueInScene(Vector3 position, Quaternion value, float handleSize)
		{
			if (_space == RotationSpace.Path) value = path.TransformRotation(value);
			EditorKit.HandlesDrawAALine(position, position + handleSize * (value * Vector3.up));
			Handles.ArrowCap(0, position, value, handleSize);
		}


		protected override Quaternion DrawKeyValueHandle(Vector3 position, Quaternion value, float handleSize)
		{
			if (_space == RotationSpace.Path)
			{
				return path.InverseTransformRotation(Handles.RotationHandle(path.TransformRotation(value), position));
			}
			else
			{
				return Handles.RotationHandle(value, position);
			}
		}

#endif // UNITY_EDITOR

	} // class RotationKeyframeList

} // namespace WhiteCat.Paths