using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using UTW_Project.Models;

public class EmailManager
{
    public static void SendConfirmationEmailEN(User user)
    {
        SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        string companyEmail = section.Network.UserName;
        string companyPassword = section.Network.Password;

        MailMessage m = new MailMessage(companyEmail, user.Email);
        m.Subject = "Verification";
        m.Body = string.Format("Hi {0}" +
            "<BR/> Thank you for your registration, please click on the " +
            "below link to complete your registration:<BR/><a href =\"{1}\"" +
            "title =\"User Email Confirm\">{1}</a>",
            user.FirstNameEN, user.URL.URL1);
        m.IsBodyHtml = true;
        SmtpClient smcl = new SmtpClient();
        smcl.Host = section.Network.Host;
        smcl.Port = section.Network.Port;
        smcl.Credentials = new NetworkCredential(companyEmail, companyPassword);
        smcl.EnableSsl = true;
        smcl.Send(m);
    }

    public static void SendConfirmationEmailAR(User user)
    {
        SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        string companyEmail = section.Network.UserName;
        string companyPassword = section.Network.Password;

        user.EmailConfirmed = false;

        MailMessage m = new MailMessage(companyEmail, user.Email);
        m.Subject = "تأكيد";
        m.Body = string.Format("أهلًا" +
            " {0}" +
            "<BR/>" +
            " شكرًا لتسجيلك, من فضلك اضغط على الرابط لنأكيد بريدك الالكتروني واكمال تسجيلك" +
            "<BR/><a href =\"{1}\" title =\"تأكيد البريد الالكتروني\">{1}</a>",
            user.FirstNameAR, user.URL.URL1);
        m.IsBodyHtml = true;

        SmtpClient smcl = new SmtpClient();
        smcl.Host = section.Network.Host;
        smcl.Port = section.Network.Port;
        smcl.Credentials = new NetworkCredential(companyEmail, companyPassword);
        smcl.EnableSsl = true;
        smcl.Send(m);
    }
    public static void SendResetPasswordEmailEN(User user)
    {
        SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        string companyEmail = section.Network.UserName;
        string companyPassword = section.Network.Password;

        MailMessage m = new MailMessage(companyEmail, user.Email);
        m.Subject = "Reset Password";
        m.Body = string.Format("Hi {0}" +
            "<BR/> You recently requested to reset your password, please " +
            "click on the link below to reset it:<BR/><a href =\"{1}\"" +
            "title =\"User Reset Password\">{1}</a>" +
            "<BR/> If you didn't make this request then you can safely ignore this email.",
            user.FirstNameEN, user.URL.URL1);
        m.IsBodyHtml = true;
        SmtpClient smcl = new SmtpClient();
        smcl.Host = section.Network.Host;
        smcl.Port = section.Network.Port;
        smcl.Credentials = new NetworkCredential(companyEmail, companyPassword);
        smcl.EnableSsl = true;
        smcl.Send(m);
    }

    public static void SendResetPasswordEmailAR(User user)
    {
        SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        string companyEmail = section.Network.UserName;
        string companyPassword = section.Network.Password;

        MailMessage m = new MailMessage(companyEmail, user.Email);
        m.Subject = "إعادة ضبط كلمة المرور";
        m.Body = string.Format("أهلًا" +
            " {0}" +
            "<BR/>" +
            "لقد طلبت إعادة ضبط كلمة مرورك, من فضلك اضغط على الرابط لإعادة الضبط" +
            "<BR/><a href =\"{1}\" title =\"إعادة ضبط كلمة مرور المستخدم\">{1}</a>" +
            "<BR/>" +
            "إذا لم تقم بهذا الطلب فيمكنك تجاهل هذه الرسالة.",
            user.FirstNameAR, user.URL.URL1);
        m.IsBodyHtml = true;
        SmtpClient smcl = new SmtpClient();
        smcl.Host = section.Network.Host;
        smcl.Port = section.Network.Port;
        smcl.Credentials = new NetworkCredential(companyEmail, companyPassword);
        smcl.EnableSsl = true;
        smcl.Send(m);
    }
}