using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AudioController : MonoBehaviour
{

    //Array of Sound files with information for each one
    public Sound[] sounds;
    //Tracks whether AudioController is already instanced
    public static AudioController instance;
    //Tracks whether another sound is playing
    private string playing = null;
    private bool stopping = false;
    private float stoppingTimer = 0f;
    private string nextSound = null;

    private Scene scene;

    //Convert names of Sound objects in sounds to int indexes of sounds.
    private int toInt(string input)
    {
        switch(input)
        {
            case "menuMusic":
                return(0);
            case "gameBuildup":
                return(1);
            case "gameMain":
                return(2);
            case "endMusic":
                return(3);
            default:
                return(-1);            
        }
    }
    //Convert int indexes of Sound objects in sounds to strings.
    private string toString(int input)
    {
        switch(input)
        {
            case 0:
                return("menuMusic");
            case 1:
                return("gameBuildup");
            case 2:
                return("gameMain");
            case 3:
                return("endMusic");
            default:
                return(null);            
        }
    }
    //Upon class being called first time.
    void Awake()
    {
        //If no instance already, make new instance
        if (instance == null)
        {
            instance = this;
        }
        //Otherwise destroy this instance as one already exists
        else
        {
            Destroy(gameObject);
            return;
        }
        //Prevents this object destroying previous versions of itself on load (music keeps playing between scene changes)
        DontDestroyOnLoad(gameObject);
        //Iterates through sounds and creates AudioSources for each clip and passes relevant properties.
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    //Plays a sound if none currently playing, otherwise sets it as next to be played, disables looping on current track and sets stopping to start next track stopping.
    public void Play(string name)
    {
        if(playing == null && toInt(name)!= -1) 
        {
            sounds[toInt(name)].source.Play();
        }
        else
        {
            nextSound = name;
            sounds[toInt(playing)].source.loop = false;
            stopping = true;
        }
    }

    void Start()
    {
        sounds[0].source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if any tracks are currently playing
        playing = null;
        foreach(Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                playing = s.name;
                Debug.Log(s.name);
            }
        }
        //When the current track is meant to be stopping, fade out the track by 5% volume every 0.1s
        stoppingTimer += Time.deltaTime;
        if(stopping && (stoppingTimer >= 1))
        {
            //Reset timer to next fade/stop point
            stoppingTimer = 0f;
            //If currently playing sound is louder than 5% lower by 5%
            if(sounds[toInt(playing)].source.volume>=0.1f)
            {
                sounds[toInt(playing)].source.volume -= 0.1f;
            }
            //Stop track as its silent, reset parameters from clip, reset stopping.
            else
            {
                stopping = false;
                sounds[toInt(playing)].source.volume = sounds[toInt(playing)].volume;
                sounds[toInt(playing)].source.Stop();
            }
            
        }
        //If no sound and one is queued up to play, play it and set relevant statuses.
        if(playing == null && nextSound != null)
        {
            sounds[toInt(nextSound)].source.Play();
            playing = nextSound;
            nextSound = null;
            
        }
        if(scene != SceneManager.GetActiveScene())
        {
            scene = SceneManager.GetActiveScene();
            if(scene.name == "GameOver")
            {
                Play("endMusic");
            }
            else if(scene.name == "GameScene")
            {
                Play("gameBuildup");
                Play("gameMain");
            }
            else
            {
                //Play("menuMusic");
            }    
            
        }
        

    }
    
}