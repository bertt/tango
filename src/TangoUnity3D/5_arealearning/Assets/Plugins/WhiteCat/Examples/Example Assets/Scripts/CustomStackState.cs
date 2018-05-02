using UnityEngine;
using UnityEngine.UI;
using WhiteCat.FSM;
using WhiteCat;

namespace WhiteCat.Example
{
	[DisallowMultipleComponent]
	public class CustomStackState : BaseStackState
	{
		public CanvasGroup canvasGroup;
		public float interactableAlpha = 1f;
		public float nonInteractableAlpha = 0.4f;

		public Image background;
		public Color interactableColor;
		public Color nonInteractableColor;

		public GameObject highlight;


		public override void OnPush()
		{
			gameObject.SetActive(true);
			if (highlight) highlight.SetActive(true);
		}


		public override void OnPop()
		{
			gameObject.SetActive(false);
			if (highlight) highlight.SetActive(false);
		}


		public override void OnEnter()
		{
			canvasGroup.interactable = true;
			canvasGroup.alpha = interactableAlpha;
			background.color = interactableColor;
		}


		public override void OnExit()
		{
			canvasGroup.interactable = false;
			canvasGroup.alpha = nonInteractableAlpha;
			background.color = nonInteractableColor;
		}


		public override void OnUpdate(float deltaTime)
		{
		}
	}
}