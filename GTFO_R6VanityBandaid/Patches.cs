// START OF MIT CODE
using BepInEx.IL2CPP.Hook;
using DropServer;
using DropServer.VanityItems;
using GameData;
using GameEvent;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using IL2Tasks = Il2CppSystem.Threading.Tasks;

namespace GTFO_R6VanityBandaid
{
    public class Patches
    {
        private List<INativeDetour> _detours = new List<INativeDetour>();
        internal unsafe void ApplyNative()
        {
            _detours.Add(INativeDetour.CreateAndApply<UpdateItems>((nint)R6VanityBandaid.GetIl2CppMethod<VanityItemInventory>(nameof(VanityItemInventory.UpdateItems), "System.Void", false, nameof(VanityItemPlayerData)), UpdateItemsPatch, out _originalUpdateItems));
        }

        private static unsafe UpdateItems _originalUpdateItems;

        public unsafe delegate void UpdateItems(IntPtr _this, IntPtr data, Il2CppMethodInfo* methodInfo);

        public unsafe void UpdateItemsPatch(IntPtr _this, IntPtr data, Il2CppMethodInfo* methodInfo)
        {
            R6VanityBandaid.LogMsg("UpdateItems patch running!");

            _originalUpdateItems.Invoke(_this, data, methodInfo);

            var __instance = new VanityItemInventory(_this);

            if (__instance.m_backednItems == null)
            {
                __instance.m_backednItems = new Il2CppSystem.Collections.Generic.List<VanityItem>(0);
            }

            foreach (VanityItemsTemplateDataBlock block in GameDataBlockBase<VanityItemsTemplateDataBlock>.GetAllBlocks())
            {
                VanityItem item = new VanityItem(ClassInjector.DerivedConstructorPointer<VanityItem>());
                item.publicName = block.publicName;
                item.type = block.type;
                item.prefab = block.prefab;
                item.flags = VanityItemFlags.Touched | VanityItemFlags.Acknowledged;
                item.id = block.persistentID;

                __instance.m_backednItems.Add(item);
            }
        }

        private static DropServer.BoosterImplants.BoosterImplantPlayerData.Category EmptyCat()
        {
            var cat = new DropServer.BoosterImplants.BoosterImplantPlayerData.Category(ClassInjector.DerivedConstructorPointer<DropServer.BoosterImplants.BoosterImplantPlayerData.Category>());
            cat.Inventory = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DropServer.BoosterImplants.BoosterImplantInventoryItem>(0);
            return cat;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DropServerClientAPIViaPlayFab), nameof(DropServerClientAPIViaPlayFab.GetInventoryPlayerDataAsync))]
        public static bool DropServerClientAPI_GetInventoryPlayerDataAsyncPatch(GetInventoryPlayerDataRequest request, ref IL2Tasks.Task<GetInventoryPlayerDataResult> __result)
        {

            R6VanityBandaid.LogMsg($"{nameof(DropServerClientAPIViaPlayFab)} -> requested {nameof(GetInventoryPlayerDataRequest)}: EntityToken:{request.EntityToken}, MaxBackendTemplateId:{request.MaxBackendTemplateId}");

            var vanity =  new DropServer.VanityItems.VanityItemPlayerData(ClassInjector.DerivedConstructorPointer<DropServer.VanityItems.VanityItemPlayerData>());

            vanity.Items = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<DropServer.VanityItems.VanityItem>(0);

            var booster = new DropServer.BoosterImplants.BoosterImplantPlayerData(ClassInjector.DerivedConstructorPointer<DropServer.BoosterImplants.BoosterImplantPlayerData>());

            booster.Advanced = EmptyCat();
            booster.Basic = EmptyCat();
            booster.Specialized = EmptyCat();

            var result = new GetInventoryPlayerDataResult(ClassInjector.DerivedConstructorPointer<GetInventoryPlayerDataResult>())
            {
                Boosters = booster,
                VanityItems = vanity,
            };

            __result = IL2Tasks.Task.FromResult(result);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AnalyticsManager), "OnGameEvent", new Type[] { typeof(GameEventData) })]
        public static bool AnalyticsManager_OnGameEventPatch() => false;
    }
}
// END OF MIT CODE
