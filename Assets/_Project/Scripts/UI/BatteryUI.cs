using TMPro;
using UnityEngine;

// Shows the flashlight battery as text, e.g. "Battery: 87%".
// Put this on the TextMeshPro text object in the HUD.
public class BatteryUI : MonoBehaviour
{
    [Tooltip("The flashlight whose battery is shown.")]
    [SerializeField] private FlashlightController flashlight;

    [Tooltip("The text to write into. If empty, the text on this GameObject is used.")]
    [SerializeField] private TMP_Text batteryText;

    private int shownPercent = -1; // -1 = nothing shown yet

    private void Awake()
    {
        if (batteryText == null) batteryText = GetComponent<TMP_Text>();

        if (flashlight == null || batteryText == null)
        {
            Debug.LogError("BatteryUI: flashlight or text is not assigned.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // Round up, so the UI only shows 0% when the battery is really empty.
        int percent = Mathf.CeilToInt(flashlight.Battery);

        // Only rewrite the text when the number changes (not every frame).
        if (percent != shownPercent)
        {
            shownPercent = percent;
            batteryText.text = "Battery: " + percent + "%";
        }
    }
}
