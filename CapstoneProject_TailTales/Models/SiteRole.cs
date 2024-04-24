using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace CapstoneProject_TailTales.Models
{
    public class SiteRole : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        /// Metodo GetRolesForUser 
        /// 1. Cerca l'utente nel db in base all'username
        ///   1.1 Ottiene l'ID del RUOLO (IdRuolo_FK) associato all'utente
        ///   1.2 Cerca il ruolo nella tabella in base al suo id
        /// Ritorna = un array di stringhe
        /// Parametro = username
        /// Per quanto riguarda le autorizzazioni andrò comunque a scrivere
        /// [Authorize(Roles = "admin")] nei controllers/actions
        /// if (User.IsInRole("admin"))
        public override string[] GetRolesForUser(string username)
        {
            using (var dbContext = new ModelDbContext())
            {
                var user = dbContext.Utenti.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    int roleId = user.IdRuolo_FK;
                    var role = dbContext.Ruoli.FirstOrDefault(r => r.IdRuolo == roleId);
                    if (role != null)
                    {
                        return new string[] { role.Ruolo };
                    }
                }
            }
            return new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}