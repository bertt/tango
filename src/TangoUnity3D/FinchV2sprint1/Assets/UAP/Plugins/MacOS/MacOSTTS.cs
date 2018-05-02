using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MacOSTTS : MonoBehaviour
{
	public static MacOSTTS instance = null;
	bool m_IsSpeaking = false;
	Process m_VoiceProcess = null;

	//////////////////////////////////////////////////////////////////////////

	void Start()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			UnityEngine.Debug.LogError("[Accessibility] Trying to create another MacOS TTS instance, when there already is one.");
			DestroyImmediate(this);
			return;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public void Speak(string msg)
	{
		if (msg.Length == 0)
			return;

		Stop();

#if (UNITY_STANDALONE_OSX || UNITY_EDITOR) && !UNITY_WEBPLAYER
		m_IsSpeaking = true;
		StartCoroutine("SpeakText", msg);
#endif
	}

	IEnumerator SpeakText(string textToSpeak)
	{
		// Replace quotation marks
		textToSpeak = textToSpeak.Replace('"', '\'');
		int speechRate = (int)((UAP_AccessibilityManager.GetSpeechRate() / 100.0f) * 175 * 2);
		string parameters = "-r " + speechRate + " " + '"' + textToSpeak + '"';

		m_VoiceProcess = new System.Diagnostics.Process();
		m_VoiceProcess.StartInfo.FileName = "say";
		m_VoiceProcess.StartInfo.Arguments = parameters;
		m_VoiceProcess.StartInfo.CreateNoWindow = true;
		m_VoiceProcess.StartInfo.RedirectStandardOutput = true;
		m_VoiceProcess.StartInfo.RedirectStandardError = true;
		m_VoiceProcess.StartInfo.UseShellExecute = false;
		m_VoiceProcess.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

		System.Threading.Thread worker = new System.Threading.Thread(() => WaitForVoiceToFinish(m_VoiceProcess)) { Name = "UAP_TTS_Proc" };
		worker.Start();

		do
		{
			yield return null;
		} while (worker.IsAlive);

		worker = null;
		m_IsSpeaking = false;
	}

	//////////////////////////////////////////////////////////////////////////

	private void WaitForVoiceToFinish(System.Diagnostics.Process process)
	{
		try
		{
			process.Start();
			process.WaitForExit(); 
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError("[Accessibility] TTS Error: " + ex);
		}
	}

	//////////////////////////////////////////////////////////////////////////

	public void Stop()
	{
		if (!m_IsSpeaking || m_VoiceProcess == null)
			return;

		m_VoiceProcess.Kill();
		m_IsSpeaking = false;
		StopCoroutine("SpeakText");
	}

	//////////////////////////////////////////////////////////////////////////

	public bool IsSpeaking()
	{
		if (!Application.isPlaying)
			return false;

		return m_IsSpeaking;
	}

	//////////////////////////////////////////////////////////////////////////

	void OnDestroy()
	{
		Stop();
	}

}
