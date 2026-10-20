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
using Sirenix.Utilities;

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
    [SerializeField, Unit(Units.Second)] private float hoverDuration = 0.5f; 

    
    [Header("History")]
    [SerializeField, Unit(Units.Second)] private float historyDuration = 1f;
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
    private enum JumpState { OnGround, RisingWithGravity, Hovering, FallingWithGravity, FallingNoGravity };
    private JumpState state = JumpState.FallingWithGravity;

    private float hoverStartTime = float.NegativeInfinity;

    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();

    private List<float> timeHistory;
    private List<Vector3> positionHistory;
#endregion

#region Properties
    public List<float> TimeHistory => timeHistory;
    public List<Vector3> PositionHistory => positionHistory;
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

        timeHistory = new List<float>();
        positionHistory = new List<Vector3>();

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

    void Start()
    {
        state = JumpState.FallingWithGravity;
    }        

#endregion 

#region Update
    void Update()
    {
        // read input in the Update frame
        move = actions.PlayerMove.Move.ReadValue<Vector2>();

        timeHistory.Add(Time.time);
        positionHistory.Add(transform.position);

        while (timeHistory[0] < Time.time - historyDuration)
        {
            timeHistory.RemoveAt(0);
            positionHistory.RemoveAt(0);
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        jumpPressedTime = Time.time;
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        Jump();
        MoveHorizontally();

        contacts.Clear();
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
        switch (state)
        {
            case JumpState.OnGround:
                if (IsOnGround())
                {
                    if (Time.time - jumpPressedTime <= jumpBufferTime)
                    {
                        state = JumpState.RisingWithGravity;

                        jumpPressedTime = float.NegativeInfinity;
                        float jumpSpeed = Mathf.Sqrt(-2 * jumpHeight * Physics2D.gravity.y);
                        rigidbody.linearVelocityY = jumpSpeed;                 
                    }
                }
                else
                {
                    state = JumpState.FallingWithGravity;
                }

                break;

            case JumpState.RisingWithGravity:

                rigidbody.gravityScale = 1;

                if (rigidbody.linearVelocity.y <= 0)
                {
                    state = JumpState.Hovering;
                    rigidbody.linearVelocityY = 0;
                    rigidbody.gravityScale = 0;

                    hoverStartTime = Time.time;
                }

                break;

            case JumpState.Hovering:

                rigidbody.linearVelocityY = 0;
                rigidbody.gravityScale = 0;

                if (Time.time - hoverStartTime > hoverDuration)
                {
                    state = JumpState.FallingWithGravity;
                    rigidbody.gravityScale = 1;                 
                }

                break;

            case JumpState.FallingWithGravity:
                rigidbody.gravityScale = 1;

                if (IsOnGround())
                {                    
                    state = JumpState.OnGround;
                    rigidbody.linearVelocityY = 0;
                }
                else  if (rigidbody.linearVelocity.y <= maxFallSpeed)
                {
                    state = JumpState.FallingNoGravity;
                    rigidbody.gravityScale = 0;
                    rigidbody.linearVelocityY = maxFallSpeed;
                }

                break;

            case JumpState.FallingNoGravity:
                rigidbody.gravityScale = 0;
                rigidbody.linearVelocityY = maxFallSpeed;

                if (IsOnGround())
                {                    
                    state = JumpState.OnGround;
                    rigidbody.linearVelocityY = 0;
                }

                break;
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

        Gizmos.color = Color.black;

        for (int i = 0; i < positionHistory.Count - 1; i++)
        {
            switch (state)
            {
                case JumpState.OnGround:
                    Gizmos.color = Color.black;
                    break;
                case JumpState.RisingWithGravity:
                    Gizmos.color = Color.red;
                    break;
                case JumpState.Hovering:
                    Gizmos.color = Color.orange;
                    break;
                case JumpState.FallingWithGravity:
                    Gizmos.color = Color.green;
                    break;
                case JumpState.FallingNoGravity:
                    Gizmos.color = Color.blue;
                    break;
            }

            Gizmos.DrawLine(positionHistory[i], positionHistory[i+1]);


        }
    }
#endregion
}
