// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes.System;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Nodes
{
    [CustomEditor(typeof(StartNode))]
    public class StartNodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.StartNode;
        public override Color nodeAccentColor => EditorColors.Nody.StickyNote;
        
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(StartNode)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Nody.Nodes.System.StartNode.html")
                .AddYouTubeButton();

        }
    }
}
