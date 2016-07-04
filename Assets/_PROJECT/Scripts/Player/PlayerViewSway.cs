#region NAMESPACES

using UnityEngine;
using System.Collections;

#endregion

#region WeaponSway Class

public class PlayerViewSway : MonoBehaviour
{

    public Vector2 ExternalVector;

    // PRIVATE BUT EDITABLE IN THE INSPECTOR
    // IF YOU WANT TO EDIT THESE EXTERNALLY THE USE PROPERTY ACCESSOR
    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("Tilt angle smoothing")]
    private float _smoothGun = 2f;

    [SerializeField]
    [Range(0.1f, 90f)]
    [Tooltip("Strafing tilt angle")]
    private float _tiltAngle = 4.25f;

    [SerializeField]
    [Range(0.01f, 0.1f)]
    [Tooltip("Minimum amount of movement")]
    private float _amount = 0.025f;

    [SerializeField]
    [Range(0.01f, 0.1f)]
    [Tooltip("Minimum amount of movement")]
    private float _maxAmount = 0.075f;

    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("Directional sway smoothing")]
    private float _smooth = 2.75f;

    [SerializeField]
    [Range(0.1f, 5f)]
    [Tooltip("Rotational sway smoothing")]
    private float _smoothRotation = 1.5f;

    // EXAMPLE PROPERTY ACCESSOR

    public float tiltAngle
    {
        get // USE THIS TO READ THE VALUE - CAN BE REMOVED
        {
            return _tiltAngle;
        }
        set // USE THIS TO ASSIGN A VALUE - CAN BE REMOVED TO
            // MAKE READ ONLY WITH GET STILL ABOVE
        {
            _tiltAngle = value;
        }
    }

    // PRIVATE SCRIPT ONLY VARIABLES
    private float _factorX;
    private float _factorY;
    private float _tiltGun;
    private float _tiltZ;
    private float _tiltX;
    private Quaternion _target;
    private Quaternion _target2;
    private Vector3 _finalPos;
    private Vector3 _startingPos;
    private Transform _transform;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    void Start()
    {
        // ON START WE NEED TO STORE OUR CURRENT LOCAL POSITION
        _startingPos = _transform.localPosition;
    }

    void Update()
    {
        // DONT NEED TO RETRIVE THIS VALUE UNLESS WE ARE MOVING IN THAT DIRECTION
        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
        {
            // GET OUR INPUT VALUE AND STORE IT
            _tiltGun = Input.GetAxisRaw("Horizontal") * _tiltAngle * -1;

            // SAVE IT AS A ROTATIONAL VALUE
            _target2 = Quaternion.Euler(0, 0, _tiltGun);

            // THEN APPLY IT USING SLERP, LOWER THE SMOOTHING THE FASTER THE TRANSISTION WILL BE
            _transform.localRotation
                = Quaternion.Slerp(_transform.localRotation,     // STARTING ROTATION
                _target2,                                       // TARGET ROTATION
                Time.deltaTime * _smoothGun);                   // TIME TO GET THERE
        }

        // GET OUR AXIS FOR POSITION
        _factorX = (-Input.GetAxisRaw("Mouse X") - ExternalVector.x) * _amount;
        _factorY = (-Input.GetAxisRaw("Mouse Y") + ExternalVector.y * 2.5f) * _amount;

        // GET OUR AXIS AGAIN FOR ROTATION
        _tiltZ = (Input.GetAxis("Mouse X") + ExternalVector.x) * _tiltAngle;
        _tiltX = (Input.GetAxis("Mouse Y") - ExternalVector.y * 2.5f) * _tiltAngle;

        // CLAMP THEM BY A CERTIAN RANGE TO PREVENT GLITCHING - BOTH AXIS
        Clamp(_factorX, -_maxAmount, _maxAmount);
        Clamp(_factorY, -_maxAmount, _maxAmount);

        // TO GET OUR FINAL SWAY POSITION LERP BETWEEN OUR STARTING AND THE AMOUNT OF NEW MOVEMENT WE DID
        _finalPos
            = new Vector3(_startingPos.x + _factorX,        // ADD OUT X MOVEMENT TO OUR X AXIS
            _startingPos.y + _factorY,                      // ADD OUR Y MOVEMENT TO OUR Y AXIS
            _startingPos.z);                                // THIS IS 0 UNLESS ITS MODIFIED ELSEWHERE

        // LERP BETWEEN STARTING POSITION AND FINAL POSITION BY OUR SMOOTHING OVER TIME
        _transform.localPosition
            = Vector3.Lerp(_transform.localPosition,         // WHERE WE STARTED
            _finalPos,                                      // WHERE WE WANT TO GO
            Time.deltaTime * _smooth);                      // BY HOW FAST

        // STORE OUR TARGET IN A VARIABLE
        _target
            = Quaternion.Euler(_tiltX,                      // AMOUNT OF X ROTATION
            0,                                              // AMOUNT OF Y ROTATION
            _tiltZ);                                        // AMOUNT OF Z ROTATION

        // THEN FINALLY APPLY OUR ROTATION
        _transform.localRotation
            = Quaternion.Slerp(_transform.localRotation,     // STARTING ROATAION
            _target,                                        // WHERE WE WANT TO GO
            Time.deltaTime * _smoothRotation);              // HOW LONG TO GET THERE
    }

    /// <summary>
    /// STATIC METHOD CAN BE CALLED FROM ANYWHERE OUTSIDE THIS SCRIPT BY USING
    /// WeaponSway.Clamp(amount, min, max);
    /// </summary>
    /// <param name="amount">Value you want to restrct</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum Value</param>
    /// <returns></returns>
    public static float Clamp(float amount, float min, float max)
    {
        // BIND INPUT AMOUNT TO RANGE BETWEEN MIN AND MAX
        return Mathf.Clamp(amount, min, max);
    }

}

#endregion