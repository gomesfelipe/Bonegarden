using System;
using UnityEngine;
using System.Collections;

public class PlayerMouseLook : MonoBehaviour
{

    public string HorizontalAxis;
    public string VerticalAxis;
    public float MouseSensitivity;
    public Transform HorzTransform;
    public Transform VertTransform;

    private float _mouseYaw;
    private float _mousePitch;

    // Use this for initialization
    void Start()
    {

    }

    private void DoMouseInput()
    {
        _mouseYaw += (Input.GetAxisRaw(HorizontalAxis) * MouseSensitivity);
        _mouseYaw %= 360f;

        _mousePitch += -(Input.GetAxisRaw(VerticalAxis) * MouseSensitivity);
        _mousePitch = Mathf.Clamp(_mousePitch, -85f, +85f);
    }

    private void ApplyRotation()
    {
        Vector3 euler = HorzTransform.localRotation.eulerAngles;
        euler.y = _mouseYaw;
        HorzTransform.localRotation = Quaternion.Lerp(HorzTransform.localRotation, Quaternion.Euler(euler), 0.5f);

        euler = VertTransform.localRotation.eulerAngles;
        euler.x = _mousePitch;
        VertTransform.localRotation = Quaternion.Lerp(VertTransform.localRotation, Quaternion.Euler(euler), 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        DoMouseInput();
        ApplyRotation();
    }

}
