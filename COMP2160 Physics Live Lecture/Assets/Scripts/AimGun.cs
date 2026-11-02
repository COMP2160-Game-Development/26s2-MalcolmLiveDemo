/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class AimGun : MonoBehaviour
{

#region Parameters
    [SerializeField, Unit(Units.DegreesPerSecond)] private float rotationSpeed;
#endregion 

#region Connected Objects
    [SerializeField, Required] private Transform target;
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
    }
#endregion 

#region Update
    void Update()
    {
        Vector2 dir = target.position - transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.right, dir);

        Vector3 eulerAngles = transform.eulerAngles;
        float currentAngle = eulerAngles.z;

        float diff = ((targetAngle - currentAngle + 180) % 360 + 360) % 360 - 180; 
        float rotation = Math.Sign(diff) * rotationSpeed * Time.deltaTime;;
        if (Math.Abs(rotation) > Math.Abs(diff))
        {
            rotation = diff;
        }

        eulerAngles.z = currentAngle + rotation;
        transform.eulerAngles = eulerAngles;
    }
#endregion

#region FixedUpdate
    void FixedUpdate()
    {        
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
