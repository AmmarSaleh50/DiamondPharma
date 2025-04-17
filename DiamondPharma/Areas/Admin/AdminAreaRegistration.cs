using Microsoft.AspNetCore.Mvc;

namespace DiamondPharma.Areas.Admin
{
    public class AdminAreaRegistration : AreaAttribute
    {
        public AdminAreaRegistration() : base("Admin") { }
    }
}
