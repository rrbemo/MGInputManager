using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace MindGrown
{
    public class ScrollingSelectable : MonoBehaviour
    {
        public ScrollRect scrollRect;
        GameObject currentlyActiveGO;
        bool wasActiveGO = false;

        void OnGUI()
        {
            currentlyActiveGO = EventSystem.current.currentSelectedGameObject;
            if (gameObject == currentlyActiveGO
                && !wasActiveGO)
            {
                float contentHeight = scrollRect.content.rect.height;
                float viewportHeight = scrollRect.viewport.rect.height;
                float distanceFromTopOfScrollContent = (scrollRect.content.transform.position.y) - transform.position.y;

                //scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp((distanceFromTopOfScrollContent) / contentHeight, 0f, 1f);
                float contentHeightWithoutViewport = contentHeight - viewportHeight;
                float ratioOfViewportToContent = (viewportHeight * 0.5f) / contentHeightWithoutViewport;

                scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp((distanceFromTopOfScrollContent / contentHeightWithoutViewport) - ratioOfViewportToContent, 0f, 1f);

                //Debug.Log(scrollRect.content.transform.position.y + " :: " + scrollRect.viewport.transform.position.y);
                //Debug.Log(scrollRect.verticalNormalizedPosition);
                wasActiveGO = true;
            }
            else if (gameObject != currentlyActiveGO)
            {
                wasActiveGO = false;
            }
        }
    }
}
