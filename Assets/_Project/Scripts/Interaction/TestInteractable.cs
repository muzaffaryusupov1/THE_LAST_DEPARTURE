using UnityEngine;

// A simple test object for the interaction system.
// Pressing E while looking at it writes a message to the Console.
public class TestInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Test object interacted successfully!", this);
    }
}
