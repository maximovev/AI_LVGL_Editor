using System;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Views
{
    public partial class ThemeEditor : ContentPage
    {
        private LvglTheme _currentTheme;
        
        public ThemeEditor()
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
        
        private async void OnSaveThemeClicked(object sender, EventArgs e)
        {
            var name = ThemeNameEntry.Text;
            var themeType = ThemeTypePicker.SelectedItem?.ToString();
            var description = ThemeDescriptionEntry.Text;
            var primaryColor = PrimaryColorPicker.SelectedItem?.ToString();
            var backgroundColor = BackgroundColorPicker.SelectedItem?.ToString();
            
            var theme = new LvglTheme
            {
                Name = name,
                ThemeType = themeType switch
                {
                    "Light" => ThemeType.Light,
                    "Dark" => ThemeType.Dark,
                    "System" => ThemeType.System,
                    "High Contrast" => ThemeType.HighContrast,
                    _ => ThemeType.Dark
                },
                Description = description,
                IsSaved = true
            };
            
            await DisplayAlert("Success", "Theme saved successfully!", "OK");
        }
        
        private async void OnDeleteThemeClicked(object sender, EventArgs e)
        {
            var confirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this theme?", "Yes", "No");
            
            if (confirmed)
            {
                await DisplayAlert("Success", "Theme deleted!", "OK");
            }
        }
    }
}