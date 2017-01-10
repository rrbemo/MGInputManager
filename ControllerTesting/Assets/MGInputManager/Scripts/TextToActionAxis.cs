using UnityEngine;
using System.Collections;
using MindGrown;
using UnityEngine.UI;

[RequireComponent (typeof(Text))]
public class TextToActionAxis : MonoBehaviour
{
    public string preText = "";
    public ActionName action;
    public string postText = "";

    private Text theText;

    void Awake()
    {
        theText = GetComponent<Text>();
    }

    void OnGUI()
    {
        theText.text = (!string.IsNullOrEmpty(preText) ? preText + " [" : "[") + InputManager.currentInputActions[action].GetAxisDescription() + (!string.IsNullOrEmpty(postText) ? "] " + postText : "]");
    }
}
