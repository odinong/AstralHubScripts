using UnityEngine;
using GorillaLocomotion;
using BepInEx;

public class WASDScript : MonoBehaviour
{
    public float speed = 12f;
    public float jumpForce = 9f;
    public float sensitivity = 0.5f;
    public bool wasden = true;
    public bool rightclick = false;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Rect winrect = new Rect(20, 20, 250, 250);
    private bool showGui = true;

    private void Update()
    {
        if (UnityInput.Current.GetKeyDown(KeyCode.RightShift))
        {
            showGui = !showGui;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (wasden && GTPlayer.Instance != null)
        {
            movementhand();
        }
    }

    private void movementhand()
    {
        Rigidbody rb = GTPlayer.Instance.bodyCollider.attachedRigidbody;
        Transform head = GTPlayer.Instance.headCollider.transform;

        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = Vector3.zero;
        bool isMoving = false;

        if (UnityInput.Current.GetKey(KeyCode.W)) { moveDirection += forward; isMoving = true; }
        if (UnityInput.Current.GetKey(KeyCode.S)) { moveDirection -= forward; isMoving = true; }
        if (UnityInput.Current.GetKey(KeyCode.A)) { moveDirection -= right; isMoving = true; }
        if (UnityInput.Current.GetKey(KeyCode.D)) { moveDirection += right; isMoving = true; }

        if (isMoving)
        {
            Vector3 targetVel = moveDirection.normalized * speed;
            targetVel.y = rb.velocity.y;
            rb.velocity = targetVel;
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (UnityInput.Current.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(GTPlayer.Instance.bodyCollider.transform.position, Vector3.down, 1.5f);
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (!showGui && wasden && GTPlayer.Instance != null)
        {
            bool canLook = !rightclick || UnityInput.Current.GetMouseButton(1);
            if (canLook && e.isMouse && (e.type == EventType.MouseMove || e.type == EventType.MouseDrag))
            {
                rotationX += e.delta.x * sensitivity;
                rotationY += e.delta.y * sensitivity;
                rotationY = Mathf.Clamp(rotationY, -90f, 90f);

                GTPlayer.Instance.headCollider.transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
            }
        }

        if (showGui)
        {
            winrect = GUI.Window(0, winrect, DrawWindow, "WASD Settings (R-Shift)");
        }
    }

    private void DrawWindow(int windowID)
    {
        wasden = GUILayout.Toggle(wasden, "Enable WASD");
        rightclick = GUILayout.Toggle(rightclick, "Right Click to Look");
        
        GUILayout.Label("Speed: " + speed.ToString("F1"));
        speed = GUILayout.HorizontalSlider(speed, 1f, 50f);

        GUILayout.Label("Jump Force: " + jumpForce.ToString("F1"));
        jumpForce = GUILayout.HorizontalSlider(jumpForce, 1f, 30f);

        GUILayout.Label("Sensitivity: " + sensitivity.ToString("F2"));
        sensitivity = GUILayout.HorizontalSlider(sensitivity, 0.01f, 2.0f);

        if (GUILayout.Button("Reset Defaults"))
        {
            speed = 12f;
            jumpForce = 9f;
            sensitivity = 0.5f;
            rightclick = false;
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
