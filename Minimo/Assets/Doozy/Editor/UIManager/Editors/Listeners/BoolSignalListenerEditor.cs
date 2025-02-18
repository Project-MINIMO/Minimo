// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Listeners;
using UnityEditor;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(BoolSignalListener), true)]
    public class BoolSignalListenerEditor : SignalListenerEditor
    {
        private BoolSignalListener castedTarget => (BoolSignalListener)target;
        private IEnumerable<BoolSignalListener> castedTargets => targets.Cast<BoolSignalListener>();

        private FluidField onBoolSignalFluidField { get; set; }
        
        private SerializedProperty propertyOnBoolSignal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onBoolSignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnBoolSignal = serializedObject.FindProperty("OnBoolSignal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("Bool")
                .SetComponentTypeText("Signal Listener");
            

            onBoolSignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a boolean parameter", propertyOnBoolSignal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(idFluidField)
                .AddSpaceBlock()
                .AddChild(onBoolSignalFluidField)
                // .AddSpaceBlock()
                // .AddChild(callbackFluidField)
                // .AddSpaceBlock()
                // .AddChild(onSignalFluidField)
                .AddEndOfLineSpace()
                ;
        }
    }
}