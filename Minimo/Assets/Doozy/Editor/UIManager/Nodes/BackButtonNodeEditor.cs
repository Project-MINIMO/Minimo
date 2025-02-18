// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(BackButtonNode))]
    public class BackButtonNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.BackButtonNode;

        private SerializedProperty propertyNodeCommand { get; set; }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(BackButtonNode)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Nodes.BackButtonNode.html")
                .AddYouTubeButton();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyNodeCommand = serializedObject.FindProperty("NodeCommand");
        }
        
        protected override void Compose()
        {
            base.Compose();

            root
                .AddSpaceBlock(2)
                .AddChild
                (
                    FluidField.Get()
                        .SetLabelText("Back Button")
                        .AddFieldContent(DesignUtils.NewEnumField(propertyNodeCommand).SetStyleFlexGrow(1))
                );
        }
    }
}
