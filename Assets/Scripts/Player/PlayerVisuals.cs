using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }
}
