namespace RopeyDVDs.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUser()
        {
            return _httpContext.HttpContext.User.Identity.Name;
        }
        public bool IsAuthenticated()
        {
            return _httpContext.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
