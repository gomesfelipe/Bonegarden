using UnityEngine;
using System.Collections;

public class PlayerViewBobbing : MonoBehaviour
{

    public PlayerMotor Motor;
    public float bobDistance;
    public float bobSpeed;
    public float midPoint;

    private Transform _transform;
    private float timer;
    private float waveSlice;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    void DoBob()
    {
        Vector3 localPosition = _transform.localPosition;

        Vector3 curDir = Motor.GetRelativeSpeed().normalized;

        if ((Mathf.Abs(curDir.x) == 0 && Mathf.Abs(curDir.z) == 0) || !Motor.IsGrounded())
        {
            timer = 0.0f;
        }
        else
        {
            waveSlice = Mathf.Sin(timer);
            if (Motor.IsDucking())
            {
                timer = timer + bobSpeed / 2;
            }
            else
            {
                timer = timer + bobSpeed;
            }
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveSlice != 0)
        {
            float translateChange = waveSlice * bobDistance;
            float totalAxes = Mathf.Abs(curDir.x) + Mathf.Abs(curDir.z);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            localPosition.y = midPoint + translateChange;
        }
        else
        {
            localPosition.y = midPoint;
        }

        _transform.localPosition = Vector3.Lerp(_transform.localPosition, localPosition, 0.2f);
    }

    void Update()
    {
        DoBob();
    }

}
