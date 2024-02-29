using System;
using System.Collections;
using System.Collections.Generic;

namespace HealthSystem
{
    [Serializable]
    public class CharacterHealthSkeleton<T> : IEnumerable<(CharacterBodyPart, T)>
    {
        public T head;
        public T stomach;
        public T leftArm;
        public T rightArm;
        public T leftLeg;
        public T rightLeg;

        public T this[CharacterBodyPart index]
        {
            get
            {
                switch (index)
                {
                    case CharacterBodyPart.Head:
                        return head;
                    case CharacterBodyPart.Torso:
                        return stomach;
                    case CharacterBodyPart.LeftArm:
                        return leftArm;
                    case CharacterBodyPart.RightArm:
                        return rightArm;
                    case CharacterBodyPart.LeftLeg:
                        return leftLeg;
                    case CharacterBodyPart.RightLeg:
                        return rightLeg;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), index, null);
                }
            }
            set
            {
                switch (index)
                {
                    case CharacterBodyPart.Head:
                        head = value;
                        break;
                    case CharacterBodyPart.Torso:
                        stomach = value;
                        break;
                    case CharacterBodyPart.LeftArm:
                        leftArm = value;
                        break;
                    case CharacterBodyPart.RightArm:
                        rightArm = value;
                        break;
                    case CharacterBodyPart.LeftLeg:
                        leftLeg = value;
                        break;
                    case CharacterBodyPart.RightLeg:
                        rightLeg = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), index, null);
                }
            }
        }

        public IEnumerator<(CharacterBodyPart, T)> GetEnumerator()
        {
            yield return (CharacterBodyPart.Head, head);
            yield return (CharacterBodyPart.Torso, stomach);
            yield return (CharacterBodyPart.LeftArm, leftArm);
            yield return (CharacterBodyPart.RightArm, rightArm);
            yield return (CharacterBodyPart.LeftLeg, leftLeg);
            yield return (CharacterBodyPart.RightLeg, rightLeg);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum CharacterBodyPart : byte
    {
        Head,
        Torso,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
    }
}