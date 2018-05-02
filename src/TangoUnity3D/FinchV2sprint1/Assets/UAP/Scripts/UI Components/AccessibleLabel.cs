using UnityEngine;
using System.Collections;

using UnityEngine.UI;

[AddComponentMenu("Accessibility/UI/Accessible Label")]
public class AccessibleLabel : UAP_BaseElement
{
	//////////////////////////////////////////////////////////////////////////

	AccessibleLabel()
	{
		m_Type = AccessibleUIGroupRoot.EUIElement.ELabel;
	}

	//////////////////////////////////////////////////////////////////////////

	public override bool IsElementActive()
	{
		// Return whether this button is usable, and visible
		if (!base.IsElementActive())
			return false;

		Text label = GetLabel();
		if (label != null)
		{
			if (!label.gameObject.activeInHierarchy || label.enabled == false)
				return false;
			else
				return true;
		}

#if ACCESS_NGUI
		UILabel nGUILabel = GetNGUILabel();
		if (nGUILabel != null)
		{
			if (!nGUILabel.gameObject.activeInHierarchy || nGUILabel.enabled == false)
				return false;
			else
				return true;
		}
#endif

		return true;
	}

	//////////////////////////////////////////////////////////////////////////

	public override bool AutoFillTextLabel()
	{
		// The label's default is to the label, so, nothing to do here
		// But override it anyway, so that it doesn't get filled with
		// the GameObject's name.

		return true;
	}

	//////////////////////////////////////////////////////////////////////////

	public override string GetText()
	{
		if (!m_TryToReadLabel)
		{
#if ACCESS_NGUI
			if (IsNameLocalizationKey())
				return Localization.Get(m_Text);
#endif
			return m_Text;
		}

		Text label = GetLabel();
		if (label != null)
			return CombinePrefix(label.text);

#if ACCESS_NGUI
		UILabel nGUILabel = GetNGUILabel();
		if (nGUILabel != null)
			return CombinePrefix(nGUILabel.text);

		//Debug.Log("Getting item text " + m_Text);

		if (IsNameLocalizationKey())
			return Localization.Get(m_Text);
#endif

		return m_Text;
	}

	//////////////////////////////////////////////////////////////////////////

	private Text GetLabel()
	{
		Text label = null;
		if (m_ReferenceElement != null)
			label = m_ReferenceElement.GetComponent<Text>();
		if (m_NameLabel != null)
			label = m_NameLabel.GetComponent<Text>();
		if (label == null)
			label = GetComponent<Text>();

		return label;
	}

	//////////////////////////////////////////////////////////////////////////

#if ACCESS_NGUI
	private UILabel GetNGUILabel()
	{
		UILabel label = null;
		if (m_ReferenceElement != null)
			label = m_ReferenceElement.GetComponent<UILabel>();
		if (m_NameLabel != null)
			label = m_NameLabel.GetComponent<UILabel>();
		if (label == null)
			label = GetComponent<UILabel>();

		return label;
	}
#endif


	//////////////////////////////////////////////////////////////////////////

	protected override void AutoInitialize()
	{
		if (m_TryToReadLabel)
		{
			Text label = GetLabel();
			if (label != null)
				m_NameLabel = label.gameObject;

#if ACCESS_NGUI
			UILabel nGUILabel = GetNGUILabel();
			if (nGUILabel != null)
				m_NameLabel = nGUILabel.gameObject;
#endif
		}
		else
		{
			m_NameLabel = null;
		}
	}

	//////////////////////////////////////////////////////////////////////////
}
