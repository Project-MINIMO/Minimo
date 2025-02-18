using TMPro;
using UnityEngine;
// ReSharper disable CheckNamespace

namespace Doozy.Sandbox.Reactor.Runtime
{
    public class IntAnimatorExample : MonoBehaviour
    {
        public TextMeshProUGUI ValueLabel;

        private int m_Value;
        public int value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                ValueLabel.text = value.ToString();
            }
        }

        private void Reset()
        {
            ValueLabel ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            value = value;
        }
    }
}
