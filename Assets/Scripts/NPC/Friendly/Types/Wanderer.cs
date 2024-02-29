using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using World;
using Random = UnityEngine.Random;

namespace NPC
{
    public class Wanderer : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private Animator m_Animator;
        private Vector3 m_SpawnPosition;

        private float m_MaxDirectionChangeFrequency = 10;
        [SerializeReference] private Transform[] m_WandererPoints;
        private int m_WanderPointIndex = 0;

        private void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponentInChildren<Animator>();

            m_SpawnPosition = transform.position;
            m_Agent.speed = 2.5f;
            StartCoroutine(GoTo());
        }

        private void Update()
        {
            const float dampTime = 0.1f;
            Vector3 speed = transform.InverseTransformDirection(m_Agent.desiredVelocity);
            m_Animator.SetFloat(AnimatorVariables.WalkY, speed.z / m_Agent.speed, dampTime, Time.deltaTime);
            m_Animator.SetFloat(AnimatorVariables.WalkX, speed.y / m_Agent.speed, dampTime, Time.deltaTime);
        }

        private IEnumerator GoTo()
        {
            while (m_WandererPoints.Length != 0)
            {
                if (!m_Agent.pathPending && m_Agent.remainingDistance < 0.5f)
                {
                    if (TimeController.DayEnum != DayEnum.NIGHT)
                    {
                        m_Agent.destination = m_WandererPoints[m_WanderPointIndex].position;
                        m_WanderPointIndex = Random.Range(0, m_WandererPoints.Length);
                    }
                    else
                    {
                        m_Agent.destination = m_SpawnPosition;
                        yield return new WaitUntil(() => TimeController.DayEnum == DayEnum.NIGHT);
                    }
                }
                yield return new WaitForSeconds(Random.Range(2, m_MaxDirectionChangeFrequency));
            }
        }

        public void SetWandererPoints(Transform[] points)
        {
            m_WandererPoints = points;
        }
    }
}