/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using Sirenix.OdinInspector;

public class CameraFollow : MonoBehaviour
{

#region Parameters
    [SerializeField, Required] private Transform player;
    [SerializeField, Required] private Transform crosshair;
    [SerializeField] private float decay = 0.5f;
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
    }
#endregion 

#region Update
    void LateUpdate()
    {    
        Vector3 position = transform.position;
        position.x = Mathf.Lerp(player.position.x, crosshair.position.x, 0.5f);
        transform.position = position;
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
