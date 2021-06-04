using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using ReportingPortal.Configuration;

namespace Abp.Authorization
{
    /// <summary>
    /// Application should inherit this class to implement <see cref="IPermissionChecker"/>.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public class CustomPermissionChecker: IPermissionChecker, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConfiguration _configuration;
        private readonly ICustomRoleManagementConfig RoleManagementConfig;

        public IIocManager IocManager { get; set; }

        public ILogger Logger { get; set; }

        public IAbpSession AbpSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomPermissionChecker(ICacheManager cacheManager, IUnitOfWorkManager unitOfWorkManager,IConfiguration configuration, ICustomRoleManagementConfig roleManagementConfig)
        {
            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _configuration = configuration;
            RoleManagementConfig = roleManagementConfig;
        }


        public virtual async Task<bool> IsGrantedAsync(string permissionName)
        {
            return AbpSession.UserId.HasValue && await IsGrantedAsync(AbpSession.UserId.Value, permissionName);
        }

        public virtual bool IsGranted(string permissionName)
        {
            return AbpSession.UserId.HasValue && IsGranted(AbpSession.UserId.Value, permissionName);
        }

        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await IsGrantedAsync(userId,GetPermissionByPermissionName(permissionName));
        }

        private  Permission GetPermissionByPermissionName(string permissionName)
        {
            using(SqlConnection conn=new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                conn.Open();

                SqlCommand cmdGetPermission = new SqlCommand("Select name from AbpPermissions where name=@permissionName", conn);
                cmdGetPermission.Parameters.AddWithValue("@permissionName", permissionName);

                var name = (cmdGetPermission.ExecuteScalar())?.ToString();

                Permission permission = new Permission(name);

                return permission;
            } 
        }
        public virtual bool IsGranted(long userId, string permissionName)
        {
            return  IsGranted(userId,GetPermissionByPermissionName(permissionName));
        }

        [UnitOfWork]
        public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }
        }

        [UnitOfWork]
        public virtual bool IsGranted(UserIdentifier user, string permissionName)
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return IsGranted(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return IsGranted(user.UserId, permissionName);
            }
        }


        private async Task<UserPermissionCacheItem> GetUserPermissionCacheItemAsync(long userId)
        {
            var cacheKey = userId + "@" + (GetCurrentTenantId() ?? 0);
            return await _cacheManager.GetUserPermissionCache().GetAsync(cacheKey, async () =>
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
                {
                    conn.Open();

                    //Checking User
                    SqlCommand cmdUsers = new SqlCommand("Select count(*) from AbpUsers where Id=@userId", conn);
                    cmdUsers.Parameters.AddWithValue("@userId", userId);

                    int result = (int)cmdUsers.ExecuteScalar();

                    if (result == 0)
                    {
                        return null;
                    }

                    var newCacheItem = new UserPermissionCacheItem(userId);

                    //Get User Roles by userId
                    SqlCommand cmdRoles = new SqlCommand("Select * from AbpUserRoles  where UserId=@userId", conn);
                    cmdRoles.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader rd = cmdRoles.ExecuteReader();

                    while (rd.Read())
                    {
                        newCacheItem.RoleIds.Add((int)rd["RoleId"]);
                    }
                    //Get User Permissions by userId
                    SqlCommand cmdPermissions = new SqlCommand("Select * from AbpPermissions where UserId=@userId", conn);
                    cmdPermissions.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader reader = cmdPermissions.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((bool)reader["IsGranted"])
                        {
                            newCacheItem.GrantedPermissions.Add(reader["Name"].ToString());
                        }
                        else
                        {
                            newCacheItem.ProhibitedPermissions.Add(reader["Name"].ToString());
                        }

                    }
                    return newCacheItem;
                }
            });
        }

        private UserPermissionCacheItem GetUserPermissionCacheItem(long userId)
        {
            var cacheKey = userId + "@" + (GetCurrentTenantId() ?? 0);
            return _cacheManager.GetUserPermissionCache().Get(cacheKey, () =>
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
                {
                    conn.Open();
                    SqlCommand cmdUsers = new SqlCommand("Select count(*) from AbpUsers where Id=@userId", conn);
                    cmdUsers.Parameters.AddWithValue("@userId", userId);

                    int result = (int)cmdUsers.ExecuteScalar();

                    if (result == 0)
                    {
                        return null;
                    }

                    var newCacheItem = new UserPermissionCacheItem(userId);

                    SqlCommand cmdRoles = new SqlCommand("Select * from AbpUserRoles  where UserId=@userId", conn);
                    cmdRoles.Parameters.AddWithValue("@userId", userId);

                    SqlDataReader rd = cmdRoles.ExecuteReader();

                    while (rd.Read())
                    {
                        newCacheItem.RoleIds.Add((int)rd["RoleId"]);
                    }
                    
                    SqlCommand cmdPermissions = new SqlCommand("Select * from AbpPermissions where UserId=@userId", conn);
                    cmdPermissions.Parameters.AddWithValue("userId", userId);

                    SqlDataReader reader = cmdPermissions.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((bool)reader["IsGranted"])
                        {
                            newCacheItem.GrantedPermissions.Add(reader["Name"].ToString());
                        }
                        else
                        {
                            newCacheItem.ProhibitedPermissions.Add(reader["Name"].ToString());
                        }

                    }
                    return newCacheItem;
                }
            });
        }

        public virtual async Task<bool> IsGrantedAsync(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = await GetRolePermissionCacheItemAsync(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        public virtual bool IsGranted(int roleId, Permission permission)
        {
            //Get cached role permissions
            var cacheItem = GetRolePermissionCacheItem(roleId);

            //Check the permission
            return cacheItem.GrantedPermissions.Contains(permission.Name);
        }

        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return MultiTenancySides.Tenant;
            }

            return AbpSession.MultiTenancySide;
        }
        private int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }

        private MultiTenancySides GetMultiTenancySides(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                return MultiTenancySides.Host;
            }

            return MultiTenancySides.Tenant;
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual async Task<bool> IsGrantedAsync(long userId, Permission permission)
        {
            //Check for multi-tenancy side
            if (!permission.MultiTenancySides.HasFlag(GetCurrentMultiTenancySide()))
            {
                return false;
            }

            //Get cached user permissions
            var cacheItem = await GetUserPermissionCacheItemAsync(userId);
            if (cacheItem == null)
            {
                return false;
            }

            //Check for user-specific value
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
            {
                return true;
            }

            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
            {
                return false;
            }

            //Check for roles
            foreach (var roleId in cacheItem.RoleIds)
            {
                if (await IsGrantedAsync(roleId, permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual bool IsGranted(long userId, Permission permission)
        {
            //Check for multi-tenancy side
            if (!permission.MultiTenancySides.HasFlag(GetCurrentMultiTenancySide()))
            {
                return false;
            }

            //Get cached user permissions
            var cacheItem = GetUserPermissionCacheItem(userId);
            if (cacheItem == null)
            {
                return false;
            }

            //Check for user-specific value
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
            {
                return true;
            }

            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
            {
                return false;
            }

            //Check for roles
            foreach (var roleId in cacheItem.RoleIds)
            {
                if (IsGranted(roleId, permission))
                {
                    return true;
                }
            }

            return false;
        }


        private RolePermissionCacheItem GetRolePermissionCacheItem(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return _cacheManager.GetRolePermissionCache().Get(cacheKey, () =>
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
                {
                    var newCacheItem = new RolePermissionCacheItem(roleId);
                    conn.Open();

                    SqlCommand cmdRolePermission = new SqlCommand("Select * from AbpRoles where Id=@roleId", conn);

                    cmdRolePermission.Parameters.AddWithValue("@roleId", roleId);

                    string role = string.Empty;
                    string tenantId = string.Empty;

                    SqlDataReader rd = cmdRolePermission.ExecuteReader();

                    while (rd.Read())
                    {
                        role = rd["Name"].ToString();
                        tenantId = (rd["TenantId"])?.ToString();
                    }

                    if (role == null)
                    {
                        throw new AbpException("There is no role with given id: " + roleId);
                    }

                    var staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                         r.RoleName == role && r.Side == GetMultiTenancySides(tenantId));

                    if (staticRoleDefinition != null)
                    {
                        SqlCommand cmdGetAllPermissions = new SqlCommand("Select Name from AbpPermissions ", conn);

                        SqlDataReader read = cmdGetAllPermissions.ExecuteReader();

                        var allPermissions = new List<Permission>();

                        while (read.Read())
                        {
                            allPermissions.Add(new Permission(read["Name"].ToString()));
                        }

                        foreach (var permission in allPermissions)
                        {
                            if (staticRoleDefinition.IsGrantedByDefault(permission))
                            {
                                newCacheItem.GrantedPermissions.Add(permission.Name);
                            }
                        }
                    }

                    SqlCommand cmdPermissionsByRoleId = new SqlCommand("select * from AbpPermissions where RoleId=@roleId", conn);
                    cmdPermissionsByRoleId.Parameters.AddWithValue("@roleId", roleId);

                    SqlDataReader reader = cmdPermissionsByRoleId.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((bool)reader["IsGranted"])
                        {
                            newCacheItem.GrantedPermissions.AddIfNotContains(reader["Name"].ToString());
                        }
                        else
                        {
                            newCacheItem.GrantedPermissions.Remove(reader["Name"].ToString());
                        }
                    }
                    return newCacheItem;
                }    
            });
        }


        private async Task<RolePermissionCacheItem> GetRolePermissionCacheItemAsync(int roleId)
        {
            var cacheKey = roleId + "@" + (GetCurrentTenantId() ?? 0);
            return await _cacheManager.GetRolePermissionCache().GetAsync(cacheKey, async () =>
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
                {
                    var newCacheItem = new RolePermissionCacheItem(roleId);

                    conn.Open();

                    SqlCommand cmdRolePermission = new SqlCommand("Select * from AbpRoles where Id=@roleId", conn);

                    cmdRolePermission.Parameters.AddWithValue("@roleId", roleId);

                    string role = string.Empty;
                    string tenantId = string.Empty;

                    SqlDataReader rd = cmdRolePermission.ExecuteReader();

                    while (rd.Read())
                    {
                        role = rd["Name"].ToString();
                        tenantId = (rd["TenantId"])?.ToString();
                    }

                    if (role == null)
                    {
                        throw new AbpException("There is no role with given id: " + roleId);
                    }

                    var staticRoleDefinition = RoleManagementConfig.StaticRoles.FirstOrDefault(r =>
                        r.RoleName == role && r.Side == GetMultiTenancySides(tenantId));

                    if (staticRoleDefinition != null)
                    {
                        SqlCommand cmdGetAllPermissions = new SqlCommand("Select Name from AbpPermissions ", conn);

                        SqlDataReader read = cmdGetAllPermissions.ExecuteReader();
                        var allPermissions = new List<Permission>();

                        while (read.Read())
                        {
                            allPermissions.Add(new Permission(read["Name"].ToString()));
                        }

                        foreach (var permission in allPermissions)
                        {
                            if (staticRoleDefinition.IsGrantedByDefault(permission))
                            {
                                newCacheItem.GrantedPermissions.Add(permission.ToString());
                            }
                        }
                    }

                    SqlCommand cmdPermissionsByRoleId = new SqlCommand("select * from AbpPermissions where RoleId=@roleId", conn);
                    cmdPermissionsByRoleId.Parameters.AddWithValue("@roleId", roleId);

                    SqlDataReader reader = cmdPermissionsByRoleId.ExecuteReader();

                    while (reader.Read())
                    {
                        if ((bool)reader["IsGranted"])
                        {
                            newCacheItem.GrantedPermissions.AddIfNotContains(reader["Name"].ToString());
                        }
                        else
                        {
                            newCacheItem.GrantedPermissions.Remove(reader["Name"].ToString());
                        }
                    }
                    return newCacheItem;
                }
                
            });
        }







    }
}