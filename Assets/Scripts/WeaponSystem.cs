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

    [Header("Audio")]
    public AudioClip gunshotClip;

    [Header("Recoil")]
    public Vector3 recoilAmount = new Vector3(0, 0.2f, -0.1f);
    public float recoilSpeed = 10f;

    [Header("Sprint Movement")]
    public float sprintBobSpeed = 5f;
    public Vector3 sprintBobAmount = new Vector3(0.05f, 0.05f, 0.05f);

    private bool isAiming = false;
    private bool isSprinting = false;
    private int currentAmmo;
    private float nextFireTime;

    private Transform weaponHolder;
    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Vector3 targetRecoilPosition;
    private float bobTimer;

    void Start()
    {
        currentAmmo = maxAmmo;
        weaponHolder = transform;
        originalPosition = weaponHolder.localPosition;
        defaultPosition = weaponHolder.localPosition;
        targetRecoilPosition = originalPosition;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = gunshotClip;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        HandleShooting();
        HandleAiming();
        HandleReloading();
        HandleRecoil();
        HandleSprinting();
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

        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, Mathf.Infinity, hitMask))
        {
            if (hitEffectPrefab && hit.collider != null)
            {
                Vector3 offsetPosition = hit.point + hit.normal * 0.01f;
                GameObject hitEffect = Instantiate(hitEffectPrefab, offsetPosition, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 60f);
            }
        }
        else
        {
            Debug.Log("Missed! Raycast did not hit anything.");
        }

        targetRecoilPosition = weaponHolder.localPosition - recoilAmount;
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

    void HandleRecoil()
    {
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetRecoilPosition, Time.deltaTime * recoilSpeed);
        targetRecoilPosition = Vector3.Lerp(targetRecoilPosition, originalPosition, Time.deltaTime * recoilSpeed);
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

    void HandleSprinting()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isAiming;

        if (isSprinting)
        {
            bobTimer += Time.deltaTime * sprintBobSpeed;
            float bobX = Mathf.Sin(bobTimer) * sprintBobAmount.x;
            float bobY = Mathf.Cos(bobTimer * 2f) * sprintBobAmount.y;
            float bobZ = Mathf.Sin(bobTimer * 0.5f) * sprintBobAmount.z;

            Vector3 bobPosition = originalPosition + new Vector3(bobX, bobY, bobZ);
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, bobPosition, Time.deltaTime * aimSpeed);
        }
        else
        {
            bobTimer = 0f;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * aimSpeed);
        }
    }
}
