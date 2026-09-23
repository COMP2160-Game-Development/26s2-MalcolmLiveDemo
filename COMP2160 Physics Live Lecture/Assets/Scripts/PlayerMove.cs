/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [Header("Vertical movement")]
    [SerializeField, Unit(Units.MetersPerSecond)] private float maxFallSpeed = -20;
    [Header("Horizontal movement")]
    [SerializeField, Unit(Units.MetersPerSecond)] private float moveSpeed = 10;
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
    private Actions actions;
    private Vector2 move;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        actions = new Actions();

        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void OnEnable()
    {
        actions.PlayerMove.Enable();   
    }

    void OnDisable()
    {
        actions.PlayerMove.Disable();           
    }
#endregion 

#region Update
    void Update()
    {
        move = actions.PlayerMove.Move.ReadValue<Vector2>();
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {
        ControlGravity();
        MoveHorizontally();
    }

    private void ControlGravity()
    {
        Vector2 velocity = rigidbody.linearVelocity;

        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
            rigidbody.linearVelocity = velocity;
            rigidbody.gravityScale = 0;
        }
        else
        {
            rigidbody.gravityScale = 1;
        }            
    }

    private void MoveHorizontally()
    {
        Vector2 velocity = rigidbody.linearVelocity;
        velocity.x = move.x * moveSpeed;
        rigidbody.linearVelocity = velocity;
    }
#endregion

#region Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Don't run in the editor
            return;
        }
    }
#endregion
}
