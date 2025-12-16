using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    public float rotationSpeed = 50.0f;
    public float mouseSensitivity = 100.0f;
    public float gravityValue = -9.81f;
    public float jumpHeight = 4.0f; // The height that the player can jump

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;
    private float pitch = 0.0f;
    private float pitchRange = 45.0f; // Limit of up/down camera rotation.

    private void Start()
    {
        // Create a new player GameObject
        GameObject player = new GameObject("Player");

        // Add a CharacterController to the player
        controller = player.AddComponent<CharacterController>();

        // Configure the CharacterController properties based on typical human proportions
        controller.height = 1.8f; // Average human height
        controller.center = new Vector3(0, controller.height / 2, 0);

        // Position the player at the specified coordinates, start higher to allow gravity to pull down
        float initialHeightAboveGround = 40.0f; // Start 10 meters above the ground
        // Position the player at the specified coordinates
        player.transform.position = new Vector3(490, controller.height / 2 + initialHeightAboveGround, 320);

        // Make the player a child of the GameObject this script is attached to (e.g., terrain)
        player.transform.SetParent(transform);

        // Add a Camera to the player
        GameObject cameraGameObject = new GameObject("PlayerCamera");
        Camera playerCamera = cameraGameObject.AddComponent<Camera>();
        cameraTransform = cameraGameObject.transform;
        cameraTransform.SetParent(player.transform);

        // Set the camera's position to approximate eye level
        cameraTransform.localPosition = new Vector3(0, controller.height - 0.2f, 0); // Slightly below the top of the character controller
        cameraTransform.localEulerAngles = new Vector3(0, 0, 0);
        cameraTransform.localEulerAngles = new Vector3(0, 45, 0); // Turns the camera 90 degrees to the right

        // Lock cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Exit play mode in Unity Editor with Left Ctrl + Q
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        // Rotation left and right
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        // Pitch up and down
        if (Input.GetKey(KeyCode.Z))
        {
            pitch = Mathf.Clamp(pitch - rotationSpeed * Time.deltaTime, -pitchRange, pitchRange);
        }
        if (Input.GetKey(KeyCode.X))
        {
            pitch = Mathf.Clamp(pitch + rotationSpeed * Time.deltaTime, -pitchRange, pitchRange);
        }
        cameraTransform.localEulerAngles = new Vector3(-pitch, cameraTransform.localEulerAngles.y, 0);

        // Movement forward, backward, left, and right
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.forward * move.z + transform.right * move.x;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch - mouseY, -pitchRange, pitchRange);
        cameraTransform.localEulerAngles = new Vector3(pitch, cameraTransform.localEulerAngles.y + mouseX, 0);

        // Reset view with 'F' key
        if (Input.GetKeyDown(KeyCode.F))
        {
            cameraTransform.localEulerAngles = new Vector3(0, 0, 0);
            pitch = 0;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the velocity is applied to the character controller). This is because gravity should be applied
        // as an acceleration (ms^-2)
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}


