using System;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Password11.Datatypes;
using Password11.Datatypes.Serializing;
using Password11.src.Ui;
using Password11.src.Util;

namespace Password11.ViewModel
{
    public class UiTag : PropertyChangable, RefClonable<UiTag>, Identifiable<Tag>
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
                onPropertyChanged();
            }
        }

        public Tag Target { get; set; }
        public string DisplayName
        {
            get => Target.DisplayName; set
            {
                if (Target.DisplayName != value)
                {
                    TextChanged?.Invoke(value);
                }
                Target.DisplayName = value;
                onPropertyChanged();
            }
        }

        public UniqueId<Tag> Identifier => Target.Identifier;
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

        public UiTag(Tag target)
        {
            this.Target = target;
        }

        public bool matches(Taggable tag)
        {
            return Target.matches(tag);
        }

        public UiTag CloneRef()
        {
            return new UiTag(Target.CloneRef());
        }

        public void Restore(UiTag state)
        {
            this.Target = state.Target;
        }
    }
}
