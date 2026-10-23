using System;
using Dalamud.Game;

namespace GatherBuddy.SeFunctions;

public class SeAddressBase
{
    public readonly IntPtr Address;

    public SeAddressBase(ISigScanner sigScanner, string signature, int offset = 0)
    {
        // TW: tolerate signatures that do not exist on the Taiwan client.
        if (!sigScanner.TryGetStaticAddressFromSig(signature, out Address))
        {
            Address = IntPtr.Zero;
            GatherBuddy.Log.Error($"{GetType().Name}: signature not found on this client, feature disabled.");
        }
        if (Address != IntPtr.Zero)
            Address += offset;
        var baseOffset = (ulong)Address.ToInt64() - (ulong)sigScanner.Module.BaseAddress.ToInt64();
        GatherBuddy.Log.Debug($"{GetType().Name} address 0x{Address.ToInt64():X16}, baseOffset 0x{baseOffset:X16}.");
    }
}
