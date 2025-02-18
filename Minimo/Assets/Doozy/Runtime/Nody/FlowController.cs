// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PartialTypeWithSinglePart

namespace Doozy.Runtime.Nody
{
    /// <summary>
    /// The Flow Controller is responsible of managing a Flow Graph.
    /// It can control the graph either as a local graph (instance) or a global graph.
    /// </summary>
    [AddComponentMenu("Doozy/Nody/Flow Controller")]
    public partial class FlowController : MonoBehaviour, IUseMultiplayerInfo
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Doozy/Nody/Flow Controller", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<FlowController>(false, true);
        }
        #endif

        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;
        /// <summary> True if Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        [SerializeField] private bool DontDestroyOnSceneChange;
        [SerializeField] private FlowGraph Flow;
        [SerializeField] private FlowType FlowType = FlowType.Global;
        [SerializeField] private ControllerBehaviour OnEnableBehaviour = ControllerBehaviour.StartFlow;
        [SerializeField] private ControllerBehaviour OnDisableBehaviour = ControllerBehaviour.StopFlow;
        [SerializeField] private UnityEvent OnStart = new UnityEvent();
        [SerializeField] private UnityEvent OnStop = new UnityEvent();
        [SerializeField] private UnityEvent OnPause = new UnityEvent();
        [SerializeField] private UnityEvent OnResume = new UnityEvent();
        [SerializeField] private UnityEvent OnBackFlow = new UnityEvent();

        /// <summary> Flag that makes sure that the Flow Controller is not destroyed when a new scene is loaded </summary>
        public bool dontDestroyOnSceneChange
        {
            get => DontDestroyOnSceneChange;
            set => DontDestroyOnSceneChange = value;
        }

        /// <summary> Flow graph managed by this controller </summary>
        public FlowGraph flow => Flow;

        /// <summary> Type of flow </summary>
        public FlowType flowType => FlowType;

        /// <summary> Behaviour of the controller OnEnable </summary>
        public ControllerBehaviour onEnableBehaviour
        {
            get => OnEnableBehaviour;
            set => OnEnableBehaviour = value;
        }

        /// <summary> Behaviour of the controller OnDisable </summary>
        public ControllerBehaviour onDisableBehaviour
        {
            get => OnDisableBehaviour;
            set => OnDisableBehaviour = value;
        }

        /// <summary> Called when the flow graph starts or restarts </summary>
        public UnityEvent onStart => OnStart ?? (OnStart = new UnityEvent());

        /// <summary> Called when the flow graph is stopped </summary>
        public UnityEvent onStop => OnStop ?? (OnStop = new UnityEvent());

        /// <summary> Called when the flow graph is paused </summary>
        public UnityEvent onPause => OnPause ?? (OnPause = new UnityEvent());

        /// <summary> Called when the flow graph is resumed </summary>
        public UnityEvent onResume => OnResume ?? (OnResume = new UnityEvent());

        /// <summary> Called when the 'Back' flow is triggered in the flow graph </summary>
        public UnityEvent onBackFlow => OnBackFlow ?? (OnBackFlow = new UnityEvent());

        #region Player Index

        [SerializeField] private MultiplayerInfo MultiplayerInfo;
        public MultiplayerInfo multiplayerInfo => MultiplayerInfo;
        public bool hasMultiplayerInfo => multiplayerInfo != null;
        public int playerIndex => multiplayerMode & hasMultiplayerInfo ? multiplayerInfo.playerIndex : inputSettings.defaultPlayerIndex;
        public void SetMultiplayerInfo(MultiplayerInfo reference) => MultiplayerInfo = reference;

        #endregion

        /// <summary> Flag used to keep track for when this FlowController has been initialized </summary>
        public bool initialized { get; private set; }

        /// <summary> Flag used to determine if this controller has been initialized and has a valid flow graph assigned to it </summary>
        public bool isValid => initialized && Flow != null && Flow.controller == this;

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;

            if (dontDestroyOnSceneChange & transform.parent != null)
            {
                Debug.LogWarning
                (
                    $"[{nameof(FlowController)}] - {name} is set to 'Don't destroy controller on scene change' but it has a parent. " +
                    $"For this to work, the controller must be a root object in the scene hierarchy (it must not have a parent)."
                );
            }

            if (dontDestroyOnSceneChange)
            {
                DontDestroyOnLoad(gameObject);
            }

            BackButton.Initialize();

            initialized = false;
        }

        protected virtual IEnumerator Start()
        {
            if (!Application.isPlaying) yield break;
            yield return null;
            SetFlowGraph(Flow);
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return; //do not execute this code in the editor
            RunBehavior(onEnableBehaviour);     //run the set behaviour when the controller is enabled
        }

        protected virtual void OnDisable()
        {
            if (!Application.isPlaying) return; //do not execute this code in the editor
            RunBehavior(onDisableBehaviour);    //run the set behaviour when the controller is disabled
        }

        protected virtual void RunBehavior(ControllerBehaviour behaviour)
        {
            // Debug.Log($"[FlowController] RunBehavior -> {behaviour}");

            switch (behaviour)
            {
                case ControllerBehaviour.Disabled:
                    //do nothing
                    break;
                case ControllerBehaviour.StartFlow:
                    StartFlow();
                    break;
                case ControllerBehaviour.RestartFlow:
                    RestartFlow();
                    break;
                case ControllerBehaviour.StopFlow:
                    StopFlow();
                    break;
                case ControllerBehaviour.PauseFlow:
                    PauseFlow();
                    break;
                case ControllerBehaviour.ResumeFlow:
                    ResumeFlow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }
        }

        private void Update()
        {
            if (!isValid) return; //do not execute this code if the controller has not been initialized or if the flow graph is not valid
            Flow.Update();        //tick the Update method of the flow graph
        }

        private void FixedUpdate()
        {
            if (!isValid) return; //do not execute this code if the controller has not been initialized or if the flow graph is not valid
            Flow.FixedUpdate();   //tick the FixedUpdate method of the flow graph
        }

        private void LateUpdate()
        {
            if (!isValid) return; //do not execute this code if the controller has not been initialized or if the flow graph is not valid
            Flow.LateUpdate();    //tick the LateUpdate method of the flow graph
        }

        /// <summary> Set a new flow graph to this controller </summary>
        /// <param name="graph"> Target flow graph </param>
        public void SetFlowGraph(FlowGraph graph)
        {
            if (isValid)
            {
                StopFlow();
                Flow.OnStart.RemoveListener(OnStartFlow);
                Flow.OnStop.RemoveListener(OnStopFlow);
                Flow.OnPause.RemoveListener(OnPauseFlow);
                Flow.OnResume.RemoveListener(OnResumeFlow);
                Flow.OnBackFlow.RemoveListener(OnBackFlowTriggered);
                Flow.controller = null;
                Flow = null;
            }
            enabled = graph != null;
            if (graph == null) return;
            Flow = flowType == FlowType.Local ? graph.Clone() : graph;
            Flow.OnStart.AddListener(OnStartFlow);
            Flow.OnStop.AddListener(OnStopFlow);
            Flow.OnPause.AddListener(OnPauseFlow);
            Flow.OnResume.AddListener(OnResumeFlow);
            Flow.OnBackFlow.AddListener(OnBackFlowTriggered);
            StartCoroutine(InitializeFlow());
        }

        /// <summary> Activate the given node (if it exists in the flow graph) </summary>
        /// <param name="node"> Note to search for in the loaded flow graph </param>
        /// <param name="fromPort"> Pass a port as the source port </param>
        public void SetActiveNode(FlowNode node, FlowPort fromPort = null)
        {
            if (!isValid) return;
            if (node == null) return;
            if (!Flow.ContainsNode(node)) return;
            Flow.SetActiveNode(node, fromPort);
        }

        /// <summary> Activate the node with the given id (if it exists in the flow graph) </summary>
        /// <param name="nodeId"> Node id to search for in the loaded flow graph </param>
        public void SetActiveNodeById(string nodeId)
        {
            if (!isValid) return;
            if (nodeId.IsNullOrEmpty()) return;
            if (!Flow.ContainsNodeById(nodeId)) return;
            Flow.SetActiveNodeByNodeId(nodeId);
        }

        /// <summary> Activate the first node with the given node name (if it exists in the flow graph) </summary>
        /// <param name="nodeName"> Node name to search for in the loaded flow graph </param>
        public void SetActiveNodeByName(string nodeName)
        {
            if (!isValid) return;
            if (nodeName.IsNullOrEmpty()) return;
            if (!Flow.ContainsNodeByName(nodeName)) return;
            Flow.SetActiveNodeByNodeName(nodeName);
        }

        /// <summary>
        /// Reset the flow graph and set its state to Idle.
        /// This means that the graph has not been started yet, thus there is no active node.
        /// </summary>
        public void ResetFlow()
        {
            if (!isValid) return;
            Flow.ResetGraph();
        }

        /// <summary>
        /// Reset the flow graph. This will reset the graph and activate the first node.
        /// <para/> Even if the graph is paused, it will reset and start from the beginning.
        /// </summary>
        public void RestartFlow()
        {
            if (!isValid) return;
            Flow.Restart();
        }

        /// <summary>
        /// Start the flow graph. This will reset the graph and activate the first node.
        /// <para/> If the graph is paused, it will resume from the last active node instead of the first node.
        /// </summary>
        public void StartFlow()
        {
            if (!isValid) return;
            Flow.Start();
        }

        /// <summary>
        /// Stop the flow graph.
        /// </summary>
        public void StopFlow()
        {
            if (!isValid) return;
            Flow.Stop();
        }

        /// <summary>
        /// Pause the flow graph.
        /// <para/> If the graph state is not Running, it will do nothing.
        /// </summary>
        public void PauseFlow()
        {
            if (!isValid) return;
            Flow.Pause();
        }

        /// <summary>
        /// Resume the flow graph.
        /// <para/> If the graph state is Idle (stopped), it will start the graph instead. 
        /// </summary>
        public void ResumeFlow()
        {
            if (!isValid) return;
            Flow.Resume();
        }

        protected virtual void OnStartFlow() =>
            onStart?.Invoke();

        protected virtual void OnStopFlow() =>
            onStop?.Invoke();

        protected virtual void OnPauseFlow() =>
            onPause?.Invoke();

        protected virtual void OnResumeFlow() =>
            onResume?.Invoke();

        protected virtual void OnBackFlowTriggered() =>
            onBackFlow?.Invoke();

        /// <summary> Initialize the flow graph and start it at the end of the second frame </summary>
        private IEnumerator InitializeFlow()
        {
            yield return null;                    //wait for the second frame
            yield return new WaitForEndOfFrame(); //wait for the end of the second frame
            if (Flow == null) yield break;        //if the flow graph is null, exit
            initialized = true;                   //set the initialized flag to true
            Flow.controller = this;               //set the flow graph controller to this
            StartFlow();                          //start the flow graph
        }
    }
}
