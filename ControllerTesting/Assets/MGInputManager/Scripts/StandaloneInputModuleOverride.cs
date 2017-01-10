using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using MindGrown;

public class StandaloneInputModuleOverride : MonoBehaviour
{
    private StandaloneInputModule sim;

    void Awake()
    {
        sim = GetComponent<StandaloneInputModule>();
    }

    void Update()
    {
        if (InputManager.waitingForKey)
        {
            sim.enabled = false;
        }
        else if (!sim.enabled)
        {
            StartCoroutine(EnableModuleOnDelay());
        }
    }

    IEnumerator EnableModuleOnDelay()
    {
        yield return new WaitForSeconds(0.15f);
        if (!InputManager.waitingForKey)
        {
            sim.enabled = true;
        }
    }

}
