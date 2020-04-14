using ColonelPanic.Database.Models;
using ColonelPanic.DatabaseCore.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ColonelPanic.Database.Contexts
{
    public class UserDataContext : DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

        public DbSet<UserXChannelFlags> UserFlags { get; set; }
        public DbSet<UserState> UserStates { get; set; }
        

    }

    public class UserDataHandler
    {
		public static DbContextOptionsBuilder<UserDataContext> OptionsBuilder { get; set; }
		static UserDataHandler()
		{
			OptionsBuilder = new DbContextOptionsBuilder<UserDataContext>();
			OptionsBuilder.UseSqlServer(ConnectionStrings.ConnectionString);
		}
        public static bool UserStateExists(string userId)
        {
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                return db.UserStates.FirstOrDefault(u => u.UserId == userId) != null;
            }
        }
        public static void AddUserStateIfMising(string userId, string username)
        {
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                if (db.UserStates.FirstOrDefault(u => u.UserId == userId) == null)
                {
                    db.UserStates.Add(new UserState(userId, username));
                    db.SaveChanges();
                }
            }
        }

        public static int IncrementTableFlipPoints(string userId, string username)
        {
            int points;
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                var userState = db.UserStates.FirstOrDefault(u => u.UserId == userId);
                if (userState != null)
                {
                    userState.TableFlipPoints++;
                    points = userState.TableFlipPoints;
                    db.UserStates.Attach(userState);
                    db.Entry(userState).State = EntityState.Modified;
                }
                else
                {
                    var usrState = new UserState(userId, username);
                    points = 1;
                    usrState.TableFlipPoints = points;
                    db.UserStates.Add(usrState);
                }
                db.SaveChanges();
            }
            return points;
        }

        public static bool UserHasFlags(string channelId, string userId)
        {
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                return null != db.UserFlags.FirstOrDefault(u => u.ChannelId == channelId && u.UserId == userId);
            }
        }

        public static bool IsUserNaughty(string userId)
        {
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                if (UserStateExists(userId))
                {
                    return db.UserStates.First(u => u.UserId == userId).IsNaughty;
                }
                else return false;
            }
        }

        public static void SetUserNaughtyState(string userId, bool state)
        {
            using (UserDataContext db = new UserDataContext(OptionsBuilder.Options))
            {
                if (UserStateExists(userId))
                {
                    var userState = db.UserStates.First(u => u.UserId == userId);
                    userState.IsNaughty = state;
                    db.UserStates.Attach(userState);
                    db.Entry(userState).State = EntityState.Modified;                    
                }
                else
                {
                    var newUserState = new UserState();
                    newUserState.UserId = userId;
                    newUserState.IsNaughty = state;
                    db.UserStates.Add(newUserState);                    
                }
                db.SaveChanges();
            }
        }

        
    }
}
