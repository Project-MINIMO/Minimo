// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/PointerExit")]
    public class PointerExitTrigger : SignalProvider, IPointerExitHandler
    {
        /// <summary> Called when the pointer exits the trigger </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public PointerExitTrigger() : base(ProviderType.Local, "Pointer", "Exit", typeof(PointerExitTrigger)) {}

        public void OnPointerExit(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
