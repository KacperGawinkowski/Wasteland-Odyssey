using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPC;
using NPC.Friendly;
using UnityEngine;
using Random = UnityEngine.Random;


public class NPCSpawnPointsController : MonoBehaviour
{
    [SerializeField] private GameObject m_ImportantNPCsParent;
    [SerializeField] private GameObject m_FillerNPCsParent;
}
