using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Extensions;
using AllReady.Models;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Extensions
{
    public static class ContactExtensions
    {       
        public static IPrimaryContactViewModel ToEditModel(this Contact contact, IPrimaryContactViewModel contactModel)
        {
            if (contact != null)
            {
                contactModel.PrimaryContactEmail = contact.Email;
                contactModel.PrimaryContactFirstName = contact.FirstName;
                contactModel.PrimaryContactLastName = contact.LastName;
                contactModel.PrimaryContactPhoneNumber = contact.PhoneNumber;
            }

            return contactModel;
        }

     }
}
