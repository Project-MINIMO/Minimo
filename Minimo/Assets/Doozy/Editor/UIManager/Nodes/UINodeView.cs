// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Nody;
using Doozy.Editor.UIManager.Nodes.PortData;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using Doozy.Runtime.UIManager.Nodes.PortData;
using UnityEngine;

namespace Doozy.Editor.UIManager.Nodes
{
    public sealed class UINodeView : FlowNodeView
    {
        public override void Dispose()
        {
            base.Dispose();

            goBackInputPortDataView.Recycle();
            portDataViews.ForEach(item => item?.Recycle());
        }

        public override Type nodeType => typeof(UINode);
        public override Texture2D nodeIconTexture => EditorTextures.Nody.Icons.UINode;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.UINode;

        private List<UIOutputPortDataView> portDataViews { get; set; } = new List<UIOutputPortDataView>();
        private GoBackInputPortDataView goBackInputPortDataView { get; set; }

        public UINodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
        }

        public override void RefreshData()
        {
            base.RefreshData();
            //check if there is a back button port
            bool hasBackButton = false;
            for (int i = 0; i < portDataViews.Count; i++)
            {
                UIOutputPortDataView item = portDataViews[i];
                if (item == null) continue;
                item.RefreshData();
                if (!item.isBackButton) continue;
                hasBackButton = true;
            }

            if (!hasBackButton) return;                          //if there is no back button port, skip the rest
            if (goBackInputPortDataView?.data == null) return;   //if there is no go back input port data, skip the rest
            if (!goBackInputPortDataView.data.CanGoBack) return; //if the go back input port data cannot go back, skip the rest
            goBackInputPortDataView.data.CanGoBack = false;      //reset the go back input port data to false
        }

        public override void RefreshPortsViews()
        {
            base.RefreshPortsViews();

            goBackInputPortDataView?.Recycle();
            inputPortViews[0].AddChild
            (
                goBackInputPortDataView =
                    GoBackInputPortDataView.Get()
                        .SetPort(flowNode.firstInputPort)
            );

            portDataViews.ForEach(item => item?.Recycle());
            portDataViews.Clear();

            int backButtonCount = 0;

            for (int i = 0; i < flowNode.outputPorts.Count; i++)
            {
                UIOutputPortDataView portDataView =
                    UIOutputPortDataView.Get()
                        .SetPort(flowNode.outputPorts[i]);

                portDataViews.Add(portDataView);
                outputPortViews[i].Insert(0, portDataView);

                // show the the back button details
                if (!portDataView.isBackButton) continue;                                      //if this is not the back button, skip it
                outputPortViews[i].SetEnabled(backButtonCount == 0);                           //enable the back button port only if we don't already have a back button port
                backButtonCount++;                                                             //increment the back button count
                outputPortViews[i].portColor = EditorColors.Nody.BackFlow;                     //change the color of the back button port
                portDataView.iconReaction.SetTextures(EditorSpriteSheets.EditorUI.Icons.Back); //change the icon of the back button port
                portDataView.iconReaction.Play();                                              //play the icon reaction
            }

            RefreshData();

            InjectAddOutputButton();
        }
    }
}
