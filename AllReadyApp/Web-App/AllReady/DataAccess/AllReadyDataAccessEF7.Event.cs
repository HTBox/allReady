﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<Event> IAllReadyDataAccess.Events
        {
            get
            {
                return _dbContext.Events
                                .Include(a => a.Location)
                                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                                .Include(a => a.Tasks)
                                .Include(a => a.RequiredSkills).ThenInclude(r => r.Skill)
                                .Include(a => a.UsersSignedUp)
                                .OrderBy(a => a.EndDateTime)
                                .ToList();
            }
        }
       
        IEnumerable<Event> IAllReadyDataAccess.EventsByPostalCode(string postalCode, int distance)
        {
            return _dbContext.Events.FromSql("EXEC GetClosestEventsByPostalCode '{0}', {1}, {2}", postalCode, 50, distance)
                .Include(a => a.Campaign);
        }

        IEnumerable<Event> IAllReadyDataAccess.EventsByGeography(double latitude, double longitude, int distance)
        {
            return _dbContext.Events.FromSql("EXEC GetClosestEvents {0}, {1}, {2}, {3}", latitude, longitude, 50, distance);
        }

        Event IAllReadyDataAccess.GetEvent(int eventId)
        {
            return _dbContext.Events
                .Include(a => a.Location)
                .Include(a => a.Campaign).ThenInclude(c => c.ManagingOrganization)
                .Include(a => a.RequiredSkills).ThenInclude(rs => rs.Skill).ThenInclude(s => s.ParentSkill)
                .Include(a => a.Tasks).ThenInclude(t => t.AssignedVolunteers).ThenInclude(tu => tu.User)
                .Include(a => a.Tasks).ThenInclude(t => t.RequiredSkills).ThenInclude(ts => ts.Skill)
                .Include(a => a.UsersSignedUp).ThenInclude(u => u.User)
                .SingleOrDefault(a => a.Id == eventId);
        }

        IEnumerable<EventSignup> IAllReadyDataAccess.GetEventSignups(int eventId, string userId)
        {
            return _dbContext.EventSignup
                        .Include(x => x.User)
                        .Include(x => x.Event)
                        .Include(x => x.Event.Campaign)                        
                        .Where(x => x.Event.Id == eventId && x.User.Id == userId)
                        .OrderBy(x => x.Event.StartDateTime)
                        .ToArray();
        }

        IEnumerable<EventSignup> IAllReadyDataAccess.GetEventSignups(string userId)
        {
            return _dbContext.EventSignup
                        .Include(x => x.User)
                        .Include(x => x.Event)
                        .Include(x => x.Event.Campaign)                        
                        .Where(x => x.User.Id == userId)
                        .OrderBy(x => x.Event.StartDateTime)
                        .ToArray();
        }

        IEnumerable<TaskSignup> IAllReadyDataAccess.GetTasksAssignedToUser(int eventId, string userId)
        {
            var unfilteredTasks = _dbContext.TaskSignups
                .Include(ts => ts.Task)
                .ThenInclude(t => t.Event)
                .ThenInclude(t => t.Campaign)
                .Include(ts => ts.User)                
                .ToList();

            var finalTasks = unfilteredTasks.Where(ts => ts.Task.Event.Id == eventId && ts.User.Id == userId && !ts.Task.Event.Campaign.Locked).ToList();

            return finalTasks;
        }

        IEnumerable<Resource> IAllReadyDataAccess.GetResourcesByCategory(string category)
        {
            return _dbContext.Resources.Where(x => x.CategoryTag == category);
        }

    }
}
