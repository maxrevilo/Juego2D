using System;

namespace Juego2D
{
    public static class UserProfileManger
    {
        private static Profile profile = null;

        public static Profile getProfile()
        {
            if (profile == null) profile = new Profile();
            return profile;
        }
    }
}
