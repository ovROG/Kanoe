using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Kanoe.Shared.Components
{
    public class PasswordInput<T> : MudTextField<T>
    {
        private bool isShow;

        protected override void OnParametersSet()
        {
            InputType = InputType.Password;
            AdornmentIcon = Icons.Material.Filled.VisibilityOff;
            Adornment = Adornment.End;
            OnAdornmentClick = new EventCallback<MouseEventArgs>(this, OnToggle); ;
            base.OnParametersSet();
        }

        void OnToggle()
        {
            if (isShow)
            {
                isShow = false;
                AdornmentIcon = Icons.Material.Filled.VisibilityOff;
                InputType = InputType.Password;
            }
            else
            {
                isShow = true;
                AdornmentIcon = Icons.Material.Filled.Visibility;
                InputType = InputType.Text;
            }
        }
    }
}
