using System.Collections;
using UnityEngine;

// A door that opens and closes around a hinge when the player presses E.
//
// Expected hierarchy:
//   Door (this script)
//     └ Hinge        <- placed on the door's edge; this is what rotates
//         └ Panel    <- the visible door with its collider
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("References")]
    [Tooltip("The pivot the door rotates around (on the door's edge).")]
    [SerializeField] private Transform hinge;

    [Header("Settings")]
    [Tooltip("How far the door opens, in degrees.")]
    [SerializeField] private float openAngle = 90f;

    [Tooltip("How long opening or closing takes, in seconds.")]
    [SerializeField] private float animationDuration = 1f;

    private bool isOpen;
    private bool isMoving;
    private Quaternion closedRotation;
    private Transform player;

    // Shown in the prompt: "Press E to Open" / "Press E to Close".
    public string InteractionText => isOpen ? "Close" : "Open";

    private void Awake()
    {
        if (hinge == null)
        {
            Debug.LogError("DoorController: hinge is not assigned.", this);
            enabled = false;
            return;
        }

        // Remember how the door looks when closed.
        closedRotation = hinge.localRotation;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }

    public void Interact()
    {
        // Ignore presses while the door is still moving.
        if (!enabled || isMoving) return;

        Quaternion target = isOpen ? closedRotation : closedRotation * Quaternion.Euler(0f, GetOpenAngle(), 0f);
        isOpen = !isOpen;
        StartCoroutine(RotateDoor(target));
    }

    // Picks the swing direction so the door opens away from the player.
    private float GetOpenAngle()
    {
        if (player == null) return openAngle;

        // Positive = player is in front of the door (hinge's forward side).
        float side = Vector3.Dot(player.position - hinge.position, hinge.forward);

        // A positive Y rotation swings the panel towards the hinge's back side,
        // so if the player is in front we use +angle, otherwise -angle.
        return side >= 0f ? openAngle : -openAngle;
    }

    // Smoothly rotates the hinge to the target rotation over animationDuration.
    private IEnumerator RotateDoor(Quaternion target)
    {
        isMoving = true;
        Quaternion start = hinge.localRotation;
        float time = 0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            // SmoothStep makes the motion start and end gently instead of snapping.
            float t = Mathf.SmoothStep(0f, 1f, time / animationDuration);
            hinge.localRotation = Quaternion.Slerp(start, target, t);
            yield return null; // wait for the next frame
        }

        hinge.localRotation = target;
        isMoving = false;
    }
}
