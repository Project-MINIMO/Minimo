// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Targets;
using UnityEditor;
using UnityEngine.UI;

namespace Doozy.Editor.Reactor.Editors.Targets
{
    [CustomEditor(typeof(ImageSpriteTarget), true)]
    public class ImageSpriteTargetEditor : ReactorSpriteTargetEditor
    {
        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(Image)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Reactor.Targets.ImageSpriteTarget.html")
                .AddYouTubeButton();

            targetObjectField
                .SetObjectType(typeof(Image));
        }
    }
}
