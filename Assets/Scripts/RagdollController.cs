using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] m_Rigidbodies;

    public void EnableRagdoll()
    {
        int layer = LayerMask.NameToLayer("InteractableNoCollision");
        foreach (var item in m_Rigidbodies)
        {
            item.isKinematic = false;
            item.gameObject.layer = layer;
        }
    }

    private void OnValidate()
    {
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var item in m_Rigidbodies)
        {
            item.isKinematic = true;
        }
    }
}
