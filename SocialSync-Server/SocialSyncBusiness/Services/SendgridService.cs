using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using SocialSyncBusiness.IServices;
using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialSyncBusiness.Services
{
    public class SendgridService : ISendgridService
    {
        private SendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;

        public SendgridService(IConfiguration configuration)
        {
            _configuration = configuration;
            _sendGridClient = new SendGridClient(_configuration["SendGrid:ClientKey"]);
        }

        public async Task<ServiceResult<string>> ForgotpasswordCode(string code,string email)
        {

            //var sendGridMessage = new SendGridMessage();
            
            //sendGridMessage.AddTo(email);
            //sendGridMessage.SetFrom(fromAddress, fromName);
            //sendGridMessage.Subject = subject;
            //sendGridMessage.SetTemplateId(templateId);
            //sendGridMessage.SetTemplateData(code);

            var response = await SendMail(code,email);

            if (response)
            {
                return new ServiceResult<string>
                {
                    Success = true,
                    Message = "Email has been send Successfully!"
                };
            }
            else
            {
                return new ServiceResult<string>
                {
                    Success = false,
                    Message = "Email not send - sendgrid Error"
                };
            }
         
        }

        private async Task<bool> SendMail(string Code,string userEmail)
        {

            string templateId = _configuration["SendGrid:SignUpImmediateEmailTemplateId"].ToString();
            string fromAddress = _configuration["SendGrid:FromAddress"];
            string fromName = _configuration["SendGrid:FromName"];
            string subject = "SocialSync Password Reset Code";
            var plainTextContent = Regex.Replace(Code, "<[^>]*>", "");
            var from = new EmailAddress(fromAddress,fromName);
            var to = new EmailAddress(userEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject,
            plainTextContent, Code);
            var response = await _sendGridClient.SendEmailAsync(msg);
         
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                //await _exceptionService.LogException(new SocialXDTOs.DTOs.ExceptionsDto
                //{
                //    CreatedDate = DateTime.UtcNow,
                //    //InnerException = message.Personalizations.Select(t => t.TemplateData).FirstOrDefault().ToString(),
                //    Message = "SendGridStatus:" + response.StatusCode.ToString() + ";TemplateId:" + message.TemplateId + ";From:" + message.From,
                //    Source = "Method:SendMail:FromEmail" + message.From.Email + ";Name:" + message.From.Name
                //});
                return false;
            }
        }
    }
}
