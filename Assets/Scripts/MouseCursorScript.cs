using UnityEngine;

public class MouseCursorScript : MonoBehaviour
{
    Vector2 mousePos;
    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }
}
