using UnityEngine;
using WhiteCat.FSM;
using UnityEngine.EventSystems;
using WhiteCat;
using System;

namespace WhiteCat.Example
{
	public class Window : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{

		[SerializeField]
		RectTransform _title;

		[SerializeField]
		RectTransform _content;

		[SerializeField]
		StackStateMachine _parent;


		CanvasGroup _canvasGroup;
		bool _isDragging;


		public void Close()
		{
			_parent.PopState();
		}


		void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}


		public void CreateChild()
		{
			var child = WindowFactory.Clone().transform as RectTransform;
			child.SetParent(_content, false);

			var self = GetComponent<StackStateMachine>();
			self.PushState(child.GetComponent<StackStateMachine>());
			child.GetComponent<Window>()._parent = self;
		}


		public void CreateBrother()
		{
			var brother = WindowFactory.Clone().transform as RectTransform;
			brother.SetParent(transform.parent, false);
			brother.offsetMin = (transform as RectTransform).offsetMin + new Vector2(80, -80f);
			brother.offsetMax = (transform as RectTransform).offsetMax + new Vector2(80, -80f);

			_parent.PushState(brother.GetComponent<StackStateMachine>());
			brother.GetComponent<Window>()._parent = _parent;
		}


		public void Destroy()
		{
			Destroy(gameObject);
		}


		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (_isDragging && _canvasGroup.interactable)
			{
				transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
			}
		}

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			_isDragging = RectTransformUtility.RectangleContainsScreenPoint(_title, eventData.position);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			_isDragging = false;
		}
	}
}