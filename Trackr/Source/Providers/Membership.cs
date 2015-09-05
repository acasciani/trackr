using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Trackr.Controllers;
using TrackrModels;

namespace Trackr.Source.Providers
{
    public class Membership : MembershipProvider
    {
        private byte[] StringToBytes(string input)
        {
            byte[] bytes = new byte[input.Length * sizeof(char)];
            System.Buffer.BlockCopy(input.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return chars.ToString();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                string encryptedPw = BytesToString(EncryptPassword(StringToBytes(password)));

                WebUser addedUser = wuc.AddNew(new WebUser()
                {
                    Email = username,
                    Password = encryptedPw,
                });

                status = MembershipCreateStatus.Success;

                return WebUserToMembershipUser(addedUser);
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                return wuc.GetWebUsers().First(i => i.Email == username).Password;
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                return WebUserToMembershipUser(wuc.GetWebUsers().First(i => i.Email == username));
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            // parse to user id key
            int? userID = providerUserKey as int?;

            using (WebUsersController wuc = new WebUsersController())
            {
                return WebUserToMembershipUser(wuc.GetWebUsers().First(i => i.UserID == userID.Value));
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            int? userID = user.ProviderUserKey as int?;

            using (WebUsersController wuc = new WebUsersController())
            {
                WebUser webUser = wuc.GetWebUsers().First(i => i.UserID == userID.Value);
                webUser.Email = user.Email;
                wuc.Update(webUser);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        private MembershipUser WebUserToMembershipUser(WebUser user)
        {
            return new MembershipUser(Name, user.Email, user.UserID, user.Email, null, null, true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}