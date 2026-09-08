using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private float lifetime = 5;

    private float lifeRemaining;

    void Awake()
    {
        lifeRemaining = lifetime;
    }

    void Update()
    {
        lifeRemaining -= Time.deltaTime;

        if (lifeRemaining <= 0)
        {
            Destroy(gameObject);
        }
    }
}
