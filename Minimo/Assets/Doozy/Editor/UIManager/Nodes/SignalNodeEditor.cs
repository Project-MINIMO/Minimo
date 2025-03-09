// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(SignalNode))]
    public class SignalNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.SignalNode;

        private SerializedProperty propertyPayload { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyPayload = serializedObject.FindProperty("Payload");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader.SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SignalNode)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Nodes.SignalNode.html")
                .AddYouTubeButton();
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddSpaceBlock(2)
                .AddChild(DesignUtils.NewPropertyField(propertyPayload));
        }
    }
}
