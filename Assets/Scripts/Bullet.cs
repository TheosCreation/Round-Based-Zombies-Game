using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //get the bullet damage from the gun
    //also make do more damage when headshot
    //public bulletDamage;
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag("Enemy"))
        {
            hitTransform.GetComponent<ZombieHealth>().TakeDamage(10f);
        }
        Destroy(gameObject);
    }
}
