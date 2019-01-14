using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;

namespace DungeonMap.Providers
{
    /// <summary>
    /// Singleton App User class. Used to maintain state of User Id, OAuth token, etc globally. 
    /// </summary>
    public sealed class AppUser
    {
        private static readonly Lazy<AppUser> lazy =
            new Lazy<AppUser>(() => new AppUser());

        public static AppUser Instance { get { return lazy.Value; } }

        private int? userId;

        public int? UserId {
            get
            {
                return userId;
            }
            set
            {
                // allows for one first time only setting of user id property. 
                if (!userId.HasValue && value.HasValue)
                {
                    userId = value;
                }
            }
        }

        public string Token { get; set; }

        private AppUser()
        {
        }
    }
}
