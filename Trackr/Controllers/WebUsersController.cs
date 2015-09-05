using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TrackrModels;

namespace Trackr.Controllers
{
    public partial class WebUsersController : ApiController, IScopable<WebUser, int>
    {
        private UserManagementEntities db = new UserManagementEntities();

        // GET: api/WebUsers
        public IQueryable<WebUser> GetWebUsers()
        {
            return db.WebUsers;
        }

        public WebUser AddNew(WebUser entity)
        {
            WebUser webUser = db.WebUsers.Add(entity);
            db.SaveChanges();
            return webUser;
        }

        public void Update(WebUser entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: api/WebUsers/5
        [ResponseType(typeof(WebUser))]
        public IHttpActionResult GetWebUser(int id)
        {
            WebUser webUser = db.WebUsers.Find(id);
            if (webUser == null)
            {
                return NotFound();
            }

            return Ok(webUser);
        }

        // PUT: api/WebUsers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWebUser(int id, WebUser webUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != webUser.UserID)
            {
                return BadRequest();
            }

            db.Entry(webUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WebUsers
        [ResponseType(typeof(WebUser))]
        public IHttpActionResult PostWebUser(WebUser webUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WebUsers.Add(webUser);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = webUser.UserID }, webUser);
        }

        // DELETE: api/WebUsers/5
        [ResponseType(typeof(WebUser))]
        public IHttpActionResult DeleteWebUser(int id)
        {
            WebUser webUser = db.WebUsers.Find(id);
            if (webUser == null)
            {
                return NotFound();
            }

            db.WebUsers.Remove(webUser);
            db.SaveChanges();

            return Ok(webUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WebUserExists(int id)
        {
            return db.WebUsers.Count(e => e.UserID == id) > 0;
        }
    }
}