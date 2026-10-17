public class Rope : Interactable
{
    protected override void Interact()
    {
        gameObject.SetActive(false);
    }
}