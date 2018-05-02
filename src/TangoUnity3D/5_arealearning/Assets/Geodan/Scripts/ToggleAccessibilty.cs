using UnityEngine;
using UnityEngine.UI;

public class ToggleAccessibilty : MonoBehaviour {

    public void OnEnable()
    {
        gameObject.GetComponent<Toggle>().isOn = UAP_AccessibilityManager.IsEnabled();        
    }

    public void OnAccessibilityEnabledToggleChanged(bool newValue)
    {
        if (UAP_AccessibilityManager.IsEnabled() == newValue)
            return;

        UAP_AccessibilityManager.EnableAccessibility(newValue, true);       
    }
}
