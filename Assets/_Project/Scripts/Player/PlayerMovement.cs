using UnityEngine;
using UnityEngine.InputSystem;

// Moves the player with WASD and applies gravity.
// Needs a CharacterController on the same GameObject.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Walking speed in units (meters) per second.")]
    [SerializeField] private float walkSpeed = 2.5f;

    [Header("Gravity")]
    [Tooltip("How strongly the player is pulled down. Real-world gravity is -9.81.")]
    [SerializeField] private float gravity = -9.81f;

    [Tooltip("Small downward push while grounded, so the player stays stuck to the floor.")]
    [SerializeField] private float groundedPull = -2f;

    private CharacterController controller;
    private InputAction moveAction;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // "Move" comes from the project-wide Input Actions asset
        // (Assets/InputSystem_Actions.inputactions -> Player map).
        moveAction = InputSystem.actions.FindAction("Player/Move");
        if (moveAction == null)
        {
            Debug.LogError("PlayerMovement: 'Player/Move' action not found in the Input Actions asset.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void Update()
    {
        // 1. Read WASD as a 2D value: x = left/right (A/D), y = forward/back (W/S).
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 2. Convert it to a direction relative to where the player is facing.
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f); // diagonal movement is not faster

        // 3. Gravity: keep pulling down, but reset when standing on the ground.
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedPull;
        }
        verticalVelocity += gravity * Time.deltaTime;

        // 4. Combine horizontal movement and gravity, then move.
        //    CharacterController.Move handles collisions with walls and floor.
        Vector3 velocity = move * walkSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
