/**
 *
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 6.3
 */

using System.Collections.Generic;
using UnityEngine;

public class SquareFactoryPooling : MonoBehaviour
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
    private List<Square> pool;
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

        pool = new List<Square>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateSquare();
        }
    }

    private Square CreateSquare()
    {
        // instanitate a square and add it to the available pool
        Square square = Instantiate(squarePrefab);
        square.gameObject.name = $"Square {squareCounter}";
        squareCounter++;

        square.transform.parent = transform;
        square.gameObject.SetActive(false);
        pool.Add(square);

        return square;
    }
#endregion 

#region Update
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {   
            GetSquareFromPool();
            spawnTimer += spawnPeriod;
        }
    }

    private Square GetSquareFromPool()
    {
        if (pool.Count == 0)
        {
            // not enough in the pool, create a new one
            CreateSquare();
        }

        Square s = pool[0];
        s.gameObject.SetActive(true);
        pool.RemoveAt(0);

        s.OnDie += OnSquareDie;
        s.transform.localPosition = Vector2.zero;      
        return s;
    }

    private void OnSquareDie(Square square)
    {
        square.OnDie -= OnSquareDie;
        square.gameObject.SetActive(false);
        pool.Add(square);
    }
#endregion 

}
