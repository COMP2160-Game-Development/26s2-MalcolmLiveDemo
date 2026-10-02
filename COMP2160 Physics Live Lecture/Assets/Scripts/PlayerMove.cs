/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [SerializeField, Unit(Units.MetersPerSecond)] private float maxFallSpeed = -10;
    [SerializeField, Unit(Units.MetersPerSecond)] private float maxSpeed = 5;
    [Header("Jumping")]   
    [SerializeField, Unit(Units.MetersPerSecond)] private float jumpSpeed = 5;
    [SerializeField, Unit(Units.Second)] private float jumpBufferTime = 0.1f;
    [SerializeField, Unit(Units.Degree)] private float groundAngle = 10;
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private Vector2 move;
    private List<ContactPoint2D> contacts;
    private float lastJumpTime = float.NegativeInfinity;
    private Vector2? lastJumpPos = null;
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
        rigidbody.mass = 1;
        rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;

        actions = new Actions();
        contacts = new List<ContactPoint2D>();
    }

    void OnEnable()
    {
        actions.PlayerMove.Enable();
        rigidbody.gravityScale = 1;

        actions.PlayerMove.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        actions.PlayerMove.Disable();        
        rigidbody.gravityScale = 0;

        actions.PlayerMove.Jump.performed -= OnJump;
    }
#endregion 

#region Update
    void Update()
    {
        // read input in the Update frame
        move = actions.PlayerMove.Move.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        lastJumpTime = Time.time;
        lastJumpPos = rigidbody.position;
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        ControlGravity();
        MoveHorizontally();
        Jump();

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

    private void MoveHorizontally()
    {
        Vector2 velocity = rigidbody.linearVelocity;
        float targetSpeed = maxSpeed * move.x;
        velocity.x = targetSpeed;
        rigidbody.linearVelocity = velocity;        
    }

    private void Jump()
    {
        if (IsOnGround() && Time.time <= lastJumpTime + jumpBufferTime)
        {
            // use up the jump
            lastJumpTime = float.NegativeInfinity;

            // In 3D we would use ForceMode.VelocityChange but this isn't
            // available in the 2D engine
            rigidbody.AddForce(jumpSpeed * rigidbody.mass * Vector2.up, ForceMode2D.Impulse);            
        }
    }

    private bool IsOnGround()
    {
        foreach (ContactPoint2D cp in contacts)
        {
            if (Mathf.Abs(cp.normal.x) <= Mathf.Sin(groundAngle * Mathf.Deg2Rad))
            {
                return true;
            }
        }

        return false;
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
            Gizmos.DrawSphere(cp.point, 0.1f);
            Gizmos.DrawLine(cp.point, cp.point + cp.normal);
        }

        if (lastJumpPos != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(lastJumpPos.Value, 0.1f);
        }
    }
#endregion
}
