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
    [SerializeField, Unit(Units.MetersPerSecond)] private float jumpHeight = 1.5f;
    [SerializeField, Unit(Units.Second)] private float jumpBufferTime = 0.1f;
    [SerializeField, Unit(Units.Degree)] private float groundAngle = 10;
    [SerializeField, Unit(Units.Second)] private float hoverPeriod = 1;
#endregion 

#region Connected Objects
#endregion

#region Components
    new private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private Vector2 move;
    private List<ContactPoint2D> contacts;
    private float lastJumpTime = float.NegativeInfinity;
    private Vector2? lastJumpPos = null;    

    private enum State { OnGround, Rising, Hovering, FallingGravity, FallingNoGravity };
    private State jumpState = State.FallingGravity;
    private float hoverTimer;

    private int historyLength = 500; // = 10s * 50fps
    private List<Vector2> positionHistory;
    private List<State> jumpStateHistory;
#endregion

#region Properties
    private float JumpSpeed
    {
        get
        {
            // v^2 = u^2 + 2gh
            // Let v = 0
            // u^2 = -2gh
            // u = sqrt(-2gh) 

            return Mathf.Sqrt(-2 * Physics2D.gravity.y * jumpHeight);
        }
    }

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
        positionHistory = new List<Vector2>();
        jumpStateHistory = new List<State>();
    }

    void OnEnable()
    {
        actions.PlayerMove.Enable();
        rigidbody.gravityScale = 1;

        actions.PlayerMove.Jump.performed += OnJumpPressed;
    }

    void OnDisable()
    {
        actions.PlayerMove.Disable();        
        rigidbody.gravityScale = 0;

        actions.PlayerMove.Jump.performed -= OnJumpPressed;
    }
#endregion 

#region Update
    void Update()
    {
        // read input in the Update frame
        move = actions.PlayerMove.Move.ReadValue<Vector2>();
    }

    void OnJumpPressed(InputAction.CallbackContext ctx)
    {
        lastJumpTime = Time.time;
        lastJumpPos = rigidbody.position;
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        MoveHorizontally();
        Jump();

        positionHistory.Add(rigidbody.position);
        jumpStateHistory.Add(jumpState);

        while (positionHistory.Count > historyLength)
        {
            positionHistory.RemoveAt(0);
            jumpStateHistory.RemoveAt(0);
        }

        contacts.Clear();
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
        switch (jumpState)
        {
            case State.OnGround:
                if (IsOnGround())
                {
                    if (Time.time <= lastJumpTime + jumpBufferTime)
                    {
                        jumpState = State.Rising;

                        // use up the jump
                        lastJumpTime = float.NegativeInfinity;

                        // In 3D we would use ForceMode.VelocityChange but this isn't
                        // available in the 2D engine
                        rigidbody.AddForce(JumpSpeed * rigidbody.mass * Vector2.up, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    jumpState = State.FallingGravity;
                }
                break;

            case State.Rising:
                if (rigidbody.linearVelocityY < 0)
                {
                    jumpState = State.Hovering;
                    hoverTimer = hoverPeriod;
                    rigidbody.linearVelocityY = 0;
                    rigidbody.gravityScale = 0;
                }
                break;

            case State.Hovering:
                hoverTimer -= Time.fixedDeltaTime;

                if (hoverTimer <= 0)
                {
                    jumpState = State.FallingGravity;
                    rigidbody.gravityScale = 1;
                }
                break;

            case State.FallingGravity:
                if (IsOnGround())
                {
                    jumpState = State.OnGround;
                }
                else if (rigidbody.linearVelocity.y <= maxFallSpeed)
                {
                    jumpState = State.FallingNoGravity;
                    rigidbody.gravityScale = 0;

                    Vector2 velocity = rigidbody.linearVelocity;
                    velocity.y = maxFallSpeed;
                    rigidbody.linearVelocity = velocity;
                }
                break;

            case State.FallingNoGravity:
                if (IsOnGround())
                {
                    jumpState = State.OnGround;
                    rigidbody.gravityScale = 1;
                }
                else
                {
                    Vector2 velocity = rigidbody.linearVelocity;
                    velocity.y = maxFallSpeed;
                    rigidbody.linearVelocity = velocity;
                }
                break;
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

        for (int i = 1; i < positionHistory.Count; i++)
        {
            switch (jumpStateHistory[i])
            {
                case State.OnGround:
                    Gizmos.color = Color.red;
                    break;
                case State.Rising:
                    Gizmos.color = Color.orange;
                    break;
                case State.Hovering:
                    Gizmos.color = Color.yellow;
                    break;
                case State.FallingGravity:
                    Gizmos.color = Color.green;
                    break;
                case State.FallingNoGravity:
                    Gizmos.color = Color.blue;
                    break;
            }

            Gizmos.DrawLine(positionHistory[i-1], positionHistory[i]);
        }
    }
#endregion
}
