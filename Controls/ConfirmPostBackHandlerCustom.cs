using System.Collections.Generic;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;

namespace electroweb.Controls
{
    public class ConfirmPostBackHandlerCustom : PostBackHandler
    {
        protected override string ClientHandlerName => "confirm";
        protected  override Dictionary<string, string> GetHandlerOptionClientExpressions()
        {
            return new Dictionary<string, string>()
            {
                ["message"] = TranslateValueOrBinding(MessageProperty)
            };
        }
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DotvvmProperty MessageProperty
            = DotvvmProperty.Register<string, ConfirmPostBackHandlerCustom>(c => c.Message, null);
    }
}