namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class CosmoMongerMembershipUserTest : BasePlayerTest
    {
        [Test]
        public void PasswordsAreSalted()
        {
            Player testPlayer = this.CreateTestPlayer();
            User userModel = testPlayer.User;
            CosmoMongerMembershipUser user = new CosmoMongerMembershipUser(userModel);
            
            user.ChangePassword("test");
            string currentPasswordHash = userModel.Password;
            user.ChangePassword("test");
            Assert.That(currentPasswordHash, Is.Not.EqualTo(userModel.Password), "Hashes for the same password should not match due to salting.");
        }

        [Test]
        public void CheckPassword()
        {
            Player testPlayer = this.CreateTestPlayer();
            User userModel = testPlayer.User;
            CosmoMongerMembershipUser user = new CosmoMongerMembershipUser(userModel);
            
            user.ChangePassword("test");

            // Verify user
            Assert.That(user.VerifyEmail(userModel.VerificationCode), Is.True, "User should validate.");

            Assert.That(user.ValidatePassword("test"), Is.True, "Password should check to be valid.");
        }
    }
}
