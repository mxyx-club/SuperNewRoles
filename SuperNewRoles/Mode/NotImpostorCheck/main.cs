using System.Collections.Generic;

namespace SuperNewRoles.Mode.NotImpostorCheck;

internal class Main
{
    public static List<int> Impostors;
    public static void ClearAndReload()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            Impostors = new();
        }
    }
}