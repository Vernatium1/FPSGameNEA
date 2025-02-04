using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; set;}

    public AudioSource ShootingChannel;

    public AudioClip M1911Shot;
    public AudioClip M16Shot;

    public AudioSource reloadingSoundM16;
    public AudioSource reloadingSound1911;

    public AudioSource emptySound1911;


    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.M16:
                ShootingChannel.PlayOneShot(M16Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                reloadingSound1911.Play();
                break;
            case WeaponModel.M16:
                reloadingSoundM16.Play();
                break;
        }
    }
}
