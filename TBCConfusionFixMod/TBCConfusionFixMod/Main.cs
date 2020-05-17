using UnityModManagerNet;
using System;
using System.Reflection;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Designers.Mechanics.Buffs;
using System.Collections.Generic;
using Kingmaker.Blueprints.Items;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.EntitySystem.Entities;
using TurnBased.Utility;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;

namespace TBCConfusionFixMod
{
    internal class Main
    {

        internal static UnityModManagerNet.UnityModManager.ModEntry.ModLogger logger;
        internal static Harmony12.HarmonyInstance harmony;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                logger = modEntry.Logger;
                harmony = Harmony12.HarmonyInstance.Create(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                //DebugError(ex);
                throw ex;
            }
            return true;

        }


        [Harmony12.HarmonyPatch(typeof(UnitAttack), "OnAction")]
        public static class UnitAttack_OnAction_Patch
        {
            [Harmony12.HarmonyPostfix]
            private static void Postfix(UnitAttack __instance)
            {
                UnitEntityData unit = __instance.Executor;
                if (!StatusWrapper.IsInCombat() || !unit.IsCurrentUnit())
                    return;
                bool attackingNearest = unit.Descriptor.State.HasCondition(UnitCondition.AttackNearest);
                bool confusedAttackNearest = false;
                if(unit.Descriptor.State.HasCondition(UnitCondition.Confusion))
                {
                    UnitPartConfusion part = unit.Get<UnitPartConfusion>();
                    if (part.State == ConfusionState.AttackNearest)
                        confusedAttackNearest = true;
                }
                if (attackingNearest || confusedAttackNearest)
                    StatusWrapper.CurrentTurn().ForceToEnd();
            }
        }
    }
}

