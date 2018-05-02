using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// FieldOfViewKeyframeList
	/// </summary>
	[AddComponentMenu("White Cat/Path/Keyframe List/Field Of View Keyframe List")]
	public class FieldOfViewKeyframeList : FloatKeyframeList<Camera>
	{
		protected override float defaultKeyValue
		{
			get { return 60f; }
		}


		public override string targetPropertyName
		{
			get { return "Field of View"; }
		}


		protected override void Apply(Camera target, float value, MoveAlongPath movingObject)
		{
			target.fieldOfView = value;
		}

	} // class FieldOfViewKeyframeList

} // namespace WhiteCat.Paths