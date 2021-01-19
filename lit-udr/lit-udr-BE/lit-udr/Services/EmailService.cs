

using lit_udr.EntityFramework;
using lit_udr.EntityFramework.Model;
using lit_udr.Model;
using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace lit_udr.Services
{
	public class EmailService
	{
		public static void SendEmail(UserDto dto, LitUdrContext context,string condition)
		{
            string fileName = string.Empty;
            string messageText = string.Empty;
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Literarno Udruzenje", "nemanja.djekic.1995.2019@gmail.comm"));
                message.To.Add(new MailboxAddress("New User", dto.Email.ToLower()));
                string HashCode = Guid.NewGuid().ToString();

                switch (condition)
                {
                    case "Registration":
                        message.Subject = "ProfileActivation";
                        if (dto.Writer)
                        {
                            message.Body = new TextPart("plain")
                            {
                                Text = @"Thanks for registering as Writer.Please activate your profile at link: http://localhost:3000/confirmRegistration/" + HashCode
                            };
                            context.NewUserData.Add(new NewUserData() { Writer = true, Hash = HashCode, processDefinitionId = dto.ProcessDefinitionId, processInstanceId = dto.ProcessInstanceId, NewUserEmmail = dto.Email.ToLower() }); ;
                            context.SaveChanges();

                            fileName = $"Literarno Udruzenje_Registration_{dto.Email.ToLower()}";
                            messageText = message.TextBody;
                        }
                        else if (dto.BetaReader)
                        {
                            message.Body = new TextPart("plain")
                            {
                                Text = @"Thanks for registering as Beta Reader. Please activate your profile at link: http://localhost:3000/confirmRegistration/" + HashCode
                            };
                            context.NewUserData.Add(new NewUserData() { Writer = false, Hash = HashCode, processDefinitionId = dto.ProcessDefinitionId, processInstanceId = dto.ProcessInstanceId, NewUserEmmail = dto.Email.ToLower() }); ; ;
                            context.SaveChanges();

                            fileName = $"Literarno Udruzenje_Registration_{dto.Email.ToLower()}";
                            messageText = message.TextBody;
                        }
                        else
                        {
                            message.Body = new TextPart("plain")
                            {
                                Text = @"Thanks for registering as Reader. Please activate your profile at link: http://localhost:3000/confirmRegistration/" + HashCode
                            };
                            context.NewUserData.Add(new NewUserData() { Writer = false, Hash = HashCode, processDefinitionId = dto.ProcessDefinitionId, processInstanceId = dto.ProcessInstanceId, NewUserEmmail = dto.Email.ToLower() }); ; ;
                            context.SaveChanges();

                            fileName = $"Literarno Udruzenje_Registration_{dto.Email.ToLower()}";
                            messageText = message.TextBody;
                        }
                        break;

                    case "WorkUpload":
                        message.Subject = "Work Upload";
                        if (dto.Writer)
                        {
                            message.Body = new TextPart("plain")
                            {
                                Text = @"Please upload two or more samples of your work at this link: http://localhost:3000/workUpload/" + dto.FirstName + " so that process can continue."
                            };

                            fileName = $"Literarno Udruzenje_Work_Upload_{dto.Email.ToLower()}";
                            messageText = message.TextBody;
                        }
                        break;

                    case "NumberOfUploads":
                        message.Subject = "Process end.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"As you tried more than three times. Your application has ended. Please try again in future."
                        };

                        fileName = $"Literarno Udruzenje_Process_End_Number_Of_Uploads_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;

                    case "Declined":
                        message.Subject = "Process end.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"Board members have declined your work. Please try again in future." + "\n" + dto.FirstName
                        };

                        fileName = $"Literarno Udruzenje_Declined_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;

                    case "Approved":
                        message.Subject = "Process.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"Board members have Approved your work!! Please pay you monthly subscription at link:http://localhost:3000/confirmSubscription/" + dto.Password + "\n" + dto.FirstName
                        };

                        fileName = $"Literarno Udruzenje_Approved_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;

                    case "NeedMoreWork":
                        message.Subject = "Process.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"Board members are not sure about your work Please upload more samples at link: http://localhost:3000/workUpload/" + dto.Password + "\n" + dto.FirstName
                        };

                        fileName = $"Literarno Udruzenje_NeedMoreWork_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;

                    case "LatePay":
                        message.Subject = "Process end.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"You did not pay your subscription in time. Please try again."
                        };

                        fileName = $"Literarno Udruzenje_Process_End_Late_Pay_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;

                    case "LateUpload":
                        message.Subject = "Process end.";
                        message.Body = new TextPart("plain")
                        {
                            Text = @"You did not upload more samples of work in time."
                        };

                        fileName = $"Literarno Udruzenje_Process_End_{dto.Email.ToLower()}";
                        messageText = message.TextBody;
                        break;
                }


                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587);


                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("nemanja.djekic.1995.2019@gmail.com", "uH987&*(1995google");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            finally
            {
                FileService.CreateFile(fileName, messageText);
            }

        }
	}
}
