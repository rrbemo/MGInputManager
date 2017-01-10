using UnityEngine;
using System.Collections;

namespace MindGrown
{
    public class InputAxis
    {
        public AxisName AxisName { get; set; }
        public string Description { get; set; }
        public float Offset { get; set; }
        public bool isTurboAxis { get; set; }

        public InputAxis()
        {
            InitializationHelper(AxisName.None, "None", 0f, false);
        }

        public InputAxis(AxisName name, string desc, float offset = 0f, bool turbo=false)
        {
            InitializationHelper(name, desc, offset, turbo);
        }

        void InitializationHelper(AxisName name, string desc, float offset, bool turbo)
        {
            if (true)
            {
                AxisName = name;
                Description = desc;
                Offset = offset;
                isTurboAxis = turbo;
            }
        }

        public float GetAxis()
        {
            if (InputManager.currentIgnoreAxisList.Contains(AxisName))
            {
                // Ignore the axis
                return 0f;
            }
            float axisValue = Input.GetAxis(AxisName.ToString()) + Offset;
            return Normalize(axisValue);
                
        }

        public float GetAxisRaw()
        {
            if (InputManager.currentIgnoreAxisList.Contains(AxisName))
            {
                // Ignore the axis
                return 0f;
            }
            float axisValue = Input.GetAxisRaw(AxisName.ToString()) + Offset;
            return Normalize(axisValue);
        }

        float Normalize(float initialValue)
        {
            float axisValue = initialValue;
            if (!isTurboAxis)
            {
                axisValue = Mathf.Clamp(axisValue, -1f, 1f);
                //axisValue = axisValue > 1f ? 1f : axisValue;
                //axisValue = axisValue < -1f ? -1f : axisValue;
            }
            return axisValue;
        }
    }
}
