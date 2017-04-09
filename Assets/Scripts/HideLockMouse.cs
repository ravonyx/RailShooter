using UnityEngine;
using System.Collections;

public class HideLockMouse : MonoBehaviour
{

    public bool lockCursor = true;

    void Update()
    {
        // pressing esc toggles between hide/show
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lockCursor = !lockCursor;
        }
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        GetComponent<CameraController>().enabled = lockCursor;

        Cursor.visible = !lockCursor;
    }
}