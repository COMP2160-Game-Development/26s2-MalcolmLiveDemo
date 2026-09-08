using UnityEngine;

public class Square : MonoBehaviour
{
#region Parameters
    [SerializeField] private float lifetime = 5;
#endregion

#region State
    private float lifeRemaining;
#endregion

#region Events
    public delegate void DieHandler(Square square);
    public event DieHandler OnDie;
#endregion

    void OnEnable()
    {
        lifeRemaining = lifetime;
    }

    void Update()
    {
        lifeRemaining -= Time.deltaTime;

        if (lifeRemaining <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // tell the factory that I have died
        OnDie?.Invoke(this);
    }
}
