using UnityEngine;
using System.Collections;

namespace MindGrown
{
    public class InputAction
    {
        public ActionName Name { get; set; }
        public string Description { get; set; }
        public KeyCode KeyCode
        {
            get
            {
                return _keycode;
            }
            set
            {
                if (value != KeyCode.None)
                {
                    // Wipe out all axis info
                }
                _keycode = value;
            }
        }

        public InputAxis InputAxis { get; set; }
        public int AxisPolarity
        {
            get
            {
                return _axisPolarity;
            }
            set
            {
                if (value < 0)
                {
                    _axisPolarity = -1;
                }
                else if (value > 0)
                {
                    _axisPolarity = 1;
                }
                else
                {
                    _axisPolarity = 0;
                }

            }
        }
        public ActionName InverseActionName { get; set; }

        public InputAction()
        {
            InitializationHelper(ActionName.None, "", null, KeyCode.None, ActionName.None, 0);
        }

        public InputAction(ActionName name, string disc, InputAxis inputAxis, ActionName inverseAction = ActionName.None, int axisPolarity = 0)
        {
            InitializationHelper(name, disc, inputAxis, KeyCode.None, inverseAction, axisPolarity);
        }

        public InputAction(ActionName name, string disc, KeyCode keycode, ActionName inverseAction = ActionName.None)
        {
            InitializationHelper(name, disc, null, keycode, inverseAction, 0);
        }

        void InitializationHelper(ActionName name, string desc, InputAxis inputAxis, KeyCode keycode, ActionName inverseAction, int axisPolarity)
        {
            if (true)
            {
                Name = name;
                Description = desc;
                InputAxis = inputAxis;
                KeyCode = keycode;
                InverseActionName = inverseAction;
                AxisPolarity = axisPolarity;
            }
        }

        #region Remapping
        public void RemapAxis(KeyCode newKeyCode, int axisPolarity)
        {
            axisPolarity = 0;
            RemapAxis(newKeyCode);
        }

        public void RemapAxis(KeyCode newKeycode)
        {
            if (newKeycode != KeyCode)
            {
                InputManager.RevokeKeyCode(newKeycode);
                if (InputAxis != null)
                {
                    if (InverseActionName != ActionName.None
                        && InputAxis.Offset == 0f)
                    {
                        InputManager.currentInputActions[InverseActionName].ResetAxis();
                    }
                    ResetAxis();
                }
                KeyCode = newKeycode;
            }
        }

        public void RemapAxis(InputAxis newInputAxis, int newAxisPolarity = 0)
        {
            bool wasTrigger = false;
            bool isTrigger = newInputAxis.Offset != 0f;

            if (InputAxis == null
                || InputAxis.Offset != 0f)
            {
                wasTrigger = true;
            }

            InputManager.RevokeAxis(newInputAxis, newAxisPolarity);
            ResetKeycode();
            InputAxis = newInputAxis;
            AxisPolarity = newAxisPolarity;
            if (InverseActionName != ActionName.None)
            {
                InputAction inverseAction = InputManager.currentInputActions[InverseActionName];
                if (!isTrigger
                    && inverseAction.InputAxis != newInputAxis
                    && inverseAction.AxisPolarity != newAxisPolarity * -1)
                {
                    inverseAction.RemapAxis(newInputAxis, newAxisPolarity * -1);
                }
                else if (isTrigger && !wasTrigger)
                {
                    inverseAction.ResetAxis();
                }
            }
        }

        public void ResetAxis()
        {
            InputAxis = null;
            AxisPolarity = 0;
        }

        public void ResetKeycode()
        {
            KeyCode = KeyCode.None;
        }
        #endregion

        public float GetAxis()
        {
            float axisValue = 0;
            if (KeyCode != KeyCode.None)
            {
                //Debug.Log(Keycode);
                // this is a key, get the status and return 0 for up, 1 for down
                axisValue = Input.GetKey(KeyCode) ? 1f : 0f;
            }

            if (InputAxis != null)
            {
                //Debug.Log(AxisName);
                //axisValue = Input.GetAxis(InputAxis.ToString());
                axisValue = InputAxis.GetAxis();
                //Debug.Log(axisValue);
                // This is an axis, get the actual axis value
                axisValue = (AxisPolarity > 0 && axisValue > 0) || (AxisPolarity < 0 && axisValue < 0) ? axisValue : 0f;
            }

            return Mathf.Abs(axisValue);
        }

        public float GetAxisRaw()
        {
            float axisValue = 0;
            if (KeyCode != KeyCode.None)
            {
                //Debug.Log(Keycode);
                // this is a key, get the status and return 0 for up, 1 for down
                axisValue = Input.GetKey(KeyCode) ? 1f : 0f;
            }

            if (InputAxis != null)
            {
                //Debug.Log(AxisName);
                //axisValue = Input.GetAxis(InputAxis.ToString());
                axisValue = InputAxis.GetAxisRaw();
                //Debug.Log(axisValue);
                // This is an axis, get the actual axis value
                axisValue = (AxisPolarity > 0 && axisValue > 0) || (AxisPolarity < 0 && axisValue < 0) ? axisValue : 0f;
            }

            return Mathf.Abs(axisValue);
        }

        public string GetAxisName()
        {
            string axisNameWithPolarity;

            if (InputAxis != null)
            {
                axisNameWithPolarity = InputAxis.AxisName.ToString();
            }
            else if (KeyCode != KeyCode.None)
            {
                axisNameWithPolarity = KeyCode.ToString();
            }
            else
            {
                return "No Assignment";
            }

            if (AxisPolarity > 0)
            {
                axisNameWithPolarity += " +";
            }
            else if (AxisPolarity < 0)
            {
                axisNameWithPolarity += " -";
            }

            return axisNameWithPolarity;
        }

        public string GetAxisDescription()
        {
            string axisNameWithPolarity;

            if (InputAxis != null)
            {
                axisNameWithPolarity = InputAxis.Description;
            }
            else if (KeyCode != KeyCode.None)
            {
                axisNameWithPolarity = KeyCodeToDescription(KeyCode);
            }
            else
            {
                return "No Assignment";
            }

            if (AxisPolarity > 0)
            {
                axisNameWithPolarity += " +";
            }
            else if (AxisPolarity < 0)
            {
                axisNameWithPolarity += " -";
            }

            return axisNameWithPolarity;
        }

        string KeyCodeToDescription(KeyCode keyCode)
        {
            string keyCodeDescription = "";
            
            switch (keyCode)
            {
                case (KeyCode.Alpha0):
                case (KeyCode.Alpha1):
                case (KeyCode.Alpha2):
                case (KeyCode.Alpha3):
                case (KeyCode.Alpha4):
                case (KeyCode.Alpha5):
                case (KeyCode.Alpha6):
                case (KeyCode.Alpha7):
                case (KeyCode.Alpha8):
                case (KeyCode.Alpha9):
                    keyCodeDescription = keyCode.ToString().Substring(5);
                    break;
                case (KeyCode.Mouse0):
                    keyCodeDescription = "Left-Click";
                    break;
                case (KeyCode.Mouse1):
                    keyCodeDescription = "Right-Click";
                    break;
                default:
                    keyCodeDescription = keyCode.ToString();
                    break;
            }

            return keyCodeDescription;
        }

        #region Property Controlled Members
        private string _name;
        private KeyCode _keycode;
        private string _axisName;
        private int _axisPolarity;
        //private InputAction _inverseAction;
        #endregion
    }
}
