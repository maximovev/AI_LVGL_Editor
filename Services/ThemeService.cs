using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;

namespace LvglProjectEditor.Services
{
    public interface IThemeService
    {
        Task<List<LvglTheme>> GetThemesAsync();
        Task<LvglTheme> GetThemeByIdAsync(string id);
        Task<bool> CreateThemeAsync(LvglTheme theme);
        Task<bool> UpdateThemeAsync(LvglTheme theme);
        Task<bool> DeleteThemeAsync(string id);
        Task<bool> CloneThemeAsync(LvglTheme source, string newName);
        Task<bool> ApplyThemeToProjectAsync(LvglTheme theme, LvglProject project);
        Task<bool> ValidateThemeAsync(LvglTheme theme);
        Task<bool> ValidateAllThemesAsync();
        Task<LvglTheme> GetDefaultThemeAsync();
    }
    
    public class ThemeService : IThemeService
    {
        private readonly List<LvglTheme> _themes = new();
        private LvglTheme _defaultTheme;
        
        public ThemeService()
        {
            _defaultTheme = new LvglTheme
            {
                Id = "default",
                Name = "Default Theme",
                Description = "Default theme for LVGL projects",
                IsSaved = true
            };
            _themes.Add(_defaultTheme);
        }
        
        public async Task<List<LvglTheme>> GetThemesAsync()
        {
            return await Task.Run(() => _themes.ToList());
        }
        
        public async Task<LvglTheme> GetThemeByIdAsync(string id)
        {
            return await Task.Run(() => _themes.FirstOrDefault(t => t.Id == id));
        }
        
        public async Task<bool> CreateThemeAsync(LvglTheme theme)
        {
            return await Task.Run(() =>
            {
                theme.Validate();
                if (theme.IsValid)
                {
                    theme.Id = Guid.NewGuid().ToString("N");
                    theme.CreatedDate = DateTime.Now;
                    _themes.Add(theme);
                    return true;
                }
                return false;
            });
        }
        
        public async Task<bool> UpdateThemeAsync(LvglTheme theme)
        {
            return await Task.Run(() =>
            {
                var existing = _themes.FirstOrDefault(t => t.Id == theme.Id);
                if (existing != null)
                {
                    foreach (var prop in theme.GetType().GetProperties())
                    {
                        var value = prop.GetValue(theme);
                        prop.SetValue(existing, value);
                    }
                    existing.IsModified = false;
                    existing.IsSaved = true;
                    return true;
                }
                return false;
            });
        }
        
        public async Task<bool> DeleteThemeAsync(string id)
        {
            return await Task.Run(() =>
            {
                if (id == _defaultTheme.Id)
                {
                    return false;
                }
                
                var theme = _themes.FirstOrDefault(t => t.Id == id);
                if (theme != null)
                {
                    _themes.Remove(theme);
                    return true;
                }
                return false;
            });
        }
        
        public async Task<bool> CloneThemeAsync(LvglTheme source, string newName)
        {
            return await Task.Run(() =>
            {
                var clone = new LvglTheme
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = newName,
                    Description = source.Description,
                    IsSaved = false
                };
                clone.Styles = source.Styles?.Select(s => s.Clone()).ToList();
                _themes.Add(clone);
                return true;
            });
        }
        
        public async Task<bool> ApplyThemeToProjectAsync(LvglTheme theme, LvglProject project)
        {
            return await Task.Run(() =>
            {
                if (theme.Styles != null)
                {
                    foreach (var style in theme.Styles)
                    {
                        // Применить стиль к элементам проекта
                    }
                }
                return true;
            });
        }
        
        public async Task<bool> ValidateThemeAsync(LvglTheme theme)
        {
            return await Task.Run(() =>
            {
                theme.Validate();
                return theme.IsValid;
            });
        }
        
        public async Task<bool> ValidateAllThemesAsync()
        {
            return await Task.Run(() =>
            {
                var allValid = true;
                foreach (var theme in _themes)
                {
                    theme.Validate();
                    if (!theme.IsValid)
                    {
                        allValid = false;
                    }
                }
                return allValid;
            });
        }
        
        public async Task<LvglTheme> GetDefaultThemeAsync()
        {
            return await Task.Run(() => _defaultTheme);
        }
    }
}