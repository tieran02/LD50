using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    AudioSource musicPlayer;
    //Track whether the music is playing or not
    bool play;
    //Tracks the speed the music should be played at (pitch)
    float playSpeed;
    //Tracks that the track has started playing so it doesn't happen until track switch occurs again.
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        //Assigns AudioSource to musicPlayer
        musicPlayer = GetComponent<AudioSource>();
        //Sets play bool to make sure music plays on startup
        play = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Logic to start initial playback/continue playback and other stuff that is continuous.
    }
    //Takes track to switch to and manages transition.
    public void SwitchTrack(int track)
    {
        
    }
    //Accessor method to allow stress changes to affect speed of main game music.
    public void SetSpeed(float speed)
    {
        //Control speedup mechanism of to accelerate currently playing music.
        //manage how stress speedup mechanism works (playing with loop and pitch and probably isPlaying properties)
    }
    
}
