using UnityEngine;

namespace WhiteCat.Paths
{
	/// <summary>
	/// MoveSpeedKeyframeList
	/// </summary>
	[AddComponentMenu("White Cat/Path/Keyframe List/Move Speed Keyframe List")]
	public class MoveSpeedKeyframeList : FloatKeyframeList<MoveAlongPathWithSpeed>
	{
		public override string targetPropertyName
		{
			get { return "Move Speed"; }
		}


		protected override void Apply(MoveAlongPathWithSpeed target, float value, MoveAlongPath movingObject)
		{
			target.speed = value;
		}

	} // class MoveSpeedKeyframeList

} // namespace WhiteCat.Paths