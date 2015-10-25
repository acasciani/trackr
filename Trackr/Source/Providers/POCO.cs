using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TrackrModels;

namespace Trackr
{
    public class ModelNotificationEventArgs : EventArgs {

    }

    public class POCO<T, C> : IDisposable where C : DbContext, new() where T:class
    {
        private C dbContext = null;

        public EventHandler ModelUpdated;


        public POCO()
        {
            dbContext = new C();
        }

        public int Update(T model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            int saved = dbContext.SaveChanges();

            ModelUpdated(this, new ModelNotificationEventArgs());
            return saved;
        }

        public int Create(T model)
        {
            dbContext.Entry(model).State = EntityState.Added;
            int added = dbContext.SaveChanges();

            ModelUpdated(this, new ModelNotificationEventArgs());
            return added;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }


    }
}