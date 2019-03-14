using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.ViewModels;

namespace TimeTracker.ViewModels.Service
{
    public static class IntervalConverter
    {
        /// <summary>
        /// Fill Days, Hours, Minutes and Seconds properties of 
        /// IEnumerable<IntervalViewModel> model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>IEnumerable<IntervalViewModel></returns>
        public static IEnumerable<IntervalViewModel> FillDHMSProp(IEnumerable<IntervalViewModel> model)
        {
            var outModel = new List<IntervalViewModel>();
            Array.ForEach(model.ToArray(), item => outModel.Add(FillDHMSProp(item)));
            return outModel;
        }

        /// <summary>
        /// Fill Days, Hours, Minutes and Seconds properties of 
        /// IntervalViewModel model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>IntervalViewModel</returns>
        public static IntervalViewModel FillDHMSProp(IntervalViewModel model)
        {
            var timeSpan = TimeSpan.FromSeconds(Convert.ToDouble(model.TotalSecond));
            model.Days = timeSpan.Days;
            model.Hours = timeSpan.Hours;
            model.Minutes = timeSpan.Minutes;
            model.Seconds = timeSpan.Seconds;
            return model;
        }
    }
}

