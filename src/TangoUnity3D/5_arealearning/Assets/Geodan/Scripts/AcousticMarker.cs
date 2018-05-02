using UnityEngine;
using System.Collections;

public class AcousticMarker : MonoBehaviour {

    public float pitchMin = 0.75f; //inSeconds
    public float pitchMax = 2f; //inSeconds
    public int cutOffMin = 300; //inSeconds
    public int cutOffMax = 22000; //inSeconds
    public float attackRangeDegrees = 45; //degrees needed to go from cutoffMin to Max

    public AudioClip beaconClip;
    public AudioClip achievementClip;
    public AudioClip goalClip;

    private AudioLowPassFilter leftLPF;
    private AudioLowPassFilter rightLPF;
    public int _id = 0;
    private AudioSource[] _sources;
    private bool _play = false;
    private AudioSource _player;
    
    void Start () {
        _sources = GetComponentsInChildren<AudioSource>();
        _player = GameObject.Find("Player").GetComponentInChildren<AudioSource>();
        foreach (var s in _sources)
        {
            if (s.name == "forLeftEar")
                leftLPF = s.GetComponent<AudioLowPassFilter>();
            if (s.name == "forRightEar")
                rightLPF = s.GetComponent<AudioLowPassFilter>();
        }

        gameObject.tag = "Marker";

        _id = GameManager.Markers.Count -1;
        Play(beaconClip);    
	}
    
    void Update()
    {
        if (_play)
        {
            var pitch = pitchMin;

            //calculate distance
            var dist = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position);

            //normalize linearly between 0-10 meter, cutoff after 10
            if (dist <= 0)
                pitch = pitchMin;
            else if (dist >= 10)
                pitch = pitchMax;
            else
                pitch = pitchMax + ((pitchMin-pitchMax) /10) * dist;

            //use pitch for sense of nearness which is strenghtened due to linear rolloff of volume (all based on distance)
            //use 3D spatial sound (stereo audio and volume intensity) for sense of direction and use low pass filter to distinguish front from back (all based on position relative to marker)
            foreach (var s in _sources)
                s.pitch = pitch;
            //calculate left ear
            if (NavigatorSystem.Dot > 0)
            {
                leftLPF.cutoffFrequency = cutOffMax;
                rightLPF.cutoffFrequency = cutOffMax;
            }
            else
            {
                leftLPF.cutoffFrequency = cutOffMin;
                rightLPF.cutoffFrequency = cutOffMin;
            }  

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && GameManager.Modus == GameManager.Modi.isNavigating)
        {
            StartCoroutine("NextMarker");
        }
    }

    public void Play(AudioClip clip) {
        _play = true;

       _player.clip = clip;
       _player.Play();

    }

    //hack remove
    public void PlayBeacon()
    {
        _play = true;


        foreach (var s in _sources)
        {
            s.clip = beaconClip;
            s.loop = true;
        }

    }

    public void Stop()
    {
        _play = false;
    }
    
    IEnumerator NextMarker()
    {
        //stop beacon sound
        _play = false;

        foreach (var s in _sources)
        {
            s.Stop();
            s.loop = false;          
        }

        if ((NavigatorSystem.curMarkerId + 1) < GameManager.Markers.Count)  //check if next marker is available
        {
            //set achievement sound
           Play(achievementClip);

            if (gameObject.name != "Marker")
            {
                SpeechSynthesisManager.Speak(gameObject.name);
            }

            //set next marker           
            NavigatorSystem.curMarkerId++; 
            NavigatorSystem.TargetMarker = GameManager.Markers[NavigatorSystem.curMarkerId];
            GameManager.Markers[NavigatorSystem.curMarkerId].GetComponent<AcousticMarker>().PlayBeacon();
            GameManager.Markers[NavigatorSystem.curMarkerId].SetActive(true);            
        }
        else //finish line!
        {
            //set goal sound
            Play(goalClip);
            SpeechSynthesisManager.Speak("bestemming bereikt");
            NavigatorSystem.isFinished = true;
        }
        
        
        //wait for sound to end
        yield return new WaitForSeconds(1f);
        
        //disable marker        
        gameObject.SetActive(false);
    }
}
