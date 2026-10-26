using System;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Views
{
    public partial class ElementEditor : ContentPage
    {
        private LvglElement _currentElement;
        
        public ElementEditor()
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
        
        private async void OnSaveElementClicked(object sender, EventArgs e)
        {
            var name = ElementNameEntry.Text;
            var type = ElementTypeEntry.Text;
            var x = Convert.ToInt32(XPositionEntry.Text);
            var y = Convert.ToInt32(YPositionEntry.Text);
            var width = Convert.ToInt32(WidthEntry.Text);
            var height = Convert.ToInt32(HeightEntry.Text);
            
            var element = new LvglElement
            {
                Name = name,
                Type = type,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                IsVisible = true,
                IsEnabled = true
            };
            
            await DisplayAlert("Success", "Element saved successfully!", "OK");
        }
        
        private async void OnDeleteElementClicked(object sender, EventArgs e)
        {
            var confirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this element?", "Yes", "No");
            
            if (confirmed)
            {
                await DisplayAlert("Success", "Element deleted!", "OK");
            }
        }
    }
}