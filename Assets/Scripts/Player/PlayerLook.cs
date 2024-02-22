using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    private Player player;
    private float yRotation = 0f;

    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    private void Start()
    {
        player = GetComponent<Player>();
        if(!IsOwner)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }
    public void ProcessLook(Vector2 input)
    {
        if(!IsOwner)
        {
            return;
        }
        float mouseX = input.x;
        float mouseY = input.y;

        yRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);
        player.cam.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) *xSensitivity);
    }
}
