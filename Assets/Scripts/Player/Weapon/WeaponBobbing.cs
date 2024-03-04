using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    private PlayerStateMachine playerState;
    private PlayerMotor playerMotor;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    Vector3 bobPosition;
    Vector3 startPosition;
    Quaternion startRotation;

    public float bobExaggeration;
    
    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;
    private void Start()
    {
        playerState = GetComponentInParent<PlayerStateMachine>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }
    void Update()
    {
        
        BobOffset();
        BobRotation();
        if (playerState.isAiming)
        {
            bobPosition = Vector3.zero;
            bobEulerRotation = Vector3.zero;
        }
        CompositePositionRotation();
    }
    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        float movementMagnitude = playerMotor.currentVelocity.magnitude;
        speedCurve += Time.deltaTime * (playerState.isGrounded ? movementMagnitude * bobExaggeration : 1f) + 0.01f;
        

        bobPosition.x = (curveCos * bobLimit.x * (playerState.isGrounded ? 1 : 0)) - (movementMagnitude * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (playerMotor.currentVelocity.normalized.y * travelLimit.y);
        bobPosition.z = -(movementMagnitude * travelLimit.z);
    }

    void BobRotation()
    {
        float movementMagnitude = playerMotor.currentVelocity.magnitude;

        bobEulerRotation.x = movementMagnitude != 0 ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRotation.y = (movementMagnitude != 0 ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (movementMagnitude != 0 ? multiplier.z * curveCos * playerMotor.currentVelocity.x : 0);
    }

}
