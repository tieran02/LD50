using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float playerSpeed = 5.0f;
    // Start is called before the first frame update
    private void Start()
    {
        //Get controller reference for character Controller.
        controller = gameObject.GetComponent<CharacterController>();
        //No Jumping allowed, vertical velocity always set to 0
        playerVelocity.y = 0f;
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
}
