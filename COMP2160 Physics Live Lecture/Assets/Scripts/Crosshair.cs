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
    [SerializeField] private float mouseScale = 5;
#endregion 

#region Connected Objects
#endregion

#region Components
#endregion

#region State
    private Actions actions;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        Cursor.visible = false;
        actions = new Actions();
    }

    void OnEnable()
    {
        actions.Crosshair.Enable();
        Cursor.visible = false;
        Debug.Log("[Crosshair.OnEnable]");
    }

    void OnDisable()
    {
        actions.Crosshair.Disable();
        Cursor.visible = true;
    }
#endregion 

#region Update
    void LateUpdate()
    {
        Vector3 mousePos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 delta = actions.Crosshair.Aim.ReadValue<Vector2>();
        mousePos += delta * mouseScale;

        Vector3 viewPos = Camera.main.ScreenToViewportPoint(mousePos);
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        transform.position = Camera.main.ViewportToWorldPoint(viewPos);
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
