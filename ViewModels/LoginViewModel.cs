using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Electro.model.Repository;

namespace electroweb.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {

        IUsuarioRepository _usuarioRepository;

        
        public string UserName { get; set; }
        public string Password { get; set; }    

         [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }  


        public LoginViewModel(IUsuarioRepository usuarioRepository){
            _usuarioRepository= usuarioRepository;

        }  

        public async Task Login()
        {
            if (await VerifyCredentials(UserName, Password)) 
            {
                // the CreateIdentity is your own method which creates the IIdentity representing the user
                var identity = CreateIdentity(UserName);
               await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                Context.RedirectToRoute("Default");        
            }
        }

        private async Task<bool> VerifyCredentials(string username, string password) 
        {
            // verify credentials and return true or false

            bool response= false;
            var usuario= await _usuarioRepository.GetSingleAsync(a=>a.CorreoElectronico==username && a.Passsword==password);
            if(usuario!=null){
                response=true;
            }else{
                response=false;
                 ErrorMessage = "Nombre de usuario o contraseña invalidos!";
                 UserName=string.Empty;
                 Password=string.Empty;
            }
            return response;
        }

        private ClaimsIdentity CreateIdentity(string username) 
        {
            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, username),

                    // add claims for each user role
                    new Claim(ClaimTypes.Role, "administrator"),
                },
                "Cookie");
            return identity;
        }


        /*
        [Required(ErrorMessage = "The e-mail address is required!")]
        [EmailAddress(ErrorMessage = "The e-mail address is not valid!")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }


        public bool AADEnabled => true;



        // The user cannot change this field in the browser so there is no point in transferring it from the client to the server
        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }


        public override Task Init()
        {
             
            if (!Context.IsPostBack && Context.GetAuthentication().User.Identity.IsAuthenticated)
            {
                // redirect to the home page if the user is already authenticated
                Context.RedirectToRoute("home");
            }

            return base.Init();
        }


        public void SignIn()
        {
            //var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
            {
                ErrorMessage = "Invalid e-mail address or password!";
            }
            else
            {
                // issue an authentication cookie
                var properties = new AuthenticationProperties()
                {
                    IsPersistent = RememberMe,
                    ExpiresUtc = RememberMe ? DateTime.UtcNow.AddMonths(1) : (DateTime?)null
                };
                Context.GetAuthentication().SignIn(properties, identity);

                // redirect to the home page
                
            }

            Context.RedirectToRoute("Home");

         ///return RedirectToPage("/Index");
        }
        */
       
        
    }
}