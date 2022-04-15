using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Xamarin.Forms;

namespace SuperCabrio.Pages
{
    public partial class InfoPage : ContentPage
    {
        public bool PassClicked, GlitchClicked, IdeasClicked;
        public InfoPage()
        {
            InitializeComponent();
        }
        async void PasswordBtnClicked(object sender, EventArgs e)
        {
            if (PassClicked == false)
            {
                PasswordInfoBtn.Text = "▼ How Can I Change My Password? ▼";
                PasswordInfoLbl.IsVisible = true;
                PassClicked = true;
            }
            else
            {
                PasswordInfoBtn.Text = "▲ How Can I Change My Password? ▲";
                PasswordInfoLbl.IsVisible = false;
                PassClicked = false;
            }
        }
        async void GlitchBtnClicked(object sender, EventArgs e)
        {
            if (GlitchClicked == false)
            {
                GlitchBtn.Text = "▼ Where Can I Report Glitches? ▼";
                GlitchLbl.IsVisible = true;
                GlitchClicked = true;
            }
            else
            {
                GlitchBtn.Text = "▲ Where Can I Report Glitches? ▲";
                GlitchLbl.IsVisible = false;
                GlitchClicked = false;
            }
        }
        async void IdeasBtnClicked(object sender, EventArgs e)
        {
            if (IdeasClicked == false)
            {
                IdeasBtn.Text = "▼ Where Can I Send In Ideas? ▼";
                IdeasLbl.IsVisible = true;
                IdeasClicked = true;
            }
            else
            {
                IdeasBtn.Text = "▲ Where Can I Send In Ideas? ▲";
                IdeasLbl.IsVisible = false;
                IdeasClicked = false;
            }
        }
        async void SendEmailClicked(object sender, EventArgs e)
        {
            try
            {
                string Address = EmailName.Text;
                string Contents = EmailEntry.Text;
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("supercabrioapp@gmail.com", "Conman00116"),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Address),
                    Subject = "Winning Picture",
                    Body = "<h1>"+Contents+"</h1>",
                    IsBodyHtml = true,
                };
                //var attachment = new Attachment(testStream, "NAMe", MediaTypeNames.Image.Jpeg);
                mailMessage.To.Add("iphoenixdevelop@gmail.com");
                smtpClient.Send(mailMessage);
                ErrorLbl.Text = "Message Sent";
                ErrorLbl.TextColor = Color.Green;
                ErrorLbl.IsVisible = true;
            }
            catch
            {
                ErrorLbl.Text = "Email Address Is Invalid";
                ErrorLbl.TextColor = Color.Red;
                ErrorLbl.IsVisible = true;
            }
        }
    }
}
