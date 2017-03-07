using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnionMall.Token;

namespace UnionMall.LIB
{
    public class TokenService
    {

        private static ServiceControllersClient tokenService = new ServiceControllersClient();

        public static void sendToken(string acctNum, string amount) 
        {
            tokenService.generateOTPSelfEnroled(acctNum, amount);
        }

        public static string validateOTP(string acctNum, string otp)
        {
            var response = tokenService.validateOTPSelfEn(acctNum, otp);
            return response;
        }
    }
}