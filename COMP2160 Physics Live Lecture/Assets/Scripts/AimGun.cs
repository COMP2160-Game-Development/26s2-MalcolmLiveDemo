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
    [SerializeField, AssetsOnly] private BulletFly bulletPrefab;
    [SerializeField, Unit(Units.Second)] private float cooldownPeriod = 0.1f;
#endregion 

#region Connected Objects
    [SerializeField, Required, ChildGameObjectsOnly] private Transform crosshair;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform muzzle;
#endregion

#region Components
#endregion

#region State
    private Actions actions;
    private float cooldownTimer = float.NegativeInfinity;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        actions = new Actions();
        crosshair.transform.parent = null; // detach
    }

    void OnEnable()
    {
        actions.Gun.Enable();        
    }

    void OnDisable()
    {
        actions.Gun.Disable();        
    }
#endregion 

#region Update
    void Update()
    {
        Aim();
        Fire();
    }

    private void Aim()
    {
        Vector2 dir = crosshair.position - transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.right, dir);

        Vector3 eulerAngles = transform.eulerAngles;
        float currentAngle = eulerAngles.z;

        // wrap the angle to the range [-180, 180]
        float diff = ((targetAngle - currentAngle + 180) % 360 + 360) % 360 - 180; 
        float rotation = Math.Sign(diff) * rotationSpeed * Time.deltaTime;;
        if (Math.Abs(rotation) > Math.Abs(diff))
        {
            rotation = diff;
        }
        eulerAngles.z = currentAngle + rotation;
        transform.eulerAngles = eulerAngles;
    }

    private void Fire()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (actions.Gun.Fire.WasPressedThisFrame() && cooldownTimer <= 0)
        {
            BulletFly bullet = Instantiate(bulletPrefab, muzzle);
            cooldownTimer = cooldownPeriod;
        }
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
