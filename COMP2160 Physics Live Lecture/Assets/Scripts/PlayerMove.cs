/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [Header("Horizontal movement")]
    [SerializeField, Unit(Units.MetersPerSecond)] private float maxSpeed = 5;
    [SerializeField, Unit(Units.Degree)] private float maxGroundAngle = 5;
    
    [Header("Vertical movement")]
    [SerializeField, Unit(Units.MetersPerSecond)] private float maxFallSpeed = -10;
    [SerializeField, Unit(Units.Meter)] private float jumpHeight = 1.5f; 
    [SerializeField, Unit(Units.Second)] private float jumpBufferTime = 0.1f; 
    
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private Vector2 move;
    private float jumpPressedTime = float.NegativeInfinity;

    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;

        actions = new Actions();
    }

    void OnEnable()
    {
        actions.PlayerMove.Enable();
        actions.PlayerMove.Jump.performed += OnJump;

        rigidbody.gravityScale = 1;
    }

    void OnDisable()
    {
        actions.PlayerMove.Disable();        
        actions.PlayerMove.Jump.performed -= OnJump;

        rigidbody.gravityScale = 0;
    }
#endregion 

#region Update
    void Update()
    {
        // read input in the Update frame
        move = actions.PlayerMove.Move.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumpPressedTime = Time.time;
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        ControlGravity();
        Jump();
        MoveHorizontally();

        contacts.Clear();
    }

    private void ControlGravity()
    {
        if (rigidbody.linearVelocity.y <= maxFallSpeed)
        {
            rigidbody.gravityScale = 0;

            Vector2 velocity = rigidbody.linearVelocity;
            velocity.y = maxFallSpeed;
            rigidbody.linearVelocity = velocity;
        }
        else
        {
            rigidbody.gravityScale = 1;
        }
    }

    private bool IsOnGround()
    {
        foreach (ContactPoint2D cp in contacts)
        {
            if (IsGroundNormal(cp))
            {   
                return true;
            }
        }
        return false;
    }

    private void Jump()
    {
        if (IsOnGround() && Time.time - jumpPressedTime <= jumpBufferTime)
        {
            jumpPressedTime = float.NegativeInfinity;

            float jumpSpeed = Mathf.Sqrt(-2 * jumpHeight * Physics2D.gravity.y);
            rigidbody.linearVelocityY = jumpSpeed;                 
        }
    }

    private void MoveHorizontally()
    {
        Vector2 velocity = rigidbody.linearVelocity;
        float targetSpeed = maxSpeed * move.x;
        velocity.x = targetSpeed;
        rigidbody.linearVelocity = velocity;        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        contacts.AddRange(collision.contacts);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        contacts.AddRange(collision.contacts);
    }

#endregion

#region Private Methods
    private bool IsGroundNormal(ContactPoint2D cp)
    {
        return cp.normal.y > 0 && Mathf.Abs(cp.normal.x) <= Mathf.Sin(maxGroundAngle * Mathf.Deg2Rad);
    }

#endregion


#region Gizmos
    void OnDrawGizmos()
    {
        if (rigidbody == null)
        {
            return;
        }

        Handles.color = Color.white;
        Handles.Label(transform.position, $"v = {rigidbody.linearVelocity}");

        Gizmos.color = Color.red;

        foreach (ContactPoint2D cp in contacts)
        {
            Gizmos.color = IsGroundNormal(cp) ? Color.blue : Color.red;                                
            Gizmos.DrawSphere(cp.point, 0.1f);            
            Gizmos.DrawLine(cp.point, cp.point + cp.normal);            
        }
    }
#endregion
}
