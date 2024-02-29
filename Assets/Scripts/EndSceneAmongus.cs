using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneAmongus : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        GameLoader.Instance.LoadMainMenu();
    }

}
