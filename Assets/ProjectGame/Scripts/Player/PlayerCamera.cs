using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 0.1f;

    private float xRotation;
    private float yRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * sensitivity;
        float mouseY = mouseDelta.y * sensitivity;


        // 横回転
        yRotation += mouseX;


        // 縦回転
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(
            xRotation,
            -90f,
            90f
        );


        transform.localRotation =
            Quaternion.Euler(
                xRotation,
                yRotation,
                0f
            );
    }
}