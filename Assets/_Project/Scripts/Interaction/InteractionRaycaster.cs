using UnityEngine;
using UnityEngine.InputSystem;

// Checks what the player is looking at.
// If it is an IInteractable within reach, shows the prompt
// and calls Interact() when E is pressed.
public class InteractionRaycaster : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera the ray is cast from. If empty, the first child Camera is used.")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("The UI prompt (\"Press E to Interact\"). It is shown and hidden by this script.")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Settings")]
    [Tooltip("How far away (in meters) the player can interact with objects.")]
    [SerializeField] private float interactDistance = 3f;

    [Tooltip("Which layers the ray can hit.")]
    [SerializeField] private LayerMask interactionLayers = Physics.DefaultRaycastLayers;

    private InputAction interactAction;
    private IInteractable currentTarget; // what we are looking at right now (or null)

    private void Awake()
    {
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        // "Interact" (E key) comes from the project-wide Input Actions asset (Player map).
        interactAction = InputSystem.actions.FindAction("Player/Interact");

        if (cameraTransform == null || interactAction == null)
        {
            Debug.LogError("InteractionRaycaster: camera or 'Player/Interact' action is missing.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        interactAction?.Enable();
    }

    private void OnDisable()
    {
        currentTarget = null;
        SetPromptVisible(false);
    }

    private void Update()
    {
        // 1. Find what we are looking at.
        currentTarget = FindTarget();

        // 2. Show the prompt only while looking at something interactable.
        SetPromptVisible(currentTarget != null);

        // 3. WasPressedThisFrame() is true only on the frame E goes down,
        //    so holding E does not trigger the object again and again.
        if (currentTarget != null && interactAction.WasPressedThisFrame())
        {
            currentTarget.Interact();
        }
    }

    private IInteractable FindTarget()
    {
        // A ray from the camera straight forward = from the center of the screen.
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            // GetComponentInParent also checks parents, so a collider on a child
            // part (e.g. a door handle) still finds the script on the main object.
            return hit.collider.GetComponentInParent<IInteractable>();
        }

        return null;
    }

    private void SetPromptVisible(bool visible)
    {
        if (interactionPrompt != null && interactionPrompt.activeSelf != visible)
        {
            interactionPrompt.SetActive(visible);
        }
    }
}
