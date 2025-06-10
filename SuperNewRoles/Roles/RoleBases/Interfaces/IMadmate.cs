using SuperNewRoles.Patches;

namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface IMadmate : IVentAvailable, IImpostorVision
{
    int CheckTask => -1;
    bool HasCheckImpostorAbility { get; }
    bool CanSeeImpostor(PlayerControl me)
    {
        return HasCheckImpostorAbility && CheckTask <= TaskCount.TaskDate(me.Data).Item1;
    }
}