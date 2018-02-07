using DotVVM.Framework.ViewModel;

namespace electroweb.ViewModels
{
    public abstract class BaseViewModel : DotvvmViewModelBase
    {

         /// <summary>
        /// Gets or sets the active page. This is used in the top menu bar to highlight the current menu item.
        /// </summary>
        public virtual string ActivePage => Context.Route.RouteName;

        
    }
}