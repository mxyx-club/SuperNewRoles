namespace SuperNewRoles.Helpers;
public static class MurderHelpers
{
    public static void RpcMurderPlayerFlags(this PlayerControl player, PlayerControl target, MurderResultFlags flags, PlayerControl SendTarget = null)
    {
        if (player == null || target == null) return;
        MessageWriter writer = RPCHelper.StartRPC(player.NetId, RpcCalls.MurderPlayer, SendTarget);
        writer.WriteNetObject(target);
        writer.Write((int)flags);
        writer.EndRPC();
        player.MurderPlayer(target, flags);
    }
    public static void RpcMurderPlayerForce(this PlayerControl player, PlayerControl target)
    {
        RpcMurderPlayerFlags(player, target,
            MurderResultFlags.Succeeded);
    }
    public static void RpcMurderPlayerFailed(this PlayerControl player, PlayerControl target)
    {
        RpcMurderPlayerFlags(player, target,
            MurderResultFlags.FailedError);
    }
    public static void RpcForceGuard(this PlayerControl shower, PlayerControl target, PlayerControl SendTarget = null)
    {
        RpcMurderPlayerFlags(shower, target,
            MurderResultFlags.FailedProtected, SendTarget);
    }
    public static void RpcMurderPlayerOnCheck(this PlayerControl player, PlayerControl target)
    {
        MurderResultFlags flags = MurderResultFlags.Succeeded;
        if (target.protectedByGuardianId > -1)
        {
            flags = MurderResultFlags.FailedProtected;
        }
        RpcMurderPlayerFlags(player, target,
            flags);
    }
}