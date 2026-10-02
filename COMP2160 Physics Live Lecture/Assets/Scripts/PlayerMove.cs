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
    [SerializeField, Unit(Units.MetersPerSecond)] private float jumpSpeed = 5;
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
    private bool jumpPressed = false;
    private List<ContactPoint2D> contacts;
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
        jumpPressed = true;
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
        if (jumpPressed)
        {
            jumpPressed = false;

            if (IsOnGround())
            {
                rigidbody.AddForce(jumpSpeed * rigidbody.mass * Vector2.up, ForceMode2D.Impulse);            
            }
        }
    }

    private bool IsOnGround()
    {
        Debug.Log($"[PlayerMove.IsOnGround] # contacts = {contacts.Count}");

        foreach (ContactPoint2D cp in contacts)
        {
            Debug.Log($"[PlayerMove.IsOnGround] n = {cp.normal} : {Mathf.Sin(groundAngle)}");

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

    }
#endregion
}
