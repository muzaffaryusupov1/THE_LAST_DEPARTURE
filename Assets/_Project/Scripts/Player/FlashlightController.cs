using UnityEngine;
using UnityEngine.InputSystem;

// Turns the flashlight on and off with F.
// Put this on the Flashlight GameObject (the one with the Spot Light).
public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Spot Light of the flashlight. If empty, the Light on this GameObject is used.")]
    [SerializeField] private Light flashlight;

    [Header("Settings")]
    [Tooltip("Is the flashlight on when the game starts?")]
    [SerializeField] private bool startOn = false;

    [Header("Battery")]
    [Tooltip("Battery at the start of the game, in percent (0-100).")]
    [Range(0f, 100f)]
    [SerializeField] private float startBattery = 100f;

    [Tooltip("How many percent of battery are used per second while the flashlight is on.")]
    [SerializeField] private float drainRate = 1f;

    private InputAction toggleAction;
    private bool isOn;
    private float battery; // current battery, 0-100

    public bool IsOn => isOn;
    public float Battery => battery;

    private void Awake()
    {
        if (flashlight == null) flashlight = GetComponent<Light>();

        // "Flashlight" (F key) comes from the project-wide Input Actions asset (Player map).
        toggleAction = InputSystem.actions.FindAction("Player/Flashlight");

        if (flashlight == null || toggleAction == null)
        {
            Debug.LogError("FlashlightController: Light or 'Player/Flashlight' action is missing.", this);
            enabled = false;
            return;
        }

        battery = startBattery;

        // Apply the starting state right away, so the light is never "wrong" for a frame.
        // (SetOn refuses to turn on if the battery is empty.)
        SetOn(startOn);
    }

    private void OnEnable()
    {
        toggleAction?.Enable();
    }

    private void Update()
    {
        // True only on the frame F goes down: one press = one toggle.
        if (toggleAction.WasPressedThisFrame())
        {
            SetOn(!isOn);
        }

        // Drain the battery only while the light is on.
        if (isOn)
        {
            battery -= drainRate * Time.deltaTime; // Time.deltaTime = seconds since last frame
            if (battery <= 0f)
            {
                battery = 0f;
                SetOn(false); // empty battery: the light goes out
            }
        }
    }

    private void SetOn(bool on)
    {
        // An empty battery cannot turn the light on.
        if (on && battery <= 0f) on = false;

        isOn = on;
        flashlight.enabled = on; // disable the Light component, not the whole GameObject
    }
}
