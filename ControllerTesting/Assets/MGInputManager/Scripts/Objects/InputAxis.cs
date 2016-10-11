using UnityEngine;
using System.Collections;

namespace MindGrown
{
    public class InputAxis
    {
        public string InputManagerName { get; set; }
        public string Description { get; set; }
        public float Offset { get; set; }
        public bool isTurboAxis { get; set; }

        public InputAxis()
        {
            InitializationHelper("None", "None", 0f, false);
        }

        public InputAxis(string name, string desc, float offset = 0f, bool turbo=false)
        {
            InitializationHelper(name, desc, offset, turbo);
        }

        void InitializationHelper(string name, string desc, float offset, bool turbo)
        {
            if (true)
            {
                InputManagerName = name;
                Description = desc;
                Offset = offset;
                isTurboAxis = turbo;
            }
        }

        public float GetAxis()
        {
            float axisValue = Input.GetAxis(InputManagerName) + Offset;
            if (!isTurboAxis)
            {
                axisValue = axisValue > 1f ? 1f : axisValue;
                axisValue = axisValue < -1f ? -1f : axisValue;
            }
            return axisValue;
        }
    }
}
