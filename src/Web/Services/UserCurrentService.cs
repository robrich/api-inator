namespace ApiInator.Web.Services {
    using System;
    using System.Security.Claims;
    using ApiInator.Web.Helpers;
    using ApiInator.Web.Models;
    using ApiInator.Web.Repositories;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc.Rendering;

    public interface IUserCurrentService {
        bool IsAuthenticated { get; }
        int? UserId { get; }
        User User { get; }
        string Name { get; }
        string AvitarUrl { get; }
        bool IsAdmin { get; }
    }

    public class UserCurrentService : IUserCurrentService {
        private readonly IUserRepository userRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserCurrentService(IUserRepository UserRepository, IHttpContextAccessor HttpContextAccessor) {
            if (UserRepository == null) {
                throw new ArgumentNullException(nameof(UserRepository));
            }
            if (HttpContextAccessor == null) {
                throw new ArgumentNullException(nameof(HttpContextAccessor));
            }
            this.userRepository = UserRepository;
            this.httpContextAccessor = HttpContextAccessor;
        }

        // FRAGILE: duplicates UserHelpers.cs

        private ClaimsPrincipal GetUser() {
            return this.httpContextAccessor.HttpContext.User;
        }

        public bool IsAuthenticated {
            get {
                ClaimsPrincipal principal = this.GetUser();
                return principal != null && principal.Identity.IsAuthenticated;
            }
        }
        
        public int? UserId {
            get {
                if (!this.IsAuthenticated) {
                    return null;
                }
                int userId = 0;
                if (!int.TryParse(this.GetUser().Identity.Name, out userId)) {
                    return null;
                }
                return userId;
            }
        }

        public User User {
            get {
                if (!this.IsAuthenticated) {
                    return null;
                }
                User user = this.httpContextAccessor.HttpContext.Items["CurrentUser"] as User;
                if (user == null) {
                    user = this.userRepository.GetById(this.UserId ?? 0);
                    this.httpContextAccessor.HttpContext.Items["CurrentUser"] = user; // cache it for the duration of the request
                }
                return user;
            }
        }

        public string Name {
            get {
                if (!this.IsAuthenticated) {
                    return null;
                }
                var user = this.GetUser();
                const string githubNameKey = "urn:github:name";
                bool hasClaim = user.HasClaim(c => c.Type == githubNameKey);
                string name = null;
                if (hasClaim) {
                    name = user.FindFirst(c => c.Type == githubNameKey).Value;
                }
                return name;
            }
        }

        public string AvitarUrl {
            get {
                if (!this.IsAuthenticated) {
                    return null;
                }
                var user = this.GetUser();
                const string githubNameKey = "urn:github:avitar";
                bool hasClaim = user.HasClaim(c => c.Type == githubNameKey);
                string name = null;
                if (hasClaim) {
                    name = user.FindFirst(c => c.Type == githubNameKey).Value;
                }
                return name;
            }
        }

        public bool IsAdmin {
            get {
                if (!this.IsAuthenticated) {
                    return false;
                }
                var user = this.GetUser();
                return user.IsInRole("admin");
            }
        }

    }
}
