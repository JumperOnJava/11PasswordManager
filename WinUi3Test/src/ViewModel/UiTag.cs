using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using Newtonsoft.Json;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.Datatypes.Serializing;

namespace WinUi3Test.src.ViewModel
{
    public class UiTag : PropertyChangable, Clonable<UiTag>
    {
        private bool selected;
        public event Action<bool> SelectedChanged;
        public event Action<string> TextChanged;
        public bool Selected
        {
            get => selected; set
            {
                if (selected != value)
                {
                    SelectedChanged?.Invoke(value);
                }
                selected = value;
                onPropertyChanged("Selected");
            }
        }

        public TagRef Target { get; set; }
        public string DisplayName
        {
            get => Target.DisplayName; set
            {
                if (Target.DisplayName != value)
                {
                    TextChanged?.Invoke(value);
                }
                Target.DisplayName = value;
                onPropertyChanged("DisplayName");
            }
        }

        public TagRef Identifier => Target.Identifier;
        public UiTag Self { get => this; }
        public ColorsScheme TagColors
        {
            get => Target.TagColors; set
            {
                Target.TagColors = value;
                onPropertyChanged("TagColor");
                onPropertyChanged("baseColor");
                onPropertyChanged("hoverColor");
                onPropertyChanged("symbolColor");
            }
        }
        
        [JsonIgnore]
        public Color baseColor => this.TagColors.BaseColor.asWinColor;
        [JsonIgnore]
        public SolidColorBrush baseColorBrush => new SolidColorBrush(this.TagColors.BaseColor.asWinColor);
        [JsonIgnore]
        public Color hoverColor => this.TagColors.HoverColor.asWinColor;
        [JsonIgnore]
        public Color symbolColor => this.TagColors.SymbolColor.asWinColor;

        [JsonIgnore]
        public Brush BaseColorBrush => Target.BaseColorBrush;

        [JsonIgnore]
        public Brush HoverColorBrush => Target.HoverColorBrush;

        [JsonIgnore]
        public Brush SymbolColorBrush => Target.SymbolColorBrush;

        public UiTag(TagRef target)
        {
            this.Target = target;
        }

        public bool matches(Taggable tag)
        {
            return Target.matches(tag);
        }

        public UiTag Clone()
        {
            return new UiTag(Target.Clone().Identifier);
        }

    }
}
