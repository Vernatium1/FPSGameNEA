using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int bulletDamage;

    private void OnCollisionEnter(Collision ObjectWeHit)
    {
        if (ObjectWeHit.gameObject.CompareTag("Target"))
        {
            CreateBulletImpactEffect(ObjectWeHit);

            Destroy(gameObject);
        }

        if (ObjectWeHit.gameObject.CompareTag("Enemy"))
        {
            if (ObjectWeHit.gameObject.GetComponent<Enemy>().isDead == false)
            {
                ObjectWeHit.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }

            CreateBloodSprayEffect(ObjectWeHit);

            Destroy(gameObject);
        }

    }

    private void CreateBloodSprayEffect(Collision ObjectWeHit)
    {
        ContactPoint contact = ObjectWeHit.contacts[0];

        GameObject bloodSprayPrefab = Instantiate(
            GlobalReferences.Instance.bloodSprayEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        bloodSprayPrefab.transform.SetParent(ObjectWeHit.gameObject.transform);
    }

    void CreateBulletImpactEffect(Collision ObjectWeHit) 
    {
        ContactPoint contact = ObjectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(ObjectWeHit.gameObject.transform);
    }
}
