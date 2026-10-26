using System;
using System.Collections.Generic;

namespace LvglProjectEditor.Models
{
    public class LvglElement
    {
        // Уникальный идентификатор
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        
        // Позиция и размер
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        // Относительные координаты (процентные)
        public int XPercent { get; set; }
        public int YPercent { get; set; }
        public int WidthPercent { get; set; }
        public int HeightPercent { get; set; }
        
        // Родительский элемент
        public string ParentId { get; set; }
        public LvglElement Parent { get; set; }
        
        // Иерархия
        public List<LvglElement> Children { get; set; }
        
        // Видимость и состояние
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSelected { get; set; }
        public bool IsLocked { get; set; }
        
        // Типы элементов LVGL
        public ElementType ElementTypeCategory { get; set; }
        public ElementType SubType { get; set; }
        
        // Свойства элемента
        public LvglStyle Style { get; set; }
        public LvglStyle ParentStyle { get; set; }
        
        // Данные элемента
        public string Data { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }
        
        // События
        public List<ElementEvent> Events { get; set; }
        
        // Метаданные
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        
        // Статус
        public bool IsModified { get; set; }
        public bool IsSaved { get; set; }
        public string ErrorMessage { get; set; }
        
        // Конструктор
        public LvglElement()
        {
            Children = new List<LvglElement>();
            Events = new List<ElementEvent>();
            IsSaved = true;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
        
        // Конструктор с параметрами
        public LvglElement(string name, string type, int x, int y, int width, int height)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name ?? string.Empty;
            Type = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            Children = new List<LvglElement>();
            Events = new List<ElementEvent>();
            IsSaved = true;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
        
        // Конструктор с типом элемента
        public LvglElement(ElementType type, string name = null, int x = 0, int y = 0, int width = 0, int height = 0)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name ?? type.ToString();
            Type = type.ToString();
            ElementTypeCategory = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            Children = new List<LvglElement>();
            Events = new List<ElementEvent>();
            IsSaved = true;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
        
        // Добавление дочернего элемента
        public void AddChild(LvglElement child)
        {
            child.Parent = this;
            child.ParentId = Id;
            Children.Add(child);
            IsModified = true;
        }
        
        // Удаление дочернего элемента
        public bool RemoveChild(string childId)
        {
            var child = Children.FirstOrDefault(c => c.Id == childId);
            if (child != null)
            {
                Children.Remove(child);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Удаление дочернего элемента по имени
        public bool RemoveChildByName(string childName)
        {
            var child = Children.FirstOrDefault(c => c.Name == childName);
            if (child != null)
            {
                Children.Remove(child);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Поиск элемента по ID
        public LvglElement FindById(string id)
        {
            if (Id == id)
                return this;
            
            foreach (var child in Children)
            {
                var found = child.FindById(id);
                if (found != null)
                    return found;
            }
            return null;
        }
        
        // Поиск элемента по имени
        public LvglElement FindByName(string name)
        {
            if (Name == name)
                return this;
            
            foreach (var child in Children)
            {
                var found = child.FindByName(name);
                if (found != null)
                    return found;
            }
            return null;
        }
        
        // Получение всех элементов рекурсивно
        public List<LvglElement> GetAllElements()
        {
            var elements = new List<LvglElement> { this };
            foreach (var child in Children)
            {
                elements.AddRange(child.GetAllElements());
            }
            return elements;
        }
        
        // Получение всех элементов определённого типа
        public List<LvglElement> FindElementsByType(ElementType type)
        {
            var elements = new List<LvglElement>();
            if (Type == type.ToString())
                elements.Add(this);
            
            foreach (var child in Children)
            {
                elements.AddRange(child.FindElementsByType(type));
            }
            return elements;
        }
        
        // Клонирование элемента
        public LvglElement Clone()
        {
            var clone = new LvglElement
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name,
                Type = Type,
                ElementTypeCategory = ElementTypeCategory,
                SubType = SubType,
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                XPercent = XPercent,
                YPercent = YPercent,
                WidthPercent = WidthPercent,
                HeightPercent = HeightPercent,
                IsVisible = IsVisible,
                IsEnabled = IsEnabled,
                IsSelected = IsSelected,
                IsLocked = IsLocked,
                Style = Style?.Clone(),
                Data = Data,
                Value = Value,
                Text = Text,
                Children = new List<LvglElement>(),
                Events = new List<ElementEvent>(),
                Parent = null,
                ParentStyle = ParentStyle,
                Author = Author,
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                IsModified = true,
                IsSaved = false
            };
            
            foreach (var child in Children)
            {
                clone.Children.Add(child.Clone());
            }
            
            foreach (var @event in Events)
            {
                clone.Events.Add(@event.Clone());
            }
            
            return clone;
        }
        
        // Сохранение элемента
        public void Save()
        {
            foreach (var child in Children)
            {
                child.Save();
            }
            IsModified = false;
            IsSaved = true;
            LastModified = DateTime.Now;
        }
        
        // Добавление события
        public void AddEvent(ElementEvent @event)
        {
            Events.Add(@event);
            @event.ElementId = Id;
            IsModified = true;
        }
        
        // Удаление события
        public bool RemoveEvent(string eventId)
        {
            var index = Events.FindIndex(e => e.Id == eventId);
            if (index >= 0)
            {
                Events.RemoveAt(index);
                IsModified = true;
                return true;
            }
            return false;
        }
        
        // Валидация элемента
        public void Validate()
        {
            IsModified = false;
            ErrorMessage = null;
            
            if (string.IsNullOrEmpty(Name))
            {
                IsModified = false;
                ErrorMessage = "Имя элемента не задано";
                return;
            }
            
            if (string.IsNullOrEmpty(Type))
            {
                IsModified = false;
                ErrorMessage = "Тип элемента не задан";
                return;
            }
            
            if (X < 0 || Y < 0 || Width <= 0 || Height <= 0)
            {
                IsModified = false;
                ErrorMessage = "Неверные координаты или размеры";
                return;
            }
        }
        
        // Проверка пересечения с другим элементом
        public bool Intersects(LvglElement other)
        {
            var left = Math.Max(X, other.X);
            var top = Math.Max(Y, other.Y);
            var right = Math.Min(X + Width, other.X + other.Width);
            var bottom = Math.Min(Y + Height, other.Y + other.Height);
            
            return left < right && top < bottom;
        }
        
        // Проверка вложенности
        public bool Contains(LvglElement element)
        {
            if (element.ParentId == Id)
                return true;
            
            foreach (var child in Children)
            {
                if (child.Contains(element))
                    return true;
            }
            return false;
        }
        
        // Получение всех родительских элементов
        public List<LvglElement> GetAllParents()
        {
            var parents = new List<LvglElement>();
            var current = Parent;
            
            while (current != null)
            {
                parents.Insert(0, current);
                current = current.Parent;
            }
            
            return parents;
        }
        
        // Форматирование координат
        public string FormatCoordinates()
        {
            return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }
        
        // Форматирование размеров
        public string FormatSize()
        {
            return $"{Width}x{Height}";
        }
    }
    
    public enum ElementType
    {
        // Базовые элементы
        Container,
        Box,
        Flex,
        Grid,
        
        // Текстовые элементы
        Label,
        Text,
        RichText,
        
        // Кнопки и интерактивные элементы
        Button,
        Checkbox,
        Toggle,
        Radio,
        Switch,
        Slider,
        ProgressBar,
        
        // Формы ввода
        Textarea,
        TextInput,
        Password,
        Select,
        List,
        Spinner,
        
        // Изображения и медиа
        Image,
        Video,
        Audio,
        Canvas,
        
        // Списки и таблицы
        List,
        Table,
        Tree,
        
        // Сложные элементы
        Chart,
        Map,
        Editor,
        Terminal,
        
        // Специальные элементы
        Overlay,
        Mask,
        Scrollable,
        ScrollableBox,
        
        // Пользовательские элементы
        Custom,
        User,
        
        // Другие
        Other
    }
    
    public class ElementEvent
    {
        public string Id { get; set; }
        public string ElementId { get; set; }
        public string EventType { get; set; }
        public string Handler { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsEnabled { get; set; }
        
        public ElementEvent()
        {
            IsEnabled = true;
        }
        
        public ElementEvent(ElementType eventType, string handler, string description = null)
        {
            EventType = eventType.ToString();
            Handler = handler;
            Description = description;
            IsEnabled = true;
        }
        
        public ElementEvent Clone()
        {
            return new ElementEvent
            {
                Id = Guid.NewGuid().ToString("N"),
                ElementId = ElementId,
                EventType = EventType,
                Handler = Handler,
                Description = Description,
                Code = Code,
                IsEnabled = IsEnabled
            };
        }
    }
}