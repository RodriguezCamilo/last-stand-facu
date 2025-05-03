using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private RectTransform crosshairUI;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        crosshairUI.position = Input.mousePosition;
    }
}
