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
        //public static List<InputAction> inputActions;
        public static Dictionary<ActionName, InputAction> inputActions;
        public static Dictionary<AxisName, InputAxis> inputAxis;
        public static bool waitingForKey = false;

        public string[] joysticks;

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
            //ResetMappings();
        }

        void Update()
        {
            UpdateJoysticks();
        }

        static void PrepareAxis()
        {
            inputAxis = new Dictionary<AxisName, InputAxis>
        {
            { AxisName.MouseX,              new InputAxis() { InputManagerName = "MouseX",              Description = "Mouse X",            isTurboAxis = true } },
            { AxisName.MouseY,              new InputAxis() { InputManagerName = "MouseY",              Description = "Mouse Y",            isTurboAxis = true } },
            { AxisName.MouseScrollwheel,    new InputAxis() { InputManagerName = "MouseScrollwheel",    Description = "Mouse Scrollwheel" } },
            { AxisName.Joystick_1,          new InputAxis() { InputManagerName = "Joystick_1",          Description = "Joystick Axis 1" } },
            { AxisName.Joystick_2,          new InputAxis() { InputManagerName = "Joystick_2",          Description = "Joystick Axis 2" } },
            { AxisName.Joystick_3,          new InputAxis() { InputManagerName = "Joystick_3",          Description = "Joystick Axis 3" } },
            { AxisName.Joystick_4,          new InputAxis() { InputManagerName = "Joystick_4",          Description = "Joystick Axis 4" } },
            { AxisName.Joystick_5,          new InputAxis() { InputManagerName = "Joystick_5",          Description = "Joystick Axis 5" } },
            { AxisName.Joystick_6,          new InputAxis() { InputManagerName = "Joystick_6",          Description = "Joystick Axis 6" } },
            { AxisName.Joystick_7,          new InputAxis() { InputManagerName = "Joystick_7",          Description = "Joystick Axis 7" } },
            { AxisName.Joystick_8,          new InputAxis() { InputManagerName = "Joystick_8",          Description = "Joystick Axis 8" } },
            { AxisName.Joystick_9,          new InputAxis() { InputManagerName = "Joystick_9",          Description = "Joystick Axis 9" } },
            { AxisName.Joystick_10,         new InputAxis() { InputManagerName = "Joystick_10",         Description = "Joystick Axis 10" } }
        };
        }

        // Assign each keycode when the game starts (get from player prefs or assign the default)
        static void PrepareActions()
        {
            bool isSavedPreferences = false;
            if (!isSavedPreferences)
            {
                inputActions = new Dictionary<ActionName, InputAction>
            {
                { ActionName.MenuNavUp,     new InputAction() { Name = ActionName.MenuNavUp,        KeyCode = KeyCode.UpArrow,      Description = "Menu Nav Up" } },
                { ActionName.MenuNavDown,   new InputAction() { Name = ActionName.MenuNavDown,      KeyCode = KeyCode.DownArrow,    Description = "Menu Nav Down" } },
                { ActionName.MenuNavLeft,   new InputAction() { Name = ActionName.MenuNavLeft,      KeyCode = KeyCode.LeftArrow,    Description = "Menu Nav Left" } },
                { ActionName.MenuNavRight,  new InputAction() { Name = ActionName.MenuNavRight,     KeyCode = KeyCode.RightArrow,   Description = "Menu Nav Right" } },
                { ActionName.MoveForeward,  new InputAction() { Name = ActionName.MoveForeward,     KeyCode = KeyCode.W,            Description = "Move Forward",       InverseActionName = ActionName.MoveBackward } },
                { ActionName.MoveBackward,  new InputAction() { Name = ActionName.MoveBackward,     KeyCode = KeyCode.S,            Description = "Move Backward",      InverseActionName = ActionName.MoveForeward } },
                { ActionName.StrafeLeft,    new InputAction() { Name = ActionName.StrafeLeft,       KeyCode = KeyCode.A,            Description = "Strafe Left",        InverseActionName = ActionName.StrafeRight } },
                { ActionName.StrafeRight,   new InputAction() { Name = ActionName.StrafeRight,      KeyCode = KeyCode.D,            Description = "Strafe Right",       InverseActionName = ActionName.StrafeLeft } }
// { ActionName.MoveForward,    new InputAction() { Name = ActionName.MoveForward,  InputAxis = inputAxis[AxisName.MouseY],             AxisPolarity = 1,   InverseActionName = ActionName.MoveForward } },
// { ActionName.MoveBackward,   new InputAction() { Name = ActionName.MoveBackward, InputAxis = inputAxis[AxisName.MouseY],             AxisPolarity = -1,  InverseActionName = ActionName.MoveBackward } },
            };
            }
        }

        public static float GetAxis(ActionName actionName)
        {
            float actionValue = 0;
            InputAction action = inputActions[actionName];
            if (action != null)
            {
                actionValue = action.GetAxis();
            }
            return actionValue;
        }

        public static void RevokeAxis(InputAxis axis, int polarity)
        {
            InputAction action = null;
            foreach (KeyValuePair<ActionName, InputAction> entry in inputActions)
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
            foreach (KeyValuePair<ActionName, InputAction> entry in inputActions)
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

                    float axisValue = Input.GetAxis(axis.InputManagerName);
                    if (axisValue == 1)
                    {
                        // This is a bad axis (either disable or -1?)
                        axis.Offset = -1f;
                        Debug.Log("BAD AXIS! " + axis.InputManagerName + " " + axis.Offset);
                    }
                    if (axisValue == -1)
                    {
                        // This is a bad axis (either disable or +1?)
                        axis.Offset = 1f;
                        Debug.Log("BAD AXIS! " + axis.InputManagerName + " " + axis.Offset);
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
                if (!string.IsNullOrEmpty(Input.inputString))
                {
                    if (Enum.IsDefined(typeof(KeyCode), Input.inputString.Substring(0, 1).ToUpper()))
                    {
                        // The axis is a keycode?
                        newKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), Input.inputString.Substring(0, 1).ToUpper());
                        axisValue = 1f;
                    }

                }
            }

            if (newInputAxis != null)
            {
                int axisValueInt = axisValue > 0 ? Mathf.CeilToInt(axisValue) : Mathf.FloorToInt(axisValue);
                inputActions[actionName].RemapAxis(newInputAxis, axisValueInt);
            }
            else if (newKeyCode != KeyCode.None)
            {
                inputActions[actionName].RemapAxis(newKeyCode);
            }

            waitingForKey = false;
            SaveMappings();
        }
        #endregion

        #region Mapping Persistance
        static void SaveMappings()
        {
            InputAction action = null;

            foreach (KeyValuePair<ActionName, InputAction> entry in inputActions)
            {
                action = entry.Value;
                PlayerPrefs.SetString(action.Name.ToString(), action.GetAxisName());
            }
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
            foreach (string actionNameString in actionNames)
            {
                playerPrefString = PlayerPrefs.GetString(actionNameString);
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
                    if (inputActions.ContainsKey(actionName))
                    {
                        action = inputActions[actionName];
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
            }

        }

        static void ResetMappings()
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
        MenuNavUp,
        MenuNavDown,
        MenuNavLeft,
        MenuNavRight,
        MenuAccept,
        MenuCancel,
        MoveForeward,
        MoveBackward,
        StrafeLeft,
        StrafeRight,
        RotateLeft,
        RotateRight,
        ShootPrimary,
        ShootSecondary,
        UseAbility,
        TargetToggle,
        FocusOnTarget,
        SettingsMenu,
        GameMenu,
        QuickSelect_1,
        QuickSelect_2,
        QuickSelect_3,
        QuickSelect_4
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
        GenericJoystick,
        PS4Controller,
        Xbox360Controller,
        XboxOneController
    }
    #endregion

}
