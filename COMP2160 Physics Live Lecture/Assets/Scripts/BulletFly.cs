/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BulletFly : MonoBehaviour
{

#region Parameters
    [SerializeField, Unit(Units.MetersPerSecond)] private float speed = 5;
#endregion 

#region Connected Objects
#endregion

#region Components
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
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
        
        transform.parent = null; // detach from gun
    }
#endregion 

#region Update
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.right, Space.Self);
    }
#endregion

#region FixedUpdate
    void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(gameObject);
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
