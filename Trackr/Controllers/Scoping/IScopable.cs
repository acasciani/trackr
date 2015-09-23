using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackrModels;

namespace Trackr
{
    public interface IScopable<T, X>
    {
        List<T> GetScopedEntities(int UserID, string permission);
        List<X> GetScopedIDs(int UserID, string permission);
        T GetScopedEntity(int UserID, string permission, X entityID);
    }
}
