using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.ViewModels
{
    public class StyleEditorViewModel
    {
        private readonly IStyleService _styleService;
        private readonly IThemeService _themeService;
        private readonly IValidationService _validationService;
        
        // Связанные данные
        public List<LvglStyle> Styles { get; set; }
        public LvglStyle SelectedStyle { get; set; }
        public List<LvglTheme> Themes { get; set; }
        public LvglTheme SelectedTheme { get; set; }
        
        // Вводные данные
        public string StyleName { get; set; }
        public string StyleType { get; set; }
        public ElementType TargetElementType { get; set; }
        public int BackgroundColor { get; set; }
        public int TextColor { get; set; }
        public int BorderRadius { get; set; }
        public int BorderWidth { get; set; }
        public int FontSize { get; set; }
        public int FontSpacing { get; set; }
        public int LineHeight { get; set; }
        public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }
        public int MarginTop { get; set; }
        public int MarginBottom { get; set; }
        public int Opacity { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
        
        // Состояние
        public bool IsStyleModified { get; set; }
        public bool IsStyleSaved { get; set; }
        public string ErrorMessage { get; set; }
        public int SelectedThemeIndex { get; set; }
        
        public StyleEditorViewModel()
        {
            _styleService = new StyleService();
            _themeService = new ThemeService();
            _validationService = new ValidationService();
            
            Styles = new List<LvglStyle>();
            Themes = new List<LvglTheme>();
            SelectedStyle = null;
            SelectedTheme = null;
            
            LoadStyles();
            LoadThemes();
        }
        
        public async void LoadStyles()
        {
            try
            {
                Styles = await _styleService.GetStylesAsync();
                
                if (Styles.Count > 0)
                {
                    SelectedStyle = Styles[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке стилей: {ex.Message}";
            }
        }
        
        public async void LoadThemes()
        {
            try
            {
                Themes = await _themeService.GetThemesAsync();
                
                if (Themes.Count > 0)
                {
                    SelectedTheme = Themes[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке тем: {ex.Message}";
            }
        }
        
        public async Task<string> GetStyleName()
        {
            return StyleName;
        }
        
        public async Task<string> GetStyleType()
        {
            return StyleType;
        }
        
        public async Task<ElementType> GetTargetElementType()
        {
            return TargetElementType;
        }
        
        public async Task<int> GetBackgroundColor()
        {
            return BackgroundColor;
        }
        
        public async Task<int> GetTextColor()
        {
            return TextColor;
        }
        
        public async Task<int> GetBorderRadius()
        {
            return BorderRadius;
        }
        
        public async Task<int> GetBorderWidth()
        {
            return BorderWidth;
        }
        
        public async Task<int> GetFontSize()
        {
            return FontSize;
        }
        
        public async Task<int> GetFontSpacing()
        {
            return FontSpacing;
        }
        
        public async Task<int> GetLineHeight()
        {
            return LineHeight;
        }
        
        public async Task<int> GetPaddingLeft()
        {
            return PaddingLeft;
        }
        
        public async Task<int> GetPaddingRight()
        {
            return PaddingRight;
        }
        
        public async Task<int> GetPaddingTop()
        {
            return PaddingTop;
        }
        
        public async Task<int> GetPaddingBottom()
        {
            return PaddingBottom;
        }
        
        public async Task<int> GetMarginLeft()
        {
            return MarginLeft;
        }
        
        public async Task<int> GetMarginRight()
        {
            return MarginRight;
        }
        
        public async Task<int> GetMarginTop()
        {
            return MarginTop;
        }
        
        public async Task<int> GetMarginBottom()
        {
            return MarginBottom;
        }
        
        public async Task<int> GetOpacity()
        {
            return Opacity;
        }
        
        public async Task<bool> CreateNewStyleAsync(ElementType targetElementType, string name = null)
        {
            try
            {
                var style = new LvglStyle
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = name ?? $"Style {Styles.Count + 1}",
                    Type = targetElementType.ToString(),
                    TargetElementType = targetElementType,
                    IsVisible = true,
                    IsEnabled = true,
                    IsModified = false,
                    IsSaved = true,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                
                var success = await _styleService.CreateStyleAsync(style);
                
                if (success)
                {
                    Styles.Add(style);
                    SelectedStyle = style;
                    StyleName = style.Name;
                    StyleType = style.Type;
                    TargetElementType = style.TargetElementType;
                    IsStyleModified = false;
                    IsStyleSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось создать стиль";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при создании стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> UpdateStyleAsync()
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                SelectedStyle.Name = StyleName;
                SelectedStyle.Type = StyleType;
                SelectedStyle.TargetElementType = TargetElementType;
                SelectedStyle.BackgroundColor = BackgroundColor;
                SelectedStyle.TextColor = TextColor;
                SelectedStyle.BorderRadius = BorderRadius;
                SelectedStyle.BorderWidth = BorderWidth;
                SelectedStyle.FontSize = FontSize;
                SelectedStyle.FontSpacing = FontSpacing;
                SelectedStyle.LineHeight = LineHeight;
                SelectedStyle.PaddingLeft = PaddingLeft;
                SelectedStyle.PaddingRight = PaddingRight;
                SelectedStyle.PaddingTop = PaddingTop;
                SelectedStyle.PaddingBottom = PaddingBottom;
                SelectedStyle.MarginLeft = MarginLeft;
                SelectedStyle.MarginRight = MarginRight;
                SelectedStyle.MarginTop = MarginTop;
                SelectedStyle.MarginBottom = MarginBottom;
                SelectedStyle.Opacity = Opacity;
                
                var success = await _styleService.UpdateStyleAsync(SelectedStyle);
                
                if (success)
                {
                    IsStyleModified = false;
                    IsStyleSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось обновить стиль";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при обновлении стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> CloneStyleAsync(string newName = null)
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                var clone = await _styleService.CloneStyleAsync(SelectedStyle, newName ?? $"Clone of {SelectedStyle.Name}");
                
                if (clone != null)
                {
                    Styles.Add(clone);
                    SelectedStyle = clone;
                    IsStyleModified = false;
                    IsStyleSaved = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось клонировать стиль";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при клонировании стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> DeleteStyleAsync()
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                var success = await _styleService.DeleteStyleAsync(SelectedStyle.Id);
                
                if (success)
                {
                    Styles.Remove(SelectedStyle);
                    SelectedStyle = Styles.Count > 0 ? Styles[0] : null;
                    
                    IsStyleModified = false;
                    IsStyleSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось удалить стиль";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при удалении стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ApplyStyleToElementAsync(LvglElement element)
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                var success = await _styleService.ApplyStyleToElementAsync(SelectedStyle, element);
                
                if (success)
                {
                    IsStyleModified = true;
                    IsStyleSaved = false;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось применить стиль к элементу";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при применении стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ValidateStyleAsync()
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateStyleAsync(SelectedStyle);
                
                if (isValid)
                {
                    IsStyleModified = false;
                    IsStyleSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Стиль не валиден";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при валидации стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> SaveStyleAsync()
        {
            if (SelectedStyle == null)
            {
                ErrorMessage = "Нет выбранного стиля";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateStyleAsync(SelectedStyle);
                
                if (isValid)
                {
                    var success = await _styleService.UpdateStyleAsync(SelectedStyle);
                    
                    if (success)
                    {
                        IsStyleModified = false;
                        IsStyleSaved = true;
                        ErrorMessage = null;
                        return true;
                    }
                    
                    ErrorMessage = "Не удалось сохранить стиль";
                    return false;
                }
                
                ErrorMessage = "Стиль не валиден";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ApplyThemeAsync(LvglTheme theme)
        {
            if (theme.Styles != null)
            {
                foreach (var style in theme.Styles)
                {
                    // Применить стиль к элементам
                }
            }
            
            return true;
        }
        
        public async Task<string> GetStyleInfoAsync()
        {
            if (SelectedStyle == null)
            {
                return "Нет выбранного стиля";
            }
            
            var info = $"ID: {SelectedStyle.Id}\n";
            info += $"Name: {SelectedStyle.Name}\n";
            info += $"Type: {SelectedStyle.Type}\n";
            info += $"Target Element: {SelectedStyle.TargetElementType}\n";
            info += $"Background Color: {SelectedStyle.BackgroundColor:X8}\n";
            info += $"Text Color: {SelectedStyle.TextColor:X8}\n";
            info += $"Border Radius: {SelectedStyle.BorderRadius}px\n";
            info += $"Border Width: {SelectedStyle.BorderWidth}px\n";
            info += $"Font Size: {SelectedStyle.FontSize}px\n";
            info += $"Font Spacing: {SelectedStyle.FontSpacing}px\n";
            info += $"Line Height: {SelectedStyle.LineHeight}px\n";
            info += $"Padding: {SelectedStyle.PaddingLeft} {SelectedStyle.PaddingRight} {SelectedStyle.PaddingTop} {SelectedStyle.PaddingBottom}\n";
            info += $"Margin: {SelectedStyle.MarginLeft} {SelectedStyle.MarginRight} {SelectedStyle.MarginTop} {SelectedStyle.MarginBottom}\n";
            info += $"Opacity: {SelectedStyle.Opacity}%\n";
            info += $"Visible: {SelectedStyle.IsVisible}\n";
            info += $"Enabled: {SelectedStyle.IsEnabled}";
            
            return info;
        }
        
        public bool HasChanges => IsStyleModified;
        
        public bool IsSaved => IsStyleSaved;
        
        public string ErrorText => ErrorMessage;
        
        public int StylesCount => Styles?.Count ?? 0;
        
        public string StyleTypeName => TargetElementType?.ToString() ?? "Unknown";
        
        public List<ElementType> AvailableElementTypes => new List<ElementType>
        {
            ElementType.Container,
            ElementType.Box,
            ElementType.Label,
            ElementType.Button,
            ElementType.Checkbox,
            ElementType.Slider,
            ElementType.TextInput,
            ElementType.Textarea,
            ElementType.Image
        };
        
        public int OpacityMin => 0;
        
        public int OpacityMax => 100;
        
        public int PaddingMin => 0;
        
        public int PaddingMax => 100;
        
        public int FontSizeMin => 8;
        
        public int FontSizeMax => 128;
        
        public int BorderRadiusMin => 0;
        
        public int BorderRadiusMax => 100;
        
        public int BorderWidthMin => 0;
        
        public int BorderWidthMax => 20;
    }
}