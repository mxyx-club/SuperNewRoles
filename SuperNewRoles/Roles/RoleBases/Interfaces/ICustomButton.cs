namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface ICustomButton
{
    CustomButtonInfo[] CustomButtonInfos { get; }
    CustomButtonInfo ButtonInfo => CustomButtonInfos.FirstOrDefault();
}