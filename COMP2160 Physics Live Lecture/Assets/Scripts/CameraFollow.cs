/**
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6000.0.53f1
 */

using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class CameraFollow : MonoBehaviour
{

#region Parameters
    [SerializeField, Range(0,1)] private float lerpFactor = 0.75f;
    [SerializeField, Unit(Units.Second)] private float delay = 0.5f;
#endregion 

#region Connected Objects
    private PlayerMove player;
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
        player = FindAnyObjectByType<PlayerMove>();

        if (player == null)
        {
            throw new NullReferenceException("Cannot find player");
        }
    }    
#endregion 

#region Update
    void LateUpdate()   // update after all
    {
        Vector3 position = transform.position;

        int i = player.TimeHistory.Count - 1;

        while (player.TimeHistory[i] > Time.time - delay && i > 0)
        {
            i--;
        }

        position.x = player.PositionHistory[i].x;

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
