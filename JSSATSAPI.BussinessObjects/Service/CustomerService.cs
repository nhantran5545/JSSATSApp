using AutoMapper;
using JSSATSAPI.BussinessObjects.IService;
using JSSATSAPI.BussinessObjects.RequestModels.CustomerReqModels;
using JSSATSAPI.BussinessObjects.ResponseModels.CustomerResponse;
using JSSATSAPI.DataAccess.IRepository;
using JSSATSAPI.DataAccess.Models;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, IMembershipRepository membershipRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _membershipRepository = membershipRepository;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request)
        {
            if (!Validator.TryValidateObject(request, new ValidationContext(request), null, true))
            {
                throw new ValidationException("Request is invalid");
            }

            var existingCustomers = await _customerRepository.GetAllAsync();
            var existingCustomersList = existingCustomers.ToList();

            var newCustomerId = GenerateCustomerId(existingCustomersList);

            var newCustomer = new Customer
            {
                CustomerId = newCustomerId,
                Name = request.Name,
                Phone = request.Phone,
                LoyaltyPoints = 0,
                TierId = 1
            };
            await _customerRepository.AddAsync(newCustomer);
            _customerRepository.SaveChanges();

            var tier = await _membershipRepository.GetByIdAsync(newCustomer.TierId);

            var customerResponse = _mapper.Map<CustomerResponse>(newCustomer);
            if (tier != null)
            {
                customerResponse.TierName = tier.TierName;
                customerResponse.DiscountPercent = tier.DiscountPercent;
            }

            return customerResponse;
        }

        private string GenerateCustomerId(List<Customer> existingCustomers)
        {
            if (existingCustomers == null || existingCustomers.Count == 0)
            {
                return "CTM001";
            }

            var lastCustomer = existingCustomers.Last();

            var lastNumber = int.Parse(lastCustomer.CustomerId.Substring(3)) + 1;

            return "CTM" + lastNumber.ToString("000");
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerResponse>>(customers);
        }


        public async Task<CustomerResponse> GetCustomerById(string customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            return _mapper.Map<CustomerResponse>(customer);
        }

    }

}
