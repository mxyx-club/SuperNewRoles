using System.Collections.Generic;

namespace SuperNewRoles.SuperNewRolesWeb;
public class RoomPlayerData
{
    public static RoomPlayerData Instance
    {
        get
        {
            if (_instance == null || _instance.Local == null)
            {
                if (PlayerControl.LocalPlayer != null) return new();
                else { return null; }
            }
            return _instance;
        }
    }
    public static RoomPlayerData _instance;
    public PlayerControl Local;

    public List<WebPlayer> WebPlayers = new();
    public RoomPlayerData()
    {
        if (_instance != null) return;
        if (PlayerControl.LocalPlayer == null) return;
        _instance = this;
        Local = PlayerControl.LocalPlayer;
        WebPlayers = new();
        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
        {
            WebPlayers.Add(WebPlayer.Create(p));
        }
    }
    public void UpdatePlayer()
    {

    }
}