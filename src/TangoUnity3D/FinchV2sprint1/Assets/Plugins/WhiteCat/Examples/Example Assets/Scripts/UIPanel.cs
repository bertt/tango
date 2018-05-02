using UnityEngine;
using UnityEngine.UI;

namespace WhiteCat.Example
{
	public class UIPanel : MonoBehaviour
	{
		public Color enabledColor;
		public Color disabledColor;


		public void SetColor(bool enabled)
		{
			GetComponent<Image>().color = enabled ? enabledColor : disabledColor;
		}
	}
}