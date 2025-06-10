namespace SuperNewRoles.Mode.HideAndSeek;

internal class main
{
    public static bool IsAllInMod;
    public static void ClearAndReloads()
    {
        IsAllInMod = true;
        foreach (ClientData client in AmongUsClient.Instance.allClients)
        {
            if (!client.IsMod())
            {
                IsAllInMod = false;
                break;
            }
        }
    }
}