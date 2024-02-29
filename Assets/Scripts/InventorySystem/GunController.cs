using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunController : MonoBehaviour
{
    [SerializeField] private VisualEffect m_VisualEffect;
    [SerializeField] private AudioSource m_AudioSource;

    [SerializeField] public GameObject foreGrip;
    [SerializeField] public GameObject grip;

    [SerializeField] public Rigidbody gunRigidbody;

    public void PlayEffect()
    {
        if (m_VisualEffect)
        {
            m_VisualEffect.Play();
        }

        if (m_AudioSource)
        {
            m_AudioSource.Play();
        }
    }
    

    public void DropWeaponObject()
    {
        gunRigidbody.transform.parent = null;
        gunRigidbody.isKinematic = false;
    }
}
