using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

