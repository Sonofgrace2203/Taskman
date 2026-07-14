using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.JSInterop;

namespace miniapp.Services
{
    public class AuthService
    {
        IJSRuntime JS;

        public AuthService(IJSRuntime jS)
        {
            JS = jS;
        }

        public bool IsLoggedIn { get; private set; }
        public string Username { get; private set; } = "";
        public string Role { get; private set; } = "";

        public string Firstname { get; private set; } = "";
        public string Lastname { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string DateOfBirth { get; private set; } = "";
        public string Password1 { get; private set; } = "";
        public string ConfirmPassword { get; private set; } = "";

        public bool userExists { get; private set; } = true;
        public UserModel newUser { get; private set; }

        public readonly List<UserModel> _user = new();

        public event Action? OnChange;
        public void Notify() => OnChange?.Invoke();

        private List<UserModel> users = new()
        {
            new UserModel { Username = "Olabode", Password = "1234", Role = "Admin", Email = "benny@mail.com" },
            new UserModel { Username = "Benedicta", Password = "2345", Role = "Admin" },
            new UserModel { Username = "Blessing", Password = "3456", Role = "User" }
        };

        public async Task<string> SignUp(string firstname, string lastname, string email, string role, DateTime? dateOfBirth, string username, string password, string confirmPassword)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            DateOfBirth = dateOfBirth?.ToString("yyyy-MM-dd") ?? "";
            Username = username;
            Password1 = password;
            ConfirmPassword = confirmPassword;
            Role = role;

            if (firstname == "" || lastname == "" || email == "" || role == "" || dateOfBirth == null || username == "" || password == "" || confirmPassword == "")
            {
                if (password != confirmPassword)
                {
                    return "Passwords do not match.";
                }
                else
                {
                    return "Please fill all required entries.";
                }
            }

            userExists = users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (userExists)
            {
                return "Username is already taken.";
            }

            newUser = new UserModel
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                DateOfBirth = dateOfBirth,
                Username = username,
                Password = password,
                Role = role
            };

            users.Add(newUser);
            await SaveUser();
            await LoadUsers();
            Notify();
            return "Success";
        }

        public bool Login(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (username == "" && password == "")
            {
                IsLoggedIn = false;
                return false;
            }

            IsLoggedIn = true;
            Username = user.Username ?? "";
            Role = user.Role ?? "";
            Firstname = user.FirstName ?? "";
            Lastname = user.LastName ?? "";
            Email = user.Email ?? "";
            DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd") ?? "";

            Notify();
            return true;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            Username = "";
            Role = "";
            Firstname = "";
            Lastname = "";
            Email = "";
            DateOfBirth = "";

            Notify();
        }

        public async Task SaveUser()
        {
            var jsonString = JsonSerializer.Serialize(users);
            await JS.InvokeVoidAsync("localStorage.setItem", "All_Users", jsonString);
        }

        bool loaded;
        public async Task LoadUsers()
        {
            if (loaded)
                return;
            var jsonString = await JS.InvokeAsync<string>("localStorage.getItem", "All_Users");
            if (!string.IsNullOrEmpty(jsonString))
            {
                var savedUsers = JsonSerializer.Deserialize<List<UserModel>>(jsonString);
                if (savedUsers != null)
                {
                    users = savedUsers;
                }
            }
            loaded = true;
        }

        public async Task DeleteAccount()
        {
            if (!IsLoggedIn) return;

            var userToRemove = users.FirstOrDefault(u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase));
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
            }

            var jsonString = JsonSerializer.Serialize(users);
            await JS.InvokeVoidAsync("localStorage.setItem", "All_Users", jsonString);

            Logout();
        }
    }
}

