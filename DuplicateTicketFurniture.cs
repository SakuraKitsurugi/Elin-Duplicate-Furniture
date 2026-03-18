using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Mod
{
    [BepInPlugin("sakura.elin.DuplicateTicketFurniture", "Duplicate Ticket Furniture", "0.2")]
    public class Main : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("sakura.elin.DuplicateTicketFurniture");
            harmony.PatchAll();
        }
    }
}

namespace Patcher
{
    [HarmonyPatch]
    public class Patch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(TraitTicketFurniture).GetNestedType("<>c__DisplayClass5_1", BindingFlags.NonPublic),
                "<TrySetHeldAct>b__1");
        }
        
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Chara), nameof(Chara.Pick))))
                .ThrowIfInvalid("failed to find Chara.Pick")
                .SetInstructionAndAdvance(
                    Transpilers.EmitDelegate(Delegate))
                .InstructionEnumeration();
        }

        private static Thing Delegate(Chara pc, Thing thingy, bool msg = true, bool stack = true)
        {
            var dupe = thingy.Duplicate(1);
            pc.Pick(dupe);
            thingy.isNPCProperty = true;
            if (thingy.trait is TraitPillow)
                thingy.noSell = false;
            return thingy;
        }
    }
}


