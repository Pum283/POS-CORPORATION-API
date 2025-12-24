namespace Application;

public class RolesMappingProfile : Profile
{
    public RolesMappingProfile()
    {
        CreateMap<RoleEntity, Role>().ReverseMap();
        CreateMap<UserRoleEntity, Role>().ReverseMap();
        CreateMap<PermissionEntity, Permission>();
        CreateMap<AddPermissionsForRoleModel, List<RolePermissionEntity>>()
            .ConvertUsing((src, dest, context) =>
            {
                return src.PermissionId.Select(permissionId => new RolePermissionEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = src.RoleId,
                    PermissionId = permissionId
            }).ToList();
        });

        CreateMap<RolePermissionEntity, GroupedPermissionModel>().ReverseMap();
        CreateMap<AddOrUpdateRoleHierarchieModel, RoleHierarchyEntity>().ReverseMap();
    }
}
