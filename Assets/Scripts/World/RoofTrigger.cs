using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class RoofTrigger : MonoBehaviour
{
	[SerializeField] private MeshRenderer m_ObjectToHide;
	[SerializeField] private Material m_BaseReplaceMaterial;

	//private Color[] m_OriginalColors;
	//private Texture[] m_OriginalColors;
	private Material[] m_OriginalMaterials;
	private Material[] m_TransparentMaterials; // array of new temporary materials for this object

	//private bool m_PlayerInArea;
	private int m_PlayerInArea;
	private float m_TransitionTimer;

	// Start is called before the first frame update
	void Start()
	{
		int length = m_ObjectToHide.sharedMaterials.Length;
		//m_OriginalColors = new Color[length];
		m_TransparentMaterials = new Material[length];
		m_OriginalMaterials = new Material[length];
		for (int i = 0; i < length; i++)
		{
			//m_OriginalColors[i] = m_ObjectToHide.sharedMaterials[i].color;
			m_OriginalMaterials[i] = m_ObjectToHide.sharedMaterials[i];
			Material mat = new Material(m_BaseReplaceMaterial)
			{
				color = m_ObjectToHide.sharedMaterials[i].color,
			};
			mat.SetTexture("_BaseColorMap", m_ObjectToHide.sharedMaterials[i].GetTexture("_BaseColorMap"));
			m_TransparentMaterials[i] = mat;
		}
	}

	private void OnDestroy()
	{
		if (m_TransparentMaterials != null)
		{
			foreach (Material material in m_TransparentMaterials)
			{
				Destroy(material);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		const float speedModifier = 5f;
		const float maxTransparency = 1f;

		if (m_PlayerInArea != 0)
		{
			if (m_TransitionTimer < maxTransparency)
			{
				if (m_TransitionTimer == 0)
				{
					m_ObjectToHide.sharedMaterials = m_TransparentMaterials;
				}

				m_TransitionTimer += Time.deltaTime * speedModifier;
				if (m_TransitionTimer > maxTransparency)
				{
					m_TransitionTimer = maxTransparency;
				}

				for (int i = 0; i < m_ObjectToHide.sharedMaterials.Length; i++)
				{
					Material material = m_ObjectToHide.sharedMaterials[i];
					//Color transparent = m_OriginalColors[i];
					Color transparent = m_OriginalMaterials[i].color;
					transparent.a = 0f;
					material.color = Color.Lerp(m_OriginalMaterials[i].color, transparent, m_TransitionTimer);
				}
			}
		}
		else
		{
			if (m_TransitionTimer > 0f)
			{
				m_TransitionTimer -= Time.deltaTime * speedModifier;
				if (m_TransitionTimer < 0f)
				{
					m_TransitionTimer = 0f;

					m_ObjectToHide.sharedMaterials = m_OriginalMaterials;
				}

				MeshRenderer mesh = m_ObjectToHide;
				for (int i = 0; i < mesh.sharedMaterials.Length; i++)
				{
					Material material = mesh.sharedMaterials[i];
					Color transparent = m_OriginalMaterials[i].color;
					transparent.a = 0f;
					material.color = Color.Lerp(m_OriginalMaterials[i].color, transparent, m_TransitionTimer);
				}
			}
		}
	}

	public void PlayerEnter()
	{
		m_PlayerInArea++;
	}

	public void PlayerExit()
	{
		m_PlayerInArea--;
	}

	private void OnValidate()
	{
		if (!m_ObjectToHide) m_ObjectToHide = GetComponent<MeshRenderer>();
	}
}