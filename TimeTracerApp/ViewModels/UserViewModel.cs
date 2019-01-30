using Newtonsoft.Json;
using System.Collections.Generic;

namespace TimeTracker.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UserViewModel
    {
        #region Constructor
        public UserViewModel()
        {
            Errors = new List<string>();
            HasErrors = false;
        }
        #endregion

        #region Properties
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; }
        #endregion
    }
}
