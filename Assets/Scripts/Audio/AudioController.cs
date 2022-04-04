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
            case "menu":
                return(0);
            case "buildup":
                return(1);
            case "game":
                return(2);
            case "end":
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
                return("menu");
            case 1:
                return("buildup");
            case 2:
                return("game");
            case 3:
                return("end");
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
            playing = name;
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
            }
        }
        //When the current track is meant to be stopping, fade out the track by 5% volume every 0.1s
        stoppingTimer += Time.deltaTime;
        if(stopping && (stoppingTimer >= 0.1) && nextSound !="game")
        {
            //Reset timer to next fade/stop point
            stoppingTimer = 0f;
            //If currently playing sound is louder than 5% lower by 5%
            if(sounds[toInt(playing)].source.volume>=0.05f)
            {
                sounds[toInt(playing)].source.volume -= 0.05f;
            }
            //Stop track as its silent, reset parameters from clip, reset stopping.
            else
            {
                stopping = false;
                sounds[toInt(playing)].source.volume = sounds[toInt(playing)].volume;
                sounds[toInt(playing)].source.Stop();
            }
            
        }
        if(stopping && nextSound == "game" && playing == "buildup")
        {
            stopping = false;
        }
        scene = SceneManager.GetActiveScene();
        if(scene.name == "GameOver")
        {
            if(playing != "end" && nextSound != "end")
            {
                Play("end");
            }
        }
        else if(scene.name == "GameScene")
        {
            if(playing == "buildup" && nextSound != "game")
            {
                Play("game");
            }
            if(playing !="buildup" && playing != "game" && nextSound!= "game")
            {
                Play("buildup");
            }
            
        }
        else if(scene.name == "MainMenu" || scene.name == "NextShiftScene")
        {
            if(playing != "menu" && nextSound != "menu")
            {
                Play("menu");
            }
        }    
            
        
        

    }
    
}
