using UnityEngine;
using System.Collections.Generic;

public class SpeechSynthesisManager : MonoBehaviour {

    private bool _initializeError = false;
    private string _inputText = "\n\n\n\n";
    private int _speechId = 0;
    private float _pitch = 1f, _speechRate = 1f;
    private int _selectedLocale = 0;
    private string[] _localeStrings;

    // Use this for initialization
    void Awake () {
        TTSManager.Initialize(transform.name, "OnTTSInit");
    }

    void Start() {
        Speak("spraak assistentie ingeschakeld");
    }

	// Update is called once per frame
	public static void Speak (string message) {
        var test = TTSManager.IsInitialized();
        if (test)
        {
            //TTSManager.Speak(message, false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnSpeechCompleted", "speech_" + (++_speechId));
            TTSManager.Speak(message, true, TTSManager.STREAM.Music, 1f, 0f, "GameManager", "OnSpeechCompleted", "speech");
        }
    }

    void OnDestroy()
    {
        TTSManager.Shutdown();
    }


    void OnTTSInit(string message)
    {
        int response = int.Parse(message);

        switch (response)
        {
            case TTSManager.SUCCESS:
                List<TTSManager.Locale> l = TTSManager.GetAvailableLanguages();
                _localeStrings = new string[l.Count];
                for (int i = 0; i < _localeStrings.Length; ++i)
                    _localeStrings[i] = l[i].Name;

                break;
            case TTSManager.ERROR:
                _initializeError = true;
                Debug.Log("error loading STT");
                break;
        }
    }

    void OnSpeechCompleted(string id)
    {
        Debug.Log("Speech '" + id + "' is complete.");
    }

    void OnSynthesizeCompleted(string id)
    {
        Debug.Log("Synthesize of speech '" + id + "' is complete.");
    }
}
