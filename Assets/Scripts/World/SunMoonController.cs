using UnityEngine;
using World;

public class SunMoonController : MonoBehaviour
{
    [SerializeField] private Light sun;

    private void Update()
    {
        if (TimeController.Instance)
        {
            UpdateLightning(TimeController.Instance.dayTime / 24);
        }
    }

    private void UpdateLightning(float timePercentage)
    {
        sun.transform.localRotation = Quaternion.Euler(new Vector3((TimeController.Instance.sunCurve.Evaluate(timePercentage) * 360f) - 90f, 30f, 0));
    }
}