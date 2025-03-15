using JSON_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace JsonDemo.Models
{
    public class UnverifiedEmail : Record
    {
        public void SetUser(User user = null)
        {
            if (user != null)
            {
                UserId = user.Id;
                Email = user.Email;
                VerificationCode = Guid.NewGuid().ToString();
            }
        }
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public int UserId { get; set; }
    }

    public static class AccountsEmailing
    {
        const string ApplicationName = "Gestionnaire de registres";

        public static void SendEmailVerification(string ActionURL, User user)
        {
            UnverifiedEmail unverifiedEmail = new UnverifiedEmail();
            unverifiedEmail.SetUser(user);
            DB.UnverifiedEmails.Add(unverifiedEmail);

            string Link = @"<br/><a href='" + ActionURL + "?code=" + unverifiedEmail.VerificationCode + @"' > Confirmez votre inscription...</a>";
           
            string Subject = ApplicationName + " - Vérification de courriel...";

            string Body = "Bonjour " + user.Name + @",<br/><br/>";

            Body += @"Merci de vous être inscrit au " + ApplicationName + ". <br/>";
            Body += @"Pour utiliser votre compte vous devez confirmer votre inscription en cliquant sur le lien suivant : <br/>";

            Body += Link;

            Body += @"<br/><br/>Ce courriel a été généré automatiquement, veuillez ne pas y répondre.";
            Body += @"<br/><br/>Si vous éprouvez des difficultés ou s'il s'agit d'une erreur, veuillez le signaler à <a href='mailto:"
                 + SMTP.OwnerEmail + "'>" + SMTP.OwnerName + "</a> (Webmestre du site ChatManager)";

            SMTP.SendEmail(user.Name, unverifiedEmail.Email, Subject, Body);
        }

        public static void SendEmailChangedVerification(string ActionURL, User user)
        {
            UnverifiedEmail unverifiedEmail = new UnverifiedEmail();
            unverifiedEmail.SetUser(user);
            DB.UnverifiedEmails.Add(unverifiedEmail);

            string Link = @"<br/><a href='" + ActionURL + "?code=" + unverifiedEmail.VerificationCode + @"' > Confirmez votre nouvelle adresse de courriel...</a>";

            string Subject = ApplicationName + " - Confirmation de changement de courriel...";

            string Body = "Bonjour " + user.Name + @",<br/><br/>";

            Body += @"Vous avez modifié votre adresse de courriel. <br/>";
            Body += @"Pour que ce changement soit pris en compte, vous devez confirmer cette adresse en cliquant sur le lien suivant : <br/>";

            Body += Link;

            Body += @"<br/><br/>Ce courriel a été généré automatiquement, veuillez ne pas y répondre.";
            Body += @"<br/><br/>Si vous éprouvez des difficultés ou s'il s'agit d'une erreur, veuillez le signaler à <a href='mailto:"
                 + SMTP.OwnerEmail + "'>" + SMTP.OwnerName + "</a> (Webmestre du site ChatManager)";

            SMTP.SendEmail(user.Name, unverifiedEmail.Email, Subject, Body);
        }
    }
}