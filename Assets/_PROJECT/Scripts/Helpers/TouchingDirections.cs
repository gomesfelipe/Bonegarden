using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{

    [Header("References")]
    protected Rigidbody _rb;
    protected CapsuleCollider collider;
    protected Animator _anim;
    [SerializeField] protected LayerMask groundLayer, ceilingLayer;
    //public ContactFilter2D contactFilter;

    [SerializeField] protected bool _isOnWall = false, _isOnCeiling = false;
    [SerializeField, Range(0, 1f)] protected float offset = 0.05f;
    [SerializeField] protected float distToGround;

    protected void Awake()
    {
        _rb ??= GetComponent<Rigidbody>();
        collider ??= GetComponent<CapsuleCollider>();
        _anim = GetComponentInChildren<Animator>();
        distToGround = collider.bounds.extents.y;
    }

    public bool IsGrounded(){
            // Adjust the Raycast origin point to be slightly above the bottom of the collider
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            return Physics.Raycast(origin, Vector3.down, out RaycastHit hit, distToGround + 0.2f);
    }
    public bool IsOnCeiling()
    {
        return Physics.SphereCast(_rb.position, collider.bounds.extents.x - offset, -Vector3.down, out RaycastHit hit, offset + 0.1f);
    }

}
