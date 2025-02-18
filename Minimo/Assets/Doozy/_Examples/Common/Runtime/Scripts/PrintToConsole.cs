using UnityEngine;
// ReSharper disable CheckNamespace

namespace Doozy.Sandbox.Common.Runtime
{
   public class PrintToConsole : MonoBehaviour
   {
      public void DebugLog(string message)
      {
         Debug.Log(message);
      }
   }
}
