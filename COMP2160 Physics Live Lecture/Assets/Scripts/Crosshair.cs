/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{

#region Parameters
    [SerializeField] float mouseScale = 5;
#endregion 

#region Connected Objects
#endregion

#region Components
#endregion

#region State
    private Actions actions;
    private Plane worldPlane;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        actions = new Actions();
        worldPlane = new Plane(transform.forward, transform.position);
    }

    void OnEnable()
    {
        actions.Crosshair.Enable();
    }

    void OnDisable()
    {
        actions.Crosshair.Disable();
    }
#endregion 

#region Update
    void Update()
    {
        Vector2 mousePos = actions.Crosshair.Aim.ReadValue<Vector2>();        
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        float t = 0;
        worldPlane.Raycast(ray, out t);
        transform.position = ray.GetPoint(t);
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
