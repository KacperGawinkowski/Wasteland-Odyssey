using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Interactions
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interactable")]
        public float interactionDistance = 5f;
        public float interactionTime = 1f;
        public string interactionText;
        public Vector3 interactableOffset;


        [FormerlySerializedAs("selectionCircleEnabled")] [FormerlySerializedAs("selectionCircleOnHover")]
        public bool outlineEnabled = true;
        private OutlineState m_OutlineState = OutlineState.Hidden;
        
        

        public Renderer[] renderers;
        private MaterialPropertyBlock m_Block;
        private static readonly int Visible = Shader.PropertyToID("_Visible");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        public CursorType cursorType;

        private enum OutlineState
        {
            Hidden,
            Selected,
            CanInteract
        }

        private void Start()
        {
            m_Block = new MaterialPropertyBlock();
        }

        public virtual void Interact()
        {
            if (TutorialManager.Instance != null && TutorialManager.Instance.GetCurrentTutorial() != null && TutorialManager.Instance.GetCurrentTutorial().objectToInteract == gameObject)
            {
                CanvasController.Instance.tutorialCanvasController.nextTutorialButton.interactable = true;
            }
        }

        public virtual bool CanInteract() => true;


        public void ShowOutline(bool canInteract)
        {
            // if (selectionCircleEnabled && m_SelectionCircle != null)
            if (outlineEnabled)
            {
                if (canInteract && m_OutlineState != OutlineState.CanInteract)
                {
                    // m_SelectionCircle.SetActive(true);
                    // m_SelectionCircleSpriteRenderer.color = Color.green;
                    m_Block.SetInt(Visible, 1);
                    m_Block.SetColor(OutlineColor, Color.green);
                    m_OutlineState = OutlineState.CanInteract;

                    foreach (Renderer r in renderers)
                    {
                        r.SetPropertyBlock(m_Block);
                    }
                }
                else if (m_OutlineState != OutlineState.Selected)
                {
                    // m_SelectionCircle.SetActive(true);
                    // m_SelectionCircleSpriteRenderer.color = Color.yellow;
                    m_Block.SetInt(Visible, 1);
                    m_Block.SetColor(OutlineColor, Color.yellow);
                    m_OutlineState = OutlineState.Selected;

                    foreach (Renderer r in renderers)
                    {
                        r.SetPropertyBlock(m_Block);
                    }
                }
            }
            else if (m_OutlineState != OutlineState.Hidden)
            {
                HideOutline();
            }
        }

        public void HideOutline()
        {
            // if (m_SelectionCircleState != SelectionCircleState.Hidden && m_SelectionCircle != null)
            if (m_OutlineState != OutlineState.Hidden)
            {
                // m_SelectionCircle.SetActive(false);
                m_Block.SetInt(Visible, 0);
                m_OutlineState = OutlineState.Hidden;

                foreach (Renderer r in renderers)
                {
                    r.SetPropertyBlock(m_Block);
                }
            }
        }
    }
}