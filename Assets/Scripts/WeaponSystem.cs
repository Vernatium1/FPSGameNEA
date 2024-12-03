using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.1f; // Time between shots
    public int maxAmmo = 30; // Ammo capacity
    public float aimSpeed = 5f; // Speed of aiming transition
    public Transform muzzlePoint; // Position for raycast origin
    public ParticleSystem muzzleFlash; // Muzzle flash effect
    public LayerMask hitMask; // LayerMask to specify what the bullets can hit

    [Header("Aiming")]
    public Vector3 aimPosition; // Position for aiming down sights
    private Vector3 defaultPosition;

    [Header("Effects")]
    public GameObject hitEffectPrefab; // Prefab for hit effect (decal or particle)
    public GameObject gunshotPrefab; // Prefab for gunshot muzzle flash effect

    [Header("Audio")]
    public AudioClip gunshotClip; // Audio clip for gunshot sound

    [Header("Recoil")]
    public Vector3 recoilAmount = new Vector3(0, 0.2f, -0.1f); // Recoil movement
    public float recoilSpeed = 10f; // Speed of returning from recoil

    [Header("Sprint Movement")]
    public float sprintBobSpeed = 5f; // Speed of the weapon bobbing during sprint
    public Vector3 sprintBobAmount = new Vector3(0.05f, 0.05f, 0.05f); // Amount of the weapon bobbing during sprint in X, Y, and Z axes

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
        // Initialize weapon settings
        currentAmmo = maxAmmo;
        weaponHolder = transform; // Assume the script is on the weapon holder
        originalPosition = weaponHolder.localPosition;
        defaultPosition = weaponHolder.localPosition;
        targetRecoilPosition = originalPosition;

        // Set up the AudioSource
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

        // Play muzzle flash and sound
        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && gunshotClip) audioSource.Play();

        // Perform raycast
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, Mathf.Infinity, hitMask))
        {
            // Ensure the hit is on a valid surface
            if (hitEffectPrefab && hit.collider != null)
            {
                Vector3 offsetPosition = hit.point + hit.normal * 0.01f; // Slight offset to prevent flickering
                GameObject hitEffect = Instantiate(hitEffectPrefab, offsetPosition, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 60f); // Destroy hit effect after 60 seconds
            }
        }
        else
        {
            Debug.Log("Missed! Raycast did not hit anything.");
        }

        // Apply recoil
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

        // Smooth transition between default and aiming positions
        Vector3 targetPosition = isAiming ? aimPosition : defaultPosition;
        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPosition, Time.deltaTime * aimSpeed);
    }

    void HandleRecoil()
    {
        // Smoothly return the weapon to its original position after recoil
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
        currentAmmo = maxAmmo; // Simple reload logic
        Debug.Log("Reloaded!");
    }

    void HandleSprinting()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isAiming;

        if (isSprinting)
        {
            // Add weapon bobbing effect
            bobTimer += Time.deltaTime * sprintBobSpeed;
            float bobX = Mathf.Sin(bobTimer) * sprintBobAmount.x;
            float bobY = Mathf.Cos(bobTimer * 2f) * sprintBobAmount.y; // Faster Y bobbing for a natural effect
            float bobZ = Mathf.Sin(bobTimer * 0.5f) * sprintBobAmount.z; // Slower Z bobbing for depth movement

            Vector3 bobPosition = originalPosition + new Vector3(bobX, bobY, bobZ);
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, bobPosition, Time.deltaTime * aimSpeed);
        }
        else
        {
            // Reset bobbing
            bobTimer = 0f;
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, originalPosition, Time.deltaTime * aimSpeed);
        }
    }
}
