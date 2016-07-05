using UnityEngine;
using System.Collections;
using DG.Tweening;
using DigitalRuby.ThunderAndLightning;

public struct Cmd
{
    public float Forwardmove;
    public float Rightmove;
}

public class PlayerMotor : MonoBehaviour
{

    [Header("Setup")]
    public Transform HeadTransform;
    public Transform TiltTransform;
    public Transform CameraTransform;
    public SphereCollider LowerCollider;
    public BoxCollider UpperCollider;
    public PlayerViewSway Sway;
    public Transform DashTransform;
    public Transform DashZRotationTransform;
    public ParticleSystem DashParticles;

    [Header("Camera params")]
    public float CameraHeightStanding = 0.615f;
    public float CameraHeightDuck = 0;
    public float CameraUnduckDistanceCheck = 1f;
    [Range(50, 120)]
    public float CameraFOV = 75;

    [Header("Movement Params")]
    public float Gravity = 500;
    public float Friction = 8;
    public float MoveSpeed = 300;
    public float DuckSpeed = 100f;
    public float RunAcceleration = 14f;
    public float RunDeacceleration = 100f;
    public float AirAcceleration = 0;
    public float AirDeacceleration = 0f;
    public float airControl = 100f;
    public float SideStrafeAcceleration = 100f;
    public float SideStrafeSpeed = 100;
    public float JumpSpeed = 210f;
    public float MoveScale = 1f;
    public float JumpGracePeriod = 0.5f;
    [Header("Dodge params")]
    public float DodgeSpeed = 10f;
    public float DodgeHeight = 0.4f;
    public float DodgeFovIncrease = 15f;
    public float DodgeTimeInterval = 0.5f;
    [Header("Tilt params")]
    public float AirHeadTilt = 1f;
    public float GroundHeadTilt = 0.5f;
    public float AirHeadTiltRate = 0.15f;
    public float GroundHeadTiltRate = 0.3f;
    [Header("World Params")]
    public LayerMask WorldMask;
    [Header("Trimp Params")]
    [Range(1, 100)]
    public float MinTrimpSpeedPercentage;
    [Header("Walljump Params")]
    [Range(0, 1)]
    public float WalljumpHeight;
    public float WalljumpMaxMagnitude;


    private Transform _transform;
    private Rigidbody _rb;
    private PlayerInput _input;
    private PlayerMouseLook _mouse;
    private bool _wishJump;
    private bool _canJump = true;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private Cmd _cmd;
    private bool _canDoubleJump;
    private bool _hasJumped;
    private bool _isDucking;
    private float _currentMoveSpeed;
    private Collision _lastCollision;
    private float _lastGraceTime;
    private bool _kegFix = false;
    private bool _duckFix = false;
    private Camera _camera;
    private Tweener _dodgeDuckTween;
    private bool _isDashing;
    private float _timeSinceLastDash;
    private Collider _lastWalljumpCollider;
    private float _currentWalljumpHeight;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _mouse = GetComponent<PlayerMouseLook>();
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
        _camera = CameraTransform.GetComponent<Camera>();
        _camera.fieldOfView = CameraFOV;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public Vector3 GetRelativeSpeed()
    {
        return HeadTransform.InverseTransformDirection(_playerVelocity);
    }

    public bool IsDucking()
    {
        return _isDucking;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    void MoveCharacter()
    {
        Command cmd = _input.GetCommand();

        _cmd = new Cmd();
        _cmd.Forwardmove = cmd.Movement.y;
        _cmd.Rightmove = cmd.Movement.x;

        _currentMoveSpeed = MoveSpeed;

        QueueJump();
        QueueDuck(cmd.Duck.Held);

        if (_isGrounded)
        {
            _canDoubleJump = true;
            GroundMove();
        }
        else
        {
            AirMove();
        }

        _rb.velocity = _playerVelocity * Time.fixedDeltaTime;

        SetHeadPosition();
    }

    public void OnCollisionEnter(Collision col)
    {
        if (IsGround(col) && Time.time > _lastGraceTime + JumpGracePeriod)
        {
            _isGrounded = true;
            _hasJumped = false;
            _currentWalljumpHeight = WalljumpHeight;
        }

        if (IsCeiling(col))
        {
            _playerVelocity.y = -1;
        }
    }

    public void OnCollisionStay(Collision col)
    {
        if (IsGround(col) && Time.time > _lastGraceTime + JumpGracePeriod)
        {
            _isGrounded = true;
            _lastCollision = col;
            _lastWalljumpCollider = null;
        }

        if (IsCeiling(col))
        {
            _playerVelocity.y -= 1;
        }

        if (IsWall(col) && !_isGrounded && _canDoubleJump && _wishJump && _canJump && col.collider != _lastWalljumpCollider)
        {
            _lastWalljumpCollider = col.collider;
            _playerVelocity = ((col.contacts[0].normal + (Vector3.up * _currentWalljumpHeight)) *
                                Mathf.Clamp(new Vector3(_playerVelocity.x, 0, _playerVelocity.z).magnitude, 0, WalljumpMaxMagnitude));
            _wishJump = false;
            _canJump = false;
            _currentWalljumpHeight /= 1.25f;
        }

        if (Vector3.Dot(col.contacts[0].normal, _playerVelocity) < 0)
        {
            _playerVelocity -= col.contacts[0].normal * Vector3.Dot(col.contacts[0].normal, _playerVelocity);
            ApplyFriction(1);
        }
    }

    public void OnCollisionExit(Collision col)
    {
        _isGrounded = false;
    }

    private bool IsGround(Collision col)
    {
        return Vector3.Dot(col.contacts[0].normal, Vector3.up) > 0.8f;
    }

    private bool IsCeiling(Collision col)
    {
        return Vector3.Dot(col.contacts[0].normal, Vector3.down) > 0.8f;
    }

    private bool IsWall(Collision col)
    {
        return (!IsGround(col) && !IsCeiling(col));
    }

    void SlopeFix()
    {
        if (_lastCollision == null)
        {
            return;
        }

        Vector3 temp = Vector3.Cross(_lastCollision.contacts[0].normal, _playerVelocity);
        Vector3 newDir = Vector3.Cross(temp, _lastCollision.contacts[0].normal);

        Debug.DrawRay(_lastCollision.contacts[0].point, newDir * 3, Color.red, 3);

        _playerVelocity = newDir;
    }

    void SlopeFix(Vector3 newVel)
    {
        if (_lastCollision == null)
        {
            return;
        }

        Vector3 temp = Vector3.Cross(_lastCollision.contacts[0].normal, _playerVelocity + newVel);
        Vector3 newDir = Vector3.Cross(temp, _lastCollision.contacts[0].normal);

        Debug.DrawRay(_lastCollision.contacts[0].point, newDir * 3, Color.red, 3);

        _playerVelocity = newDir;
    }

    bool UnduckCheck()
    {
        Ray ray = new Ray(_transform.position, Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction * CameraUnduckDistanceCheck, Color.green, 2f);

        return Physics.Raycast(ray, CameraUnduckDistanceCheck, WorldMask);
    }

    void QueueJump()
    {
        bool button = _input.GetCommand().Jump.Held;

        if (button && !_isDucking)
        {
            if (!_wishJump && _canJump)
            {
                _kegFix = false;
                _wishJump = true;
            }
        }
        if (!button)
        {
            _canJump = true;
            _wishJump = false;
        }

        //_wishJump = button;
    }

    void QueueDuck(bool queue)
    {
        if (!_isGrounded)
        {
            _isDucking = false;
            return;
        }

        if (!_isDashing)
        {
            if (queue)
            {
                _isDucking = true;
                if (!_duckFix && _isGrounded)
                {
                    _duckFix = true;
                    _playerVelocity.x *= 1.25f;
                    _playerVelocity.z *= 1.25f;
                }
            }
            else
            {
                if (!UnduckCheck())
                {
                    _isDucking = false;
                    _duckFix = false;
                }
                else
                {
                    _isDucking = true;
                }
            }
        }

        UpperCollider.enabled = !_isDucking;

        if (_isDucking)
        {
            _currentMoveSpeed = DuckSpeed;
        }
    }

    void ApplyFriction(float t)
    {
        Vector3 vec = _playerVelocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        // Only if the player is on the ground then apply friction
        if (_isGrounded)
        {
            control = speed < RunDeacceleration ? RunDeacceleration : speed;
            drop = control * Friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        _playerVelocity.x *= newspeed;
        // playerVelocity.y *= newspeed;
        _playerVelocity.z *= newspeed;
    }

    void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(_playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        _playerVelocity.x += accelspeed * wishdir.x;
        _playerVelocity.z += accelspeed * wishdir.z;
    }

    void GroundMove()
    {
        Vector3 wishdir;
        Vector3 wishvel;

        // Do not apply friction if the player is queueing up the next jump
        if (!_wishJump)
        {
            //if (_isDucking && _playerVelocity.magnitude > MathHelper.Percentage(MoveSpeed, DuckslideMinSpeedPercentage))
            if (_isDashing)
            {
                ApplyFriction(0.03f);
            }
            else
            {
                ApplyFriction(1.0f);
            }
        }
        else
        {
            ApplyFriction(0f);
        }

        float scale = CmdScale();

        wishdir = new Vector3(_cmd.Rightmove, 0, _cmd.Forwardmove);
        wishdir = HeadTransform.TransformDirection(wishdir);
        wishdir.Normalize();

        float wishspeed = wishdir.magnitude;
        wishspeed *= _currentMoveSpeed;

        Accelerate(wishdir, wishspeed, RunAcceleration);

        // Reset the gravity velocity
        //_playerVelocity.y = 0;

        SlopeFix();

        if (_wishJump)
        {
            _canJump = false;
            //Debug.Break();
            _lastGraceTime = Time.time;
            _playerVelocity.y = JumpSpeed;
            if (!_kegFix)
            {
                _kegFix = true;
            }
            else
            {
                //Debug.Log(_playerVelocity.);
                _wishJump = false;
            }
            _hasJumped = true;
            _lastCollision = null;
        }

        DoHeadTilt(GroundHeadTilt, GroundHeadTiltRate);
    }

    void AirMove()
    {
        float accel;

        float scale = CmdScale();

        Vector3 wishdir = new Vector3(_cmd.Rightmove, 0, _cmd.Forwardmove);
        wishdir = HeadTransform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= _currentMoveSpeed;

        wishdir.Normalize();
        wishspeed *= scale;

        // CPM: Air control
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(_playerVelocity, wishdir) < 0)
        {
            accel = AirDeacceleration;
        }
        else
        {
            accel = AirAcceleration;
        }
        // If the player is ONLY strafing left or right
        if (_cmd.Forwardmove == 0 && _cmd.Rightmove != 0)
        {
            if (wishspeed > SideStrafeSpeed)
            {
                wishspeed = SideStrafeSpeed;
            }
            accel = SideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if (airControl > 0f)
        {
            AirControl(wishdir, wishspeed2);
        }


        // Apply gravity
        _playerVelocity.y -= Gravity * Time.deltaTime;

        DoHeadTilt(AirHeadTilt, AirHeadTiltRate);
    }

    void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (_cmd.Forwardmove == 0 || wishspeed == 0)
            return;

        zspeed = _playerVelocity.y;
        _playerVelocity.y = 0;
        // Next two lines are equivalent to idTech's VectorNormalize()
        speed = _playerVelocity.magnitude;
        _playerVelocity.Normalize();

        dot = Vector3.Dot(_playerVelocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            _playerVelocity.x = _playerVelocity.x * speed + wishdir.x * k;
            _playerVelocity.y = _playerVelocity.y * speed + wishdir.y * k;
            _playerVelocity.z = _playerVelocity.z * speed + wishdir.z * k;

            _playerVelocity.Normalize();
        }

        _playerVelocity.x *= speed;
        _playerVelocity.y = zspeed; // Note this line
        _playerVelocity.z *= speed;
    }

    float CmdScale()
    {
        int max;
        float total;
        float scale;

        max = (int)Mathf.Abs(_cmd.Forwardmove);
        if (Mathf.Abs(_cmd.Rightmove) > max)
        {
            max = (int)Mathf.Abs(_cmd.Rightmove);
        }
        if (max == 0)
        {
            return 0f;
        }

        total = Mathf.Sqrt(_cmd.Forwardmove * _cmd.Forwardmove + _cmd.Rightmove * _cmd.Rightmove);
        scale = _currentMoveSpeed * max / (MoveScale * total);

        return scale;
    }

    void DoHeadTilt(float magnitude, float rate)
    {
        Vector3 rot = TiltTransform.localRotation.eulerAngles;

        float value = CameraTransform.InverseTransformDirection(_rb.velocity).x;

        rot.z = -value * magnitude;
        TiltTransform.localRotation = Quaternion.Lerp(TiltTransform.localRotation, Quaternion.Euler(rot), rate);
    }

    void DoTrimp()
    {
        if (MathHelper.Percentage(_currentMoveSpeed, _playerVelocity.magnitude) > MinTrimpSpeedPercentage)
        {

        }
    }

    void SetHeadPosition()
    {
        Vector3 pos = HeadTransform.localPosition;

        if (_isDucking)
        {
            pos.y = CameraHeightDuck;
        }
        else
        {
            pos.y = CameraHeightStanding;
        }

        HeadTransform.localPosition = Vector3.Lerp(HeadTransform.localPosition, pos, 0.25f);
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void Update()
    {
        Debug.DrawRay(_transform.position, _playerVelocity.normalized * 4, Color.magenta, 2);

        Command cmd = _input.GetCommand();

        if (cmd.Stunt.Down)
        {
            if (!_isGrounded)
            {
                if (_hasJumped && _canDoubleJump)
                {
                    if (_playerVelocity.magnitude > 0)
                    {
                        Vector3 dir = HeadTransform.InverseTransformDirection(_playerVelocity.normalized);

                        float frontFlip = 0;
                        float sideFlip = 0;

                        if (Mathf.Abs(dir.z) >= Mathf.Abs(dir.x))
                        {
                            if (dir.z > 0)
                            {
                                frontFlip = 1;
                            }
                            else if (dir.z < 0)
                            {
                                frontFlip = -1;
                            }
                        }
                        else
                        {
                            if (dir.x > 0)
                            {
                                sideFlip = -1;
                            }
                            else if (dir.x < 0)
                            {
                                sideFlip = 1;
                            }
                        }

                        _canDoubleJump = false;
                        _mouse.enabled = false;
                        _wishJump = false;
                        _playerVelocity.y = JumpSpeed * 1.25f;
                        LowerCollider.enabled = false;
                        Sway.ExternalVector = new Vector2(sideFlip, frontFlip);
                        CameraTransform.DOLocalRotate(new Vector3(360 * frontFlip, 0, 360 * sideFlip), 0.75f, RotateMode.LocalAxisAdd)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(
                                delegate
                                {
                                    _mouse.enabled = true;
                                    LowerCollider.enabled = true;
                                    Sway.ExternalVector = Vector2.zero;
                                });
                    }
                }
            }
            else if (!_isDucking && !_isDashing && Time.time > _timeSinceLastDash + DodgeTimeInterval)
            {
                Vector3 dir = new Vector3(cmd.Movement.x, 0, cmd.Movement.y).normalized;
                if (dir != Vector3.zero)
                {
                    SlopeFix(HeadTransform.TransformDirection(dir) * DodgeSpeed);
                    _timeSinceLastDash = Time.time;

                    DashTransform.rotation =
                        Quaternion.LookRotation(dir);
                    DashZRotationTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
                    _camera.fieldOfView = CameraFOV;

                    _isDucking = true;
                    _isDashing = true;

                    DOTween.Sequence().PrependInterval(0.2f).OnComplete(delegate
                    {
                        _isDashing = false;
                        _isDucking = false;
                    }).Play();

                    _camera.DOFieldOfView(CameraFOV + DodgeFovIncrease, 0.1f).OnComplete(delegate
                    {
                        _camera.DOFieldOfView(CameraFOV, 0.1f);
                    });
                    DashParticles.Play();
                }
            }
        }
    }

}
