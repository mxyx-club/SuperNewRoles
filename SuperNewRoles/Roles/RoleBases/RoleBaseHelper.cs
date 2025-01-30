using System;
using System.Collections.Generic;

namespace SuperNewRoles.Roles.RoleBases;
public static class RoleBaseHelper
{
    public static Dictionary<RoleId, Type> allRoleIds = new()
    {
        //{ RoleId.SidekickWaveCannon, typeof(RoleBase<SidekickWaveCannon>) },
    };

    public static void SetUpOptions()
    {
        //new SidekickWaveCannon().SetUpOption();
    }
}