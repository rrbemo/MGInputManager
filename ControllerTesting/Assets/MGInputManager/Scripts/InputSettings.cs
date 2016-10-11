using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using MindGrown;

public class InputSettings : MonoBehaviour
{
    public Transform menuPanel;
    public GameObject rowPrefab;
    List<GameObject> buttonRows;

    void Start()
    {
        buttonRows = new List<GameObject>();
        InputAction action = null;
        foreach (KeyValuePair<ActionName, InputAction> entry in InputManager.inputActions)
        {
            action = entry.Value;

            // Create a new button row
            Transform newRow = Instantiate(rowPrefab).transform;
            newRow.name = action.Name.ToString();
            newRow.FindChild("Title").GetComponent<Text>().text = action.Description;
            newRow.FindChild("Button").FindChild("Text").GetComponent<Text>().text = action.GetAxisName();
            newRow.FindChild("Button").GetComponent<Button>().onClick.AddListener(() => StartAssignment());
            newRow.transform.SetParent(menuPanel);
            buttonRows.Add(newRow.gameObject);
        }
        menuPanel.gameObject.SetActive(false);
        string[] names = Input.GetJoystickNames();
        foreach (string name in names)
        {
            Debug.Log(name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !menuPanel.gameObject.activeSelf)
        {
            menuPanel.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menuPanel.gameObject.activeSelf)
        {
            menuPanel.gameObject.SetActive(false);
        }
    }

    void UpdateButtons()
    {
        foreach (GameObject row in buttonRows)
        {
            InputAction action = null;
            if (Enum.IsDefined(typeof(ActionName), row.name))
            {
                action = InputManager.inputActions[(ActionName)Enum.Parse(typeof(ActionName), row.name)];
                row.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = action.GetAxisName();
            }
            else
            {
                // remove row?
                row.transform.FindChild("Button").FindChild("Text").GetComponent<Text>().text = "UNKNOWN";
            }
        }
    }

    #region Button Event Handlers
    public void StartAssignment()
    {
        if (!InputManager.waitingForKey)
        {
            string actionName = EventSystem.current.currentSelectedGameObject.transform.parent.name;

            if (Enum.IsDefined(typeof(ActionName), actionName))
            {

                StartCoroutine(InputManager.AssignNewAxisForAction((ActionName)Enum.Parse(typeof(ActionName), actionName)));
                StartCoroutine(RecoveryPeriod());
            }
        }
    }
    #endregion

    #region Coroutines
    IEnumerator RecoveryPeriod()
    {
        // If we are waiting for a key... well... wait
        while (InputManager.waitingForKey)
        {
            yield return null;
        }
        // Just wait a bit so you can't click a button and start the assignment process again instantly
        yield return new WaitForSeconds(0.15f);
        UpdateButtons();
    }
    #endregion
}
