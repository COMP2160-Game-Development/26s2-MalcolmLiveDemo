/**
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6.3
 */

using System.Collections.Generic;
using UnityEngine;

public class SquareFactory : MonoBehaviour
{

#region Parameters
    [SerializeField] private Square squarePrefab;
    [SerializeField] private float spawnPeriod = 1f;
#endregion 

#region Components
#endregion

#region State
    private float spawnTimer;
    private int squareCounter = 0;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        spawnTimer = spawnPeriod;
    }
#endregion 

#region Update
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {   
            CreateSquare();
            spawnTimer += spawnPeriod;
        }
    }

    private Square CreateSquare()
    {
        Square square = Instantiate(squarePrefab);
        square.gameObject.name = $"Square {squareCounter}";
        squareCounter++;

        square.transform.parent = transform;
        square.transform.localPosition = Vector2.zero;

        square.OnDie += OnSquareDie;

        return square;
    }

    private void OnSquareDie(Square square)
    {
        // unsub from this event
        square.OnDie -= OnSquareDie;
        Destroy(square.gameObject);
    }
#endregion 

}
