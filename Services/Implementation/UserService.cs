using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;

namespace CareSchedule.Services.Implementation
{
    public class UserService(IUserRepository _userrepo) : IUserService
    {
        public List<UserDto> SearchUser(UserSearchQuery q)
        {
            var (items, _) = _userrepo.Search(q.Name, q.Role, q.Email, q.Phone, q.Status,
                                          q.Page, q.PageSize, q.SortBy, q.SortDir);
            var list = new List<UserDto>(items.Count);
            foreach (var u in items) list.Add(Map(u));
            return list;
        }

        public UserDto GetUser(int id)
        {
            var u = _userrepo.Get(id);
            if (u is null) throw new KeyNotFoundException("User not found.");
            return Map(u);
        }

        public UserDto CreateUser(UserCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Role)) throw new ArgumentException("Role is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required.");

            var e = new User
            {
                Name = dto.Name.Trim(),
                Role = dto.Role.Trim(),
                Email = dto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                Status = "Active"
            };
            e = _userrepo.Create(e);
            return Map(e);
        }

        public UserDto UpdateUser(int id, UserUpdateDto dto)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) e.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Role)) e.Role = dto.Role.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Email)) e.Email = dto.Email.Trim();
            if (dto.Phone is not null) e.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();

            _userrepo.Update(e);
            return Map(e);
        }

        public void DeactivateUser(int id)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");
            if (e.Status != "Inactive") { e.Status = "Inactive"; _userrepo.Update(e); }
        }

        public void ActivateUser(int id)
        {
            var e = _userrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("User not found.");
            if (e.Status != "Active") { e.Status = "Active"; _userrepo.Update(e); }
        }

         public void LockUser(int id)
        {
            throw new NotImplementedException();
        }

        public void UnlockUser(int id)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(int id)
        {
            throw new NotImplementedException();
        }
        
        private static UserDto Map(User u) => new()
        {
            UserId = u.UserId,
            Name = u.Name,
            Role = u.Role,
            Email = u.Email,
            Phone = u.Phone,
            Status = u.Status
        };
    }
}