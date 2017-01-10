using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RemapButtonClick : MonoBehaviour, IPointerClickHandler
{
    public InputSettings inputSettings;

    public void OnPointerClick(PointerEventData eventData)
    {
        inputSettings.StartAssignment(gameObject, eventData);
    }
}
