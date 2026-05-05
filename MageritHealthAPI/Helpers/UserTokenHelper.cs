using MageritHealthAPI.Models.DTOs;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MageritHealthAPI.Helpers
{
    public class UserTokenHelper
    {
        private IHttpContextAccessor contextAccesor;

        public UserTokenHelper(IHttpContextAccessor contextAccesor)
        {
            this.contextAccesor = contextAccesor;
        }

        public ClaimModel GetInfoUser()
        {
            Claim userClaim = this.contextAccesor.HttpContext.User.FindFirst(c => c.Type == "UserData");
            string encryptedJson = userClaim.Value;

            string userInfoJson = CifradoHelper.DescifrarString(encryptedJson);
            ClaimModel userInfo = JsonConvert.DeserializeObject<ClaimModel>(userInfoJson);

            return userInfo;
        }

        public int GetUserId()
        {
            return int.Parse(GetInfoUser().IdUsuario);
        }
    }
}
