// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/UIDeselected")]
    public class UIDeselectedTrigger : SignalProvider, IDeselectHandler
    {
        /// <summary> Called when a Selectable is deselected </summary>
        public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();
        
        public UIDeselectedTrigger() : base(ProviderType.Local, "UI", "Deselected", typeof(UIDeselectedTrigger)) {}

        public void OnDeselect(BaseEventData eventData)
        {
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}