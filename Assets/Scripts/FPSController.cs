using UnityEngine;
using UnityEngine.InputSystem; // new Input System

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; 

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float Sprint = 2f;
    [Header("Look")]
    [Tooltip("Mouse sensitivity (try ~0.1 to 0.5 and adjust)")]
    public float mouseSensitivity = 0.12f;
    public float maxLookAngle = 89f;
    private float MouseX;
    private float MouseY;
    [Header("Physics")]
    public float gravity = -9.81f;

    CharacterController controller;
    float Y_Axis_Pitch = 0f;           // camera up/down
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        OnEnableCursor();

    }

    void Update()
    {
        HandleLook();
        HandleMove();
        CheckCursorToggle();
        Debug.Log("Mouse X axis = " + MouseX);
        Debug.Log("Mouse Y axis = " + MouseY);
    }

    void HandleLook()
    {
        var mouse = Mouse.current;
        if (mouse == null) return; // no mouse available

        Vector2 delta = mouse.delta.ReadValue();

        MouseX = delta.x * mouseSensitivity;
        MouseY = delta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * MouseX);

        Y_Axis_Pitch -= MouseY;
        Y_Axis_Pitch = Mathf.Clamp(Y_Axis_Pitch, -maxLookAngle, maxLookAngle);
        if (cameraTransform != null)
            cameraTransform.localEulerAngles = new Vector3(Y_Axis_Pitch, 0f, 0f);
    }

    void HandleMove()
    {
        //old input system
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return; // no keyboard available
        }
        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();  // for diaognal movements
        }
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        
        if(keyboard.leftShiftKey.isPressed)
        {

            move *= moveSpeed * Sprint;
        }
        else
        {
            move *= moveSpeed;
        }
            
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f; 

        velocity.y += gravity * Time.deltaTime;

        // combine movement + vertical velocity
        Vector3 final = (move + new Vector3(0f, velocity.y, 0f)) * Time.deltaTime;
        controller.Move(final);
        Debug.Log("Move speed : " + move);


    }

    // Press Escape to toggle cursor (useful during testing)
    void CheckCursorToggle()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                OnDisableCursor();
            }
            else
            {
                OnEnableCursor();
            }
        }
    }

    void OnDisableCursor()
    {
        // restore cursor when script disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnEnableCursor()
    {
        // restore cursor when script disabled
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
