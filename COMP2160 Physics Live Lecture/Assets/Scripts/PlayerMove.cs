/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{

#region Parameters
    [SerializeField] private float maxFallSpeed = -20;
    [SerializeField] private float gravity = -10;
#endregion 

#region Connected Objects
#endregion

#region Components
    private Rigidbody2D rigidbody;
#endregion

#region State
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;
    }
#endregion 

#region Update
    void Update()
    {
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
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
