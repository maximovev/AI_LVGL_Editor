using System;
using System.Collections.Generic;

namespace LvglProjectEditor.Models
{
    public class LvglStyle
    {
        // Уникальный идентификатор
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        
        // Родительский стиль
        public string ParentStyleId { get; set; }
        public LvglStyle ParentStyle { get; set; }
        
        // Настройки отступов
        public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingBottom { get; set; }
        
        public int MarginLeft { get; set; }
        public int MarginRight { get; set; }
        public int MarginTop { get; set; }
        public int MarginBottom { get; set; }
        
        // Цвета
        public int BackgroundColor { get; set; }
        public int TextColor { get; set; }
        public int BorderColor { get; set; }
        public int ShadowColor { get; set; }
        
        // Размеры
        public int BorderRadius { get; set; }
        public int BorderWidth { get; set; }
        public int ShadowBlur { get; set; }
        public int ShadowOffsetX { get; set; }
        public int ShadowOffsetY { get; set; }
        
        // Настройки шрифта
        public int FontId { get; set; }
        public int FontSize { get; set; }
        public int FontSpacing { get; set; }
        public int LineHeight { get; set; }
        
        // Настройки выравнивания
        public int Align { get; set; }
        public int FlexAlign { get; set; }
        public int FlexDirection { get; set; }
        public int FlexWrap { get; set; }
        public int FlexGrow { get; set; }
        public int FlexShrink { get; set; }
        public int FlexBasis { get; set; }
        
        // Настройки границ
        public int BorderTopWidth { get; set; }
        public int BorderRightWidth { get; set; }
        public int BorderBottomWidth { get; set; }
        public int BorderLeftWidth { get; set; }
        
        public int BorderTopColor { get; set; }
        public int BorderRightColor { get; set; }
        public int BorderBottomColor { get; set; }
        public int BorderLeftColor { get; set; }
        
        // Настройки градиента
        public int GradientStartColor { get; set; }
        public int GradientEndColor { get; set; }
        public int GradientDirection { get; set; }
        
        // Настройки opacity
        public int Opacity { get; set; }
        
        // Настройки анимации
        public bool HasAnimation { get; set; }
        public int AnimationDuration { get; set; }
        public int AnimationDelay { get; set; }
        public int AnimationIterationCount { get; set; }
        
        // Типы стилей
        public StyleType StyleType { get; set; }
        public ElementType TargetElementType { get; set; }
        
        // Метаданные
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        
        // Статус
        public bool IsModified { get; set; }
        public bool IsSaved { get; set; }
        public string ErrorMessage { get; set; }
        
        // Конструктор
        public LvglStyle()
        {
            Id = Guid.NewGuid().ToString("N");
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsSaved = true;
        }
        
        // Конструктор с параметрами
        public LvglStyle(string name, ElementType targetElementType, int fontSize = 16, int backgroundColor = 0xFFFFFFFF)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name ?? string.Empty;
            TargetElementType = targetElementType;
            FontSize = fontSize;
            BackgroundColor = backgroundColor;
            
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            IsSaved = true;
        }
        
        // Клонирование стиля
        public LvglStyle Clone()
        {
            var clone = new LvglStyle
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name,
                Type = Type,
                ParentStyleId = ParentStyleId,
                PaddingLeft = PaddingLeft,
                PaddingRight = PaddingRight,
                PaddingTop = PaddingTop,
                PaddingBottom = PaddingBottom,
                MarginLeft = MarginLeft,
                MarginRight = MarginRight,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                BackgroundColor = BackgroundColor,
                TextColor = TextColor,
                BorderColor = BorderColor,
                ShadowColor = ShadowColor,
                BorderRadius = BorderRadius,
                BorderWidth = BorderWidth,
                ShadowBlur = ShadowBlur,
                ShadowOffsetX = ShadowOffsetX,
                ShadowOffsetY = ShadowOffsetY,
                FontId = FontId,
                FontSize = FontSize,
                FontSpacing = FontSpacing,
                LineHeight = LineHeight,
                Align = Align,
                FlexAlign = FlexAlign,
                FlexDirection = FlexDirection,
                FlexWrap = FlexWrap,
                FlexGrow = FlexGrow,
                FlexShrink = FlexShrink,
                FlexBasis = FlexBasis,
                BorderTopWidth = BorderTopWidth,
                BorderRightWidth = BorderRightWidth,
                BorderBottomWidth = BorderBottomWidth,
                BorderLeftWidth = BorderLeftWidth,
                BorderTopColor = BorderTopColor,
                BorderRightColor = BorderRightColor,
                BorderBottomColor = BorderBottomColor,
                BorderLeftColor = BorderLeftColor,
                GradientStartColor = GradientStartColor,
                GradientEndColor = GradientEndColor,
                GradientDirection = GradientDirection,
                Opacity = Opacity,
                HasAnimation = HasAnimation,
                AnimationDuration = AnimationDuration,
                AnimationDelay = AnimationDelay,
                AnimationIterationCount = AnimationIterationCount,
                StyleType = StyleType,
                TargetElementType = TargetElementType,
                Author = Author,
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                IsModified = true,
                IsSaved = false
            };
            
            return clone;
        }
        
        // Сохранение стиля
        public void Save()
        {
            IsModified = false;
            IsSaved = true;
            LastModified = DateTime.Now;
        }
        
        // Применение стиля к элементу
        public void ApplyToElement(LvglElement element)
        {
            if (element.Style != null)
            {
                element.Style.ParentStyleId = Id;
                element.Style.ParentStyle = this;
            }
            else
            {
                element.Style = this;
            }
        }
        
        // Получение значения цвета в формате HEX
        public string GetColorHex(int color)
        {
            return color.ToString("X8");
        }
        
        // Получение значения цвета в формате RGB
        public string GetColorRGB(int color)
        {
            int r = (color >> 24) & 0xFF;
            int g = (color >> 16) & 0xFF;
            int b = (color >> 8) & 0xFF;
            return $"rgb({r}, {g}, {b})";
        }
        
        // Валидация стиля
        public void Validate()
        {
            IsModified = false;
            ErrorMessage = null;
            
            if (string.IsNullOrEmpty(Name))
            {
                IsModified = false;
                ErrorMessage = "Имя стиля не задано";
                return;
            }
            
            if (FontSize < 8 || FontSize > 128)
            {
                IsModified = false;
                ErrorMessage = "Размер шрифта должен быть от 8 до 128";
                return;
            }
            
            if (BorderRadius < 0)
            {
                IsModified = false;
                ErrorMessage = "Радиус скругления не может быть отрицательным";
                return;
            }
        }
        
        // Установить цвет элемента
        public void SetElementColor(LvglElement element, int color)
        {
            BackgroundColor = color;
            IsModified = true;
        }
        
        // Установить размер шрифта
        public void SetFontSize(LvglElement element, int fontSize)
        {
            FontSize = fontSize;
            IsModified = true;
        }
        
        // Получить все стили в иерархии
        public List<LvglStyle> GetAllStyles()
        {
            var styles = new List<LvglStyle> { this };
            if (ParentStyle != null)
            {
                styles.AddRange(ParentStyle.GetAllStyles());
            }
            return styles;
        }
        
        // Форматирование отступов
        public string FormatPadding()
        {
            return $"Padding: {PaddingLeft} {PaddingRight} {PaddingTop} {PaddingBottom}";
        }
        
        // Форматирование границ
        public string FormatBorder()
        {
            return $"Border: {BorderWidth}px, {BorderColor:X8}";
        }
    }
    
    public enum StyleType
    {
        Global,           // Глобальный стиль
        Element,          // Стиль элемента
        Container,        // Стиль контейнера
        Widget,           // Стиль виджета
        Theme,            // Стиль темы
        Preset,           // Стиль пресета
        Custom            // Пользовательский стиль
    }
}