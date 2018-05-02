//#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
//#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

/// <summary>This is the main object to handle all the accessibility in an app. Please use the premade Accessibility Manager prefab.</summary>
[AddComponentMenu("Accessibility/Core/UAP Manager")]
public class UAP_AccessibilityManager : MonoBehaviour
{
	//////////////////////////////////////////////////////////////////////////
	// Version Info
	// 
	public static string PluginVersion = "1.0.3";
	public static float PluginVersionAsFloat = 1.03f;

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The state for accessibility on the very first launch of your app.
	/// Leave this at false unless this is an app purely for the sight impaired.
	/// </summary>
	public bool m_DefaultState = false;

	/// <value>Automatically turn on accessibility if active NVDA/VoiceOver/TalkBack is detected.</value>
	public bool m_AutoTurnOnIfScreenReaderDetected = true;
	public bool m_RecheckAutoEnablingOnResume = false;
	/// <value>Turn accessibility mode on or off, depending on what it was when the app was last closed.</value>
	public bool m_SaveEnabledState = true;

	/// <value>If true (Default), the manager will block direct touch to the screen.
	/// 	The plugin listens for UI control gestures, such as left and right swipe.
	/// 	Buttons will not be selectable with single tap anymore.
	/// 	Set this to false if you don't want this part active.
	/// 	</value>
	public bool m_HandleUI = true;
	private bool m_BlockInput = false;

	/// <value>If true, will recognize magic gestures.
	/// for functions like Back, Exit and Pause, and call the appropriate callbacks.
	/// This can be active even if the UI handling is disabled.</value>
	public bool m_HandleMagicGestures = true;

	/// <value>Reads out the UI element that is under the user's finger.
	/// If m_HandleUI is false, this setting will be ignored.
	/// As this is a standard accessibility feature on smart phones,
	/// do not turn this off unless you have a good reason.</value>
	public bool m_ExploreByTouch = true;
	//	public float m_ExploreByTouchDelay = 0.4f;

	/// <summary>
	/// Reads out disabled but visible interactive UI elements.
	/// This means buttons, toggles and sliders will be read out, 
	/// even when they are disabled (but they must be visible).
	/// The plugin will read out "Disabled" after the name.
	/// Default is true, as this emulates the behavior from iOS VoiceOver.
	/// </summary>
	public bool m_ReadDisabledInteractables = true;

	/// <summary>
	/// When reaching the last element on the screen, the focus will jump back around to the first element if this is set to true.
	/// By default, this is false, which triggers a sound effect when the border of the screen is reached.
	/// </summary>
	public bool m_CyclicMenus = false;

	/// <value>How many seconds after selecting a UI element will the usage hint be triggered.</value>
	float m_HintDelay = 0.6f;
	float m_DisabledDelay = 0.2f;
	float m_ValueDelay = 0.25f;
	float m_TypeDelay = 0.4f;

	[Header("Debug/Testing")]
	public bool m_WindowsUseMouseSwipes = false;
	public bool m_WindowsUseExploreByTouch = false;

	private Canvas m_CanvasRoot = null;

	/// <summary>
	/// Quick way to enable/disable the plugin during development in Editor only.
	/// This variable will be ignored in any standalone builds.
	/// </summary>
	public bool m_EditorOverride = false;
	public bool m_EditorEnabledState = false;


	[Header("Windows")]
	public bool m_WindowsTTS = true;
	public int m_WindowsTTSVolume = 100;
	//public int m_WindowsTTSRate = 3;
	public bool m_WindowsUseKeys = true;
	private KeyCode m_NextElementKey = KeyCode.DownArrow;
	private KeyCode m_PreviousElementKey = KeyCode.UpArrow;
	private KeyCode m_NextContainerKey = KeyCode.RightArrow;
	private KeyCode m_PreviousContainerKey = KeyCode.LeftArrow;
	private bool m_UseTabAndShiftTabForContainerJumping = true;
	private KeyCode m_InteractKey = KeyCode.Return;
	private KeyCode m_SliderIncrementKey = KeyCode.UpArrow;
	private KeyCode m_SliderDecrementKey = KeyCode.DownArrow;
	private KeyCode m_DropDownPreviousKey = KeyCode.UpArrow;
	private KeyCode m_DropDownNextKey = KeyCode.DownArrow;
	private KeyCode m_AbortKey = KeyCode.Escape;
	private KeyCode m_PauseKey = KeyCode.Escape;
	private KeyCode m_DownKey = KeyCode.DownArrow;
	private KeyCode m_UpKey = KeyCode.UpArrow;
	private KeyCode m_RightKey = KeyCode.RightArrow;
	private KeyCode m_LeftKey = KeyCode.LeftArrow;

	[Header("Android")]
	public bool m_AndroidTTS = true;
	public bool m_AndroidUseUpAndDownForElements = false;

	[Header("iOS")]
	public bool m_iOSTTS = true;
#if UNITY_IOS
	private iOSGestures m_iOSGestures = null;
#endif

	[Header("Mac OS")]
	public bool m_MacOSTTS = true;

	//////////////////////////////////////////////////////////////////////////

	/// <value>SFX when the UI element is changed</value>
	public AudioClip m_UINavigationClick = null;
	/// <value>SFX when a button is pressed</value>
	public AudioClip m_UIInteract = null;
	/// <value>SFX when an element receives exclusive focus, such as a slider</value>
	public AudioClip m_UIFocusEnter = null;
	/// <value>SFX when an element loses exclusive focus, such as a slider</value>
	public AudioClip m_UIFocusLeave = null;
	/// <value>SFX when navigation reaches the end of the screen</value>
	public AudioClip m_UIBoundsReached = null;
	/// <value>SFX when popup opens</value>
	public AudioClip m_UIPopUpOpen = null;
	/// <value>SFX when popup closes</value>
	public AudioClip m_UIPopUpClose = null;

	[Header("Types")] // Not currently used, and made private to hide
	private AudioClip m_DisabledAsAudio = null;
	private AudioClip m_ButtonAsAudio = null;
	private AudioClip m_ToggleAsAudio = null;
	private AudioClip m_SliderAsAudio = null;
	private AudioClip m_TextEditAsAudio = null;
	private AudioClip m_DropDownAsAudio = null;

	[Header("Special Phrases")]
	private string m_Phrase_EnablingAccessibility = "Enabling Accessibility";
	private string m_Phrase_DisabledAccessibility = "Disabled Accessibility. Tap three times with three fingers to turn back on.";
	private string m_Phrase_DropdownItemIndex = "Item XX of YY";
	private string m_Phrase_DropdownItemSelected = "TT selected";

	[Header("Default Hints for Mobile devices")]
	private string m_Mobile_General = "Swipe left and right to navigate. Swipe up and down to quickly jump between groups of elements. Swipe up with two fingers to read the entire screen from the top. Double tap to activate an item.";
	private string m_Mobile_HintButton = "Double tap to select.";
	private string m_Mobile_HintToggle = "Double tap to change.";
	private string m_Mobile_HintTextEdit = "Double tap to edit.";
	private string m_Mobile_HintDropdown = "Double tap to change.";
	private string m_Mobile_HintSlider = "Double tap to activate, then swipe up and down to change the value.";

	[Header("Default Hints for Windows")]
	private string m_Win_General = "Use the up and down arrow keys to navigate. Use Tab and Shift-Tab to quickly jump between groups of elements. Press return to activate an item.";
	private string m_Win_HintButton = "Press return to select.";
	private string m_Win_HintToggle = "Press return to change.";
	private string m_Win_HintTextEdit = "Press return to edit.";
	private string m_Win_HintDropdown = "Press return to change.";
	private string m_Win_HintSlider = "Press return to activate, then use up and down arrows to change the value.";

	[Header("Other Resources")]
	public AudioSource m_AudioPlayer = null;
	public AudioSource m_SFXPlayer = null;
	public RectTransform m_Frame = null;
	public GameObject m_FrameTemplate = null;
	public GameObject m_TouchBlocker = null;
	public Text m_DebugOutputLabel = null;

	//////////////////////////////////////////////////////////////////////////

	public bool m_AllowVoiceOverGlobal = true;

#if UNITY_IOS && !UNITY_EDITOR
	private float m_ReCheckVoiceOverTimer = 1.0f;
#endif

	//////////////////////////////////////////////////////////////////////////

	// NGUI support
#if ACCESS_NGUI
	private GameObject m_NGUITouchBlocker = null;
	private UIWidget m_NGUIItemFrame = null;
	private UIRoot m_UIRoot = null;
	List<UICenterOnChild> m_AddedScrollCenterComponents = new List<UICenterOnChild>();
#endif

	//////////////////////////////////////////////////////////////////////////

	private UAP_AudioQueue m_AudioQueue = null;

	//////////////////////////////////////////////////////////////////////////

	static private UAP_AccessibilityManager instance = null;
	static bool isDestroyed = false;
	static bool m_IsInitialized = false;
	static bool m_IsEnabled = false;
	static bool m_Paused = false;

	List<AccessibleUIGroupRoot> m_ActiveContainers = new List<AccessibleUIGroupRoot>();
	List<List<AccessibleUIGroupRoot>> m_SuspendedContainers = new List<List<AccessibleUIGroupRoot>>();
	AccessibleUIGroupRoot.Accessible_UIElement m_CurrentItem = null;
	AccessibleUIGroupRoot.Accessible_UIElement m_PreviousItem = null;

	static List<AccessibleUIGroupRoot> m_ContainersToActivate = new List<AccessibleUIGroupRoot>();

	int m_ActiveContainerIndex = -1;
	//bool m_TTSRatesSet = false;
	bool m_ReadItemNextUpdate = false;
	bool m_CurrentElementHasSoleFocus = false;


	// Callbacks
	//////////////////////////////////////////////////////////////////////////
	public delegate void OnPauseToggleCallbackFunc();
	private OnPauseToggleCallbackFunc m_OnPauseToggleCallbacks = null;
	public delegate void OnTapEvent();
	private OnTapEvent m_OnTwoFingerSingleTapCallbacks = null;
	private OnTapEvent m_OnThreeFingerSingleTapCallbacks = null;
	private OnTapEvent m_OnThreeFingerDoubleTapCallbacks = null;
	public delegate void OnSwipeEvent();
	private OnTapEvent m_OnTwoFingerSwipeUpHandler = null;
	private OnTapEvent m_OnTwoFingerSwipeDownHandler = null;
	public delegate void OnAccessibilityModeChanged(bool enabled);
	private OnAccessibilityModeChanged m_OnAccessibilityModeChanged = null;


	// Swipe Detection
	//////////////////////////////////////////////////////////////////////////
	public enum ESDirection
	{
		EUp = 0,
		EDown,
		ELeft,
		ERight,
	};

	bool m_SwipeActive = false;
	int m_SwipeTouchCount = 0;
	bool m_SwipeWaitForLift = false;
	Vector2 m_SwipeStartPos = new Vector2();
	Vector2 m_SwipeCurrPos = new Vector2();
	float m_SwipeDeltaTime = 0.0f;

	// Double Tap Detection
	//////////////////////////////////////////////////////////////////////////
	float m_DoubleTap_LastTapTime = -1.0f;
	/// <value>Maximum delay for a second tap to occur to still trigger a double tap.
	/// This value also influences the delay in which Explore By Touch is triggered,
	/// so do not raise this value arbitrarily.</value>
	public float m_DoubleTapTime = 0.2f; // in seconds
	bool m_DoubleTapFoundThisFrame = false;

	// Magic Tap (Two Finger Double Tap) Detection
	//////////////////////////////////////////////////////////////////////////
	float m_MagicTap_LastTapTime = -1.0f;
	int m_MagicTap_TouchCountHelper = 0;
	bool m_WaitingForMagicTap = false;

	// Triple Tap (Three Fingers) Detection
	//////////////////////////////////////////////////////////////////////////
	int m_TripleTap_Count = 0;
	float m_TripleTap_LastTapTime = -1.0f;
	int m_TripleTap_TouchCountHelper = 0;
	bool m_WaitingForThreeFingerTap = false;

	// Explore By Touch
	//////////////////////////////////////////////////////////////////////////
	float m_TouchWaitTimeout = -1.0f;
	bool m_ExploreByTouch_IsActive = false;
	float m_ExploreByTouch_WaitTimer = -1.0f;
	Vector3 m_ExploreByTouch_StartPosition = new Vector3(0, 0, 0);
	float m_ExploreByTouch_SingleTapWaitTimer = -1.0f;
	Vector3 m_ExploreByTouch_SingleTapStartPosition = new Vector3(0, 0, 0);

	// Read from Top/from Current
	//////////////////////////////////////////////////////////////////////////
	private bool m_ContinuousReading = false;
	private bool m_ContinuousReading_WaitInputClear = false;

	//////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////


	UAP_AccessibilityManager()
	{
		//#if !UNITY_EDITOR_WIN
		//if (instance == null || !m_IsInitialized)
		//{
		//#endif
		//  instance = this;
		//}
		m_ActiveContainers.Clear();
	}

	//////////////////////////////////////////////////////////////////////////

	void Awake()
	{
		// Only one accessibility manager can exist
		if (instance != this && instance != null)
		{
			Debug.Log("[Accessibility] Found another UAP Accessibility Manager in the scene. Destroying this one.");
			DestroyImmediate(gameObject);
			return;
		}

		instance = this;
	}

	//////////////////////////////////////////////////////////////////////////

	void Start()
	{
		Initialize();
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	//////////////////////////////////////////////////////////////////////////

	void OnDestroy()
	{
		if (instance == this)
			isDestroyed = true;
	}

	//////////////////////////////////////////////////////////////////////////

	static void Initialize()
	{
		if (m_IsInitialized)
			return;
		m_IsInitialized = true;

		if (instance == null)
		{
			GameObject newInstance = Instantiate(Resources.Load("Accessibility Manager")) as GameObject;
			instance = newInstance.GetComponent<UAP_AccessibilityManager>();
		}

		if (instance.m_CanvasRoot == null)
			instance.m_CanvasRoot = instance.transform.GetChild(0).GetComponent<Canvas>();

		if (instance.m_AudioPlayer == null)
		{
			instance.m_AudioPlayer = instance.m_CanvasRoot.gameObject.GetComponent<AudioSource>();
			if (instance.m_AudioPlayer == null || instance.m_AudioPlayer == instance.m_SFXPlayer)
			{
				instance.m_AudioPlayer = instance.m_CanvasRoot.gameObject.AddComponent<AudioSource>();
			}
		}
		if (instance.m_SFXPlayer == null)
			instance.m_SFXPlayer = instance.m_CanvasRoot.gameObject.AddComponent<AudioSource>();

		if (instance.m_AudioQueue == null)
		{
			GameObject newAudioQueue = new GameObject("Audio Queue");
			instance.m_AudioQueue = newAudioQueue.AddComponent<UAP_AudioQueue>();
			instance.m_AudioQueue.Initialize();
			newAudioQueue.transform.SetParent(instance.transform, false);
		}

		InitNGUI();

		HideElementFrame();

		// Decide whether to automatically turn on
		m_IsEnabled = ShouldAutoEnable();
		instance.EnableTouchBlocker(m_IsEnabled);
		SavePluginEnabledState();

		if (m_IsEnabled)
		{
#if UNITY_IOS
			if (instance.m_iOSGestures == null)
			{
				GameObject newGestureDetector = new GameObject("UAP_IOSGestures");
				instance.m_iOSGestures = newGestureDetector.AddComponent<iOSGestures>();
				instance.m_iOSGestures.SetUAP(instance);
				newGestureDetector.transform.SetParent(instance.transform, false);
			}
			instance.m_iOSGestures.StartRecognition();
#endif
		}

		//Debug.Log("[Accessibility] Initializing UAP Accessibility Manager");
	}

	//////////////////////////////////////////////////////////////////////////

	private static void InitNGUI()
	{
#if ACCESS_NGUI
		if (UICamera.eventHandler == null)
			return;

		// Create a touch blocker and item frame for NGUI
		GameObject uiCamObj = UICamera.eventHandler.gameObject;
		if (uiCamObj == null)
		{
			Camera uiCam = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("UI"));
			if (uiCam != null)
				uiCamObj = uiCam.gameObject;
		}
		if (uiCamObj != null)
		{
			instance.m_UIRoot = uiCamObj.GetComponentInParent<UIRoot>();

			// Create Touch Blocker
			if (instance.m_NGUITouchBlocker != null)
				DestroyImmediate(instance.m_NGUITouchBlocker);
			instance.CreateNGUITouchBlocker();

			// Create Item Frame
			if (instance.m_NGUIItemFrame != null)
				DestroyImmediate(instance.m_NGUIItemFrame.transform.parent.gameObject);
			CreateNGUIItemFrame();
		}
		else
		{
			Debug.LogWarning("[Accessibility] Could not find NGUI Root. Tried getting UI Camera for handling events and UI camera for layer 'UI'. Cannot create touch blocker and item highlight frame.");
		}
#endif

		HideElementFrame();
		instance.EnableTouchBlocker(m_IsEnabled);
	}

	//////////////////////////////////////////////////////////////////////////

	void OnSceneLoaded(Scene newScene, LoadSceneMode sceneLoadMode)
	{
		Debug.Log("[Accessibility] Level loaded, clearing everything.");
		InitNGUI();
		m_ActiveContainers.Clear();
		m_SuspendedContainers.Clear();
		m_AudioQueue.Stop();
		m_CurrentItem = null;
	}

	//////////////////////////////////////////////////////////////////////////

	private static void CreateNGUIItemFrame()
	{
#if ACCESS_NGUI
		GameObject newItemFrame = new GameObject("Accessibility Plugin - NGUI Item Frame");
		DontDestroyOnLoad(newItemFrame);
		newItemFrame.layer = instance.m_UIRoot.gameObject.layer;
		newItemFrame.transform.SetParent(instance.m_UIRoot.transform, false);
		newItemFrame.transform.localPosition = new Vector3(0, 0, 0);
		newItemFrame.transform.localScale = new Vector3(1, 1, 1);
		UIPanel itemPanel = newItemFrame.AddComponent<UIPanel>();
		itemPanel.depth = 11000;
		//itemPanel.sortingOrder = 11000;

		GameObject childObj = new GameObject("Item Frame");
		childObj.layer = instance.m_UIRoot.gameObject.layer;
		childObj.transform.SetParent(newItemFrame.transform, false);
		childObj.transform.localPosition = new Vector3(0, 0, 0);
		childObj.transform.localScale = new Vector3(1, 1, 1);

		UI2DSprite itemFrame = childObj.AddComponent<UI2DSprite>();
		itemFrame.depth = 11001;
		Image unityImage = instance.m_Frame.gameObject.GetComponent<Image>();
		itemFrame.sprite2D = unityImage.sprite;
		itemFrame.color = unityImage.color;
		itemFrame.type = UIBasicSprite.Type.Sliced;
		itemFrame.border = new Vector4(10, 10, 10, 10);
		itemFrame.centerType = UIBasicSprite.AdvancedType.Invisible;
		instance.m_NGUIItemFrame = itemFrame as UIWidget;
#endif
	}

	//////////////////////////////////////////////////////////////////////////

#if ACCESS_NGUI
	private void CreateNGUITouchBlocker()
	{
		/*
			GameObject newItemFrame = new GameObject("Accessibility Plugin - NGUI Item Frame");
			DontDestroyOnLoad(newItemFrame);
			newItemFrame.layer = instance.m_UIRoot.gameObject.layer;
			newItemFrame.transform.SetParent(instance.m_UIRoot.transform, false);
			newItemFrame.transform.localPosition = new Vector3(0, 0, 0);
			newItemFrame.transform.localScale = new Vector3(1, 1, 1);
			UIPanel itemPanel = newItemFrame.AddComponent<UIPanel>();
			itemPanel.depth = 11000;
			//itemPanel.sortingOrder = 11000;

			GameObject childObj = new GameObject("Item Frame");
			childObj.layer = instance.m_UIRoot.gameObject.layer;
			childObj.transform.SetParent(newItemFrame.transform, false);
			childObj.transform.localPosition = new Vector3(0, 0, 0);
			childObj.transform.localScale = new Vector3(1, 1, 1);
	
		 */
		m_NGUITouchBlocker = new GameObject("Accessibility Plugin - NGUI Touch Blocker");
		DontDestroyOnLoad(m_NGUITouchBlocker);
		m_NGUITouchBlocker.layer = instance.m_UIRoot.gameObject.layer;
		m_NGUITouchBlocker.transform.SetParent(instance.m_UIRoot.transform, false);
		m_NGUITouchBlocker.transform.localPosition = new Vector3(0, 0, 0);
		m_NGUITouchBlocker.transform.localScale = new Vector3(1, 1, 1);
		UIPanel itemPanel = m_NGUITouchBlocker.AddComponent<UIPanel>();
		itemPanel.depth = 11000;
		//itemPanel.sortingOrder = 11000;

		GameObject childObj = new GameObject("Blocker");
		childObj.layer = instance.m_UIRoot.gameObject.layer;
		childObj.transform.SetParent(m_NGUITouchBlocker.transform, false);
		childObj.transform.localPosition = new Vector3(0, 0, 0);
		childObj.transform.localScale = new Vector3(1, 1, 1);

		childObj.AddComponent<BoxCollider>();
		UITexture blockingTexture = childObj.AddComponent<UITexture>();
		blockingTexture.depth = 12000;
		blockingTexture.autoResizeBoxCollider = true;
		blockingTexture.width = instance.m_UIRoot.manualWidth;
		blockingTexture.height = instance.m_UIRoot.activeHeight;
		blockingTexture.SetAnchor(instance.m_UIRoot.transform);
	}
#endif

	//////////////////////////////////////////////////////////////////////////

	private static bool ShouldAutoEnable()
	{
#if UNITY_ANDROID
		bool talkBackEnabled = IsTalkBackEnabled();
		if (talkBackEnabled /*IsTalkBackEnabledAndTouchExploreActive()*/)
			instance.Say_Internal("Please suspend TalkBack during play.", false);
#endif


#if UNITY_EDITOR
		if (instance.m_EditorOverride)
			return instance.m_EditorEnabledState;
#endif

		bool shouldTurnOn = instance.m_DefaultState;
		// If there is a saved value, this overrides the default
		bool hasSavedState = (instance.m_SaveEnabledState ? PlayerPrefs.HasKey("UAP_Enabled_State") : false);
		if (hasSavedState)
			shouldTurnOn = (PlayerPrefs.GetInt("UAP_Enabled_State", 0) == 1);

		// If a screen reader is detected, the plugin might turn itself on automatically
		// But only if the plugin state wasn't saved as off already
		if (!hasSavedState && instance.m_AutoTurnOnIfScreenReaderDetected)
		{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			if (WindowsTTS.IsScreenReaderDetected())
				shouldTurnOn = true;
#elif UNITY_ANDROID
			if (talkBackEnabled)
				shouldTurnOn = true;
#elif UNITY_IOS
			// Detect VoiceOver running
			if (iOSTTS.VoiceOverDetected())
				shouldTurnOn = true;
#endif
		}

		return shouldTurnOn;
	}
	//////////////////////////////////////////////////////////////////////////

	private static void SavePluginEnabledState()
	{
#if UNITY_EDITOR
		// Don't save current state if this was set via the Editor
		if (instance.m_EditorOverride)
		{
			PlayerPrefs.SetInt("UAP_Enabled_State", instance.m_DefaultState ? 1 : 0);
			PlayerPrefs.Save();
			return;
		}
#endif
		PlayerPrefs.SetInt("UAP_Enabled_State", m_IsEnabled ? 1 : 0);
		PlayerPrefs.Save();
	}

	//////////////////////////////////////////////////////////////////////////

	static void ReadItem(AccessibleUIGroupRoot.Accessible_UIElement element, bool quickOnly = false)
	{
		Initialize();

		if (!m_IsEnabled || m_Paused)
			return;

		if (instance.m_AudioPlayer.isPlaying)
			instance.m_AudioPlayer.Stop();

		if (element == null)
			return;

		UpdateElementFrame(ref element);

		// Read the element
		SpeakElement_Text(ref element);

		// This adds a pause if needed
		instance.ReadDisabledState();

		// Read value adds a pause if needed
		instance.ReadValue();

		// Read type, unless suppressed
		if (element.ReadType())
		{
			if (instance.m_ContinuousReading)
				instance.SayPause(instance.m_TypeDelay * 0.5f);
			else
				instance.SayPause(instance.m_TypeDelay);
			instance.ReadType();
		}

		// Read hint, but not in quick mode (repeats and continuous reading)
		if (!quickOnly)
		{
			instance.SayPause(instance.m_HintDelay);
			instance.ReadHint();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private static void UpdateElementFrame(ref AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		if (instance.m_Frame == null)
		{
			GameObject newFrame = Instantiate(instance.m_FrameTemplate, instance.m_TouchBlocker.transform) as GameObject;
			instance.m_Frame = (newFrame).transform as RectTransform;

			//Instantiate(instance.m_FrameTemplate, instance.m_TouchBlocker.transform);
		}

#if ACCESS_NGUI
		if (instance.m_NGUIItemFrame == null)
		{
			InitNGUI();
			if (instance.m_NGUIItemFrame == null) 
				return;
		}
#endif

		HideElementFrame();


		RectTransform rect = GetElementRect(ref element);
		// Place the frame around the element
		if (rect != null)
		{
			instance.m_Frame.gameObject.SetActive(true);

			// If the owning Canvas is set to 'Screen Space - Camera', then the coordinate calculate won't be correct,
			// Since the Accessibility Canvas uses 'Screen Space - Overlay'.
			Canvas owner = GetOwningCanvas(ref element);
			// There must be a owning canvas, or there's something else seriously wrong
			if (owner == null)
				return;

//			if (owner.renderMode != RenderMode.ScreenSpaceOverlay && owner.worldCamera != null)
			{
				instance.m_Frame.transform.SetParent(rect.transform, false);
				instance.m_Frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.height);
				instance.m_Frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width);
			}
/*
			else
			{
				Vector2 anchorMin;
				Vector2 anchorMax;
				Vector2 centerPos;
				AccessibleUIGroupRoot.GetAbsoluteAnchors(rect, out anchorMin, out anchorMax, out centerPos);
				instance.m_Frame.anchoredPosition = rect.TransformPoint(0.5f, 0.5f, 0);
				instance.m_Frame.offsetMin = new Vector2(0, 0);
				instance.m_Frame.offsetMax = new Vector2(0, 0);

				instance.m_Frame.position = rect.TransformPoint(0.5f, 0.5f, 0);

				// Take Pivot into account
				Vector3 rectAdjustedPos = rect.position;
				rectAdjustedPos.x -= (rect.pivot.x - 0.5f) * rect.rect.width;
				rectAdjustedPos.y -= (rect.pivot.y - 0.5f) * rect.rect.height;
				instance.m_Frame.position = rectAdjustedPos;

				// Note: If any RectTransform in the hierarchy of the current UI element contains scaling, this won't quite work,
				// because the TransformPoint function and rect width and height will be scaled. The screen width and height won't work appropriately.
				// The frame will appear scaled up or down.
				float w = ((RectTransform)instance.m_CanvasRoot.transform).rect.width * rect.lossyScale.x;
				float h = ((RectTransform)instance.m_CanvasRoot.transform).rect.height * rect.lossyScale.y;
				instance.m_Frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width * 1.03f * w / (float)Screen.width);
				instance.m_Frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.height * 1.03f * h / (float)Screen.height);
			}
*/
		}

#if ACCESS_NGUI
		GameObject refObject = element.m_Object.GetTargetGameObject();
		UIWidget widget = refObject.GetComponent<UIWidget>();
		if (widget == null)
			widget = refObject.GetComponentInChildren<UIWidget>();

		if (widget != null)
		{
			NGUITools.SetActive(instance.m_NGUIItemFrame.gameObject, true);

			instance.m_NGUIItemFrame.transform.position = widget.worldCenter;
			//Vector2 delta = widget.worldCorners[1] - widget.worldCorners[0];
			//delta = delta * (1.0f / widget.transform.lossyScale.x);
			Bounds bounds = widget.CalculateBounds(instance.m_UIRoot.transform);
			//float x = bounds.center.x + Screen.width / 2 - bounds.size.x / 2;
			//float y = Screen.height - (bounds.center.y + Screen.height / 2 + bounds.size.y / 2);
			float w = bounds.size.x + 15;
			float h = bounds.size.y + 15;
			//Debug.Log("Item Frame Width=" + w.ToString("0") + " Height=" + h.ToString("0"));
			instance.m_NGUIItemFrame.width = Mathf.CeilToInt(w);
			instance.m_NGUIItemFrame.height = Mathf.CeilToInt(h);

			// If the item is inside a scroll view, it might need scrolling
			bool autoScroll = !refObject.GetComponentInParent<UAP_ScrollControl>();
			if (element.m_Object.m_IsInsideScrollView && autoScroll)
			{
				UIScrollView scrollView = element.m_Object.GetTargetGameObject().GetComponentInParent<UIScrollView>();
				if (scrollView != null) // Safety First!
				{
					// Is the element already visible?
					//Rect screenRect = new Rect (0,0, Screen.width, Screen.height);
					UIPanel scrollViewWidget = scrollView.GetComponent<UIPanel>();
					Vector3[] sidesView = scrollViewWidget.GetSides(scrollViewWidget.transform.parent);
					Vector3[] sidesObject = widget.GetSides(scrollViewWidget.transform.parent);
					bool isVisible = true;
					if (sidesObject[0].x < sidesView[0].x)
						isVisible = false;
					if (sidesObject[1].y > sidesView[1].y)
						isVisible = false;
					if (sidesObject[2].x > sidesView[2].x)
						isVisible = false;
					if (sidesObject[3].y < sidesView[3].y)
						isVisible = false;

					//Debug.Log("View Sides: " + sidesView[0] + " " + sidesView[1] + " " + sidesView[2] + " " + sidesView[3]);
					//Debug.Log("Element Sides: " + sidesObject[0] + " " + sidesObject[1] + " " + sidesObject[2] + " " + sidesObject[3] + " Inside? " + isVisible);

					if (!isVisible)
					{
						//						Debug.Log("Not visible, so ..");
						scrollView.dragEffect = UIScrollView.DragEffect.None;
						if (scrollView.centerOnChild == null)
						{
							//							Debug.Log("Attaching center on child on " + scrollView.name);
							scrollView.centerOnChild = scrollView.gameObject.AddComponent<UICenterOnChild>();
							instance.m_AddedScrollCenterComponents.Add(scrollView.centerOnChild);
						}
						scrollView.centerOnChild.enabled = true;
						scrollView.centerOnChild.CenterOn(element.m_Object.GetTargetGameObject().transform);
					}
				}
			}
		}
#endif
	}

	//////////////////////////////////////////////////////////////////////////

	private static void HideElementFrame()
	{
#if ACCESS_NGUI
		if (instance.m_NGUIItemFrame != null)
			NGUITools.SetActive(instance.m_NGUIItemFrame.gameObject, false);
#endif

		if (instance.m_Frame != null && instance.m_Frame.gameObject != null)
			instance.m_Frame.gameObject.SetActive(false);
	}

	//////////////////////////////////////////////////////////////////////////

	private static Canvas GetOwningCanvas(ref AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		GameObject refObject = element.m_Object.gameObject;// GetTargetGameObject();
		return refObject.GetComponentInParent<Canvas>();
	}

	//////////////////////////////////////////////////////////////////////////

	private static RectTransform GetElementRect(ref AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		GameObject refObject = element.m_Object.m_UseTargetForOutline ? element.m_Object.GetTargetGameObject() : element.m_Object.gameObject;
		RectTransform rect = refObject.GetComponent<RectTransform>();

		if (rect != null)
		{
			// Is the element inside a scroll view that needs to be adjusted?
			bool autoScroll = !refObject.GetComponentInParent<UAP_ScrollControl>();
			if (element.m_Object.m_IsInsideScrollView && autoScroll)
			{
				// Get the ScollView
				ScrollRect scrollRect = refObject.GetComponentInParent<ScrollRect>();

				// It should not be null, but always put safety first
				if (scrollRect != null)
				{
					// Is the element (fully) visible?
					bool isVisible = true;

					if (scrollRect.viewport == null)
					{
						scrollRect.viewport = (RectTransform)scrollRect.transform.Find("Viewport").transform;
						if (scrollRect.viewport == null)
							scrollRect.viewport = (RectTransform)scrollRect.transform.GetChild(0);
					}

					RectTransform viewportRect = (RectTransform)scrollRect.viewport.transform;
					Vector3[] scrollViewWorldCorners = new Vector3[4];
					viewportRect.GetWorldCorners(scrollViewWorldCorners);
					Vector3[] elementWorldCorners = new Vector3[4];
					rect.GetWorldCorners(elementWorldCorners);

					bool xInvsibile = elementWorldCorners[0].x < scrollViewWorldCorners[0].x || elementWorldCorners[2].x > scrollViewWorldCorners[2].x;
					bool yInvsibile = elementWorldCorners[0].y < scrollViewWorldCorners[0].y || elementWorldCorners[2].y > scrollViewWorldCorners[2].y;
					if (xInvsibile || yInvsibile)
						isVisible = false;

					//Vector2 elementPos = rect.TransformPoint(0.5f, 0.5f, 0);
					//Debug.Log("Scroll view corners: " + scrollViewWorldCorners[0] + " and " + scrollViewWorldCorners[2] + " and item pos " + elementPos + " Visible: " + isVisible + " x " + xInvsibile + " y " + yInvsibile);

					// Scrolling a page to make the element visible
					if (!isVisible)
					{
						Vector3[] contentWorldCorners = new Vector3[4];
						scrollRect.content.GetWorldCorners(contentWorldCorners);

						// Scroll in x direction
						if (scrollRect.horizontal && xInvsibile)
						{

							// Safety First! Always consider user errors!
							if (contentWorldCorners[2].x == 0.0f)
								contentWorldCorners[2].x = 0.01f;

							// Instead of the element center, use the correct corner point instead!
							float xVal = (elementWorldCorners[0].x < scrollViewWorldCorners[0].x) ? elementWorldCorners[0].x : elementWorldCorners[2].x;
							float scrollPosX = (xVal - contentWorldCorners[0].x) / contentWorldCorners[2].x;
							scrollRect.horizontalNormalizedPosition = scrollPosX;
						}

						// Scroll in y direction
						if (scrollRect.vertical && yInvsibile)
						{
							// Safety First! Always consider user errors!
							if (contentWorldCorners[2].y == 0.0f)
								contentWorldCorners[2].y = 0.01f;

							// Instead of the element center, use the correct corner point instead!
							float yVal = (elementWorldCorners[0].y < scrollViewWorldCorners[0].y) ? elementWorldCorners[0].y : elementWorldCorners[2].y;
							float scrollPosY = (yVal - contentWorldCorners[0].y) / contentWorldCorners[2].y;
							scrollRect.verticalNormalizedPosition = scrollPosY;
						}
					} // if item is not visible
				} // scroll view safety check
			} // inside scroll view
		} // end of uGUI

		return rect;
	}

	//////////////////////////////////////////////////////////////////////////

	private bool IsPositionOverElement(Vector2 fingerPos, AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		RectTransform rect = GetElementRect(ref element);
		if (rect != null)
		{
			//Vector2 anchorMin;
			//Vector2 anchorMax;

			CanvasScaler owner = element.m_Object.GetComponentInParent<CanvasScaler>();
			float wVal = (float)Screen.width;
			float hVal = (float)Screen.height;
			if (owner != null)
			{
				if (owner.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
				{
					hVal = owner.referenceResolution.y;
					wVal = owner.referenceResolution.x;
				}
			}
			float wScale = Screen.width / wVal;
			float hScale = Screen.height / hVal;

			Vector2 rectAdjustedPos = rect.position;
			float rectWidth = rect.rect.width;
			float rectheight = rect.rect.height;

			//rectAdjustedPos.x *= wScale;
			//rectAdjustedPos.y *= hScale;
			rectWidth *= wScale;
			rectheight *= hScale;

			rectAdjustedPos.x -= (rect.pivot.x - 0.5f) * rectWidth;
			rectAdjustedPos.y -= (rect.pivot.y - 0.5f) * rectheight;

			bool isInside = true;
			Vector2 testPosition = rectAdjustedPos - fingerPos;

			Vector2 delta = new Vector2(0, 0);
			delta.x = rectWidth * 0.5f;
			delta.y = rectheight * 0.5f;
			if (delta.x < Mathf.Abs(testPosition.x))
				isInside = false;
			if (delta.y < Mathf.Abs(testPosition.y))
				isInside = false;

			if (isInside)
				return true;
			//Debug.Log(element.m_Object.name + " (" + isInside + "):" + "FingerPos " + fingerPos + " Element Pos " + rect.position + " with a width and height of " + rectWidth + " x " + rectheight);
			/*
							AccessibleUIGroupRoot.GetAbsoluteAnchors(rect, out anchorMin, out anchorMax);
							anchorMin.x *= wScale;
							anchorMax.x *= wScale;
							anchorMin.y *= hScale;
							anchorMax.y *= hScale;

							//Debug.Log("Element " + element.m_Object.name + " rect: (" + anchorMin.x + "," + anchorMin.y + ") to (" + anchorMax.x + "," + anchorMax.y + ")");

							if (fingerPos.x > anchorMin.x && fingerPos.x < anchorMax.x && fingerPos.y > anchorMin.y && fingerPos.y < anchorMax.y)
									return true;
									*/
		}

#if ACCESS_NGUI
		UIWidget widget = element.m_Object.gameObject.GetComponent<UIWidget>();
		Camera uiCam = (widget != null) ? NGUITools.FindCameraForLayer(widget.gameObject.layer) : null;
		if (widget != null && uiCam != null)
		{
			//Bounds bounds = widget.CalculateBounds(instance.m_UIRoot.transform);

			//float x = bounds.center.x + Screen.width / 2;// - bounds.size.x / 2;
			//float y = Screen.height - (bounds.center.y + Screen.height / 2 + bounds.size.y / 2);

			// TODO: There seems to be an issue with scaling?
			//float w = bounds.size.x;
			//float h = bounds.size.y;

			bool isInside = true;
			Vector2 centerPos = uiCam.WorldToScreenPoint(widget.transform.position);
			float w = widget.width;
			float h = widget.height;
			Vector2 testPosition = centerPos - fingerPos;

			Vector2 delta = new Vector2(0, 0);
			delta.x = w * 0.5f;
			delta.y = h * 0.5f;
			if (delta.x < Mathf.Abs(testPosition.x))
				isInside = false;
			if (delta.y < Mathf.Abs(testPosition.y))
				isInside = false;

			//Debug.Log("Element " + widget.name + " pos: " + centerPos + " width: " + w + " height: " + h + " And finger pos is " + fingerPos + " which is " + (isInside ? "inside" : "outside"));

			if (isInside)
				return true;
		}
#endif

		return false;
	}

	//////////////////////////////////////////////////////////////////////////

	private void SayAudio(AudioClip clip, string altText, UAP_AudioQueue.EAudioType type, bool allowVoiceOver, UAP_AudioQueue.EInterrupt interrupts = UAP_AudioQueue.EInterrupt.None)
	{
		if (clip == null && (altText.Length < 1 || altText == null))
			return;

		if (clip != null)
			m_AudioQueue.QueueAudio(clip, type, interrupts);
		else
			m_AudioQueue.QueueAudio(altText, type, allowVoiceOver, interrupts);
	}

	private void SayPause(float durationInSec)
	{
		m_AudioQueue.QueuePause(durationInSec);
	}

	//////////////////////////////////////////////////////////////////////////

	void ReadDisabledState()
	{
		if (m_CurrentItem == null)
			return;

		// If this a generally interactable type?
		if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EButton ||
			m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ESlider ||
			m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EToggle ||
			m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ETextEdit ||
			m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EDropDown)
		{
			if (!m_CurrentItem.m_Object.IsInteractable())
			{
				SayPause(m_DisabledDelay);
				SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	void ReadType()
	{
		if (m_CurrentItem == null)
			return;

		switch (m_CurrentItem.m_Type)
		{
			case AccessibleUIGroupRoot.EUIElement.EButton:
				SayAudio(m_ButtonAsAudio, "Button", UAP_AudioQueue.EAudioType.Element_Type, m_CurrentItem.m_Object.m_AllowVoiceOver);
				//if (!m_CurrentItem.m_Object.IsInteractable())
				//  SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint);
				break;

			case AccessibleUIGroupRoot.EUIElement.ESlider:
				SayAudio(m_SliderAsAudio, "Slider", UAP_AudioQueue.EAudioType.Element_Type, m_CurrentItem.m_Object.m_AllowVoiceOver);
				//if (!m_CurrentItem.m_Object.IsInteractable())
				//  SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint);
				break;

			case AccessibleUIGroupRoot.EUIElement.EToggle:
				SayAudio(m_ToggleAsAudio, "Toggle", UAP_AudioQueue.EAudioType.Element_Type, m_CurrentItem.m_Object.m_AllowVoiceOver);
				//if (!m_CurrentItem.m_Object.IsInteractable())
				//  SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint);
				break;

			case AccessibleUIGroupRoot.EUIElement.ETextEdit:
				SayAudio(m_TextEditAsAudio, "Text Edit Box", UAP_AudioQueue.EAudioType.Element_Type, m_CurrentItem.m_Object.m_AllowVoiceOver);
				//if (!m_CurrentItem.m_Object.IsInteractable())
				//  SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint);
				break;

			case AccessibleUIGroupRoot.EUIElement.EDropDown:
				SayAudio(m_DropDownAsAudio, "Dropdown List", UAP_AudioQueue.EAudioType.Element_Type, m_CurrentItem.m_Object.m_AllowVoiceOver);
				//if (!m_CurrentItem.m_Object.IsInteractable())
				//  SayAudio(m_DisabledAsAudio, "Disabled", UAP_AudioQueue.EAudioType.Element_Hint);
				break;
		};
	}

	//////////////////////////////////////////////////////////////////////////

	void ReadValue(bool allowPause = true, bool interrupt = false)
	{
		if (m_CurrentItem == null)
			return;

		// Support Value reading as AudioClip
		AudioClip audio = m_CurrentItem.m_Object.GetCurrentValueAsAudio();
		string valueText = m_CurrentItem.m_Object.GetCurrentValueAsText();

		if (audio != null || (valueText != null && valueText.Length > 0))
		{
			if (allowPause)
				SayPause(instance.m_ValueDelay);
			SayAudio(audio, valueText, UAP_AudioQueue.EAudioType.Element_Text, m_CurrentItem.m_Object.m_AllowVoiceOver, interrupt ? UAP_AudioQueue.EInterrupt.Elements : UAP_AudioQueue.EInterrupt.None);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	void ReadHint()
	{
		if (m_CurrentItem == null)
			return;

		if (m_CurrentItem.m_Object.m_CustomHint)
		{
			SayAudio(m_CurrentItem.m_Object.m_HintAsAudio, m_CurrentItem.m_Object.GetCustomHint(), UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
		}
		else
		{
			switch (m_CurrentItem.m_Type)
			{
				case AccessibleUIGroupRoot.EUIElement.EButton:
					if (Application.isMobilePlatform)
						SayAudio(null, m_Mobile_HintButton, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					else
						SayAudio(null, m_Win_HintButton, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					break;

				case AccessibleUIGroupRoot.EUIElement.ETextEdit:
					if (Application.isMobilePlatform)
						SayAudio(null, m_Mobile_HintTextEdit, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					else
						SayAudio(null, m_Win_HintTextEdit, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					break;

				case AccessibleUIGroupRoot.EUIElement.EToggle:
					if (Application.isMobilePlatform)
						SayAudio(null, m_Mobile_HintToggle, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					else
						SayAudio(null, m_Win_HintToggle, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					break;

				case AccessibleUIGroupRoot.EUIElement.EDropDown:
					if (Application.isMobilePlatform)
						SayAudio(null, m_Mobile_HintDropdown, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					else
						SayAudio(null, m_Win_HintDropdown, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					break;

				case AccessibleUIGroupRoot.EUIElement.ESlider:
					if (Application.isMobilePlatform)
						SayAudio(null, m_Mobile_HintSlider, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					else
						SayAudio(null, m_Win_HintSlider, UAP_AudioQueue.EAudioType.Element_Hint, m_CurrentItem.m_Object.m_AllowVoiceOver);
					break;
			};
		}

	}

	//////////////////////////////////////////////////////////////////////////

	private static void SpeakElement_Text(ref AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		instance.SayAudio(element.m_Object.m_TextAsAudio, element.m_Object.GetText(), UAP_AudioQueue.EAudioType.Element_Hint, element.AllowsVoiceOver(), UAP_AudioQueue.EInterrupt.Elements);
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Temporarily suspend the core accessibility features.
	/// This is useful if your game requires direct touch input for gameplay.
	/// You can call this function regardless of whether the accessibility
	/// mode is enabled or not (if it is not, it will NOT enable it). There is 
	/// no need to query the state of the plugin with IsEnabled() first.
	/// When accessibility is suspended but not disabled, the plugin will continue
	/// to listen to magic gestures (unless that is disabled in the Settings).
	/// Please see the documentation for a more thorough explanation.
	/// To turn off the accessibility completely, use EnableAccessibility() instead.
	/// </summary>
	/// <param name="pause">true to pause, false to resume</param>
	static public void PauseAccessibility(bool pause)
	{
		if (isDestroyed)
			return;

		Initialize();

		StopContinuousReading();
		
		if (m_Paused == pause)
			return;

		m_Paused = pause;

		instance.m_AudioQueue.Stop();
		instance.m_AudioPlayer.Stop();
		instance.EnableTouchBlocker(!pause);
		if (pause)
			HideElementFrame();

		// 		if (!pause && m_IsEnabled)
		// 		{
		// 			EnableAccessibility(true);
		// 		}
	}

	//////////////////////////////////////////////////////////////////////////

	private static void StopContinuousReading()
	{
		Initialize();

		instance.m_ContinuousReading = false;
		instance.m_ContinuousReading_WaitInputClear = false;
	}

	//////////////////////////////////////////////////////////////////////////

	private void EnableTouchBlocker(bool enable)
	{
		// Enable/Disable the full screen overlay
		if (!enable || !m_IsEnabled)
		{
			m_TouchBlocker.SetActive(false);
#if ACCESS_NGUI
			NGUITools.SetActive(m_NGUITouchBlocker, false);
#endif
		}
		else
		{
			if (!m_Paused)
			{
				m_TouchBlocker.SetActive(m_HandleUI);
#if ACCESS_NGUI
				NGUITools.SetActive(m_NGUITouchBlocker, m_HandleUI);
#endif
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This function temporarily blocks input from users to the Accessibility System.
	/// Use this to have the accessibility plugin ignore any input from users, but keep the touch blocker
	/// active that prevents blind users from accidentally hitting any of your
	/// UI elements.<br><br>
	/// This function is useful if you have a native dialog open, if you want to wait for a sound or animation to finish,
	/// or need an announcement to be done before you accept user input again.<br><br>
	/// If the Accessibility Plugin is inactive or paused, or is not set to handle UI 
	/// this will have no effect.<br>
	/// This will not stop speech from playing, so you can still use the TTS functionality. Use this 
	/// instead of ::PauseAccessibility if you still need speech.
	/// </summary>
	/// <param name="block">true if you want to suspend the input, false to restore</param>
	static public void BlockInput(bool block, bool stopSpeakingOnBlock = true)
	{
		Initialize();

		StopContinuousReading();
		
		instance.m_BlockInput = block;
		//		instance.m_CurrentItem = null;

		if (block && stopSpeakingOnBlock)
			StopSpeaking();
	}

	//////////////////////////////////////////////////////////////////////////

	static public void ElementRemoved(AccessibleUIGroupRoot.Accessible_UIElement element)
	{
		if (element != instance.m_CurrentItem || element == null)
			return;

		if (instance.m_PreviousItem == element)
			instance.m_PreviousItem = null;

		instance.UpdateCurrentItem(true);

		ReadItem(instance.m_CurrentItem);
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Called by a UI container to make its content (un-)available for interaction.
	/// You don't need to call this function from your own code unless you write 
	/// a custom implementation of AccessibleUIGroupRoot class.
	/// </summary>
	/// <param name="container">The UI container containing the accessible UI elements.</param>
	/// <param name="activate">Whether to enable or disable the elements.</param>
	static public void ActivateContainer(AccessibleUIGroupRoot container, bool activate)
	{
		Initialize();

		/*
				if (activate)
				{
					Debug.Log("Activating UI Container " + container.name);
				}
				else
				{
					Debug.Log("Deactivating UI Container " + container.name);
				}
		*/

		if (activate)
		{
			if (m_ContainersToActivate.Contains(container))
				return;

			// Sort by priority
			bool added = false;
			for (int i = 0; i < m_ContainersToActivate.Count; ++i)
			{
				if (container.m_Priority > m_ContainersToActivate[i].m_Priority)
				{
					m_ContainersToActivate.Insert(i, container);
					added = true;
					//Debug.Log("Activating UI Container " + container.name);
					break;
				}
			}
			if (!added)
			{
				m_ContainersToActivate.Add(container);
				//Debug.Log("Activating UI Container " + container.name);
			}
		}
		else
		{
			if (m_ContainersToActivate.Contains(container))
				m_ContainersToActivate.Remove(container);

			instance.ActivateContainer_Internal(container, false, false);
			instance.m_ReadItemNextUpdate = !instance.m_BlockInput;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private AccessibleUIGroupRoot GetActivePopup()
	{
		foreach (AccessibleUIGroupRoot container in m_ActiveContainers)
		{
			if (container.m_PopUp)
				return container;
		}

		return null;
	}

	//////////////////////////////////////////////////////////////////////////

	private void ActivateContainer_Internal(AccessibleUIGroupRoot container, bool activate, bool readCurrentItem = true)
	{
		if (container == null)
			return;

		//Debug.Log("Internally changing state of UI Container " + container.name + " to " + activate);

		if (activate)
		{
			// Don't activate the container again if it is already active
			if (m_ActiveContainers.Contains(container))
				return;

			// Check if this is a popup (which will suspend the currently active containers)
			if (container.m_PopUp)
			{
				// Play popup sound
				if (m_IsEnabled)
					PlaySFX(m_UIPopUpOpen);

				// Suspend all currently active containers
				List<AccessibleUIGroupRoot> newSuspendedList = new List<AccessibleUIGroupRoot>();
				foreach (AccessibleUIGroupRoot activeContainer in m_ActiveContainers)
					newSuspendedList.Add(activeContainer);
				m_SuspendedContainers.Add(newSuspendedList);
				m_ActiveContainers.Clear();
				SelectNothing();
			}
			else
			{
				// This is not a popup, but what if there is one open. Can this join? 
				AccessibleUIGroupRoot activePopup = GetActivePopup();
				// Only relevant if we HAVE a popup, AND the popup doesn't allow external containers to join (which is the default though)
				if (activePopup != null && !activePopup.m_AllowExternalJoining)
				{
					// Is this container a child of the popup?
					if (!container.transform.IsChildOf(activePopup.transform))
					{
						// We have an external container, and it cannot join the popup due to its settings.
						// Push it in the background instead.
						if (m_SuspendedContainers.Count > 0)
						{
							// There are already containers in the background, let's push it into the last one (best guess)
							List<AccessibleUIGroupRoot> suspendedList = m_SuspendedContainers[m_SuspendedContainers.Count - 1];

							// Add by priority
							bool wasAdded = false;
							for (int i = 0; i < suspendedList.Count; ++i)
							{
								if (container.m_Priority > suspendedList[i].m_Priority)
								{
									suspendedList.Insert(i, container);
									wasAdded = true;
									break;
								}
							}
							if (!wasAdded)
								suspendedList.Add(container);
						}
						else
						{
							// There are no suspended containers yet, so create a new entry
							List<AccessibleUIGroupRoot> newSuspendedList = new List<AccessibleUIGroupRoot>();
							newSuspendedList.Add(container);
							m_SuspendedContainers.Add(newSuspendedList);
						}

						// Nothing else to do, really
						return;
					}
				}
			}


			// Sort by priority
			bool added = false;
			for (int i = 0; i < m_ActiveContainers.Count; ++i)
			{
				if (container.m_Priority > m_ActiveContainers[i].m_Priority)
				{
					m_ActiveContainers.Insert(i, container);
					added = true;
					break;
				}
			}
			if (!added)
				m_ActiveContainers.Add(container);


			// Is this the only active container now? In that case, start reading first item
			if (m_ActiveContainers.Count == 1)
			{
				m_ActiveContainerIndex = 0;
				ReadContainerName();
				m_CurrentItem = m_ActiveContainers[0].GetCurrentElement(m_CyclicMenus);
				UpdateCurrentItem();
				if (readCurrentItem)
					ReadItem(m_CurrentItem);
			}
			else if (m_CurrentItem == null)
			{
				// There is more than one active container, but no active item (empty containers, or all disabled items?)
				UpdateCurrentItem();
				if (readCurrentItem)
					ReadItem(m_CurrentItem);
			}
			else
			{
				// In this case the container that was just added is not the first and only one on screen,
				// and the current container currently has the focus. Nothing to do here.
			}
		}
		else
		{
			// Don't deactivate something that wasn't even active
			if (!m_ActiveContainers.Contains(container))
			{
				// Check if the container that is to be deactivated is in the suspended lists
				// and if it is, remove it from there
				foreach (List<AccessibleUIGroupRoot> newSuspendedList in m_SuspendedContainers)
				{
					if (newSuspendedList.Contains(container))
					{
						newSuspendedList.Remove(container);
						if (newSuspendedList.Count == 0)
							m_SuspendedContainers.Remove(newSuspendedList);

						break;
					}
				}


				return;
			}

			//Debug.Log("Deactivating UI Container " + container.name);

			// Check if this was a popup (which suspended previously active containers)
			if (container.m_PopUp)
			{
				// Play popup close sound
				if (m_IsEnabled)
					PlaySFX(m_UIPopUpClose);

				m_ActiveContainers.Clear();

				// Reactivate the suspended containers (pop one stack only though!)
				if (m_SuspendedContainers.Count > 0)
				{
					List<AccessibleUIGroupRoot> suspendedList = m_SuspendedContainers[m_SuspendedContainers.Count - 1];
					foreach (AccessibleUIGroupRoot suspendedContainer in suspendedList)
						m_ActiveContainers.Add(suspendedContainer);
					m_SuspendedContainers.RemoveAt(m_SuspendedContainers.Count - 1);
				}

				// TODO: Restore previously active container index instead.
				// That would give the smoothest ride.
				if (m_ActiveContainers.Count > 0)
					m_ActiveContainerIndex = 0;
				else
					m_ActiveContainerIndex = -1;
			}
			else
			{
				int index = m_ActiveContainers.IndexOf(container);
				if (index == m_ActiveContainerIndex)
				{
					m_ActiveContainerIndex = m_ActiveContainers.Count - (index + 2);
				}
				else if (index < m_ActiveContainerIndex)
					--m_ActiveContainerIndex;
				m_ActiveContainers.Remove(container);

			}

			// Read next item, if there is an active container
			if (m_ActiveContainerIndex < 0)
			{
				m_CurrentItem = null;
				m_PreviousItem = null;
			}
			else
			{
				UpdateCurrentItem();
				if (readCurrentItem)
					ReadItem(m_CurrentItem);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void SelectNothing()
	{
		m_ActiveContainerIndex = 0;

		if (m_PreviousItem != null && m_PreviousItem.m_Object != null)
			m_PreviousItem.m_Object.HoverHighlight(false);
		if (m_CurrentItem != null && m_CurrentItem != m_PreviousItem && m_CurrentItem.m_Object != null)
			m_CurrentItem.m_Object.HoverHighlight(false);

		m_CurrentItem = null;
		m_PreviousItem = null;

		HideElementFrame();
	}

	//////////////////////////////////////////////////////////////////////////

	private void UpdateCurrentItem(bool makeSureItemIsSelected = false)
	{
		// This function verifies that the current item 
		// is indeed valid and if not, it will select the next
		// valid item if needed
		m_CurrentItem = null;

		if (m_ActiveContainers.Count == 0)
		{
			m_ActiveContainerIndex = -1;
			if (m_PreviousItem != null)
				m_PreviousItem.m_Object.HoverHighlight(false);
			return;
		}

		int containerIndex = m_ActiveContainerIndex;
		if (containerIndex < 0)
			containerIndex = 0;

		for (int i = 0; i < m_ActiveContainers.Count; ++i)
		{
			int index = i + containerIndex;
			if (index >= m_ActiveContainers.Count)
				index = index - m_ActiveContainers.Count;

			m_CurrentItem = m_ActiveContainers[index].GetCurrentElement(m_CyclicMenus);
			if (m_CurrentItem == null)
			{
				if (makeSureItemIsSelected)
				{
					DecrementUIElement();
					UpdateCurrentItem();
					return;
				}
			}
			if (m_CurrentItem != null)
			{
				m_ActiveContainerIndex = index;

				if (m_PreviousItem != m_CurrentItem)
				{
					if (m_PreviousItem != null)
						m_PreviousItem.m_Object.HoverHighlight(false);
					m_CurrentItem.m_Object.HoverHighlight(true);
				}

				m_PreviousItem = m_CurrentItem;
				return;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Returns whether Accessibility mode is enabled or not.
	/// This does not consider paused state. If the Accessibility mode is active, but
	/// currently paused, this function will still return true.
	/// If you need to know whether the mode is active, query IsActive() instead.
	/// </summary>
	/// <returns>true if the plugin is active, false otherwise</returns>
	public static bool IsEnabled()
	{
		if (isDestroyed)
			return false;

		Initialize();
		return m_IsEnabled;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Returns whether Accessibility mode is enabled and unpaused.
	/// If the plugin is enabled, but currently paused, this function will return true.
	/// Please use IsEnabled() if you want to query whether the plugin is enabled at all. 
	/// </summary>
	/// <returns>true if plugin is enabled and unpaused</returns>
	public static bool IsActive()
	{
		if (isDestroyed)
			return false;

		Initialize();
		return m_IsEnabled && !m_Paused;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Convenience function to check whether the current platform is officially supported.
	/// Has no further effect on the plugin.
	/// </summary>
	/// <returns></returns>
	public static bool IsCurrentPlatformSupported()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
			return true;

		return false;
	}

	//////////////////////////////////////////////////////////////////////////

	public static void EnableMagicGestures(bool enable)
	{
		Initialize();

		instance.m_HandleMagicGestures = enable;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Use this to completely enable/disable the accessibility plugin.
	/// If accessibility is disabled, there will be no reaction to any user input, 
	/// calls to Say() will be ignored, and calls to PauseAccessibility() will have no effect.<br>
	/// Use this to turn on/off accessibility via your app's menu.<br>
	/// You can also use the Plugin Toggle component to do this automatically. See <a href="UIElements.html#PluginToggle">Plugin Toggle Documentation</a>.
	/// 
	/// Note that the user can still turn on accessibility via the three finger triple tap.
	/// Disable this Magic Gesture individually if your want this to be disabled too.
	/// This is inadvisable though, as it leaves blind users who accidentally
	/// turn off accessibility with no option of turning it back on.
	/// </summary>
	/// <param name="enable">wheter to enable or disable plugin</param>
	/// <param name="readNotification">if true, the plugin will announce the state change via TTS, so that the user gets audio confirmation</param>
	public static void EnableAccessibility(bool enable, bool readNotification = false)
	{
		if (isDestroyed)
			return;

		if (enable == m_IsEnabled)
			return;

		Initialize();

		if (enable)
		{
#if UNITY_IOS
			if (instance.m_iOSGestures == null)
			{
				GameObject newGestureDetector = new GameObject("UAP_IOSGestures");
				instance.m_iOSGestures = newGestureDetector.AddComponent<iOSGestures>();
				instance.m_iOSGestures.SetUAP(instance);
				newGestureDetector.transform.SetParent(instance.transform, false);
			}
			instance.m_iOSGestures.StartRecognition();
#endif
		}
		else
		{
#if ACCESS_NGUI
			foreach (UICenterOnChild centerHelper in instance.m_AddedScrollCenterComponents)
			{
				if (centerHelper != null)
					centerHelper.enabled = false;
			}
#endif
#if UNITY_IOS
			if (instance.m_iOSGestures != null)
			{
				instance.m_iOSGestures.StopRecognition();
			}
#endif
		}

		StopContinuousReading();

		if (enable)
		{
			// TODO: Check if this is the first time that accessibility was turned on ever
			if (PlayerPrefs.GetInt("Accessibility_FirstTimeStartup", 1) == 1)
			{
				if (Application.isMobilePlatform)
					instance.SayAudio(null, instance.m_Mobile_General, UAP_AudioQueue.EAudioType.App, true, UAP_AudioQueue.EInterrupt.All);
				else
					instance.SayAudio(null, instance.m_Win_General, UAP_AudioQueue.EAudioType.App, true, UAP_AudioQueue.EInterrupt.All);
				PlayerPrefs.SetInt("Accessibility_FirstTimeStartup", 0);
				readNotification = false;
			}

			bool wasOn = m_IsEnabled;
			m_IsEnabled = true;
			m_Paused = false;
			instance.OnEnable();

			if (readNotification)
				instance.Say_Internal(instance.m_Phrase_EnablingAccessibility, false, true, UAP_AudioQueue.EInterrupt.All);

			if (!wasOn)
			{
				instance.StartScreenOver();
			}
		}
		else // if turn off
		{
			StopSpeaking();

			if (readNotification)
				instance.Say_Internal(instance.m_Phrase_DisabledAccessibility, false, true, UAP_AudioQueue.EInterrupt.All);

			HideElementFrame();
			m_IsEnabled = false;
			instance.OnDisable();
		}

		SavePluginEnabledState();

		// Callback to app
		if (instance.m_OnAccessibilityModeChanged != null)
			instance.m_OnAccessibilityModeChanged(enable);
	}

	//////////////////////////////////////////////////////////////////////////

	void OnEnable()
	{
		// if not paused, make sure the overlay is enabled, too
		EnableTouchBlocker(true);
	}

	void OnDisable()
	{
		if (m_AudioQueue != null)
			m_AudioQueue.Stop();

		// if not paused, make sure the overlay is disabled, too
		if (m_TouchBlocker != null)
			EnableTouchBlocker(false);
	}

	//////////////////////////////////////////////////////////////////////////

	void UpdateMagicTapDetection()
	{
		bool magicTapFound = false;

		if (m_WaitingForMagicTap /*&& Input.touchCount == 0*/)
		{
			if (Time.time > m_MagicTap_LastTapTime + m_DoubleTapTime)
			{
				m_WaitingForMagicTap = false;

				// Notify two-finger single tap callbacks - this can no longer become a Magic Tap
				if (m_OnTwoFingerSingleTapCallbacks != null)
					m_OnTwoFingerSingleTapCallbacks();
			}
		}

		//if (Input.GetMouseButtonDown(0) && Input.touchCount == 2)
		if (m_MagicTap_TouchCountHelper < 2 && Input.touchCount == 2)
		{
			// Was there another two finger touch recently?
			if (Time.time < m_MagicTap_LastTapTime + m_DoubleTapTime)
			{
				magicTapFound = true;
				m_WaitingForMagicTap = false;
			}
			else
			{
				m_WaitingForMagicTap = true;
			}
			m_MagicTap_LastTapTime = Time.time;
		}

		m_MagicTap_TouchCountHelper = Input.touchCount;

		if (magicTapFound)
		{
			m_MagicTap_LastTapTime = -1.0f;
			if (m_HandleMagicGestures)
			{
				// MAGIC PAUSE GESTURE DETECTED
				// Notify Listeners 
				if (m_OnPauseToggleCallbacks != null)
					m_OnPauseToggleCallbacks();
			}
		}

	}

	//////////////////////////////////////////////////////////////////////////

	void UpdateThreeFingerTapDetection()
	{
		if (m_WaitingForThreeFingerTap /*&& Input.touchCount == 0*/)
		{
			if (Time.time > m_TripleTap_LastTapTime + m_DoubleTapTime)
			{
				m_WaitingForThreeFingerTap = false;

				//m_DebugOutputLabel.text = "Three finger touch ABORTED - Count: " + m_TripleTap_Count;

				// Notify three-finger single tap callbacks - this can no longer become a Triple Tap
				if (m_TripleTap_Count == 1)
				{
					if (m_IsEnabled)
						if (m_OnThreeFingerSingleTapCallbacks != null)
							m_OnThreeFingerSingleTapCallbacks();
				}
				else if (m_TripleTap_Count == 2)
				{
					if (m_IsEnabled)
						if (m_OnThreeFingerDoubleTapCallbacks != null)
							m_OnThreeFingerDoubleTapCallbacks();
				}

				m_TripleTap_Count = 0;
			}
		}

		bool tripleThreeTapFound = false;
		if (m_TripleTap_TouchCountHelper < 3 && Input.touchCount == 3)
		{
			if (Time.time < m_TripleTap_LastTapTime + m_DoubleTapTime)
			{
				m_TripleTap_Count += 1;

				if (m_TripleTap_Count == 3)
				{
					m_WaitingForThreeFingerTap = false;
					tripleThreeTapFound = true;
				}
			}
			else
			{
				m_WaitingForThreeFingerTap = true;
				m_TripleTap_Count = 1;
			}
			m_TripleTap_LastTapTime = Time.time;
			m_DebugOutputLabel.text = "Three finger touch - Count: " + m_TripleTap_Count;
		}

		m_TripleTap_TouchCountHelper = Input.touchCount;

		if (tripleThreeTapFound)
		{
			m_DebugOutputLabel.text = "Three finger triple tap detected.";

			m_TripleTap_LastTapTime = -1.0f;
			m_TripleTap_Count = 0;

			if (m_HandleMagicGestures)
			{
				ToggleAccessibility();
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////
	
	static public void ToggleAccessibility()
	{
		Initialize();

		instance.ToggleAccessibility_Internal();
	}


	private void ToggleAccessibility_Internal()
	{
		bool wasEnabled = IsEnabled();
		if (!wasEnabled)
			instance.Say_Internal(m_Phrase_EnablingAccessibility, false, true, UAP_AudioQueue.EInterrupt.All);

		EnableAccessibility(!wasEnabled);

		if (wasEnabled)
			Say_Internal(m_Phrase_DisabledAccessibility, false, true, UAP_AudioQueue.EInterrupt.All);
	}

	//////////////////////////////////////////////////////////////////////////

	// TODO: Detect double finger tap for pause/unpause event, one finger swipe right and left
	// TODO: detect the two finger Z scrub for "Back"
	// TODO: reading from the top of the page - stoppable

	//////////////////////////////////////////////////////////////////////////

	void Update()
	{
		UpdateContainerActivations();

		// Enable/Disable shortcut detection runs even when accessibility is disabled
		UpdateThreeFingerTapDetection();

#if UNITY_IOS && !UNITY_EDITOR
		// iOS VoiceOver issue
		m_ReCheckVoiceOverTimer -= Time.unscaledDeltaTime;
		if (m_ReCheckVoiceOverTimer < 0.0f)
		{
			iOSTTS.VoiceOverDetected();
			m_ReCheckVoiceOverTimer = 1.0f;
		}
#endif

		if (!m_IsEnabled)
			return;


		// Magic Gestures (Pause) should be handled in paused mode as well
		if (m_HandleMagicGestures)
			HandlePauseGestures();


		if (m_Paused)
			return;

#if UNITY_STANDALONE || UNITY_EDITOR
		// Windows applications might not want mouse swipes
		if (m_WindowsUseMouseSwipes)
#endif
		{
			UpdateSwipeDetection();
		}


		// Detect if the user is touching the screen in any way. If so, stop
		if (m_ContinuousReading && !m_ContinuousReading_WaitInputClear)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			// Any mouse or key press cancels continuous reading
			if (Input.anyKey)
#else
			if (Input.touchCount > 0)
#endif
			{
				StopContinuousReading();
			}
		}

		// Don't interrupt continuous reading because the user didn't lift their
		// finger fast enough. Wait for all input to clear first
		if (m_ContinuousReading_WaitInputClear)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			// Any mouse or key press cancels continuous reading
			if (!Input.anyKey)
#else
			if (!(Input.touchCount > 0))
#endif
				m_ContinuousReading_WaitInputClear = false;
		}

		if (HandleUI())
		{
			if (m_ContinuousReading)
				UpdateContinuousReading();

			// Make sure that the Unity UI doesn't mess with our focus.
			// This can cause issue on Windows.
			if (!m_CurrentElementHasSoleFocus && EventSystem.current != null)
				EventSystem.current.SetSelectedGameObject(null);

			if (m_CurrentElementHasSoleFocus)
			{
				if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ETextEdit)
				{
					// On mobile devices, closing the on-screen keyboard should end the text input
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
				if (!TouchScreenKeyboard.visible)
				{
					m_CurrentItem.m_Object.InteractEnd();
					LeaveFocussedItem();
				}
#endif
				}
			}

			if (m_CurrentItem != null)
			{
				UpdateElementFrame(ref m_CurrentItem);
			}
			else
			{
				HideElementFrame();
			}

#if UNITY_STANDALONE || UNITY_EDITOR
			// Windows applications might not want mouse swipes
			if (m_WindowsUseMouseSwipes)
#endif
			{
				UpdateDoubleTapDetection();
			}

			if (m_TouchWaitTimeout < 0.0f)
			{
				// Don't do explore by touch if the current element has sole focus
				if (m_ExploreByTouch && !m_CurrentElementHasSoleFocus)
					UpdateExploreByTouch();
			}
			else
				m_TouchWaitTimeout -= Time.deltaTime;
		}

#if UNITY_STANDALONE || UNITY_EDITOR
		if (m_WindowsUseKeys)
			UpdateKeyboardInput();
#endif

	}

	//////////////////////////////////////////////////////////////////////////

	private void UpdateContinuousReading()
	{
		// Check if audio queue is empty
		if (m_AudioQueue.IsCompletelyEmpty())
		{
			// Increase current item and check if there are any elements left to read
			// Store index, just in case this is the last item on the screen
			//int prevIndex = m_ActiveContainers[m_ActiveContainerIndex].GetCurrentElementIndex();
			int activeContainerIdx = m_ActiveContainerIndex;
			int prevContainerIdx = m_ActiveContainerIndex;
			bool sameContainer = m_ActiveContainers[activeContainerIdx].IncrementCurrentItem(false);
			if (!sameContainer)
			{
				// Is this the last container?
				if (activeContainerIdx == m_ActiveContainers.Count - 1)
				{
					StopContinuousReading();
				}
				else
				{
					// Jump to the next container and jump to start (but careful, in case that one contains no active elements)
					bool invalid = true;
					do
					{
						++activeContainerIdx;
						if (activeContainerIdx < m_ActiveContainers.Count)
						{
							m_ActiveContainers[activeContainerIdx].JumpToFirst();
							invalid = m_ActiveContainers[activeContainerIdx].GetCurrentElementIndex() < 0;
						}
						else
						{
							invalid = false;
						}
					} while (invalid);
					// Check that the newly found entry is valid
					if (activeContainerIdx == m_ActiveContainers.Count - 1 && m_ActiveContainers[activeContainerIdx].GetCurrentElementIndex() < 0)
					{
						StopContinuousReading();
					}
					else
					{
						// Copy the value
						m_ActiveContainerIndex = activeContainerIdx;
					}
				}
			}

			if (m_ContinuousReading)
			{
				m_CurrentItem = m_ActiveContainers[m_ActiveContainerIndex].GetCurrentElement(false);

				// Read Container Name
				if (prevContainerIdx != m_ActiveContainerIndex)
					ReadContainerName();

				// Read current item
				UpdateCurrentItem();
				ReadItem(m_CurrentItem, true);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void HandlePauseGestures()
	{
		// Pause on Android and iOS
#if (UNITY_ANDROID || UNITY_IOS)
		UpdateMagicTapDetection();
#endif


		// Pause on Windows
		if (m_PauseKey != KeyCode.None && Input.GetKeyDown(m_PauseKey))
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			// Notify Listeners 
			if (m_OnPauseToggleCallbacks != null)
				m_OnPauseToggleCallbacks();
#endif
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void UpdateExploreByTouch()
	{
		if (!HandleUI())
		{
			CancelExploreByTouch();
			m_ExploreByTouch_SingleTapWaitTimer = -1.0f;
			return;
		}

		bool updateExploreByTouch = true;
#if UNITY_STANDALONE || UNITY_EDITOR
		if (!m_WindowsUseExploreByTouch)
			updateExploreByTouch = false;
#else
		if (Input.touchCount > 1)
			updateExploreByTouch = false;
#endif
		if (!updateExploreByTouch)
		{
			CancelExploreByTouch();
			m_ExploreByTouch_SingleTapWaitTimer = -1.0f;
			return;
		}

		bool isTouching = false;
#if UNITY_STANDALONE || UNITY_EDITOR
		if (Input.GetMouseButton(0))
			isTouching = true;
#else
		isTouching = Input.touchCount == 1;
#endif

		if (m_ExploreByTouch_IsActive)
		{
			if (!isTouching)
			{
				// Explore by Touch was active, but apparently the finger was lifted
				CancelExploreByTouch();
				m_ExploreByTouch_SingleTapWaitTimer = -1.0f;
			}
			else
			{
				//m_DebugOutputLabel.text = "Explore by Touch";
				// Explore By Touch While finger is down
				// **********************************
				// Detect what element is under the user's finger
				Vector3 fingerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				m_DebugOutputLabel.text = "Explore by Touch - Drag active";
				ExploreBytTouch_SelectElementUnderFinger(fingerPos);
				return;
			}
		}
		else //m_ExploreByTouch_IsActive is false
		{
			// Handle Single Taps
			if (isTouching)
			{
				if (m_ExploreByTouch_SingleTapWaitTimer > 0.0f)
				{
					m_ExploreByTouch_SingleTapWaitTimer -= Time.deltaTime;
				}
				else
				{
					// Only start the timer if the touch occurred this frame
					// (i.e. it should not be restarted once it is up)
					if (Input.GetMouseButtonDown(0))
					{
						m_ExploreByTouch_SingleTapWaitTimer = m_DoubleTapTime;
						m_ExploreByTouch_SingleTapStartPosition = Input.mousePosition;
					}
				}
			}
			else
			{
				if (m_ExploreByTouch_SingleTapWaitTimer > 0.0f)
				{
					m_ExploreByTouch_SingleTapWaitTimer -= Time.deltaTime;
					if (m_ExploreByTouch_SingleTapWaitTimer < 0.0f)
					{
						// Only if there was no swipe! Finger pos should be close to start pos
						Vector3 delta = Input.mousePosition - m_ExploreByTouch_SingleTapStartPosition;
						float maxDelta = Screen.dpi * 0.2f; // finger should not have moved more than a fifth of an inch
						// account for those cases where dpi couldn't be found with a best guess
						if (maxDelta == 0.0f)
							maxDelta = (0.5f * (Screen.width + Screen.height)) * 0.1f; // not more than a tenth of the screen

						if (delta.sqrMagnitude < (maxDelta * maxDelta))
						{
							m_DebugOutputLabel.text = "Explore by Touch Single Tap \nFinger Delta: " + delta.magnitude.ToString("000.00000") + " on a screen with " + Screen.dpi + " dpi. (Start: " + m_ExploreByTouch_StartPosition + " End: " + Input.mousePosition + ")";
							// No double tap incoming, and finger barely moved
							// Single Tap! Select current element
							Vector3 fingerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
							m_CurrentItem = null;
							ExploreBytTouch_SelectElementUnderFinger(fingerPos);
						}
					}
				}
			}

			// Touch Explore is not yet active
			if (m_ExploreByTouch_WaitTimer < 0.0f)
			{
				// Start the wait time and save the position
				m_ExploreByTouch_WaitTimer = m_DoubleTapTime; // m_ExploreByTouchDelay;
				m_ExploreByTouch_StartPosition = Input.mousePosition;
			}
			else
			{
				// Was the finger lifted?
				if (!isTouching)
				{
					CancelExploreByTouch();
				}
				else
				{
					m_ExploreByTouch_WaitTimer -= Time.deltaTime;
					if (m_ExploreByTouch_WaitTimer < 0.0f)
					{
						// Stop waiting for a single tap
						m_ExploreByTouch_SingleTapWaitTimer = -1.0f;

						// Check that there was no fast movement
						Vector2 deltaPos = Input.mousePosition - m_ExploreByTouch_StartPosition;
						float distance = deltaPos.magnitude;
						float ratio = distance / m_DoubleTapTime; // m_ExploreByTouchDelay;
						if (ratio < 600.0f)
						{
							m_DebugOutputLabel.text = "Explore by Touch. Ratio: " + ratio.ToString("0.#####") + " distance: " + distance.ToString("0.####");
							m_ExploreByTouch_IsActive = true;
							m_CurrentItem = null;
							Vector3 fingerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
							ExploreBytTouch_SelectElementUnderFinger(fingerPos);
						}
						else
						{
							CancelExploreByTouch();
						}
					}
				}

			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void ExploreBytTouch_SelectElementUnderFinger(Vector3 fingerPos)
	{
		bool elementFound = false;
		// Test current item first
		// Don't re-select what's already selected
		if (m_CurrentItem != null && IsPositionOverElement(fingerPos, m_CurrentItem))
			return;

		// 				if (m_CurrentItem != null)
		// 					Debug.Log("current item is " + m_CurrentItem.m_Object.name);

		//Debug.Log("Finger Pos is " + fingerPos);
		for (int c = 0; c < m_ActiveContainers.Count; ++c)
		{
			List<AccessibleUIGroupRoot.Accessible_UIElement> elements = m_ActiveContainers[c].GetElements();
			for (int e = 0; e < elements.Count; ++e)
			{
				if (!elements[e].m_Object.IsElementActive())
					continue;

				bool isOver = IsPositionOverElement(fingerPos, elements[e]);
				if (isOver)
				{
					elementFound = true;

					// Does this container allow explore by touch?
					if (m_ActiveContainers[c].m_AllowTouchExplore)
					{
						// Select this element, unless this IS the one currently selected
						m_CurrentItem = elements[e];
						m_ActiveContainerIndex = c;
						m_ActiveContainers[c].SetActiveElementIndex(e, m_CyclicMenus);
						UpdateCurrentItem();
						ReadItem(m_CurrentItem);

						//Debug.Log("Element under finger is " + elements[e].m_Object.name);
					}
					else
					{
						// If not, proceed depending on whether this container was already active or not
						if (m_ActiveContainerIndex == c)
						{
							// Do nothing, container is already active
						}
						else
						{
							// Make this container active, read it's name (if any) and select the current element
							m_ActiveContainerIndex = c;
							ReadContainerName();
							m_CurrentItem = m_ActiveContainers[c].GetCurrentElement(m_CyclicMenus);
							UpdateCurrentItem();
							ReadItem(m_CurrentItem);
						}
					}

					break;
				}
			}
			if (elementFound)
				break;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void ReadContainerName()
	{
		if (!m_IsEnabled)
			return;

		string containerName = m_ActiveContainers[m_ActiveContainerIndex].GetContainerName();
		if (containerName.Length > 0)
			SayAudio(null, containerName, UAP_AudioQueue.EAudioType.Container_Name, true);
	}

	//////////////////////////////////////////////////////////////////////////

	private void CancelExploreByTouch()
	{
		m_ExploreByTouch_IsActive = false;
		m_ExploreByTouch_WaitTimer = -1.0f;
	}

	//////////////////////////////////////////////////////////////////////////

	void UpdateDoubleTapDetection()
	{
		m_DoubleTapFoundThisFrame = false;

		// This will work on touch as well
		if (Input.GetMouseButtonDown(0) && Input.touchCount == 1)
		{
			if (Time.time < m_DoubleTap_LastTapTime + m_DoubleTapTime)
			{
				// Ignore if the finger count changed
				m_DoubleTapFoundThisFrame = true;
			}
			m_DoubleTap_LastTapTime = Time.time;
		}

		if (m_DoubleTapFoundThisFrame)
		{
			if (m_CurrentItem == null)
				return;

			// Prevent Explore by touch for a moment
			m_TouchWaitTimeout = 0.5f;
			CancelExploreByTouch();
			m_ExploreByTouch_SingleTapWaitTimer = -1.0f;

			// Normal, one finger double tap
			if (m_CurrentElementHasSoleFocus)
			{
				m_CurrentItem.m_Object.InteractEnd();
				LeaveFocussedItem();
			}
			else
			{
				if (m_CurrentItem.m_Object.IsInteractable())
					InteractWithElement(m_CurrentItem);
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public static bool GetSpeakDisabledInteractables()
	{
		Initialize();

		return instance.m_ReadDisabledInteractables;
	}

	//////////////////////////////////////////////////////////////////////////

	private void LeaveFocussedItem()
	{
		PlaySFX(m_UIFocusLeave);
		m_CurrentElementHasSoleFocus = false;
		Say_Internal("");

		if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ETextEdit)
		{
			ReadValue(false, true); // Repeat the text that was just typed in
		}
		else if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EDropDown)
		{
			// DropDowns are special
			// "TT selected"
			string toSay = m_Phrase_DropdownItemSelected;
			string valueText = m_CurrentItem.m_Object.GetCurrentValueAsText();
			toSay = toSay.Replace("TT", valueText);
			SayAudio(null, toSay, UAP_AudioQueue.EAudioType.Element_Text, m_CurrentItem.m_Object.m_AllowVoiceOver, UAP_AudioQueue.EInterrupt.Elements);
		}

	}

	//////////////////////////////////////////////////////////////////////////

	void CancelFocussedItem()
	{
		PlaySFX(m_UIFocusLeave);
		m_CurrentItem.m_Object.InteractAbort();
		// Stop speech
		Say_Internal("");
		m_CurrentElementHasSoleFocus = false;
	}

	//////////////////////////////////////////////////////////////////////////

	private bool IsActiveContainer2DNavigation()
	{
		return (m_ActiveContainerIndex >= 0 && m_ActiveContainers[m_ActiveContainerIndex].m_2DNavigation);
	}

	////////////////////////////////////////////////////////////////////////// 
	//////////////////////////////////////////////////////////////////////////

	void UpdateKeyboardInput()
	{
		if (m_CurrentElementHasSoleFocus)
		{
			// Interact with element (slider up / down etc)
			if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ESlider)
			{
				bool readValue = false;
				if (Input.GetKeyDown(m_SliderIncrementKey))
				{
					(m_CurrentItem.m_Object).Increment();
					readValue = true;
				}
				else if (Input.GetKeyDown(m_SliderDecrementKey))
				{
					(m_CurrentItem.m_Object).Decrement();
					readValue = true;
				}
				if (readValue)
				{
					ReadValue(false, true);
				}
			}
			else if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EDropDown)
			{
				bool readValue = false;
				bool boundReached = false;
				if (Input.GetKeyDown(m_DropDownNextKey))
				{
					if (!(m_CurrentItem.m_Object).Increment())
						boundReached = true;
					readValue = true;
				}
				else if (Input.GetKeyDown(m_DropDownPreviousKey))
				{
					if (!(m_CurrentItem.m_Object).Decrement())
						boundReached = true;
					readValue = true;
				}
				if (readValue)
				{
					ReadValue(false, true);
					ReadDropdownItemIndex(ref m_CurrentItem);
					// TODO: Queue delayed hint on how to accept the slider value

				}
				if (boundReached)
				{
					PlaySFX(m_UIBoundsReached);
				}
			}

			if (Input.GetKeyDown(m_InteractKey))
			{
				m_CurrentItem.m_Object.InteractEnd();
				LeaveFocussedItem();
			}

			if (Input.GetKeyDown(m_AbortKey))
			{
				CancelFocussedItem();
			}

		}
		else
		{
			if (HandleUI())
			{
				// If the current container wants 2D navigation, don't 
				// handle the regular left and right swipes
				if (IsActiveContainer2DNavigation())
				{
					//Debug.Log("Handling 2D Navigation");
					if (Input.GetKeyDown(m_DownKey))
						Navigate2DUIElement(ESDirection.EDown);
					if (Input.GetKeyDown(m_UpKey))
						Navigate2DUIElement(ESDirection.EUp);
					if (Input.GetKeyDown(m_RightKey))
						Navigate2DUIElement(ESDirection.ERight);
					if (Input.GetKeyDown(m_LeftKey))
						Navigate2DUIElement(ESDirection.ELeft);

					if (Input.GetKeyDown(m_DownKey) || Input.GetKeyDown(m_UpKey) || Input.GetKeyDown(m_RightKey) || Input.GetKeyDown(m_LeftKey))
					{
						//Debug.Log("Navigated");
						return;
					}
				}
				else
				{
					// Arrow keys to navigate
					if (Input.GetKeyDown(m_NextElementKey))
					{
						IncrementUIElement();
						return;
					}
					if (Input.GetKeyDown(m_PreviousElementKey))
					{
						DecrementUIElement();
						return;
					}
					if (Input.GetKeyDown(m_PreviousContainerKey) || (m_UseTabAndShiftTabForContainerJumping && Input.GetKeyDown(KeyCode.Tab) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
					{
						int currentContainerIndex = m_ActiveContainerIndex;
						if (DecrementContainer(true))
						{
							UpdateCurrentItem();
							PlaySFX(m_UINavigationClick);
						}
						else
						{
							PlaySFX(m_UIBoundsReached);
						}
						if (currentContainerIndex != m_ActiveContainerIndex)
							ReadContainerName();
						ReadItem(m_CurrentItem);
						return;
					}
					if (Input.GetKeyDown(m_NextContainerKey) || (m_UseTabAndShiftTabForContainerJumping && Input.GetKeyDown(KeyCode.Tab)))
					{
						int currentContainerIndex = m_ActiveContainerIndex;
						if (IncrementContainer(true))
						{
							UpdateCurrentItem();
							PlaySFX(m_UINavigationClick);
						}
						else
						{
							PlaySFX(m_UIBoundsReached);
						}
						if (currentContainerIndex != m_ActiveContainerIndex)
							ReadContainerName();
						ReadItem(m_CurrentItem);
						return;
					}
				}

				if (Input.GetKeyDown(m_InteractKey))
				{
					// Interact with current element
					InteractWithElement(m_CurrentItem);
				}
			}

			// TODO: Backspace to go Back/Cancel, Escape for Pause
		}

	}

	//////////////////////////////////////////////////////////////////////////

	void InteractWithElement(AccessibleUIGroupRoot.Accessible_UIElement item)
	{
		if (item == null)
			return;

		if (!item.m_Object.IsInteractable())
			return;

		// This will interrupt the type, hint etc reading of the current element
		Say_Internal("");

		switch (item.m_Type)
		{
			case AccessibleUIGroupRoot.EUIElement.EButton:
				PlaySFX(m_UIInteract);
				item.m_Object.Interact();
				break;
			case AccessibleUIGroupRoot.EUIElement.EToggle:
				PlaySFX(m_UIInteract);
				item.m_Object.Interact();
				// After something was toggled, repeat the item
				ReadItem(m_CurrentItem);
				break;
			case AccessibleUIGroupRoot.EUIElement.ESlider:
				// Sliders are special, they need the full focus
				PlaySFX(m_UIFocusEnter);
				item.m_Object.Interact();
				m_CurrentElementHasSoleFocus = true;
				// TODO: Queue delayed focus hint on how to change the slider value
				break;
			case AccessibleUIGroupRoot.EUIElement.ETextEdit:
				// Text Edit Fields are VERY special, they need the full focus
				PlaySFX(m_UIFocusEnter);
				item.m_Object.Interact();
				m_CurrentElementHasSoleFocus = true;
				// TODO: Queue delayed focus hint on how to work the edit field
				break;
			case AccessibleUIGroupRoot.EUIElement.EDropDown:
				// DropDowns are special, they need the full focus
				PlaySFX(m_UIFocusEnter);
				item.m_Object.Interact();
				m_CurrentElementHasSoleFocus = true;
				ReadValue(false, true);
				ReadDropdownItemIndex(ref item);
				// TODO: Queue delayed focus hint on how to change the slider value

				break;

		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void ReadDropdownItemIndex(ref AccessibleUIGroupRoot.Accessible_UIElement item)
	{
		// Read out "Element X of Y selected"
		int itemCount = ((AccessibleDropdown)item.m_Object).GetItemCount();
		int selectedIndex = ((AccessibleDropdown)item.m_Object).GetSelectedItemIndex();

		// Build string depending on setting
		// "Item XX of YY"
		string toSay = m_Phrase_DropdownItemIndex;
		toSay = toSay.Replace("XX", selectedIndex.ToString("0"));
		toSay = toSay.Replace("YY", itemCount.ToString("0"));

		SayPause(m_TypeDelay);
		SayAudio(null, toSay, UAP_AudioQueue.EAudioType.Element_Text, m_CurrentItem.m_Object.m_AllowVoiceOver);
	}

	//////////////////////////////////////////////////////////////////////////

	private void UpdateContainerActivations()
	{
		bool readCurrentItem = m_ReadItemNextUpdate;
		m_ReadItemNextUpdate = false;
		if (m_ContainersToActivate.Count > 0)
		{
			readCurrentItem = true;
			foreach (AccessibleUIGroupRoot container in m_ContainersToActivate)
				ActivateContainer_Internal(container, true, false);

			//Debug.Log("Activated " + m_ContainersToActivate.Count + " containers");

			m_ContainersToActivate.Clear();
		}

		UpdateCurrentItem();
		if (readCurrentItem)
			ReadItem(m_CurrentItem);
	}

	//////////////////////////////////////////////////////////////////////////

	void CancelTaps()
	{
		// Cancel three finger taps
		m_TripleTap_TouchCountHelper = 0;
		m_TripleTap_LastTapTime = -1.0f;
		m_WaitingForThreeFingerTap = false;
		m_TripleTap_Count = 0;

		// Cancel double taps
		m_ExploreByTouch_SingleTapWaitTimer = -1.0f;
		m_DoubleTap_LastTapTime = -1;

		// Cancel Magic Taps (two finger double tap)
		m_MagicTap_TouchCountHelper = 0;
		m_MagicTap_LastTapTime = -1.0f;
		m_WaitingForMagicTap = false;
	}

	//////////////////////////////////////////////////////////////////////////

	public void OnSwipe(ESDirection dir, int fingerCount)
	{
		CancelExploreByTouch();
		CancelTaps();

		if (m_CurrentElementHasSoleFocus)
		{
			if (fingerCount == 1)
			{
				// Update Slider etc
				if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.ESlider)
				{
					bool readValue = false;
					if (dir == ESDirection.EUp)
					{
						((AccessibleSlider)m_CurrentItem.m_Object).Increment();
						readValue = true;
					}
					else if (dir == ESDirection.EDown)
					{
						((AccessibleSlider)m_CurrentItem.m_Object).Decrement();
						readValue = true;
					}
					if (readValue)
					{
						ReadValue(false, true);
					}
				}
				else if (m_CurrentItem.m_Type == AccessibleUIGroupRoot.EUIElement.EDropDown)
				{
					bool readValue = false;
					bool boundReached = false;
					if (dir == ESDirection.EDown || dir == ESDirection.ERight)
					{
						if (!(m_CurrentItem.m_Object).Increment())
							boundReached = true;
						readValue = true;
					}
					else if (dir == ESDirection.EUp || dir == ESDirection.ELeft)
					{
						if (!(m_CurrentItem.m_Object).Decrement())
							boundReached = true;
						readValue = true;
					}
					if (readValue)
					{
						ReadValue(false, true);
						ReadDropdownItemIndex(ref m_CurrentItem);
						// TODO: Queue delayed hint on how to accept the slider value

					}
					if (boundReached)
					{
						PlaySFX(m_UIBoundsReached);
					}
				}
			} // finger count == 1
		}
		else // element does NOT have sole focus
		{
			if (fingerCount == 1)
			{
#if UNITY_ANDROID && !UNITY_EDITOR
				// Android uses up and down like left and right
				if (m_AndroidUseUpAndDownForElements)
				{
					if (dir == ESDirection.EDown)
						dir = ESDirection.ERight;
					if (dir == ESDirection.EUp)
						dir = ESDirection.ELeft;
				}
#endif

				if (HandleUI())
				{
					if (IsActiveContainer2DNavigation())
					{
						Navigate2DUIElement(dir);
					}
					else
					{
						if (dir == ESDirection.ERight)
						{
							IncrementUIElement();
						}
						else if (dir == ESDirection.ELeft)
						{
							DecrementUIElement();
						}
						else if (dir == ESDirection.EDown)
						{
							int currentContainerIndex = m_ActiveContainerIndex;
							if (IncrementContainer(true))
							{
								UpdateCurrentItem();
								PlaySFX(m_UINavigationClick);
							}
							else
							{
								PlaySFX(m_UIBoundsReached);
							}
							if (currentContainerIndex != m_ActiveContainerIndex)
								ReadContainerName();
							ReadItem(m_CurrentItem);
						}
						else if (dir == ESDirection.EUp)
						{
							int currentContainerIndex = m_ActiveContainerIndex;
							if (DecrementContainer(true))
							{
								UpdateCurrentItem();
								PlaySFX(m_UINavigationClick);
							}
							else
							{
								PlaySFX(m_UIBoundsReached);
							}
							if (currentContainerIndex != m_ActiveContainerIndex)
								ReadContainerName();
							ReadItem(m_CurrentItem);
						}
					} // if 2D Navigation or not

				} // if HandleUI()
			} // if finger count == single finger
			else if (fingerCount == 2)
			{
				// Use callback handler overrides or default handling
				if (dir == ESDirection.EUp)
				{
					if (m_OnTwoFingerSwipeUpHandler != null)
					{
						m_OnTwoFingerSwipeUpHandler();
					}
					else
					{
						// Read From Top
						StartReadingfromTop();
					}
				}
				else if (dir == ESDirection.EDown)
				{
					if (m_OnTwoFingerSwipeDownHandler != null)
					{
						m_OnTwoFingerSwipeDownHandler();
					}
					else
					{
						// Read From Current Element
						StartReadingFromCurrentElement();
					}
				}
			}
			else if (fingerCount == 3)
			{
				if (dir == ESDirection.EUp)
				{
					if (HandleUI())
					{
						// TODO: Callback?
					}
				}
			}
		}


	}

	//////////////////////////////////////////////////////////////////////////

	private void StartReadingfromTop()
	{
		if (HandleUI() && m_ActiveContainers.Count > 0)
		{
			int prevContainerIdx = m_ActiveContainerIndex;
			m_ActiveContainerIndex = 0;
			m_ActiveContainers[m_ActiveContainerIndex].JumpToFirst();
			UpdateCurrentItem();
			if (m_CurrentItem != null)
			{
				if (prevContainerIdx != 0)
					ReadContainerName();
				ReadItem(m_CurrentItem, true);
			}
			m_ContinuousReading = true;
			m_ContinuousReading_WaitInputClear = true;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Starts automatically reading everything on screen, starting with the selected item.
	/// </summary>
	public static void ReadFromCurrent()
	{
			// IsEnabled calls Initialize()
		if (!IsEnabled())
			return;

		instance.StartReadingFromCurrentElement();
	}

	private void StartReadingFromCurrentElement()
	{
		if (HandleUI() && m_ActiveContainers.Count > 0)
		{
			UpdateCurrentItem();
			if (m_CurrentItem != null)
				ReadItem(m_CurrentItem, true);
			m_ContinuousReading = true;
			m_ContinuousReading_WaitInputClear = true;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Starts automatically reading everything on screen, starting with the first item.
	/// The first item is determined as the top-most item (or the item with the smallest manual traversal order)
	/// in the first UI Group (with the highest priority).
	/// </summary>
	public static void ReadFromTop()
	{
		// IsEnabled calls Initialize()
		if (!IsEnabled())
			return;

		instance.StartReadingfromTop();
	}

	private void Navigate2DUIElement(ESDirection direction)
	{
		if (!HandleUI())
			return;

		UpdateCurrentItem();

		//Debug.Log("Navigating " + direction.ToString());

		if (m_ActiveContainerIndex >= 0)
		{
			//Debug.Log("Asking container to move focus");
			bool sameScreen = m_ActiveContainers[m_ActiveContainerIndex].MoveFocus2D(direction);
			//Debug.Log("Container moved focus and returned " + sameScreen);

			int currentContainerIndex = m_ActiveContainerIndex;

			// If the edge of the container was hit, the behavior
			// is determined by the setting on that container.
			// Either jump to the next container, or stop at the edge
			bool playNavClickSFX = true;
			if (!sameScreen)
			{
				if (!m_ActiveContainers[m_ActiveContainerIndex].IsConstrainedToContainer(direction) && m_ActiveContainers.Count > 1)
				{
					if (IncrementContainer())
						m_ActiveContainers[m_ActiveContainerIndex].JumpToFirst();
					else
						PlaySFX(m_UIBoundsReached);
				}
				else
				{
					// Play ding sound to let the user know this is the end of navigation
					PlaySFX(m_UIBoundsReached);
					playNavClickSFX = false;
				}
			}

			UpdateCurrentItem();
			if (playNavClickSFX)
				PlaySFX(m_UINavigationClick);

			if (currentContainerIndex != m_ActiveContainerIndex)
				ReadContainerName();

			ReadItem(m_CurrentItem);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void DecrementUIElement()
	{
		if (!HandleUI())
			return;

		//Debug.Log("Jumping to previous UI element");
		AccessibleUIGroupRoot.Accessible_UIElement prevItem = m_CurrentItem;
		int currentContainerIndex = m_ActiveContainerIndex;

		UpdateCurrentItem();
		if (m_ActiveContainerIndex >= 0)
		{
			bool sameScreen = m_ActiveContainers[m_ActiveContainerIndex].DecrementCurrentItem(m_CyclicMenus);

			// Handle Rollovers, jump to the next container
			if (!sameScreen && m_ActiveContainers.Count > 1)
			{
				if (DecrementContainer())
					m_ActiveContainers[m_ActiveContainerIndex].JumpToLast();
			}

			UpdateCurrentItem();

			if (prevItem != m_CurrentItem)
				PlaySFX(m_UINavigationClick);
			else
				PlaySFX(m_UIBoundsReached);

			if (currentContainerIndex != m_ActiveContainerIndex)
				ReadContainerName();

			ReadItem(m_CurrentItem);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private bool DecrementContainer(bool resetToStartItem = false)
	{
		if (!HandleUI())
			return false;

		if (m_ActiveContainerIndex > 0 || m_CyclicMenus)
		{
			--m_ActiveContainerIndex;
			if (m_ActiveContainerIndex < 0)
				m_ActiveContainerIndex = m_ActiveContainers.Count - 1;
		}
		else
		{
			// No rollover
			return false;
		}

		if (resetToStartItem)
			if (m_ActiveContainers.Count > 0)
				m_ActiveContainers[m_ActiveContainerIndex].ResetToStart();

		return true;
	}

	//////////////////////////////////////////////////////////////////////////

	private void IncrementUIElement()
	{
		if (!HandleUI())
			return;

		//Debug.Log("Jumping to next UI element");

		AccessibleUIGroupRoot.Accessible_UIElement prevItem = m_CurrentItem;
		int currentContainerIndex = m_ActiveContainerIndex;

		UpdateCurrentItem();
		if (m_ActiveContainerIndex >= 0)
		{
			bool sameScreen = m_ActiveContainers[m_ActiveContainerIndex].IncrementCurrentItem(m_CyclicMenus);

			// Handle Rollovers, jump to the next container
			if (!sameScreen && m_ActiveContainers.Count > 1)
			{
				if (IncrementContainer())
					m_ActiveContainers[m_ActiveContainerIndex].JumpToFirst();
			}

			UpdateCurrentItem();

			if (prevItem != m_CurrentItem)
				PlaySFX(m_UINavigationClick);
			else
				PlaySFX(m_UIBoundsReached);

			if (currentContainerIndex != m_ActiveContainerIndex)
				ReadContainerName();

			ReadItem(m_CurrentItem);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	void PlaySFX(AudioClip clip)
	{
		if (clip == null || m_SFXPlayer == null)
			return;

		m_SFXPlayer.PlayOneShot(clip, 0.8f);
	}

	//////////////////////////////////////////////////////////////////////////

	private bool IncrementContainer(bool resetToStartItem = false)
	{
		if (!HandleUI())
			return false;

		//Debug.Log("Jumping to next UI container");
		if (m_ActiveContainerIndex < m_ActiveContainers.Count - 1 || m_CyclicMenus)
		{
			++m_ActiveContainerIndex;
			if (m_ActiveContainerIndex >= m_ActiveContainers.Count)
				m_ActiveContainerIndex = 0;
		}
		else
		{
			// No rollover
			return false;
		}

		if (resetToStartItem)
			if (m_ActiveContainers.Count > 0)
				m_ActiveContainers[m_ActiveContainerIndex].ResetToStart();

		return true;
	}

	//////////////////////////////////////////////////////////////////////////

	void UpdateSwipeDetection()
	{
		// No swiping if ExploreByTouch is active
		if (m_ExploreByTouch_IsActive)
			return;

		if (!m_SwipeActive)
		{
			if (m_SwipeWaitForLift)
			{
#if UNITY_STANDALONE || UNITY_EDITOR
				if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
#else
				if (Input.touchCount == 0)
#endif
					m_SwipeWaitForLift = false;
			}
#if UNITY_STANDALONE || UNITY_EDITOR
			else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
#else
			else if (Input.touchCount > 0)
#endif
			{
				m_SwipeActive = true;
				m_SwipeStartPos = new Vector2(0, 0);
				m_SwipeDeltaTime = 0.0f;
#if UNITY_STANDALONE || UNITY_EDITOR
				m_SwipeTouchCount = Input.GetMouseButton(1) ? 2 : 1;
				m_SwipeStartPos = Input.mousePosition;
#else
				m_SwipeTouchCount = Input.touchCount;
				// Calculate the average finger position
				if (Input.touchCount > 0)
				{
					foreach (Touch t in Input.touches)
						m_SwipeStartPos += t.position;
					m_SwipeStartPos /= (float)Input.touchCount;
				}
#endif
			}
		}
		else
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
#else
			if (Input.touchCount < m_SwipeTouchCount)
#endif
			{
				// Decide whether this was a valid swipe
				// by time and distance ratio
				Vector2 deltaPos = m_SwipeCurrPos - m_SwipeStartPos;
				float distance = deltaPos.magnitude;
				bool swipeFound = true;

				// Minimum Swipe Time?
				if (m_SwipeDeltaTime < 0.08f)
					swipeFound = false;

				// TODO: Calculate a real life size/pixel ratio
				// Minimum Distance? Should be a 17th of the screen height
				float minDist = Screen.height * (1.0f / 17.0f);
				if (deltaPos.magnitude < minDist)
				{
					m_DebugOutputLabel.text = "No swipe, minimum distance no reached";
					swipeFound = false;
				}

				// Time / Distance Ratio?
				float ratio = distance / m_SwipeDeltaTime;
				if (ratio < 600.0f)
				{
					m_DebugOutputLabel.text = "No swipe, distance/time ratio to small";
					swipeFound = false;
				}

				//Debug.Log("Distance: " + distance + " which is " + (distance / (float)Screen.height) + " ratio of the screen height. (minimum distance: " + minDist + ") Swipe time: " + m_SwipeDeltaTime + " and dist/time ratio of " + ratio);

				if (swipeFound)
				{
					// Determine direction
					ESDirection dir = ESDirection.EUp;
					float distH = m_SwipeCurrPos.x - m_SwipeStartPos.x;
					float distV = m_SwipeCurrPos.y - m_SwipeStartPos.y;
					if (Mathf.Abs(distH) > Mathf.Abs(distV))
					{
						if (distH > 0)
							dir = ESDirection.ERight;
						else
							dir = ESDirection.ELeft;
					}
					else
					{
						if (distV > 0)
							dir = ESDirection.EUp;
						else
							dir = ESDirection.EDown;
					}
					//Debug.Log("Swipe Found: " + dir);

					m_DebugOutputLabel.text = "Swipe found. Direction " + dir + " - Finger Count: " + m_SwipeTouchCount;

					//bool threeFingerUpwardsSwipe = (dir == ESDirection.EUp && m_SwipeTouchCount == 3);
					bool isActive = (!m_Paused && m_IsEnabled && HandleUI());
					if (isActive /*|| threeFingerUpwardsSwipe*/)
					{
#if UNITY_IOS && !UNITY_EDITOR
						// On iOS, UAP uses native swipe detection
						CancelExploreByTouch();
						CancelTaps();
#else
						OnSwipe(dir, m_SwipeTouchCount);
#endif
					}
				}

				m_SwipeActive = false;
				if (m_SwipeTouchCount > 0)
					m_SwipeWaitForLift = true;
				else
					m_SwipeWaitForLift = false;
			}
			else
			{
				// Ongoing swipe, update position
#if UNITY_STANDALONE || UNITY_EDITOR
				m_SwipeCurrPos = Input.mousePosition;
#else
				m_SwipeCurrPos = new Vector2(0, 0);
				if (Input.touchCount > 0)
				{
					foreach (Touch t in Input.touches)
						m_SwipeCurrPos += t.position;
					m_SwipeCurrPos /= (float)Input.touchCount;
				}
				m_SwipeTouchCount = Input.touchCount;
#endif
				m_SwipeDeltaTime += Time.deltaTime;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////


	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Say something using platform independent Text To Speech.
	/// If you disabled TTS in the settings, or TTS is not available on the 
	/// current platform, the call will be ignored.
	/// </summary>
	/// <param name="textToSay">string to speak using Text-to-Speech</param>
	/// <param name="canBeInterrupted">You can prevent interuption and force this line to be finished, 
	/// regardless of what the user does. Generally not recommended.</param>
	/// <param name="allowVoiceOver">iOS only - whether regular VoiceOver (if enabled) should speak the text. 
	/// This should only be set to false if you also offer speech settings.
	/// It can make sense to suppress VoiceOver if you really (really!) 
	/// need to prevent the game audio volume being temporarily lowered for the speech.
	/// Don't touch this otherwise, as blind users much prefer to be able to use their own, 
	/// custom VoiceOver voice and speech settings. (Did I stress this point enough?)</param>
	/// <param name="interrupts">By default, this call will interrupt regular UI announcements, 
	/// but not previous calls to this Say() function from within your own code. Change to ALL to interrupt everything.</param>
	public static void Say(string textToSay, bool canBeInterrupted = true, bool allowVoiceOver = true, UAP_AudioQueue.EInterrupt interrupts = UAP_AudioQueue.EInterrupt.Elements)
	{
		// IsEnabled calls Initialize()
		if (!IsEnabled())
			return;

		instance.Say_Internal(textToSay, canBeInterrupted, allowVoiceOver, interrupts);
	}

	private void Say_Internal(string textToSay, bool canBeInterrupted = true, bool allowVoiceOver = true, UAP_AudioQueue.EInterrupt interrupts = UAP_AudioQueue.EInterrupt.Elements)
	{
		m_AudioQueue.QueueAudio(textToSay, UAP_AudioQueue.EAudioType.App, allowVoiceOver, interrupts, canBeInterrupted);
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This function returns whether there is currently voice audio playing.
	/// This includes Text-To-Speech voice or pre-recorded audio clips, if 
	/// those were provided. It does not include sound effects.<br><br>
	/// Apps can use this if they want to wait for everything to be finished
	/// before triggering more audio. Works especially well in combination
	/// with temporary preventing input via BlockInput during the wait, 
	/// to make sure not further voice output is started.<br><br>
	/// Please note that the function is not 100% exact. Depending on the TTS system,
	/// it isn't always possible to find out whether the voice is still speaking. 
	/// So instead the system estimates an approximate speaking duration.
	/// </summary>
	/// <returns></returns>
	public static bool IsSpeaking()
	{
		Initialize();

		return instance.m_AudioQueue.IsPlaying();
	}

	//////////////////////////////////////////////////////////////////////////

	void OnApplicationPause(bool paused)
	{
		if (!m_IsInitialized)
			return;

		if (paused)
			SavePluginEnabledState();

		StopContinuousReading();

		// Should we turn on accessibility?
		if (!m_IsEnabled && !paused)
		{
			if (m_RecheckAutoEnablingOnResume)
			{
				if (ShouldAutoEnable())
				{
					EnableAccessibility(true);
					if (m_OnAccessibilityModeChanged != null)
						m_OnAccessibilityModeChanged(true);
				}
			}
		}

		if (!m_IsEnabled || m_Paused)
			return;

		if (!paused)
		{
#if UNITY_ANDROID
			if (IsTalkBackEnabledAndTouchExploreActive())
				instance.Say_Internal("Please suspend TalkBack during play.", false);
#endif

			StartScreenOver();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void StartScreenOver()
	{
		// Reset current container and start from the top
		if (/*m_ActiveContainerIndex >= 0 && */m_ActiveContainers.Count > 0)
		{
			m_ActiveContainers[/*m_ActiveContainerIndex*/ 0].ResetToStart();
			UpdateCurrentItem();
			ReadItem(m_CurrentItem);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public static bool IsTalkBackEnabledAndTouchExploreActive()
	{
#if (UNITY_ANDROID) && !UNITY_EDITOR
		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (var context = activity.Call<AndroidJavaObject>("getApplicationContext"))
				{
					using (AndroidJavaObject accManager = context.Call<AndroidJavaObject>("getSystemService", new object[] { context.GetStatic<string>("ACCESSIBILITY_SERVICE") }))
					{
						bool amManagerEnabled = accManager.Call<bool>("isEnabled");
						bool touchExplorationEnabled = accManager.Call<bool>("isTouchExplorationEnabled");
						return (amManagerEnabled && touchExplorationEnabled);
					}
				}
			}
		}
#else
		return false;
#endif
	}


	/// <summary>
	/// Is Google TalkBack activated (on or suspended).
	/// It is impossible to tell whether TalkBack is currently suspended, as the system 
	/// still reports it as active in that case.
	/// </summary>
	/// <returns>Returns true if TalkBack is either active or currently suspended, and false if it is inactive.</returns>
	public static bool IsTalkBackEnabled()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		var plugin = new AndroidJavaClass("com.metalpopgames.checkaccessibility.CheckAccessibilityState");

		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

		return plugin.CallStatic<bool>("isAccessibilityEnabled", context);
#else
		return false;
#endif
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Register to be notified when the user made the Pause gesture / Magic Tap.
	/// See the documentation on <a href="MagicGestures.html">Magic Gestures</a>.
	/// 
	/// The plugin can only detect that the gesture was triggered. It does not 
	/// maintain your app's paused state (because you might have other things/buttons trigger
	/// a pause). That's why there is no bool parameter to the callback function
	/// Do not forget to call UnregisterOnPauseToggledCallback() when your object gets destroyed.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void RegisterOnPauseToggledCallback(OnPauseToggleCallbackFunc func)
	{
		Initialize();

		instance.m_OnPauseToggleCallbacks += func;
	}

	/// <summary>
	/// Unregister from notifications when the user made the Pause gesture / Magic Tap.
	/// See the documentation on <a href="MagicGestures.html">Magic Gestures</a>.
	/// </summary>
	/// <param name="func">Member function that was registered.</param>
	static public void UnregisterOnPauseToggledCallback(OnPauseToggleCallbackFunc func)
	{
		Initialize();

		instance.m_OnPauseToggleCallbacks -= func;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Register to be notified when the user tapped the screen once with two fingers.
	/// Using this callback ensures that the user did not intend to do a magic tap (two finger double tap).
	/// It is fired only after the double tap threshold time is up.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void RegisterOnTwoFingerSingleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnTwoFingerSingleTapCallbacks += func;
	}

	/// <summary>
	/// Unregister from callback for two finger single taps.
	/// </summary>
	/// <param name="func">Member function that was registered.</param>
	static public void UnregisterOnTwoFingerSingleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnTwoFingerSingleTapCallbacks -= func;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Register to be notified when the user tapped the screen once with three fingers.
	/// Using this callback ensures that the user did not intend to do a three finger double or triple tap.
	/// It is fired only after the double tap threshold time is up.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void RegisterOnThreeFingerSingleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnThreeFingerSingleTapCallbacks += func;
	}

	/// <summary>
	/// Unregister from callback for three finger single taps.
	/// </summary>
	/// <param name="func">Member function that was registered.</param>
	static public void UnregisterOnThreeFingerSingleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnThreeFingerSingleTapCallbacks -= func;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Register to be notified when the user tapped the screen twice with three fingers.
	/// Using this callback ensures that the user did not intend to do a three finger triple tap.
	/// It is fired only after the double tap threshold time is up.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void RegisterOnThreeFingerDoubleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnThreeFingerDoubleTapCallbacks += func;
	}

	/// <summary>
	/// Unregister from callback for three finger double taps.
	/// </summary>
	/// <param name="func">Member function that was registered.</param>
	static public void UnregisterOnThreeFingerDoubleTapCallback(OnTapEvent func)
	{
		Initialize();

		instance.m_OnThreeFingerDoubleTapCallbacks -= func;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Get notified when the accessibility mode turns on/off.
	/// The Accessibility Plugin can be enabled by the user with a three finger triple tap at any time. 
	/// In addition, if the app is resumed from suspension, and a screen reader is now detected and 
	/// the plugin wasn't enabled yet, it will then enable itself as well.
	/// This serves as a safeguard for blind people that accidentally disable accessibility mode and 
	/// exit and re-enter the app in hopes to restore accessibility mode.<br><br>
	/// </summary>
	/// <param name="func">function to receive the callback</param>
	static public void RegisterAccessibilityModeChangeCallback(OnAccessibilityModeChanged func)
	{
		Initialize();

		instance.m_OnAccessibilityModeChanged += func;
	}

	/// <summary>
	/// Stop receiving notifications when the accessibility mode state changes.
	/// </summary>
	/// <param name="func">the callback function that should no longer receive calls</param>
	static public void UnregisterAccessibilityModeChangeCallback(OnAccessibilityModeChanged func)
	{
		Initialize();

		instance.m_OnAccessibilityModeChanged -= func;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Overrides the regular two finger swipe up function, which is Read-From-Top.
	/// Use this to handle the gesture yourself. This will stop the 
	/// Read-From-Top feature from working until ::ResetTwoFingerSwipeUpHandler is called.<br><br>
	/// This can be very useful during gameplay, if the gesture is used to read out stats or help
	/// instead.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void SetTwoFingerSwipeUpHandler(OnTapEvent func)
	{
		Initialize();

		instance.m_OnTwoFingerSwipeUpHandler = func;
	}

	static public void ResetTwoFingerSwipeUpHandler()
	{
		Initialize();

		instance.m_OnTwoFingerSwipeUpHandler = null;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Overrides the regular two finger swipe down function, which is Read-From-Current-Element.
	/// Use this to handle the gesture yourself. This will stop the 
	/// Read-From-Current-Element feature from working until ::ResetTwoFingerSwipeDownHandler is called.<br><br>
	/// This can be very useful during gameplay, if the gesture is used to read out stats or help
	/// instead.
	/// </summary>
	/// <param name="func">Member function to call when the gesture is detected.</param>
	static public void SetTwoFingerSwipeDownHandler(OnTapEvent func)
	{
		Initialize();

		instance.m_OnTwoFingerSwipeDownHandler = func;
	}

	static public void ResetTwoFingerSwipeDownHandler()
	{
		Initialize();

		instance.m_OnTwoFingerSwipeDownHandler = null;
	}

	//////////////////////////////////////////////////////////////////////////

	static public void SelectElement(GameObject element, bool forceRepeatItem = false)
	{
		if (element == null)
			return;

		UAP_BaseElement accesssBase = element.GetComponent<UAP_BaseElement>();
		if (accesssBase == null)
		{
			Debug.LogWarning("[Accessibility] SelectElement: No Accessibility component found on gameobject " + element.name);
			return;
		}

		accesssBase.SelectItem(forceRepeatItem);
	}

	//////////////////////////////////////////////////////////////////////////

	static public void MakeActiveContainer(AccessibleUIGroupRoot container, bool forceRepeatActiveItem = false)
	{
		if (instance == null)
			return;

		if (!instance.m_ActiveContainers.Contains(container))
			return;

		// Stop continuous reading instantly
		StopContinuousReading();

		int i = 0;
		for (; i < instance.m_ActiveContainers.Count; ++i)
		{
			if (instance.m_ActiveContainers[i] == container)
				break;
		}

		AccessibleUIGroupRoot.Accessible_UIElement element = instance.m_ActiveContainers[i].GetCurrentElement(instance.m_CyclicMenus);
		bool alreadySelected = (instance.m_CurrentItem == element);

		instance.m_CurrentElementHasSoleFocus = false;
		instance.m_CurrentItem = element;
		instance.m_ActiveContainerIndex = i;
		instance.UpdateCurrentItem();

		// Read only if the item was already selected (unless forceRepeat is on)
		if (!alreadySelected || forceRepeatActiveItem)
			ReadItem(instance.m_CurrentItem);

	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Retrieve the GameObject containing the UI element that currently has the focus.
	/// This might be null. 
	/// </summary>
	/// <returns></returns>
	static public GameObject GetCurrentFocusObject()
	{
		Initialize();

		if (instance.m_CurrentItem == null)
			return null;

		return instance.m_CurrentItem.m_Object.gameObject;
	}

	//////////////////////////////////////////////////////////////////////////

	private bool HandleUI()
	{
		if (m_BlockInput)
		{
			return false;
		}

		return m_HandleUI;
	}

	//////////////////////////////////////////////////////////////////////////

	static public bool UseAndroidTTS()
	{
		Initialize();

		return instance.m_AndroidTTS;
	}

	static public bool UseiOSTTS()
	{
		Initialize();

		return instance.m_iOSTTS;
	}

	static public bool UseWindowsTTS()
	{
		Initialize();

		return instance.m_WindowsTTS;
	}

	static public bool UseMacOSTTS()
	{
		Initialize();

		return instance.m_MacOSTTS;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This function refreshes the internal order in which the UI elements are stepped through by their on screen position.
	/// You might want to call this function if you have an intro animation that builds up your menu screen and the animation 
	/// moves the buttons, labels etc around. If you find the order in which the menu is traversed is wrong, this function 
	/// should be called after the animation is done and it will recalculate the order based on the current positions of the elements.<br><br>
	/// You should provide the parent of the top most object that moved, to make sure to include all containers underneath. 
	/// If you don't specify a parent, this function will recalculate ALL containers currently in the scene, which is considerably slower.
	/// </summary>
	/// <param name="parent"></param>
	static public void RecalculateUIElementsOrder(GameObject parent = null)
	{
		if (parent != null)
		{
			AccessibleUIGroupRoot[] allContainers = parent.GetComponentsInChildren<AccessibleUIGroupRoot>();
			foreach (AccessibleUIGroupRoot container in allContainers)
				container.RefreshContainer();
		}
		else
		{
			AccessibleUIGroupRoot[] allContainers = FindObjectsOfType(typeof(AccessibleUIGroupRoot)) as AccessibleUIGroupRoot[];
			foreach (AccessibleUIGroupRoot container in allContainers)
				container.RefreshContainer();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Get the current speech rate in the range from 1 to 100.
	/// The plugin will take care of loading and saving the speech rate.
	/// </summary>
	/// <returns>speech rate</returns>
	static public int GetSpeechRate()
	{
		Initialize();
		return instance.m_AudioQueue.GetSpeechRate();
	}

	/// <summary>
	/// Set the speech rate in the range 1 to 100. Not supported on all platforms.
	/// The speech rate will not affect VoiceOver. If your app is only using VoiceOver, 
	/// you don't need to touch this at all.
	/// If the rate is out of range, it will be clamped to a valid value. 
	/// Function will return the new (capped) value.
	/// The plugin will take care of loading and saving the speech rate.
	/// </summary>
	/// <returns>new speech rate</returns>
	static public int SetSpeechRate(int speechRate)
	{
		Initialize();
		return instance.m_AudioQueue.SetSpeechRate(speechRate);
	}

	//////////////////////////////////////////////////////////////////////////

	static public void StopSpeaking()
	{
		Initialize();
		instance.m_AudioQueue.Stop();
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This function is called from the Accessible UI component if the <i>Is Localization Key</i> checkbox is ticked.
	/// 
	/// If NGUI support is enabled, this function will use NGUI's inhouse localization function to localize the text.
	/// In all other cases, you can use this function to hook up your own localization system/plugin and make the appropriate call.
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	static public string Localize(string key)
	{
#if ACCESS_NGUI
		if (key.Length > 0)
			return Localization.Get(key);
#endif
		return key;
	}

	//////////////////////////////////////////////////////////////////////////

	static public bool IsVoiceOverAllowed()
	{
		if (instance == null)
			return true;

		return instance.m_AllowVoiceOverGlobal;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Converts an integer number into a string and adds in separators between thousands to help the voice output read the numbers correctly.
	/// The separators are culture specific either commas or points.<br>
	/// Example: While the US uses commas to separate thousands (1,200.00), most European countries use dots (1.200,00).
	/// </summary>
	/// <param name="intNumber"></param>
	/// <returns></returns>
	static public string FormatNumberToCurrentLocale(int intNumber)
	{
		string formattedNumber = string.Format(CultureInfo.CurrentCulture, "{0:n0}", intNumber);
		//Debug.Log(CultureInfo.CurrentCulture.ToString() + " " + formattedNumber);
		return formattedNumber;
	}

}

