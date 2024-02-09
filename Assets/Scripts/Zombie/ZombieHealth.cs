using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    [SerializeField] private RoundSpawner roundSpawner;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        if(health <= 0)
        {
            RoundSpawner.onZombieKilled.Invoke();
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
