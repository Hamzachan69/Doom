using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController CC; // Reference to the CharacterController component for movement

    public float speed = 5.0f; // Base speed for player movement

    private Vector3 inputVector;    // Stores raw input from keyboard (WASD/Arrow keys)
    private Vector3 movementVector; // Stores final movement direction and speed (with gravity)

    private float Gravity = -9.81f;      // Simulates gravity force
    private float SprintMultiplier = 2;  // Factor to multiply speed when sprinting (Left Shift)

    // ✅ Animator fields to handle animations
    public Animator AnimatorControl;        // Reference to Animator (drag Player’s Animator here in Inspector)
    public bool playWalkAnimation = true;   // Toggle walk animation on/off
    public bool playRunAnimation = true;    // Toggle run animation on/off
    public bool playIdleAnimation = true;   // Toggle idle animation on/off

    private void Start()
    {
        // Get the CharacterController component on this GameObject at start
        CC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Check input every frame
        getInput();

        // Apply movement to the player
        PlayerMovement();
    }

    public void getInput()
    {
        float h = 0f; // Horizontal input (A/D or Left/Right arrows)
        float v = 0f; // Vertical input (W/S or Up/Down arrows)

        // Keyboard input mapping for forward/backward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;

        // Keyboard input mapping for left/right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;

        // Store input as movement vector (X = left/right, Z = forward/backward)
        inputVector = new Vector3(h, 0f, v);

        // Normalize to ensure diagonal movement isn't faster than straight
        inputVector.Normalize();

        // Convert local input direction into world direction (relative to where player is facing)
        inputVector = transform.TransformDirection(inputVector);

        // Check if player is pressing any movement keys (magnitude > 0.1)
        bool isMoving = inputVector.magnitude > 0.1f;

        // Check if player is sprinting (holding Left Shift)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // ✅ Animator logic
        if (AnimatorControl != null)
        {
            // Play walk animation only if moving and not sprinting
            if (playWalkAnimation)
            {
                AnimatorControl.SetBool("Walk", isMoving && !isSprinting);
            }

            // Play run animation if sprinting while moving
            if (playRunAnimation)
            {
                AnimatorControl.SetBool("run", isMoving && isSprinting);
                // ⚠️ NOTE: Animator parameter must be exactly "run" in your Animator
            }

            // Play idle animation if player is not moving
            if (playIdleAnimation)
            {
                AnimatorControl.SetBool("Idle", !isMoving);
            }
        }

        // Calculate movement vector with speed and gravity applied
        if (isSprinting)
        {
            // Sprinting speed + gravity applied on Y axis
            movementVector = (inputVector * speed * SprintMultiplier) + (Vector3.up * Gravity);
        }
        else
        {
            // Normal speed + gravity applied on Y axis
            movementVector = (inputVector * speed) + (Vector3.up * Gravity);
        }
    }

    public void PlayerMovement()
    {
        // Actually move the character each frame
        CC.Move(movementVector * Time.deltaTime);
    }
}
