using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

/// <summary>This is the base for all accessibility UI components. Use this to set values directly.</summary>
[AddComponentMenu("Accessibility/Core/UAP Base Element")]
public class UAP_BaseElement : MonoBehaviour
{
	public bool m_ForceStartHere = false;
	public int m_ManualPositionOrder = -1;
	public GameObject m_ManualPositionParent = null;
	public bool m_UseTargetForOutline = false;

	// TODO: These should be readonly - they are only for debugging purposes
	public int m_PositionOrder = 0;
	public int m_SecondaryOrder = 0;
	public Vector2 m_Pos = new Vector2(0, 0);

	[Header("Element Name")]
	public AudioClip m_TextAsAudio = null;

	public string m_Prefix = "";

	/// <summary>
	/// This variable contains the text that will be read aloud if this UI element receives focus.
	/// This variable will only be used if the plugin is not reading text directly from a text label.
	/// </summary>
	public string m_Text = "";

	/// <summary>
	/// Label that contains the name or description of the edit box.
	/// Use this to directly read localized content.<br><br>
	/// </summary>
	public GameObject m_NameLabel = null;

	/// <summary>
	/// Uses the text inside m_NameLabel and requests translation.
	/// </summary>
	public bool m_IsLocalizationKey = false;

	//! Only works if there is a text label component either on this GameObject or
	//! set as a reference.
	public bool m_TryToReadLabel = true;

	//! Only set this if the accessibility component doesn't sit on the same GameObject
	public GameObject m_ReferenceElement = null;
	//! In very specific circumstances it can make sense to deliberately prevent using VoiceOver from speaking this element.
	//! VoiceOver lowers the rest of the volume of the application, which is sometimes unneccessary.
	public bool m_AllowVoiceOver = true;
	//! In very specific circumstances you might want to suppress the reading of the type
	public bool m_ReadType = true;

	[HideInInspector]
	public bool m_WasJustAdded = true;

	[HideInInspector]
	public AccessibleUIGroupRoot.EUIElement m_Type = AccessibleUIGroupRoot.EUIElement.EUndefined;

	// My owner
	AccessibleUIGroupRoot AUIContainer = null;

	public bool m_CustomHint = false;
	public AudioClip m_HintAsAudio = null;
	public string m_Hint = "";
	public bool m_HintIsLocalizationKey = false;

	[HideInInspector]
	public bool m_IsInsideScrollView = false;

	//private bool m_ContainerRefreshNeeded = false;
	private bool m_HasStarted = false;

	[HideInInspector]
	public bool m_IsInitialized = false;

	//////////////////////////////////////////////////////////////////////////

	void Reset()
	{
		AutoFillTextLabel();
		m_IsInitialized = false;
		Initialize();
	}

	//////////////////////////////////////////////////////////////////////////

	public void Initialize()
	{
		if (m_IsInitialized)
			return;

		AutoInitialize();

		if (!m_IsLocalizationKey)
			m_Text = GetText();
		m_TryToReadLabel = (m_NameLabel != null);

		m_IsInitialized = true;
	}

	/// <summary>
	/// Do not call this function directly, use Initialize() instead.
	/// </summary>
	protected virtual void AutoInitialize()
	{
		if (m_IsInitialized)
			return;
	}

	//////////////////////////////////////////////////////////////////////////

	void OnEnable()
	{
		if (!m_HasStarted)
			return;

		RegisterWithContainer();
		// Tell container to earmark a refresh next frame
		//		m_ContainerRefreshNeeded = true;
		CancelInvoke("RefreshContainerNextFrame");
		Invoke("RefreshContainerNextFrame", 0.5f);
		//		RefreshContainerNextFrame();
	}

	//////////////////////////////////////////////////////////////////////////

	void Start()
	{
		m_HasStarted = true;
		RegisterWithContainer();
		// Tell container to earmark a refresh next frame
		//		m_ContainerRefreshNeeded = true;
		CancelInvoke("RefreshContainerNextFrame");
		Invoke("RefreshContainerNextFrame", 0.5f);
		//		RefreshContainerNextFrame();
	}

	//////////////////////////////////////////////////////////////////////////
	
	private void GetContainer()
	{
		Transform t = transform;
		while (t != null && AUIContainer == null)
		{
			AUIContainer = t.gameObject.GetComponent<AccessibleUIGroupRoot>();
			t = t.parent;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void RegisterWithContainer()
	{
		GetContainer();

		if (AUIContainer == null)
		{
			Debug.LogError("[Accessibility] Could not find an Accessibility UI Container in any parent object of " + gameObject.name + "! This UI element will be unaccessible.");
			return;
		}

		AUIContainer.CheckForRegister(this);

		UAP_SelectionGroup[] groups = GetComponentsInParent<UAP_SelectionGroup>();
		foreach (UAP_SelectionGroup group in groups)
			group.AddElement(this);
	}

	//////////////////////////////////////////////////////////////////////////

	void RefreshContainerNextFrame()
	{
		GetContainer();

		if (AUIContainer == null)
		{
			Debug.LogError("[Accessibility] Could not find an Accessibility UI Container in any parent object of " + gameObject.name + "! This UI element will be unaccessible.");
			return;
		}

		AUIContainer.RefreshNextUpdate();
	}

	//////////////////////////////////////////////////////////////////////////

/*
	void Update()
	{
		if (m_ContainerRefreshNeeded)
		{
			m_ContainerRefreshNeeded = false;
			// Tell container to earmark a refresh next frame
			RefreshContainerNextFrame();
		}
	}
*/

	//////////////////////////////////////////////////////////////////////////

	public virtual bool AutoFillTextLabel()
	{
		bool found = false;

		// Unity UI
		if (m_NameLabel != null)
		{
			Text label = m_NameLabel.GetComponent<Text>();
			if (label != null)
			{
				m_Text = label.text;
				found = true;
			}

#if ACCESS_NGUI
			// NGUI
			UILabel nGUILabel = m_NameLabel.GetComponent<UILabel>();
			if (nGUILabel != null)
			{
				m_Text = nGUILabel.text;
				found = true;
			}
#endif
		}

		if (!found)
			m_Text = gameObject.name;

		return found;
	}

	//////////////////////////////////////////////////////////////////////////

	void OnDestroy()
	{
	//	Debug.Log("Destroying Element" + gameObject.name);
		if (AUIContainer != null)
		{
			//Debug.Log("Unregstering " + gameObject.name);
			AUIContainer.UnRegister(this);
		}

		UAP_SelectionGroup[] groups = GetComponentsInParent<UAP_SelectionGroup>();
		foreach (UAP_SelectionGroup group in groups)
			group.RemoveElement(this);

	}

	//////////////////////////////////////////////////////////////////////////

	public virtual bool IsInteractable()
	{
		return false;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual void Interact()
	{
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual void InteractEnd()
	{
	}

	//////////////////////////////////////////////////////////////////////////

	//! End the interaction but - if it makes sense - restore previous value
	public virtual void InteractAbort()
	{
		// This is the default behaviour, for safety reasons.
		// Override this function if you need to restore a previous value
		InteractEnd();
	}

	//////////////////////////////////////////////////////////////////////////

	protected string CombinePrefix(string text)
	{
		if (m_Prefix.Length == 0)
			return text;

		if (m_Prefix.IndexOf("{0}") != -1)
			return m_Prefix.Replace("{0}", text);

		return m_Prefix + " " + text;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual string GetText()
	{
		if (m_TryToReadLabel)
			AutoFillTextLabel();

#if ACCESS_NGUI
		if (IsNameLocalizationKey())
			return Localization.Get(m_Text);
#endif

		// Only use prefix if there is a text label associated
		if (m_TryToReadLabel)
			return CombinePrefix(m_Text);
		else
			return m_Text;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual string GetCurrentValueAsText()
	{
		return "";
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual AudioClip GetCurrentValueAsAudio()
	{
		return null;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual bool IsElementActive()
	{
		if (!enabled)
			return false;

		if (!gameObject.activeInHierarchy)
			return false;

		return true;
	}

	//////////////////////////////////////////////////////////////////////////

	public void SelectItem(bool forceRepeatItem = false)
	{
		if (!IsElementActive())
		{
			Debug.LogWarning("[Accessibility] Trying to select element '" + GetText() + "' (" + gameObject.name + ") but the element is not active/interactable/visible.");
			return;
		}

		if (!m_HasStarted)
		{
			// TODO: delay the rest of this function until Start()
			//return;
		}

		SelectItem_Internal(forceRepeatItem);
		return;
	}

	//////////////////////////////////////////////////////////////////////////

	private void SelectItem_Internal(bool forceRepeatItem)
	{
		if (AUIContainer == null)
		{
			// TODO: If this doesn't work, delay the rest of this function until Start()
			RegisterWithContainer();
			if (AUIContainer == null)
			{
				Debug.LogWarning("[Accessibility] SelectItem: " + gameObject.name + " is not placed within an Accessibility UI container. Can't be selected. Aborting.");
				return;
			}
		}

		// Notify the parent container, so that it can notify the manager
		AUIContainer.SelectItem(this, forceRepeatItem);
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual bool Increment()
	{
		return false;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual bool Decrement()
	{
		return false;
	}

	//////////////////////////////////////////////////////////////////////////

	public virtual void HoverHighlight(bool enable)
	{
		// TODO: Send Hover notification to that UI element (uGUI and NGUI)
	}

	//////////////////////////////////////////////////////////////////////////

	public GameObject GetTargetGameObject()
	{
		if (m_ReferenceElement != null)
			return m_ReferenceElement.gameObject;

		return gameObject;
	}

	//////////////////////////////////////////////////////////////////////////

	public bool IsNameLocalizationKey()
	{
		return (m_IsLocalizationKey);
	}

	//////////////////////////////////////////////////////////////////////////

	public string GetCustomHint()
	{
		if (m_HintIsLocalizationKey)
			return UAP_AccessibilityManager.Localize(m_Hint);
		return m_Hint;
	}
}
