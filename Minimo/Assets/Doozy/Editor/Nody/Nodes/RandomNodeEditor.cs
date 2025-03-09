// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Editor.Nody.Nodes.PortData;
using Doozy.Runtime.Nody.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(RandomNode))]
    public class RandomNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.RandomNode;

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(RandomNode)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Nody.Nodes.RandomNode.html")
                .AddYouTubeButton();

            RefreshNodeEditor();
        }

        public override void RefreshNodeEditor()
        {
            base.RefreshNodeEditor();
            flowNode.outputPorts.ForEach(port => portsContainer.AddChild(new RandomNodeOutputPortDataEditor(port, nodeView)));
        }
    }
}