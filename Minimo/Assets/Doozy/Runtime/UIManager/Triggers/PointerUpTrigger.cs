// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/PointerUp")]
    public class PointerUpTrigger : SignalProvider, IPointerUpHandler
    {
        /// <summary> Called when the pointer is released </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public PointerUpTrigger() : base(ProviderType.Local, "Pointer", "Up", typeof(PointerUpTrigger)) {}

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
