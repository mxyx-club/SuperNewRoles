namespace SuperNewRoles.Roles.RoleBases.Interfaces;
/// <summary>
/// 全員視点のFixedUpdateを使用する際に使うインターフェース
/// </summary>
public interface IFixedUpdaterAll
{
    /// <summary>
    /// DefaultモードでのFixedUpdate
    /// </summary>
    void FixedUpdateAllDefault();
    /// <summary>
    /// SHRモードでのFixedUpdate
    /// </summary>
    virtual void FixedUpdateAllSHR() { }
}