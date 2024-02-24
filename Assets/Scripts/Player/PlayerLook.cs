using Unity.Netcode;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private Player player;
    private float yRotation = 0f;

    private void Start()
    {
        player = GetComponent<Player>();
        
    }
    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        yRotation -= (mouseY * Time.deltaTime) * player.ySensitivity;
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);
        player.cam.transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * player.xSensitivity);
    }
}
