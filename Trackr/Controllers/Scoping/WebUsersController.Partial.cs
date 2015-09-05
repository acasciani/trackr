using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TrackrModels;

namespace Trackr.Controllers
{
    public partial class WebUsersController : ApiController, IScopable<WebUser, int>
    {
        [Route("api/WebUsersController/GetScoped/{UserID}/{permission}")]
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

        private List<int> GetScopedIDs(ScopeAssignment scopeAssignment)
        {
            List<int> userIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    userIDs.AddRange(db.WebUsers.Select(i => i.UserID));
                    break;

                default: break;
            }

            return userIDs;
        }

        private IQueryable<WebUser> GetScopedEntities(ScopeAssignment scopeAssignment)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);
            return db.WebUsers.Where(i => scopedIDs.Contains(i.UserID));
        }
    }
}