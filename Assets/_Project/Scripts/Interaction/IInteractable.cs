// The interaction contract.
// Any object the player can interact with (door, radio, switch, document...)
// implements this interface and decides for itself what Interact() does.
public interface IInteractable
{
    // The action word shown in the prompt: "Press E to " + InteractionText.
    // Example: a closed door returns "Open", an open door returns "Close".
    string InteractionText { get; }

    // Called once when the player looks at this object and presses E.
    void Interact();
}
