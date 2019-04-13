using System;
using System.Net.Mail;

namespace W1.CurrencyUpdater
{
    public static class EmailSender
    {
        public static void Send(string[] emails, string msg)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.yandex.ru", 25);
                SmtpServer.Credentials = new System.Net.NetworkCredential("orenleto@ya.ru", "1234qwerR!");
                SmtpServer.EnableSsl = true;

                mail.From = new MailAddress("orenleto@ya.ru");
                foreach (var email in emails)
                {
                    mail.To.Add(email);
                }
                
                mail.Subject = "Test Mail";
                mail.Body = msg;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Application.Logger.Error(ex.Message);
            }
        }
    }
}
