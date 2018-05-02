using UnityEngine;
using WhiteCat.FSM;

namespace WhiteCat.Example
{
	public class CustomStackSM : BaseStackStateMachine
	{
		public BaseStackState defaultState;
		public RectTransform focus;
		public float focusDamping = 1f;

		RectTransform focusTarget;


		void Start()
		{
			PushStateComponent(defaultState);
		}


		void Update()
		{
			OnUpdate(Time.unscaledDeltaTime);

			if (!focusTarget) focusTarget = rectTransform;
			float t = focusDamping * Time.deltaTime;

			focus.pivot = Vector2.Lerp(focus.pivot, focusTarget.pivot, t);
			focus.anchorMin = Vector2.Lerp(focus.anchorMin, focusTarget.anchorMin, t);
			focus.anchorMax = Vector2.Lerp(focus.anchorMax, focusTarget.anchorMax, t);
			focus.offsetMin = Vector2.Lerp(focus.offsetMin, focusTarget.offsetMin, t);
			focus.offsetMax = Vector2.Lerp(focus.offsetMax, focusTarget.offsetMax, t);
		}


		protected override void OnStateChanged(IStackState prevState, IStackState currentState)
		{
			focusTarget = (currentState as CustomStackState).rectTransform;
		}
	}
}