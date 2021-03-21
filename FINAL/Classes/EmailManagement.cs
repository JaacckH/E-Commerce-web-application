using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class EmailManagement
    {
        public static String SendEmail(String receiver, String subject, String Message)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("ouiouiservices@gmail.com", "OuiOui741");
                MailMessage msgobj = new MailMessage();
                msgobj.To.Add(receiver);
                msgobj.From = new MailAddress("ouiouiservices@gmail.com");
                msgobj.Subject = subject;
                msgobj.Body = Message;
                client.Send(msgobj);
            }
            catch { }
            return "";
        }

        public static void sendPasswordReset(String email)
        {
            if (UserFunctions.emailIsRegistered(email))
            {
                String newpassword = "";
                String characters = "ABCDEFGHIJKLMONPQRSTUVWQYZ0123456789abcdefghijklmnopqrstuvwxyz";
                Random rand = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int r = rand.Next(characters.Length);
                    newpassword += characters.ToCharArray()[r];
                }

                try
                {
                    EmailManagement.SendEmail(email, "Oui Oui Fashion - Password Reset", "Hi, your new password is:" + newpassword);
                    DBFunctions.sendQuery("UPDATE Users SET Password ='" + UserFunctions.hashSingleValue(newpassword) + "' WHERE Email ='" + email + "';");
                }
                catch { }
            }
        }
    }
}
