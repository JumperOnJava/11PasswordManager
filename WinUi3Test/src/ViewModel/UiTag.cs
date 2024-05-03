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
    public class UiTag : PropertyChangable, Tag
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

        public Tag Tag { get; }
        public string DisplayName
        {
            get => Tag.DisplayName; set
            {
                if (Tag.DisplayName != value)
                {
                    TextChanged?.Invoke(value);
                }
                Tag.DisplayName = value;
                onPropertyChanged("DisplayName");
            }
        }

        public long Identifier => Tag.Identifier;
        public UiTag Self { get => this; }
        public ColorsScheme TagColors
        {
            get => Tag.TagColors; set
            {
                Tag.TagColors = value;
                onPropertyChanged("TagColor");
                onPropertyChanged("baseColor");
                onPropertyChanged("hoverColor");
                onPropertyChanged("symbolColor");
            }
        }
        public Color baseColor => this.TagColors.baseColor.asWinColor;
        public SolidColorBrush baseColorBrush => new SolidColorBrush(this.TagColors.baseColor.asWinColor);
        public Color hoverColor => this.TagColors.hoverColor.asWinColor;
        public Color symbolColor => this.TagColors.symbolColor.asWinColor;

        public UiTag(Tag tag)
        {
            this.Tag = tag;
        }

        public bool matches(Taggable tag)
        {
            return Tag.matches(tag);
        }
    }
}
