using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using SocialSyncBusiness.IServices;
using SocialSyncDTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialSyncBusiness.Services.Queue
{
    public class ForgotPasswordInvocable : IInvocable, IInvocableWithPayload<ResetPasswordDataDto>
    {

        public ResetPasswordDataDto Payload { get; set; }
        private ISendgridService _sendGridService;
        private readonly ILogger<ForgotPasswordInvocable> _logger;
        public ForgotPasswordInvocable(ILogger<ForgotPasswordInvocable> logger, ISendgridService sendGridService)
        {
            _logger = logger;
            _sendGridService = sendGridService;
        }

        public async Task Invoke()
        {
            try
            {
                _logger.LogInformation($"ForgotPasswordInvocable invoking method with input: {System.Text.Json.JsonSerializer.Serialize(Payload)}");

                if (Payload != null) 
                {
                    _logger.LogInformation($"ForgotPasswordInvocable :Sending Reset code to User");

                    await _sendGridService.ForgotpasswordCode(Payload.ResetCode,Payload.Email);

                }
                else
                {
                    _logger.LogError($"Payload is null");
                }

            }
            catch (Exception ex)

            {

                _logger.LogError($"Error in ForgotPasswordInvocable an exception message is: {ex.Message}");

            }
        }
    }
}
