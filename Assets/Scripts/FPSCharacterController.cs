using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCharacterController : MonoBehaviour
{
    // Public settings for easy adjustment in the Inspector
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float mouseSensitivity = 100f;
    public float crouchHeight = 0.5f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 5f;
    public Transform cameraTransform;

    // Internal variables
    private CharacterController characterController;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private bool isGrounded;
    private float verticalRotation = 0f;
    private bool isCrouching = false;
    private float targetHeight;

    void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();

        // Lock the cursor to the game window and hide it
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize the target height to standing height
        targetHeight = standHeight;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        SmoothCrouchTransition();
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the camera up and down
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotate the player left and right
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure the player sticks to the ground
        }

        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Determine the desired movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Determine speed based on player state (walking, running, crouching)
        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        // Move the character
        characterController.Move(move * speed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            targetHeight = isCrouching ? crouchHeight : standHeight;
        }
    }

    void SmoothCrouchTransition()
    {
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        float targetCameraHeight = targetHeight / 2;
        Vector3 cameraPosition = cameraTransform.localPosition;
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetCameraHeight, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = cameraPosition;
    }
}
