using custom_window.Core;
using Firebase.Auth;

namespace custom_window.HelperClasses.AuthHelper
{
    public static class AuthHelper
    {
        public static void SaveUserToSettings(FirebaseAuth newUser)
        {
            Properties.Settings.Default.localId = newUser.User.LocalId;
            Properties.Settings.Default.federatedId = newUser.User.FederatedId;
            Properties.Settings.Default.firstName = newUser.User.FirstName;
            Properties.Settings.Default.LastName = newUser.User.LastName;
            Properties.Settings.Default.displayName = newUser.User.DisplayName;
            Properties.Settings.Default.email = newUser.User.Email;
            Properties.Settings.Default.isEmailverified = newUser.User.IsEmailVerified;
            Properties.Settings.Default.photoUrl = newUser.User.PhotoUrl;
            Properties.Settings.Default.phoneNumber = newUser.User.PhoneNumber;

            Properties.Settings.Default.token = newUser.FirebaseToken;
            Properties.Settings.Default.created = newUser.Created;
            Properties.Settings.Default.expiresIn = newUser.ExpiresIn;
            Properties.Settings.Default.refreshToken = newUser.RefreshToken;

            Properties.Settings.Default.Save();
        }

        public static FirebaseUser RetrieveUserFromSettings()
        {
            var user = new FirebaseUser
            {
                FirebaseToken = Properties.Settings.Default.token,
                Created = Properties.Settings.Default.created,
                RefreshToken = Properties.Settings.Default.refreshToken,
                ExpiresIn = Properties.Settings.Default.expiresIn,
                LocalId = Properties.Settings.Default.localId,
                FederatedId = Properties.Settings.Default.federatedId,
                FirstName = Properties.Settings.Default.firstName,
                LastName = Properties.Settings.Default.LastName,
                DisplayName = Properties.Settings.Default.displayName,
                Email = Properties.Settings.Default.email,
                IsEmailVerified = Properties.Settings.Default.isEmailverified,
                PhoneNumber = Properties.Settings.Default.phoneNumber,
                PhotoUrl = Properties.Settings.Default.photoUrl
            };

            return user;
        }
    }
}