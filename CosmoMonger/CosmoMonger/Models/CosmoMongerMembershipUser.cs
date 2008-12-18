namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    public class CosmoMongerMembershipUser : MembershipUser
    {
        private User user = null;

        //public override string Comment { get; set; }
        //public override DateTime CreationDate { get; }

        public override string Email 
        {
            get
            {
                return this.user.Email;
            }
            set
            {
                this.user.UpdateProfile(this.user.UserName, this.user.Email);
            }
        }

        public override bool IsApproved
        {
            get
            {
                return this.user.Active;
            }
            set
            {
                this.user.Active = value;
            }
        }

        public override bool IsLockedOut
        {
            get
            {
                return !this.IsApproved;
            }
        }
        
        //public override DateTime LastActivityDate { get; set; }
        //public override DateTime LastLockoutDate { get; }
        public override DateTime LastLoginDate 
        {
            get
            {
                return this.user.LastLogin.GetValueOrDefault(DateTime.Now);
            }
            set
            {
                this.user.LastLogin = value;
            }
        }

        //public override DateTime LastPasswordChangedDate { get; }
        public override string ProviderName 
        {
            get
            {
                return "CosmoMongerMembershipProvider";
            }
        }

        public override string UserName
        {
            get
            {
                return this.user.UserName;
            }
        }

        public CosmoMongerMembershipUser(User u)
        {
            this.user = u;
        }

        /*
        // Summary:
        //     Updates the password for the membership user in the membership data store.
        //
        // Parameters:
        //   oldPassword:
        //     The current password for the membership user.
        //
        //   newPassword:
        //     The new password for the membership user.
        //
        // Returns:
        //     true if the update was successful; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     oldPassword is an empty string.-or-newPassword is an empty string.
        //
        //   System.ArgumentNullException:
        //     oldPassword is null.-or-newPassword is null.
        public virtual bool ChangePassword(string oldPassword, string newPassword);
        //
        // Summary:
        //     Updates the password question and answer for the membership user in the membership
        //     data store.
        //
        // Parameters:
        //   password:
        //     The current password for the membership user.
        //
        //   newPasswordQuestion:
        //     The new password question value for the membership user.
        //
        //   newPasswordAnswer:
        //     The new password answer value for the membership user.
        //
        // Returns:
        //     true if the update was successful; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     password is an empty string.-or-newPasswordQuestion is an empty string.-or-newPasswordAnswer
        //     is an empty string.
        //
        //   System.ArgumentNullException:
        //     password is null.
        public virtual bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer);
        //
        // Summary:
        //     Gets the password for the membership user from the membership data store.
        //
        // Returns:
        //     The password for the membership user.
        public virtual string GetPassword();
        //
        // Summary:
        //     Gets the password for the membership user from the membership data store.
        //
        // Parameters:
        //   passwordAnswer:
        //     The password answer for the membership user.
        //
        // Returns:
        //     The password for the membership user.
        public virtual string GetPassword(string passwordAnswer);
        //
        // Summary:
        //     Resets a user's password to a new, automatically generated password.
        //
        // Returns:
        //     The new password for the membership user.
        public virtual string ResetPassword();
        //
        // Summary:
        //     Resets a user's password to a new, automatically generated password.
        //
        // Parameters:
        //   passwordAnswer:
        //     The password answer for the membership user.
        //
        // Returns:
        //     The new password for the membership user.
        public virtual string ResetPassword(string passwordAnswer);
        //
        // Summary:
        //     Returns the user name for the membership user.
        //
        // Returns:
        //     The System.Web.Security.MembershipUser.UserName for the membership user.
        public override string ToString();
        //
        // Summary:
        //     Clears the locked-out state of the user so that the membership user can be
        //     validated.
        //
        // Returns:
        //     true if the membership user was successfully unlocked; otherwise, false.
        public virtual bool UnlockUser();
         * */
    }
}
