using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private float playerSpeed = 5.0f;
    private int playerStress = 0f;

    public StressBar stressBar;
    // Start is called before the first frame update
    private void Start()
    {
        //Get controller reference for character Controller.
        controller = gameObject.GetComponent<CharacterController>();
        //Sets player initial stress
        playerStress = 0;
        //Initialises stress bar to 0 current stress and 100 max stress.
        stressBar.SetMaxStress(100);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Gets horizontal and vertical input and translates it to a vector.
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Passes the absolute movement vector (direction *time since last frame * speed)
        controller.Move(move * Time.deltaTime * playerSpeed);
        //If there is no movement input, tell the character controller how to move the player.
        if (move != Vector3.zero)
        {
            //Rotates player to the direction of movement
            controller.transform.forward = move;
        }
    }

    public void addStress(int stress)
    {
        playerStress += stress;
        stressBar.setStress(playerStress);

    }
}
