using Dalamud.Game;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.WKS;
using GatherBuddy.Plugin;

namespace GatherBuddy.SeFunctions;

public sealed class CurrentBait : SeAddressBase
{
    public CurrentBait(ISigScanner sigScanner)
        : base(sigScanner, "8B 0D ?? ?? ?? ?? 3B D9 75")
    {
        Dalamud.Interop.InitializeFromAttributes(this);
    }

    public unsafe uint Current
    {
        get 
        {
            var territoryId = Dalamud.ClientState.TerritoryType;
            if (GatherBuddy.GameData.Territories.TryGetValue(territoryId, out var territory) && territory.Data.TerritoryIntendedUse.RowId is 60)
            {
                var cosmicManager = WKSManager.Instance();
                if (cosmicManager != null)
                    return cosmicManager->FishingBait;
            }

            return Address != System.IntPtr.Zero ? *(uint*)Address : 0u;
        }
    }

    private delegate byte ExecuteCommandDelegate(int id, int unk1, uint baitId, int unk2, int unk3);

    // TW: fallible so a missing signature only disables bait switching.
    [Signature("E8 ?? ?? ?? ?? 41 C6 04 24", Fallibility = Fallibility.Fallible)]
    private readonly ExecuteCommandDelegate? _executeCommand = null;

    public enum ChangeBaitReturn
    {
        Success,
        AlreadyEquipped,
        NotInInventory,
        InvalidBait,
        UnknownError,
    }

    public static unsafe int HasItem(uint itemId)
        => InventoryManager.Instance()->GetInventoryItemCount(itemId);

    public ChangeBaitReturn ChangeBait(uint baitId)
    {
        if (baitId == Current)
            return ChangeBaitReturn.AlreadyEquipped;

        if (baitId == 0 || !GatherBuddy.GameData.Bait.ContainsKey(baitId))
            return ChangeBaitReturn.InvalidBait;

        if (HasItem(baitId) <= 0)
            return ChangeBaitReturn.NotInInventory;

        if (_executeCommand == null)
            return ChangeBaitReturn.UnknownError;

        return _executeCommand(701, 4, baitId, 0, 0) == 1 ? ChangeBaitReturn.Success : ChangeBaitReturn.UnknownError;
    }
}
