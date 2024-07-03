using JSSATSAPI.BussinessObjects.IService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var fromPhoneNumber = new PhoneNumber(_configuration["Twilio:FromPhoneNumber"]);
            var toPhoneNumberObj = new PhoneNumber(toPhoneNumber);

            await MessageResource.CreateAsync(
                body: message,
                from: fromPhoneNumber,
                to: toPhoneNumberObj
            );
        }
    }
}
