// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Runtime.Common
{
    /// <summary>
    /// Specialized class that allows call a method on the main thread.
    /// This is useful when you have to call a Unity API from a thread that is not the main thread.
    /// </summary>
    public class SyncContext : SingletonBehaviour<SyncContext>
    {
        /// <summary> Task scheduler that runs tasks on the main thread </summary>
        public static TaskScheduler unityTaskScheduler { get; private set; }

        /// <summary> Unity's main thread </summary>
        public static int unityThread { get; private set; }

        /// <summary> Synchronization context for the main thread </summary>
        public static SynchronizationContext unitySynchronizationContext { get; private set; }

        /// <summary> Queue of tasks to be executed on the main thread </summary>
        public static Queue<Action> runInUpdate { get; } = new Queue<Action>();

        /// <summary> Returns TRUE if the current thread is the Unity main thread </summary>
        public static bool isOnUnityThread => unityThread == Thread.CurrentThread.ManagedThreadId;

        protected override void Awake()
        {
            base.Awake();

            unitySynchronizationContext = SynchronizationContext.Current;
            unityThread = Thread.CurrentThread.ManagedThreadId;
            unityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private void Update()
        {
            //if there are no actions in the queue, do nothing
            while (runInUpdate.Count > 0)
            {
                Action action = null;
                lock (runInUpdate)
                {
                    if (runInUpdate.Count > 0)
                        action = runInUpdate.Dequeue();
                }
                action?.Invoke();
            }
        }
        
        public static void Initialize()
        {
            _ = instance;
        }
        
        /// <summary> Runs the given action on the main thread </summary>
        /// <param name="action"> Action to run on the main thread </param>
        public static void RunOnUnityThread(Action action)
        {
            Initialize();
            
            if (unityThread == Thread.CurrentThread.ManagedThreadId) //if we are already on the main thread, just run the action
            {
                action();
                return;
            }

            //if we are not on the main thread, add the action to the queue
            lock (runInUpdate)
            {
                runInUpdate.Enqueue(action);
            }
        }
    }
}
