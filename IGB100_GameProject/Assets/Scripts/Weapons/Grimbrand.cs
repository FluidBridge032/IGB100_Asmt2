using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grimbrand : MonoBehaviour
{
    Animation animation;
    public GameObject uiController;
    // fire variables
    public float damage = 50.0f;
    public float fireRate;
    private float fireTimer = 0.0f;

    public float magazine = 12;
    public float maxMagazine = 12;
    public float reloadCD = 2.0f;
    private float reloadTimer;
    public bool reloading = false;
    

    // fire
    public GameObject muzzleFlash;
    public GameObject bulletHit;
    public GameObject muzzle;
    public GameObject raycastTarget;

    // audio
    public GameObject fireSound;
    public GameObject fireSoundEmpty;
    public GameObject reloadSound;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
        uiController.GetComponent<UIController>().UpdateMagazineBar(magazine, maxMagazine, reloading);
    }

    // Update is called once per frame
    void Update()
    {
        fireRate = GameObject.FindWithTag("Player").GetComponent<Player>().GrimbrandFireRate;
        WeaponFiring();
        Reload();
    }

    private void WeaponFiring()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > fireTimer && !reloading) 
        {
            if (magazine > 0) // fire
            {
                Instantiate(fireSound, transform.position, transform.rotation);
                Instantiate(muzzleFlash, muzzle.transform.position, muzzle.transform.rotation);
                animation.Play("Fire");
                Ray ray = new Ray(muzzle.transform.position, muzzle.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit HitInfo, 50f))
                {
                    if (HitInfo.transform.tag == "Enemy")
                    {
                        HitInfo.transform.GetComponent<Enemy>().TakeDamage(damage);
                        Instantiate(bulletHit, HitInfo.transform.position, HitInfo.transform.rotation);
                    }
                }
                magazine--;
                uiController.GetComponent<UIController>().UpdateMagazineBar(magazine, maxMagazine, reloading);
                fireTimer = Time.time + fireRate;
            }
            else
            {
                Instantiate(fireSoundEmpty, transform.position, transform.rotation);
                // need reload
                uiController.GetComponent<UIController>().UpdateMagazineBar(magazine, maxMagazine, reloading);
            }
        }
    }

    private void Reload()
    {
        if (Input.GetKey("r") && magazine != maxMagazine && !reloading) 
        {
            reloading = true;
            reloadTimer = Time.time;
            uiController.GetComponent<UIController>().UpdateMagazineBar(magazine, maxMagazine, reloading);
            Instantiate(reloadSound, transform.position, transform.rotation);
        }
        if (reloading)
        {
            // animation

            if(Time.time > reloadTimer + reloadCD)
            {
                magazine = maxMagazine;
                reloading = false;
                uiController.GetComponent<UIController>().UpdateMagazineBar(magazine, maxMagazine, reloading);
               
            }
            
        }
    }
}
