using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace TimeTracker.Controllers
{
    public class ControllersSettings 
    {
        #region Constructor
        public ControllersSettings()
        {
            // Instantiate a single JsonSerializerSettings object
            // that can be reused multiple times.
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        #region Shared properties
        protected JsonSerializerSettings JsonSettings
        {
            get; private set;
        }
        #endregion
    }
}
