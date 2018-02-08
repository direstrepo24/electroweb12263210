using System.Security.Claims;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace electroweb.ViewModels
{
    public abstract class BaseViewModel : DotvvmViewModelBase
    {

         /// <summary>
        /// Gets or sets the active page. This is used in the top menu bar to highlight the current menu item.
        /// </summary>
        public virtual string ActivePage => Context.Route.RouteName;

        public void SignOut()
        {
            // sign out
            var identity = (ClaimsIdentity)Context.HttpContext.User.Identity;
             Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Context.RedirectToRoute("Login");
         
        }

        
    }
}