using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    public class AssignVolunteersViewModel
    {
        public int EventId { get; set; }

        /// <summary>
        /// Users that should be assigned to the task
        /// </summary>
        public string[] UserId  { get; set; }

        public int TaskId { get; set; }

        public bool NotifyUsers { get; set; }
    }

}
