using System;
using UnityEngine;

namespace TestyAndStareRzeczy.sus_test
{
    [RequireComponent(typeof(Animator))]
    [ExecuteInEditMode]
    public class IKControl : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public bool ikActive = false;
        [NonSerialized] public Transform rightHandObj;
        [NonSerialized] public Transform leftHandObj;
        [NonSerialized] public Transform rightFeetObj;
        [NonSerialized] public Transform leftFeetObj;
        [NonSerialized] public Transform lookObj;


        //a callback for calculating IK
        void OnAnimatorIK()
        {
            //if the IK is active, set the position and rotation directly to the goal.
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }

                if (rightFeetObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFeetObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFeetObj.rotation);
                }

                if (leftFeetObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFeetObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFeetObj.rotation);
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }

        public void ClearIK()
        {
            rightHandObj = null;
            leftHandObj = null;
            rightFeetObj = null;
            leftFeetObj = null;
            lookObj = null;
        }

        private void OnValidate()
        {
            if (!animator) animator = GetComponent<Animator>();
        }
    }
}