// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using DefineSymbolsUtils = Doozy.Editor.Common.Utils.DefineSymbolsUtils;

namespace Doozy.Editor.UIManager.Input
{
    public static class DefineSymbolsForInput
    {
        [DefineSymbols(nameof(DefineSymbolsForInput))]
        public static void Run()
        {
            #if ENABLE_INPUT_SYSTEM

            if (DefineSymbolsUtils.HasGlobalDefine("LEGACY_INPUT_MANGER"))
            {
                DefineSymbolsUtils.RemoveGlobalDefine("LEGACY_INPUT_MANGER"); //remove the wrongly spelled define
                DefineSymbolsUtils.AddGlobalDefine("LEGACY_INPUT_MANAGER");   //add the correct spelled define
                return;
            }

            //check for the new input system define
            if (!DefineSymbolsUtils.HasGlobalDefine("INPUT_SYSTEM_PACKAGE"))
            {
                //new input system define not found
                //check for the old input system define
                if (DefineSymbolsUtils.HasGlobalDefine("LEGACY_INPUT_MANAGER"))
                {
                    //old input system define found
                    #if !ENABLE_LEGACY_INPUT_MANAGER
                    //old input system not enabled -> add new input system define
                    DefineSymbolsUtils.RemoveGlobalDefine("LEGACY_INPUT_MANAGER"); //remove the old input system define
                    DefineSymbolsUtils.AddGlobalDefine("INPUT_SYSTEM_PACKAGE");    //add the new input system define
                    #endif
                    return;
                }

                DefineSymbolsUtils.AddGlobalDefine("INPUT_SYSTEM_PACKAGE"); //add the new input system define
                return;
            }

            #elif ENABLE_LEGACY_INPUT_MANAGER
            if (!DefineSymbolsUtils.HasGlobalDefine("LEGACY_INPUT_MANAGER")) //check for the old input system define
            {
                DefineSymbolsUtils.RemoveGlobalDefine("INPUT_SYSTEM_PACKAGE"); //remove the new input system define
                DefineSymbolsUtils.AddGlobalDefine("LEGACY_INPUT_MANAGER");     //add the new input system define
                return;
            }

            #endif
        }
    }
}
