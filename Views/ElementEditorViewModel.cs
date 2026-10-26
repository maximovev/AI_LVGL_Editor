using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvglProjectEditor.Models;
using LvglProjectEditor.Services;

namespace LvglProjectEditor.ViewModels
{
    public class ElementEditorViewModel
    {
        private readonly IElementService _elementService;
        private readonly IStyleService _styleService;
        private readonly IValidationService _validationService;
        
        // Связанные данные
        public List<LvglElement> Elements { get; set; }
        public LvglElement SelectedElement { get; set; }
        public List<LvglStyle> Styles { get; set; }
        public LvglStyle SelectedStyle { get; set; }
        
        // Вводные данные
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public int ElementX { get; set; }
        public int ElementY { get; set; }
        public int ElementWidth { get; set; }
        public int ElementHeight { get; set; }
        public int ElementPadding { get; set; }
        public int ElementFontSize { get; set; }
        public bool ElementIsVisible { get; set; }
        public bool ElementIsEnabled { get; set; }
        
        // Состояние
        public bool IsElementModified { get; set; }
        public bool IsElementSaved { get; set; }
        public string ErrorMessage { get; set; }
        public int SelectedStyleIndex { get; set; }
        public ElementType ElementTypeCategory { get; set; }
        
        public ElementEditorViewModel()
        {
            _elementService = new ElementService();
            _styleService = new StyleService();
            _validationService = new ValidationService();
            
            Elements = new List<LvglElement>();
            Styles = new List<LvglStyle>();
            SelectedElement = null;
            SelectedStyle = null;
            
            LoadElements();
        }
        
        public async void LoadElements()
        {
            try
            {
                Elements = await _elementService.GetElementsAsync();
                
                if (Elements.Count > 0)
                {
                    SelectedElement = Elements[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке элементов: {ex.Message}";
            }
        }
        
        public async Task<string> GetElementName()
        {
            return ElementName;
        }
        
        public async Task<string> GetElementType()
        {
            return ElementType;
        }
        
        public async Task<int> GetElementX()
        {
            return ElementX;
        }
        
        public async Task<int> GetElementY()
        {
            return ElementY;
        }
        
        public async Task<int> GetElementWidth()
        {
            return ElementWidth;
        }
        
        public async Task<int> GetElementHeight()
        {
            return ElementHeight;
        }
        
        public async Task<bool> CreateNewElementAsync(ElementType type, string name = null, int x = 0, int y = 0, int width = 0, int height = 0)
        {
            try
            {
                var element = new LvglElement
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = name ?? $"Element {Elements.Count + 1}",
                    Type = type.ToString(),
                    ElementTypeCategory = type,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    IsVisible = true,
                    IsEnabled = true,
                    IsSelected = false,
                    IsModified = false,
                    IsSaved = true,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now
                };
                
                var success = await _elementService.CreateElementAsync(element);
                
                if (success)
                {
                    Elements.Add(element);
                    SelectedElement = element;
                    ElementName = element.Name;
                    ElementType = element.Type;
                    ElementX = element.X;
                    ElementY = element.Y;
                    ElementWidth = element.Width;
                    ElementHeight = element.Height;
                    IsElementModified = false;
                    IsElementSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось создать элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при создании элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> UpdateElementAsync()
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                SelectedElement.Name = ElementName;
                SelectedElement.Type = ElementType;
                SelectedElement.X = ElementX;
                SelectedElement.Y = ElementY;
                SelectedElement.Width = ElementWidth;
                SelectedElement.Height = ElementHeight;
                SelectedElement.IsVisible = ElementIsVisible;
                SelectedElement.IsEnabled = ElementIsEnabled;
                
                var success = await _elementService.UpdateElementAsync(SelectedElement);
                
                if (success)
                {
                    IsElementModified = false;
                    IsElementSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось обновить элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при обновлении элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> CloneElementAsync(string newName = null)
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var clone = await _elementService.CloneElementAsync(SelectedElement, newName ?? $"Clone of {SelectedElement.Name}");
                
                if (clone != null)
                {
                    Elements.Add(clone);
                    SelectedElement = clone;
                    IsElementModified = false;
                    IsElementSaved = false;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось клонировать элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при клонировании элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> DeleteElementAsync()
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var success = await _elementService.DeleteElementAsync(SelectedElement.Id);
                
                if (success)
                {
                    Elements.Remove(SelectedElement);
                    SelectedElement = Elements.Count > 0 ? Elements[0] : null;
                    
                    IsElementModified = false;
                    IsElementSaved = true;
                    ErrorMessage = null;
                    
                    return true;
                }
                
                ErrorMessage = "Не удалось удалить элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при удалении элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> AddChildElementAsync(LvglElement child)
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var success = await _elementService.AddChildAsync(SelectedElement, child);
                
                if (success)
                {
                    IsElementModified = true;
                    IsElementSaved = false;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось добавить дочерний элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при добавлении дочернего элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> RemoveChildElementAsync(string childId)
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var success = await _elementService.RemoveChildAsync(SelectedElement, childId);
                
                if (success)
                {
                    IsElementModified = true;
                    IsElementSaved = false;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось удалить дочерний элемент";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при удалении дочернего элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ApplyStyleAsync(LvglStyle style)
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var success = await _styleService.ApplyStyleToElementAsync(style, SelectedElement);
                
                if (success)
                {
                    IsElementModified = true;
                    IsElementSaved = false;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Не удалось применить стиль";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при применении стиля: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> ValidateElementAsync()
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateElementAsync(SelectedElement);
                
                if (isValid)
                {
                    IsElementModified = false;
                    IsElementSaved = true;
                    ErrorMessage = null;
                    return true;
                }
                
                ErrorMessage = "Элемент не валиден";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при валидации элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<bool> SaveElementAsync()
        {
            if (SelectedElement == null)
            {
                ErrorMessage = "Нет выбранного элемента";
                return false;
            }
            
            try
            {
                var isValid = await _validationService.ValidateElementAsync(SelectedElement);
                
                if (isValid)
                {
                    var success = await _elementService.UpdateElementAsync(SelectedElement);
                    
                    if (success)
                    {
                        IsElementModified = false;
                        IsElementSaved = true;
                        ErrorMessage = null;
                        return true;
                    }
                    
                    ErrorMessage = "Не удалось сохранить элемент";
                    return false;
                }
                
                ErrorMessage = "Элемент не валиден";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при сохранении элемента: {ex.Message}";
                return false;
            }
        }
        
        public async Task<string> GetElementInfoAsync()
        {
            if (SelectedElement == null)
            {
                return "Нет выбранного элемента";
            }
            
            var info = $"ID: {SelectedElement.Id}\n";
            info += $"Name: {SelectedElement.Name}\n";
            info += $"Type: {SelectedElement.Type}\n";
            info += $"Position: X={SelectedElement.X}, Y={SelectedElement.Y}\n";
            info += $"Size: Width={SelectedElement.Width}, Height={SelectedElement.Height}\n";
            info += $"Visible: {SelectedElement.IsVisible}\n";
            info += $"Enabled: {SelectedElement.IsEnabled}\n";
            info += $"Children Count: {SelectedElement.Children?.Count ?? 0}";
            
            return info;
        }
        
        public bool HasChanges => IsElementModified;
        
        public bool IsSaved => IsElementSaved;
        
        public string ErrorText => ErrorMessage;
        
        public int ElementsCount => Elements?.Count ?? 0;
        
        public int ChildrenCount => SelectedElement?.Children?.Count ?? 0;
        
        public string ElementTypeName => ElementTypeCategory?.ToString() ?? "Unknown";
        
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
    }
}