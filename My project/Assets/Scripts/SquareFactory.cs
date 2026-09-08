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
    [SerializeField] private int poolSize = 10;
#endregion 

#region Components
#endregion

#region State
    private float spawnTimer;
    private List<Square> available;
    private List<Square> inUse;
#endregion

#region Properties
#endregion

#region Events
#endregion

#region Init & Destroy
    void Awake()
    {
        spawnTimer = spawnPeriod;

        available = new List<Square>();

        for (int i = 0; i < poolSize; i++)
        {
            Square s = CreateSquare();
            s.gameObject.SetActive(false);
            available.Add(s);
        }
    }

    private Square CreateSquare()
    {
        Square square = Instantiate(squarePrefab);
        square.transform.parent = transform;

        return square;
    }
#endregion 

#region Update
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {   
            if (available.Count > 0)
            {
                GetSquareFromPool();
            }
            spawnTimer += spawnPeriod;
        }
    }

    private Square GetSquareFromPool()
    {
        Square s = available[0];
        s.gameObject.SetActive(true);
        available.RemoveAt(0);          
        s.transform.localPosition = Vector2.zero;      
        return s;
    }

#endregion 

}
