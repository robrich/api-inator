namespace ApiInator.Web.ViewModels.Manage
{
    using System.Collections.Generic;
    using Microsoft.AspNet.Mvc.Rendering;

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
