using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public float aimSpeed = 5f;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public LayerMask hitMask;

    [Header("Aiming")]
    public Vector3 aimPosition;
    private Vector3 defaultPosition;

    [Header("Effects")]
    public GameObject hitEffectPrefab;
    public GameObject gunshotPrefab;
    public GameObject bulletHolePrefab; // Bullet hole decal prefab

    [Header("Audio")]
    public AudioClip gunshotClip;

    private bool isAiming = false;
    private int currentAmmo;
    private float nextFireTime;

    private Transform weaponHolder;
    private AudioSource audioSource;

    void Start()
    {
        currentAmmo = maxAmmo;
        weaponHolder = transform;
        defaultPosition = weaponHolder.localPosition;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = gunshotClip;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        HandleShooting();
        HandleAiming();
        HandleReloading();
    }

    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && gunshotClip) audioSource.Play();

        if (gunshotPrefab)
 if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, Mathf.Infinity, hitMask))
        {
            // Spawn the hit effect (e.g., sparks, dust, etc.)
            if (hitEffectPrefab)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 2f); 
            }

            // Spawn bullet hole at the point of impact
            if (bulletHolePrefab)
            {
                SpawnBulletHole(hit.point, hit.normal);
            }
        }
    }

    // Function to spawn bullet hole decal at the impact point
    void SpawnBulletHole(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Instantiate the bullet hole at the hit point and orient it correctly
        GameObject bulletHole = Instantiate(bulletHolePrefab, hitPoint, Quaternion.LookRotation(hitNormal));
        
        // Optionally, destroy the bullet hole after a short time to avoid clutter
        Destroy(bulletHole, 5f);
    }

    void HandleAiming()
    {
        if (Input.GetButton("Fire2"))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        Vector3 targetPosition = isAiming ? aimPosition : defaultPosition;
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPosition, Time.deltaTime * aimSpeed);
    }

    void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Reloaded!");
    }
}
