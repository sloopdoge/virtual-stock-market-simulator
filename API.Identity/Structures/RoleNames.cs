using API.Identity.Enums;

namespace API.Identity.Structures;

public readonly struct RoleNames
{
    public static readonly string Guest = RoleEnum.Guest.ToString();
    public static readonly string User = RoleEnum.User.ToString();
    public static readonly string Admin = RoleEnum.Admin.ToString();
    public static readonly string ApiUser = RoleEnum.ApiUser.ToString();
}