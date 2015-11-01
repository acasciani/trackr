using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TrackrModels;

namespace Trackr
{
    public partial class PlayersController : OpenAccessBaseApiController<TrackrModels.Player, TrackrModels.ClubManagement>, IScopable<Player, int>
    {
        public List<Player> GetScopedEntities(int UserID, string permission)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<Player> players = new List<Player>();
            foreach (ScopeAssignment assignment in assignments)
            {
                players.AddRange(GetScopedEntities(assignment));
            }

            return players;
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

        public Player GetScopedEntity(int UserID, string permission, int entityID)
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
            List<int> playerIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    playerIDs.AddRange(Get().Select(i => i.PlayerID));
                    break;

                default: break;
            }

            return playerIDs;
        }

        private IQueryable<Player> GetScopedEntities(ScopeAssignment scopeAssignment)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);
            return Get().Where(i => scopedIDs.Contains(i.PlayerID));
        }
    }
}