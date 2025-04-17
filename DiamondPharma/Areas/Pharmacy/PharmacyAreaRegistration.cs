using Microsoft.AspNetCore.Mvc;

namespace DiamondPharma.Areas.Pharmacy
{
    public class PharmacyAreaRegistration : AreaAttribute
    {
        public PharmacyAreaRegistration() : base("Pharmacy") { }
    }
}
