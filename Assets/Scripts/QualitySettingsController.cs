using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Timeline;
using TMPro;

public class QualitySettingsController : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    //private HDRenderPipelineAsset pipelineAsset;

    private void Start()
    {
        // Get the HDRenderPipelineAsset from the current render pipeline
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(QualitySettings.names[i]));
        }

        dropdown.value = QualitySettings.GetQualityLevel();
        dropdown.RefreshShownValue();
    }

    public void SetQualityLevel(int i)
    {
        QualitySettings.SetQualityLevel(i);
    }
}
