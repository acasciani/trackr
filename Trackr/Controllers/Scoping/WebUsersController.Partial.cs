using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TrackrModels;

namespace Trackr
{
    public partial class WebUsersController : OpenAccessBaseApiController<TrackrModels.WebUser, TrackrModels.UserManagement>, IScopable<WebUser, int>
    {
        [Route("api/WebUsers/GetScoped/{UserID}/{permission}")]
        [HttpGet]
        public List<WebUser> GetScopedEntities(int UserID, string permission)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<WebUser> users = new List<WebUser>();
            foreach (ScopeAssignment assignment in assignments)
            {
                users.AddRange(GetScopedEntities(assignment));
            }

            return users;
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<int> ids = new List<int>();
            foreach (ScopeAssignment assignment in assignments)
            {
                ids.AddRange(GetScopedIDs(assignment));
            }

            return ids;
        }

        public WebUser GetScopedEntity(int UserID, string permission, int entityID)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            foreach (ScopeAssignment assignment in assignments)
            {
                if (GetScopedIDs(assignment).Contains(entityID))
                {
                    return Get(entityID);
                }
            }

            return null;
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            foreach (ScopeAssignment assignment in assignments)
            {
                if (GetScopedIDs(assignment).Contains(entityID))
                {
                    return true;
                }
            }

            return false;
        }

        private List<int> GetScopedIDs(ScopeAssignment scopeAssignment)
        {
            List<int> userIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    userIDs.AddRange(Get().Select(i => i.UserID));
                    break;

                default: break;
            }

            return userIDs;
        }

        private IQueryable<WebUser> GetScopedEntities(ScopeAssignment scopeAssignment)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);
            return Get().Where(i => scopedIDs.Contains(i.UserID));
        }
    }
}