using System;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Views
{
    public partial class StyleEditor : ContentPage
    {
        private LvglStyle _currentStyle;
        
        public StyleEditor()
        {
            InitializeComponent();
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (IsFirstAppearingThisTime)
            {
                await Task.Delay(100);
            }
        }
        
        private async void OnSaveStyleClicked(object sender, EventArgs e)
        {
            var name = StyleNameEntry.Text;
            var type = StyleTypeEntry.Text;
            var fontSize = Convert.ToInt32(FontSizeEntry.Text);
            var padding = Convert.ToInt32(PaddingEntry.Text);
            
            var style = new LvglStyle
            {
                Name = name,
                Type = type,
                FontSize = fontSize,
                PaddingLeft = padding,
                PaddingRight = padding,
                PaddingTop = padding,
                PaddingBottom = padding,
                IsSaved = true
            };
            
            await DisplayAlert("Success", "Style saved successfully!", "OK");
        }
        
        private async void OnDeleteStyleClicked(object sender, EventArgs e)
        {
            var confirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this style?", "Yes", "No");
            
            if (confirmed)
            {
                await DisplayAlert("Success", "Style deleted!", "OK");
            }
        }
    }
}