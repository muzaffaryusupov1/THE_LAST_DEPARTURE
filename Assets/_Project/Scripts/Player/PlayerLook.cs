using UnityEngine;
using UnityEngine.InputSystem;

// Mouse look for a first-person player.
// Left/right mouse turns the whole Player (body).
// Up/down mouse tilts only the camera (head).
public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera that looks up and down. If empty, the first child Camera is used.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Mouse")]
    [Tooltip("Degrees of rotation per pixel of mouse movement.")]
    [SerializeField] private float mouseSensitivity = 0.1f;

    [Tooltip("How far the player can look up or down, in degrees.")]
    [SerializeField] private float maxLookAngle = 85f;

    private InputAction lookAction;
    private float cameraPitch; // current up/down angle of the camera

    private void Awake()
    {
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        // "Look" comes from the project-wide Input Actions asset (Player map).
        lookAction = InputSystem.actions.FindAction("Player/Look");

        if (cameraTransform == null || lookAction == null)
        {
            Debug.LogError("PlayerLook: camera or 'Player/Look' action is missing.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        lookAction?.Enable();

        // Hide the cursor and keep it in the middle of the screen.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // Give the cursor back (also runs when you leave Play Mode).
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Mouse movement since the last frame, in pixels.
        // Not multiplied by Time.deltaTime: the delta is already "per frame".
        Vector2 look = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        // Left/right: rotate the Player body around the vertical (Y) axis.
        transform.Rotate(0f, look.x, 0f);

        // Up/down: tilt the camera, limited so the view never flips over.
        cameraPitch -= look.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
