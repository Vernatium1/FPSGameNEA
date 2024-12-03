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

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f; // Amount of recoil
    public float recoilSpeed = 5f; // Speed of recoil return to default position

    private bool isAiming = false;
    private int currentAmmo;
    private floatFireTime;

    private Transform weaponHolder;
    private AudioSource audioSource;

    // Recoil management
    private Vector3 recoilPosition;

    void Start()
    {
        currentAmmo = maxAmmo;
        weaponHolder = transform;
        defaultPosition = weaponHolder.localPosition;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = gunshotClip;
        audioSource.playOnAwake = false;

        recoilPosition = defaultPosition;
    }

    void Update()
    {
        HandleShooting        HandleAiming();
        HandleReloading();

        // Apply recoil movement (return to default position)
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, recoilPosition, Time.deltaTime * recoilSpeed);
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
       Ammo--;

        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && gunshotClip) audioSource.Play();

        if (gunshotPrefab)
        {
            Instantiate(gunshotPrefab, muzzlePoint.position, muzzlePoint.rotation);
        }
        
        // Apply recoil effect: offset the gun position upward
        recoilPosition = defaultPosition + new Vector3(0f, recoilAmount, 0f);

        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, hit, Mathf.Infinity, hitMask))
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

    // to spawn bullet hole decal at the impact point
    void SpawnBulletHole(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Instantiate the bullet hole at the hit point and orient it correctly
        GameObject bulletHole = Instantiate(bulletHolePrefab, hitPoint, Quaternion.LookRotation(hitNormal));
        
        // Optionally, destroy the bullet hole after a short time to avoid clutter
        Destroy(bulletHole, 5f);
    }

    void HandleAiming()
    {
        if (Input.GetButton("2"))
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
        current = maxAmmo;
        recoilPosition = defaultPosition; // Reset recoil when reloading
        Debug.Log("Reloaded!");
    }
}
