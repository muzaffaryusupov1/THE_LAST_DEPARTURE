// The interaction contract.
// Any object the player can interact with (door, radio, switch, document...)
// implements this interface and decides for itself what Interact() does.
public interface IInteractable
{
    // Called once when the player looks at this object and presses E.
    void Interact();
}
