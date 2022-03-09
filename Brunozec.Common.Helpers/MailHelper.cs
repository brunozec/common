using System.Net;
using System.Net.Mail;
using System.Text;

namespace Brunozec.Common.Helpers;

public class MailHelper
{
    private string _name;

    private string _address;

    private string _user;

    private string _password;

    private string _smtpHost;

    private int _smtpPort;

    private bool _smtpSsl;

    private bool _smtpTsl;

    public MailHelper(string name, string address)
    {
        _name = name;
        _address = address;
    }

    public MailHelper(string name, string address, string user, string password, string smtpHost, int smtpPort, bool smtpSsl)
    {
        _name = name;
        _address = address;
        _user = user;
        _password = password;
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _smtpSsl = smtpSsl;
    }

    public void SetDefaults(string name, string address, string user, string password, string host, int port, bool ssl, bool tls)
    {
        _name = name;
        _address = address;
        _user = user;
        _password = password;
        _smtpHost = host;
        _smtpPort = port;
        _smtpSsl = ssl;
        _smtpTsl = tls;
    }

    public void Send(string subject, string body, List<string> to, Attachment[] attachments)
    {
        var mail = new MailMessage
        {
            Body = body,
            BodyEncoding = Encoding.UTF8,
            From = new MailAddress(_user, _name, Encoding.UTF8),
            IsBodyHtml = true,
            Priority = MailPriority.High,
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Sender = new MailAddress(_user),
        };

        foreach (var at in attachments)
        {
            mail.Attachments.Add(at);
        }

        foreach (var item in to)
        {
            mail.To.Add(new MailAddress(item));
        }

        var client = new SmtpClient(_smtpHost, _smtpPort)
        {
            EnableSsl = _smtpSsl,
            Credentials = new NetworkCredential(_user, _password)
        };

        client.Send(mail);
    }

    public void Send(string subject, string body, Attachment[] attachments, params string[] to)
    {
        Send(subject, body, to.ToList(), attachments);
    }
}