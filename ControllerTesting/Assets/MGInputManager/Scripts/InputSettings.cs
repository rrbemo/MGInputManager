using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using MindGrown;

public class InputSettings : MonoBehaviour
{
    public GameObject actionRowPrefab;
    public Transform remappingContainer;

    public Button resetToDefaultButton;

    [Header("Interface Toggles")]
    public Toggle keyboardToggle;
    public Toggle ps4Toggle;
    public Toggle xboxOneToggle;

    private List<GameObject> actionRows;
    private Toggle currentlyActiveInterfaceToggle;

    void Start()
    {
        keyboardToggle.name = InputInterfaceType.KeyboardMouse.ToString();
        ps4Toggle.name = InputInterfaceType.PS4Controller.ToString();
        xboxOneToggle.name = InputInterfaceType.XboxOneController.ToString();
        keyboardToggle.onValueChanged.RemoveAllListeners();
        ps4Toggle.onValueChanged.RemoveAllListeners();
        xboxOneToggle.onValueChanged.RemoveAllListeners();
        resetToDefaultButton.onClick.RemoveAllListeners();
        keyboardToggle.onValueChanged.AddListener(KeyboardInterfaceToggled);
        ps4Toggle.onValueChanged.AddListener(PS4InterfaceToggled);
        xboxOneToggle.onValueChanged.AddListener(XboxOneInterfaceToggled);
        resetToDefaultButton.onClick.AddListener(() => ResetToDefaultMappings());

        InitializeActionButtons();
        ActivateCurrentInterfaceToggle();
    }

    void InitializeActionButtons()
    {

        actionRows = new List<GameObject>();
        InputAction action = null;
        ActionRowController rowController = null;
        foreach (KeyValuePair<ActionName, InputAction> entry in InputManager.currentInputActions)
        {
            action = entry.Value;
            Transform newRow = Instantiate(actionRowPrefab).transform;
            rowController = newRow.GetComponent<ActionRowController>();

            newRow.SetParent(remappingContainer);
            newRow.localPosition = Vector3.zero;
            newRow.localScale = Vector3.one;
            newRow.localRotation = Quaternion.identity;
            newRow.name = action.Name.ToString();
            rowController.actionTitle.text = action.Description;
            rowController.actionButtonText.text = action.GetAxisDescription();
            rowController.actionButton.GetComponent<RemapButtonClick>().inputSettings = this;

            actionRows.Add(newRow.gameObject);
        }

        rowController = actionRows[0].GetComponent<ActionRowController>();
        // Xbox toggle needs to move to have down and right move to mapping buttons.
        Navigation xboxToggleNav = xboxOneToggle.navigation;
        xboxToggleNav.selectOnDown = rowController.actionButton;
        xboxToggleNav.selectOnRight = rowController.actionButton;
        xboxOneToggle.navigation = xboxToggleNav;

        // Set the first button with explicit navigation to link in with rest of page.
        Navigation firstButtonNav = new Navigation();
        firstButtonNav.mode = Navigation.Mode.Explicit;
        firstButtonNav.selectOnUp = xboxOneToggle;
        firstButtonNav.selectOnDown = actionRows[1].GetComponent<ActionRowController>().actionButton;
        rowController.actionButton.navigation = firstButtonNav;

        rowController = actionRows[actionRows.Count - 1].GetComponent<ActionRowController>();
        // Set the last button with explicit navigation
        Navigation lastButtonNav = new Navigation();
        lastButtonNav.mode = Navigation.Mode.Explicit;
        lastButtonNav.selectOnUp = actionRows[actionRows.Count - 2].GetComponent<ActionRowController>().actionButton;
        lastButtonNav.selectOnDown = resetToDefaultButton;
        rowController.actionButton.navigation = lastButtonNav;

        // Set up the resetbutton nav
        Navigation resetButtonNav = new Navigation();
        resetButtonNav.mode = Navigation.Mode.Explicit;
        resetButtonNav.selectOnUp = rowController.actionButton;
        resetToDefaultButton.navigation = resetButtonNav;

        //string[] names = Input.GetJoystickNames();
        //foreach (string name in names)
        //{
        //    Debug.Log(name);
        //}
    }

    void UpdateButtons()
    {
        foreach (GameObject row in actionRows)
        {
            InputAction action = null;
            ActionRowController rowController = null;
            if (Enum.IsDefined(typeof(ActionName), row.name))
            {
                action = InputManager.currentInputActions[(ActionName)Enum.Parse(typeof(ActionName), row.name)];
                rowController = row.GetComponent<ActionRowController>();
                rowController.actionButtonText.text = action.GetAxisDescription();
            }
            else
            {
                // remove row?
                rowController.actionButtonText.text = "UNKNOWN";
            }
        }
    }

    void ActivateCurrentInterfaceToggle()
    {
        switch (InputManager.GetCurrentInterface())
        {
            case (InputInterfaceType.KeyboardMouse):
                keyboardToggle.isOn = true;
                break;
            case (InputInterfaceType.PS4Controller):
                ps4Toggle.isOn = true;
                break;
            case (InputInterfaceType.XboxOneController):
                xboxOneToggle.isOn = true;
                break;
            default:
                break;
        }
    }

    void SetHighlightGameObject(GameObject go, bool isEnabled)
    {
        GameObject parentGO = go.transform.parent.gameObject;
        Image highlight = null;
        if (parentGO != null)
        {
            highlight = parentGO.GetComponent<Image>();
        }

        if (highlight != null)
        {
            highlight.enabled = isEnabled;
        }
    }

    #region Button Event Handlers
    public void StartAssignment(GameObject go = null, PointerEventData eventData = null)
    {
        if (!InputManager.waitingForKey)
        {
            string actionName = EventSystem.current.currentSelectedGameObject.transform.parent.name;

            if (Enum.IsDefined(typeof(ActionName), actionName))
            {
                if (go != null)
                {
                    SetHighlightGameObject(go, true);
                }
                StartCoroutine(InputManager.AssignNewAxisForAction((ActionName)Enum.Parse(typeof(ActionName), actionName)));
                StartCoroutine(RecoveryPeriod(go));
            }
        }
    }

    //public void InterfaceToggleChanged(bool value)
    //{
    //    if (value)
    //    {
    //        Toggle activeToggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
    //        if (currentlyActiveInterfaceToggle != activeToggle)
    //        {
    //            currentlyActiveInterfaceToggle = activeToggle;
    //            if (Enum.IsDefined(typeof(InputInterfaceType), currentlyActiveInterfaceToggle.name))
    //                InputManager.SetCurrentInterface((InputInterfaceType)Enum.Parse(typeof(InputInterfaceType), currentlyActiveInterfaceToggle.name));
    //            UpdateButtons();
    //        }
    //    }
    //}

    public void KeyboardInterfaceToggled(bool value)
    {
        if (value)
        {
            InputManager.SetCurrentInterface(InputInterfaceType.KeyboardMouse);
            UpdateButtons();
            InputManager.SaveMappings();
        }
    }

    public void PS4InterfaceToggled(bool value)
    {
        if (value)
        {
            InputManager.SetCurrentInterface(InputInterfaceType.PS4Controller);
            UpdateButtons();
            InputManager.SaveMappings();
        }
    }

    public void XboxOneInterfaceToggled(bool value)
    {
        if (value)
        {
            InputManager.SetCurrentInterface(InputInterfaceType.XboxOneController);
            UpdateButtons();
            InputManager.SaveMappings();
        }
    }

    public void ResetToDefaultMappings()
    {
        InputManager.ResetMappings();
        UpdateButtons();
    }
    #endregion

    #region Coroutines
    IEnumerator RecoveryPeriod(GameObject go = null, PointerEventData eventData = null)
    {
        // If we are waiting for a key... well... wait
        while (InputManager.waitingForKey)
        {
            yield return null;
        }
        // Just wait a bit so you can't click a button and start the assignment process again instantly
        yield return new WaitForSeconds(0.15f);
        if (go != null)
        {
            SetHighlightGameObject(go, false);
        }
        UpdateButtons();
        InputManager.SaveMappings();
    }
    #endregion
}
