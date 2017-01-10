using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MindGrown
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        public static Dictionary<AxisName, InputAxis> inputAxis;

        public static Dictionary<InputInterfaceType, List<AxisName>> ignoreAxisLists;
        public static List<AxisName> currentIgnoreAxisList;

        public static Dictionary<InputInterfaceType, Dictionary<ActionName, InputAction>> interfaceInputActions;
        public static Dictionary<ActionName, InputAction> currentInputActions;

        public static bool waitingForKey = false;

        public string[] joysticks;
        private static InputInterfaceType currentInterface;
        private Event keyEvent;
        private KeyCode keyEventKeyCode;

        // TODO: >> This menuPanel code should be moved to a different handler (Menu or Settings Handler)
        public GameObject menuPanel;
        // <<
    
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            PrepareAxis();
            PrepareActions();
            LoadMappings();
            UpdateCurrentInterface();
            //ResetMappings();
        }

        void Update()
        {
            UpdateJoysticks();

            // TODO: >> This menuPanel code should be moved to a different handler (Menu or Settings Handler)
            if (Input.GetKeyDown(KeyCode.Escape) && !menuPanel.gameObject.activeSelf)
            {
                menuPanel.gameObject.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && menuPanel.gameObject.activeSelf)
            {
                menuPanel.gameObject.SetActive(false);
            }
            // <<

        }

        void OnGUI()
        {
            keyEvent = Event.current;
            if (keyEvent != null && keyEvent.isKey)
            {
                keyEventKeyCode = keyEvent.keyCode;
            }
        }

        static void PrepareAxis()
        {
            inputAxis = new Dictionary<AxisName, InputAxis>
            {
                { AxisName.MouseX,              new InputAxis() { AxisName = AxisName.MouseX,           Description = "Mouse X",            isTurboAxis = true } },
                { AxisName.MouseY,              new InputAxis() { AxisName = AxisName.MouseY,           Description = "Mouse Y",            isTurboAxis = true } },
                { AxisName.MouseScrollwheel,    new InputAxis() { AxisName = AxisName.MouseScrollwheel, Description = "Mouse Scrollwheel" } },
                { AxisName.Joystick_1,          new InputAxis() { AxisName = AxisName.Joystick_1,       Description = "Joystick Axis 1" } },
                { AxisName.Joystick_2,          new InputAxis() { AxisName = AxisName.Joystick_2,       Description = "Joystick Axis 2" } },
                { AxisName.Joystick_3,          new InputAxis() { AxisName = AxisName.Joystick_3,       Description = "Joystick Axis 3" } },
                { AxisName.Joystick_4,          new InputAxis() { AxisName = AxisName.Joystick_4,       Description = "Joystick Axis 4" } },
                { AxisName.Joystick_5,          new InputAxis() { AxisName = AxisName.Joystick_5,       Description = "Joystick Axis 5" } },
                { AxisName.Joystick_6,          new InputAxis() { AxisName = AxisName.Joystick_6,       Description = "Joystick Axis 6" } },
                { AxisName.Joystick_7,          new InputAxis() { AxisName = AxisName.Joystick_7,       Description = "Joystick Axis 7" } },
                { AxisName.Joystick_8,          new InputAxis() { AxisName = AxisName.Joystick_8,       Description = "Joystick Axis 8" } },
                { AxisName.Joystick_9,          new InputAxis() { AxisName = AxisName.Joystick_9,       Description = "Joystick Axis 9" } },
                { AxisName.Joystick_10,         new InputAxis() { AxisName = AxisName.Joystick_10,      Description = "Joystick Axis 10" } }
            };

            ignoreAxisLists = new Dictionary<InputInterfaceType, List<AxisName>>()
            {
                { InputInterfaceType.KeyboardMouse, new List<AxisName>()
                {
                    AxisName.Joystick_1,
                    AxisName.Joystick_2,
                    AxisName.Joystick_3,
                    AxisName.Joystick_4,
                    AxisName.Joystick_5,
                    AxisName.Joystick_6,
                    AxisName.Joystick_7,
                    AxisName.Joystick_8,
                    AxisName.Joystick_9,
                    AxisName.Joystick_10
                }
                },
                { InputInterfaceType.PS4Controller, new List<AxisName>()
                {
                    //AxisName.MouseScrollwheel,
                    //AxisName.MouseX,
                    //AxisName.MouseY
                }
                },
                { InputInterfaceType.XboxOneController, new List<AxisName>()
                {
                    //AxisName.MouseScrollwheel,
                    //AxisName.MouseX,
                    //AxisName.MouseY
                    AxisName.Joystick_3,
                    AxisName.Joystick_6
                }
                }
            };
        }

        // Assign each keycode when the game starts (get from player prefs or assign the default)
        static void PrepareActions()
        {
            interfaceInputActions = new Dictionary<InputInterfaceType, Dictionary<ActionName, InputAction>>()
            { { InputInterfaceType.KeyboardMouse, new Dictionary<ActionName, InputAction>()
                {
                    { ActionName.MoveForward,   new InputAction() { Name = ActionName.MoveForward,      KeyCode = KeyCode.W,                Description = "Move Forward",       InverseActionName = ActionName.MoveBackward } },
                    { ActionName.MoveBackward,  new InputAction() { Name = ActionName.MoveBackward,     KeyCode = KeyCode.S,                Description = "Move Backward",      InverseActionName = ActionName.MoveForward } },
                    { ActionName.StrafeRight,   new InputAction() { Name = ActionName.StrafeRight,      KeyCode = KeyCode.D,                Description = "Strafe Right",       InverseActionName = ActionName.StrafeLeft } },
                    { ActionName.StrafeLeft,    new InputAction() { Name = ActionName.StrafeLeft,       KeyCode = KeyCode.A,                Description = "Strafe Left",        InverseActionName = ActionName.StrafeRight } }
                }
                },
                { InputInterfaceType.PS4Controller, new Dictionary<ActionName, InputAction>()
                {
                    { ActionName.MoveForward,   new InputAction() { Name = ActionName.MoveForward,      InputAxis = inputAxis[AxisName.Joystick_2], AxisPolarity = -1,       Description = "Move Forward",           InverseActionName = ActionName.MoveBackward } },
                    { ActionName.MoveBackward,  new InputAction() { Name = ActionName.MoveBackward,     InputAxis = inputAxis[AxisName.Joystick_2], AxisPolarity = 1,      Description = "Move Backward",          InverseActionName = ActionName.MoveForward } },
                    { ActionName.StrafeRight,   new InputAction() { Name = ActionName.StrafeRight,      InputAxis = inputAxis[AxisName.Joystick_1], AxisPolarity = 1,       Description = "Strafe Right",       InverseActionName = ActionName.StrafeLeft } },
                    { ActionName.StrafeLeft,    new InputAction() { Name = ActionName.StrafeLeft,       InputAxis = inputAxis[AxisName.Joystick_1], AxisPolarity = -1,      Description = "Strafe Left",            InverseActionName = ActionName.StrafeRight } },
                }
                },
                { InputInterfaceType.XboxOneController, new Dictionary<ActionName, InputAction>()
                {
                    { ActionName.MoveForward,   new InputAction() { Name = ActionName.MoveForward,      InputAxis = inputAxis[AxisName.Joystick_2], AxisPolarity = -1,       Description = "Move Forward",           InverseActionName = ActionName.MoveBackward } },
                    { ActionName.MoveBackward,  new InputAction() { Name = ActionName.MoveBackward,     InputAxis = inputAxis[AxisName.Joystick_2], AxisPolarity = 1,      Description = "Move Backward",          InverseActionName = ActionName.MoveForward } },
                    { ActionName.StrafeRight,   new InputAction() { Name = ActionName.StrafeRight,      InputAxis = inputAxis[AxisName.Joystick_1], AxisPolarity = 1,       Description = "Strafe Right",       InverseActionName = ActionName.StrafeLeft } },
                    { ActionName.StrafeLeft,    new InputAction() { Name = ActionName.StrafeLeft,       InputAxis = inputAxis[AxisName.Joystick_1], AxisPolarity = -1,      Description = "Strafe Left",            InverseActionName = ActionName.StrafeRight } }
                } }
            };

            SetCurrentInterface(InputInterfaceType.KeyboardMouse);
        }

        static void UpdateCurrentInterface()
        {
            // Should probably do something to check what the last active interface was.
            currentInputActions = interfaceInputActions[currentInterface];
            currentIgnoreAxisList = ignoreAxisLists[currentInterface];
        }

        public static void SetCurrentInterface(InputInterfaceType type)
        {
            //if (currentInterface != type)
            //{
                currentInterface = type;
                UpdateCurrentInterface();
            //}
        }

        public static InputInterfaceType GetCurrentInterface()
        {
            return currentInterface;
        }

        public static float GetAxis(ActionName actionName)
        {
            float axisValue = 0f;
            InputAction action = currentInputActions[actionName];
            if (action != null)
            {
                axisValue = action.GetAxis();
            }
            return axisValue;
        }
        
        public static float GetAxisRaw(ActionName actionName)
        {
            float axisValue = 0f;
            InputAction action = currentInputActions[actionName];
            if (action != null)
            {
                axisValue = action.GetAxisRaw();
            }
            return axisValue;
        }

        public static void RevokeAxis(InputAxis axis, int polarity)
        {
            InputAction action = null;
            foreach (KeyValuePair<ActionName, InputAction> entry in currentInputActions)
            {
                action = entry.Value;

                if (action.InputAxis == axis
                    && action.AxisPolarity == polarity)
                {
                    action.ResetAxis();
                }
            }
        }

        public static void RevokeKeyCode(KeyCode keyCode)
        {
            InputAction action = null;
            foreach (KeyValuePair<ActionName, InputAction> entry in currentInputActions)
            {
                action = entry.Value;

                if (action.KeyCode == keyCode)
                {
                    action.ResetKeycode();
                }
            }
        }
        void UpdateJoysticks()
        {
            string[] newJoysticks = Input.GetJoystickNames();
            if (joysticks.Length != newJoysticks.Length)
            {
                StopCoroutine("CalibrateJoystick");
                if (joysticks.Length < newJoysticks.Length)
                {
                    // Check to see what joysticks are new
                    string[] freshJoysticks = newJoysticks.Except(joysticks).ToArray();
                    // "Calibrate" new joysticks
                    foreach (string js in freshJoysticks)
                    {
                        StartCoroutine("CalibrateJoystick", Array.LastIndexOf(newJoysticks, js));
                    }

                }
                // update joysticks
                joysticks = newJoysticks;
            }
        }

        #region Coroutines
        IEnumerator CalibrateJoystick(int index)
        {
            int numberOfCecks = 1;
            // for three frames
            for (int f = 0; f < numberOfCecks; f++)
            {
                // Check each axis
                foreach (KeyValuePair<AxisName, InputAxis> entry in inputAxis)
                {
                    InputAxis axis = entry.Value;
                    if (axis == null)
                        continue;

                    float axisValue = Input.GetAxis(axis.AxisName.ToString());
                    if (axisValue == 1)
                    {
                        // This is a bad axis (either disable or -1?)
                        axis.Offset = -1f;
                        Debug.Log("BAD AXIS! " + axis.AxisName + " " + axis.Offset);
                    }
                    if (axisValue == -1)
                    {
                        // This is a bad axis (either disable or +1?)
                        axis.Offset = 1f;
                        Debug.Log("BAD AXIS! " + axis.AxisName + " " + axis.Offset);
                    }
                }
                // Check each button
                yield return null;
            }
        }

        public static IEnumerator AssignNewAxisForAction(ActionName actionName)
        {
            // In Case any other class wants to see if I'm searching for a key
            waitingForKey = true;
            // Cycle through all the axis and see if any have changed
            InputAxis newInputAxis = null;
            KeyCode newKeyCode = KeyCode.None;
            float axisValue = 0;
            Event keyEvent = null;

            while (newKeyCode == KeyCode.None && newInputAxis == null)
            {
                yield return null;
                // Check Axis
                foreach (KeyValuePair<AxisName, InputAxis> entry in inputAxis)
                {
                    InputAxis axis = entry.Value;
                    if (axis == null)
                        continue;

                    axisValue = axis.GetAxis();

                    if (axisValue > 0.5f || axisValue < -0.5f)
                    {
                        newInputAxis = axis;
                        break;
                    }
                }

                if (newKeyCode != KeyCode.None || newInputAxis != null)
                    break;

                // Check Mouse Buttons
                for (int m = 0; m < 3; m++)
                {
                    if (Input.GetMouseButtonDown(m))
                    {
                        newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Mouse" + m);
                        axisValue = 1f;
                        break;
                    }
                }

                if (newKeyCode != KeyCode.None || newInputAxis != null)
                    break;

                string joystickButton = "";
                // Check Joystick buttons
                for (int j = 0; j < Input.GetJoystickNames().Length; j++)
                {
                    for (int b = 0; b < 20; b++)
                    {
                        joystickButton = "Joystick";
                        joystickButton += j == 0 ? "" : j.ToString();
                        joystickButton += "Button" + b.ToString();
                        if (Enum.IsDefined(typeof(KeyCode), joystickButton)
                            && Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), joystickButton)))
                        {
                            // The axis is a keycode?
                            newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), joystickButton);
                            axisValue = 1f;
                            break;
                        }
                    }
                    if (newKeyCode != KeyCode.None)
                        break;
                }

                if (newKeyCode != KeyCode.None || newInputAxis != null)
                    break;

                // Check Keycodes
                // TODO: Get the keycode not the string so that "Tab" and others are remappable.
                //if (!string.IsNullOrEmpty(Input.inputString))
                //{
                //    if (Enum.IsDefined(typeof(KeyCode), Input.inputString.Substring(0, 1).ToUpper()))
                //    {
                //        // The axis is a keycode?
                //        newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), Input.inputString.Substring(0, 1).ToUpper());
                //        axisValue = 1f;
                //    }
                //}

                keyEvent = instance.keyEvent;
                if (keyEvent != null && keyEvent.isKey)
                {
                    Debug.Log(keyEvent.keyCode.ToString());
                    newKeyCode = keyEvent.keyCode;
                    axisValue = 1f;
                }
            }

            if (newInputAxis != null)
            {
                int axisValueInt = axisValue > 0 ? Mathf.CeilToInt(axisValue) : Mathf.FloorToInt(axisValue);
                currentInputActions[actionName].RemapAxis(newInputAxis, axisValueInt);
            }
            else if (newKeyCode != KeyCode.None)
            {
                currentInputActions[actionName].RemapAxis(newKeyCode);
            }

            waitingForKey = false;
            //SaveMappings();
        }
        #endregion

        #region Mapping Persistance
        public static void SaveMappings()
        {
            InputInterfaceType interfaceType;
            Dictionary<ActionName, InputAction> actions;
            InputAction action = null;

            foreach (KeyValuePair<InputInterfaceType, Dictionary<ActionName, InputAction>> entry in interfaceInputActions)
            {
                interfaceType = entry.Key;
                actions = entry.Value;
                foreach (KeyValuePair<ActionName, InputAction> innerEntry in actions)
                {
                    action = innerEntry.Value;
                    PlayerPrefs.SetString(interfaceType.ToString() + action.Name.ToString(), action.GetAxisName());
                    //Debug.Log("SAVE-->Key: " + interfaceType.ToString() + action.Name.ToString() + " Value: " + action.GetAxisName());
                }
            }
            SaveCurrentInterface();
        }

        static void SaveCurrentInterface()
        {
            // Save the currently selected input type
            PlayerPrefs.SetString("CurrentInputInterface", currentInterface.ToString());
        }

        static void LoadMappings()
        {
            InputAction action = null;
            ActionName actionName = ActionName.None;
            AxisName axisName = AxisName.None;
            string[] actionNames = Enum.GetNames(typeof(ActionName));
            string playerPrefString = "";
            string playerPrefPolarityString = "";
            string axisNameString = "";
            int axisPolarity = 0;
            //InputInterfaceType loadingInterface = InputInterfaceType.None;
            foreach (string interfaceType in Enum.GetNames(typeof(InputInterfaceType)))
            {
                if (!interfaceType.Equals("None"))
                {
                    if (Enum.IsDefined(typeof(InputInterfaceType), interfaceType))
                        currentInputActions = interfaceInputActions[(InputInterfaceType)Enum.Parse(typeof(InputInterfaceType), interfaceType)];
                        //loadingInterface = (InputInterfaceType)Enum.Parse(typeof(InputInterfaceType), interfaceType);

                    foreach (string actionNameString in actionNames)
                    {
                        playerPrefString = PlayerPrefs.GetString(interfaceType + actionNameString);
                        if (actionNameString == "None")
                            continue;

                        // Reset all values
                        actionName = ActionName.None;
                        axisName = AxisName.None;
                        axisNameString = "";
                        axisPolarity = 0;
                        playerPrefPolarityString = "";

                        // Make sure polarity doesn't end up out of range
                        if (playerPrefString.Length > 2)
                        {
                            playerPrefPolarityString = playerPrefString.Substring(playerPrefString.Length - 2);
                        }

                        // Get the action
                        if (Enum.IsDefined(typeof(ActionName), actionNameString))
                        {
                            actionName = (ActionName)Enum.Parse(typeof(ActionName), actionNameString);
                            //if (interfaceInputActions[loadingInterface].ContainsKey(actionName))
                            //{
                            //    action = interfaceInputActions[loadingInterface][actionName];
                            //}
                            if (currentInputActions.ContainsKey(actionName))
                            {
                                action = currentInputActions[actionName];
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        // Based on prefpolarity, get the axis name and polarity value
                        if (playerPrefPolarityString.Equals(" +"))
                        {
                            axisNameString = playerPrefString.Substring(0, playerPrefString.Length - 2);
                            axisPolarity = 1;
                        }
                        else if (playerPrefPolarityString.Equals(" -"))
                        {
                            axisNameString = playerPrefString.Substring(0, playerPrefString.Length - 2);
                            axisPolarity = -1;
                        }
                        else
                        {
                            axisNameString = playerPrefString;
                        }

                        // Reset all the axis and go to next action if it is saved as no assignment
                        if (axisNameString.Equals("No Assignment"))
                        {
                            action.ResetKeycode();
                            action.ResetAxis();
                            continue;
                        }
                        if (axisNameString.Equals(""))
                        {
                            // Remove the pref and continue?
                            continue;
                        }

                        // Set it to an axis if it is an axis
                        if (Enum.IsDefined(typeof(AxisName), axisNameString))
                        {
                            axisName = (AxisName)Enum.Parse(typeof(AxisName), axisNameString);
                            if (inputAxis.ContainsKey(axisName))
                            {
                                action.RemapAxis(inputAxis[axisName], axisPolarity);
                            }
                        }
                        else if (Enum.IsDefined(typeof(KeyCode), axisNameString))
                        {
                            action.RemapAxis((KeyCode)Enum.Parse(typeof(KeyCode), axisNameString));
                        }

                        //Debug.Log("LOAD-->Key: " + interfaceType.ToString() + actionNameString + " Value: " + action.GetAxisName());
                    }
                }
            }

            string newCurrentInterface = PlayerPrefs.GetString("CurrentInputInterface");
            if (Enum.IsDefined(typeof(InputInterfaceType), newCurrentInterface))
            {
                SetCurrentInterface((InputInterfaceType)Enum.Parse(typeof(InputInterfaceType), newCurrentInterface));
            }
        }

        public static void ResetMappings()
        {
            PrepareActions();
            SaveMappings();
        }
        #endregion
    }

    #region Actions Enum
    public enum ActionName
    {
        None,
        MoveForward,
        MoveBackward,
        StrafeLeft,
        StrafeRight
    }
    #endregion

    #region Axis Enum
    public enum AxisName
    {
        None,
        MouseX,
        MouseY,
        MouseScrollwheel,
        Joystick_1,
        Joystick_2,
        Joystick_3,
        Joystick_4,
        Joystick_5,
        Joystick_6,
        Joystick_7,
        Joystick_8,
        Joystick_9,
        Joystick_10
    }
    #endregion

    #region Input Interface Type Enum
    public enum InputInterfaceType
    {
        None,
        KeyboardMouse,
        PS4Controller,
        XboxOneController
    }
    #endregion

}
