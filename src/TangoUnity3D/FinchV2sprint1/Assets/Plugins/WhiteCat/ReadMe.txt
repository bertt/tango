White Cat's Toolbox
Asset Store: https://www.assetstore.unity3d.com/#!/content/32356?aid=1101lGxh
Documentation: https://github.com/silentlamb1991/WhiteCatToolbox/wiki
Support E-mail: whitecat1991@outlook.com




---------- Version 3.6 ----------

Detail:
	Now state machine supports sub-state machine.
	Add dynamic GI updating option for material properties animations.
	Remove TypedUnionValue, rename UnionValue to Union8.
	Improve editor scripts of Tween.
	Improve ScriptableAssetSingletonWithEditor.
	Add namespace for example scripts.
	Fix HDR color picker not working in Unity 5.3.




---------- Version 3.5 ----------

Cautions:
	This version is NOT backwards compatible.

Detail:
	Add DataFile (old IOKit removed).
	Add TweenRigidbodyPosition and TweenRigidbodyRotation.
	Add HDR support for TweenColor (Requires Unity 5.3 or higher).
	Fix "_EmissionColor" not show in TweenColor popup list.
	Change base class of State and StateMachine.
	Add MoveAlongPathPhysics component (with example).
	Add new Subpath example.




---------- Version 3.4 ----------

Cautions:
	This version is NOT backwards compatible.

Detail:
	Add modularized namespace for scripts.
	Change MaterialType.Shared to Specified, add new RendererShared type.
	Add Gradient for TweenColor.
	Use LinkedList improve GameObjectPool.
	Add Scenes script (Assets -> Create -> White Cat -> Scenes Script).
	Add UpdateMode for Tweener.
	Fix Quaternion lerp-method from Lerp to Slerp in RotationKeyframeList.
	Fix default value of rotation is invalid in RotationKeyframeList.
	Fix Path gizmos error.
	Fix CreateAsset error.
	Improve FSM.




---------- Version 3.3 ----------

Detail:
	Fix bug of multi-inspector drawing.
	Modify file extensions of assets which created by QuickCreateAsset.




---------- Version 3.2 ----------

Detail:
	Add CardinalPath component (and example).
	Add new example about scripting of Path.
	Fix bug of GetSetAttribute of example script.
	Fix bug of SetCardinalCurve of CubicSpline.




---------- Version 3.1 ----------

Detail:
	Add GameObjectPool component (and example).
	Add MoveSpeedKeyframeList component.
	Add global Tween animations (ambient, fog, etc).
	Add TweenCameraFieldOfView component.
	Add TweenCameraBackgroundColor component.
	Add TweenLightShadowStrength component (update example).
	Add TweenSpriteRendererColor component (update example).
	Draw an arrow at end of path, for distinguishing direction.




---------- Version 3.0 ----------

Cautions:
	Required version of Unity upgraded to 5.2.0.
	This version is NOT backwards compatible.

Detail:
	Totally refactor.
	Add State Machine and Stack State Machine.
	Redesign Path and Tween.