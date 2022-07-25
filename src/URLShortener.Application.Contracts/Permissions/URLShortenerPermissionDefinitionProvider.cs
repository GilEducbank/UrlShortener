using URLShortener.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace URLShortener.Permissions;

public class URLShortenerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(URLShortenerPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(URLShortenerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<URLShortenerResource>(name);
    }
}
