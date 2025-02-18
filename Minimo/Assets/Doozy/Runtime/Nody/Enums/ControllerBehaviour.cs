// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Nody
{
    /// <summary> Defines how a FlowController can behave without any outside input </summary>
    public enum ControllerBehaviour
    {
        /// <summary> Do nothing </summary>
        Disabled,
        
        /// <summary> Start the Flow (or Resume if it was paused) </summary>
        StartFlow,
        
        /// <summary> Restart the Flow (even if it's paused or running) </summary>
        RestartFlow,
        
        /// <summary> Stop the Flow </summary>
        StopFlow,
        
        /// <summary> Pause the Flow (if it's running) </summary>
        PauseFlow,
        
        /// <summary> Resume the Flow (or Start it it's not paused) </summary>
        ResumeFlow
    }
}
