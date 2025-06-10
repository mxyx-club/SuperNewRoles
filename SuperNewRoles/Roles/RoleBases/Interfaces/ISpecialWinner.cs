using static SuperNewRoles.Patches.CheckGameEndPatch;

namespace SuperNewRoles.Roles.RoleBases.Interfaces;

public interface ISpecialWinner
{
    bool CheckAndEndGame(ShipStatus __instance, PlayerStatistics statistics);
}
