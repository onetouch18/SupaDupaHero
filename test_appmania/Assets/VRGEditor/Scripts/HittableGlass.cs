using UnityEngine;
using System.Collections;

public class HittableGlass : MonoBehaviour, IHitable
{
    public float ModifierValue = 2;
    public float HP = 100;

    public GameObject Origin;
    public GameObject Braked;
    public Sound DestroySound;

    private AudioSource _source;
    public bool IsBreaked { get; private set; }

    private void Start()
    {
        GetComponent<NavMeshObstacle>().carving = true;
        _source = GetComponent<AudioSource>();
    }

    public void Hit(object sender, float damage)
    {
        HP -= CalculateDamage(damage);
        if (HP <= 0)
        {
            Break();
        }
    }

    public float CalculateDamage(float damage)
    {
        return damage * ModifierValue;
    }

    public void Break()
    {
        if (IsBreaked) return;
        IsBreaked = true;
        Origin.SetActive(false);
        Braked.SetActive(true);
        Braked.GetComponent<Animation>().Play();
        PlaySoundOneShot(DestroySound);
        var obs = GetComponent<NavMeshObstacle>();
        obs.carving = false;
        obs.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    public void PlaySoundOneShot(Sound sound)
    {
        if (_source.isPlaying) return;
        var so = sound.Clip;
        if (so == null) return;
        _source.PlayOneShot(so);
    }
}
