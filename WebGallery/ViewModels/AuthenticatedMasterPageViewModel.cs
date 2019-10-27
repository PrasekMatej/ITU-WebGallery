using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;

namespace WebGallery.ViewModels
{

    [Authorize]
    public class AuthenticatedMasterPageViewModel : MasterPageViewModel
    {
        [Bind(Direction.ServerToClientFirstRequest)]
        public string Username => Context.HttpContext.User.Identity.Name;
    }
}

