using AutoMapper;
using JSSATSAPI.BussinessObjects.RequestModels.AccountReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.AccountResponseModels;
using JSSATSAPI.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Mapper
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            #region Mapper_Response
            CreateMap<Account, AccountDetailResponse>()
                      .ReverseMap();
            CreateMap<Account, AccountResponse>()
                       .ReverseMap();
            #endregion
            #region Mapper_Request
            CreateMap<AccountSignUpRequest, Account>()
                       .ReverseMap();
            #endregion
        }
    }
}