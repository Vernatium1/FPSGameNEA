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
    public GameObject bulletHolePrefab;

    [Header("Audio")]
    public AudioClip gunshotClip;

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;
    public float recoilSpeed = 5f;

    private bool isAiming = false;
    private int currentAmmo;
    private floatFireTime;

    private Transform weaponHolder;
    private AudioSource audioSource;

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
        HandleShooting();        
        HandleAiming();
        HandleReloading();

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
        
        recoilPosition = defaultPosition + new Vector3(0f, recoilAmount, 0f);

        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, hit, Mathf.Infinity, hitMask))
        {
            if (hitEffectPrefab)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 2f); 
            }

            if (bulletHolePrefab)
            {
                SpawnBulletHole(hit.point, hit.normal);
            }
        }
    }

    void SpawnBulletHole(Vector3 hitPoint, Vector3 hitNormal)
    {
        GameObject bulletHole = Instantiate(bulletHolePrefab, hitPoint, Quaternion.LookRotation(hitNormal));
        
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
        recoilPosition = defaultPosition;
        Debug.Log("Reloaded!");
    }
}
