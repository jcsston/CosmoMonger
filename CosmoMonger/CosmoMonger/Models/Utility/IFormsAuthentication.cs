﻿namespace CosmoMonger.Models.Utility
{
    using System;

    /// <summary>
    /// The FormsAuthentication type is sealed and contains static members, so it is difficult to
    /// unit test code that calls its members. The interface and helper class below demonstrate
    /// how to create an abstract wrapper around such a type in order to make the AccountController
    /// code unit testable.
    /// </summary>
    public interface IFormsAuthentication
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
    }
}