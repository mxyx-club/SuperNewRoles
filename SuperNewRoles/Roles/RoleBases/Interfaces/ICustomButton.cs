namespace SuperNewRoles.Roles.RoleBases.Interfaces;
public interface ICustomButton
{
    public CustomButtonInfo[] CustomButtonInfos { get; }
    public CustomButtonInfo ButtonInfo => CustomButtonInfos.FirstOrDefault();
}