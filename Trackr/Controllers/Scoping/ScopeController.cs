using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;

namespace Trackr.Controllers
{
    public class ScopeController
    {
        public IEnumerable<ScopeAssignment> GetScopeAssignments(int userID, string permission)
        {
            using (UserManagementEntities db = new UserManagementEntities())
            {
                Permission permObject = db.Permissions.Where(i=>i.PermissionName == permission).FirstOrDefault();

                if(permObject==null){
                    throw new Exception("Permission does not exist in the database.");
                }

                var allScopeAssignments = db.ScopeAssignments
                    .Include("Permission")
                    .Include("Role")
                    .Where(i=>i.UserID == userID);

                var allRoleAssignments = allScopeAssignments.Where(i => !i.IsDeny && i.RoleID.HasValue && i.Role.Permissions.Contains(permObject));
                var allPermissionAssignments = allScopeAssignments.Where(i => !i.IsDeny && i.Permission == permObject);

                return allRoleAssignments.Union(allPermissionAssignments);
            }
        }
    }
}