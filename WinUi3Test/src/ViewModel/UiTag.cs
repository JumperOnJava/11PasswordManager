using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.ViewModel
{
    public class UiTag : PropertyChangable
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
        public Color baseColor => this.TagColors.BaseColor.asWinColor;
        public SolidColorBrush baseColorBrush => new SolidColorBrush(this.TagColors.BaseColor.asWinColor);
        public Color hoverColor => this.TagColors.HoverColor.asWinColor;
        public Color symbolColor => this.TagColors.SymbolColor.asWinColor;

        public Brush BaseColorBrush => Target.BaseColorBrush;

        public Brush HoverColorBrush => Target.HoverColorBrush;

        public Brush SymbolColorBrush => Target.SymbolColorBrush;

        public UiTag(TagRef target)
        {
            this.Target = target;
        }

        public bool matches(Taggable tag)
        {
            return Target.matches(tag);
        }

        public Tag Clone()
        {
            return Target.Clone();
        }
    }
}
