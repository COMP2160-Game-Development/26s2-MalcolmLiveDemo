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
    private List<ContactPoint2D> contacts;s
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

        actions = new Actions();
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

            rigidbody.AddForce(jumpSpeed * rigidbody.mass * Vector2.up, ForceMode2D.Impulse);
        }
    }

    private bool IsOnGround()
    {
        foreach (ContactPoint2D cp in contacts)
        {
            if (cp.normal.x < Mathf.Sin(groundAngle))
            {
                
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

    }
#endregion
}
