using Ecommerce_Apis.OrderModule.DTOs;
using Ecommerce_Apis.OrderModule.Models;
using Ecommerce_Apis.OrderModule.Repositories.InterFace;
using Ecommerce_Apis.ProductModule.DTOs;
using Ecommerce_Apis.Utills;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce_Apis.Data;
using Ecommerce_Apis.UserModule.Models;

namespace Ecommerce_Apis.OrderModule.Repositories
{
    public class AddressRepositories : IAddressRepositories
    {
        private readonly ApplicationDbContext _context;

        public AddressRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAddress(AddAddressRequest request)
        {
            var address = new Address
            {
                UserId = request.UserId,
                City = request.City,
                Street = request.Street,
                PostalCode = request.PostalCode,
                Region = request.Region
            };

            _context.Addresses.Add(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Address> GetAddressById(int id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        

        public async Task<bool> UpdateAddress(UpdateAddressDTO request)
        {
            var address = await _context.Addresses.FindAsync(request.Id);
            if (address == null) return false;

            address.City = request.City;
            address.Street = request.Street;
            address.PostalCode = request.PostalCode;
            address.Region = request.Region;

            _context.Addresses.Update(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Address>> GetAddress(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
    }
}

