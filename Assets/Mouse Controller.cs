using UnityEngine;
using UnityEngine.InputSystem; // Required for Unity's new Input System (supports modern input devices)

// Controls player rotation using mouse input
public class MouseController : MonoBehaviour
{
    public float mousesensitivity = 1.5f; // Controls how fast the player rotates with mouse movement
    public float smoothing = 10f;         // Higher values make rotation smoother (more damped)

    private float xMousePos;              // Raw mouse movement on the X-axis
    private float smoothedMousepos;       // Smoothed mouse input value
    private float currentpos;             // Current total rotation applied to the player

    private Vector2 mouseInput;           // Holds raw mouse delta (X and Y movement)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Hides and locks the cursor to the center of the screen
    }

    void Update()
    {
        Getinput();       // Get raw mouse input
        modifyInput();    // Apply sensitivity and smoothing
        movePlayer();     // Rotate the player based on processed input
    }

    void Getinput()
    {
        // Read raw mouse movement from the new Input System
        mouseInput = Mouse.current.delta.ReadValue();

        // Extract only the horizontal movement (X-axis)
        xMousePos = mouseInput.x;
    }

    void modifyInput()
    {
        // Scale the input with sensitivity
        xMousePos = mousesensitivity * xMousePos;

        // Smoothly interpolate from the previous smoothed value to the new one
        smoothedMousepos = Mathf.Lerp(smoothedMousepos, xMousePos, 1f / smoothing);
    }

    void movePlayer()
    {
        // Accumulate the smoothed movement to create continuous rotation
        currentpos += smoothedMousepos;

        // Apply the rotation around the Y-axis (turning left and right)
        transform.localRotation = Quaternion.AngleAxis(currentpos, Vector3.up);
    }
}
