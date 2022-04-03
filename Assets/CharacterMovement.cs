using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 5.0f;
    private float gravityValue = -9.81f;
    private float playerStress = 0;

    public StressBar stressBar;
    // Start is called before the first frame update
    private void Start()
    {
        //Get controller reference for character Controller.
        controller = gameObject.GetComponent<CharacterController>();
        //Sets player initial stress
        playerStress = 0;
        //Initialises stress bar to 0 current stress.
        stressBar.Initialize();

        
    }

    // Update is called once per frame
    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
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
        if(!groundedPlayer)
        {
        //playerVelocity.y += gravityValue * Time.deltaTime;
        //controller.Move(playerVelocity * Time.deltaTime);
        
        }
        //controller.height = -1f;
    

    }

    public void addStress(float stress)
    {
        //Store increased stress value.
        playerStress = Mathf.Clamp(playerStress + stress,0,100);
        //Update stress bar
        stressBar.setStress(playerStress);

        if(playerStress >= 100)
        {
            SceneManager.LoadScene("GameOver");
        }

    }
}
