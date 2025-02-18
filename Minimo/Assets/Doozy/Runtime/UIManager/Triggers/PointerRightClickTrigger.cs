// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/PointerRightClick")]
    public class PointerRightClickTrigger : SignalProvider, IPointerClickHandler
    {
        /// <summary> Called when pointer right button is clicked over the trigger </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public PointerRightClickTrigger() : base(ProviderType.Local, "Pointer", "Right Click", typeof(PointerRightClickTrigger)) {}

        public void OnPointerClick(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            if (eventData.button != PointerEventData.InputButton.Right) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
