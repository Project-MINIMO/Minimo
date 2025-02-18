// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.UIManager.Components;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Events
{
    /// <summary> UnityEvent with a UIToggle parameter </summary>
    [Serializable]
    public class UIToggleEvent : UnityEvent<UIToggle>
    {
    }
}
