﻿using System;
using System.Collections.Generic;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Password11.ColorLib;
using Password11.Datatypes;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.ViewModel;

/// <summary>
///     Takes input account, clones it, and allows to safely do changes
///     call Finish() function to get return
///     make sure object is not used after calling Finish, otherwise undefined behaviour can happen
/// </summary>
public class AccountEditor : PropertyChangable
{
    public static readonly DependencyProperty BaseColorBrushProperty =
        DependencyProperty.Register(nameof(BaseColorBrush), typeof(object), typeof(AccountEditor),
            new PropertyMetadata(default));

    private readonly IEnumerable<Tag> tagsList;

    private readonly Account target;

    private AccountEditor(Account target, IEnumerable<Tag> tagsList)
    {
        this.target = target.CloneRef();
        this.tagsList = tagsList;
    }

    public string TargetApp
    {
        get => target.TargetApp;
        set
        {
            target.TargetApp = value;
            onPropertyChanged();
        }
    }

    public string DisplayName
    {
        get => target.DisplayName;
        set
        {
            target.DisplayName = value;
            onPropertyChanged();
        }
    }

    public string Username
    {
        get => target.Username;
        set
        {
            target.Username = value;
            onPropertyChanged();
        }
    }

    public string Email
    {
        get => target.Email;
        set
        {
            target.Email = value;
            onPropertyChanged();
        }
    }

    public string Password
    {
        get => target.Password;
        set
        {
            target.Password = value;
            onPropertyChanged();
        }
    }

    public Dictionary<string, FieldData> Fields
    {
        get => target.Fields;
        set => target.Fields = value;
    }

    public ColorsScheme Colors
    {
        get => target.Colors;
        set
        {
            target.Colors = value;
            onPropertyChanged();
            onPropertyChanged(nameof(BaseColorBindable));
            onPropertyChanged(nameof(BaseColorBrush));
        }
    }

    public List<UniqueId<Tag>> Tags
    {
        get => target.Tags;
        set
        {
            target.Tags = value;
            onPropertyChanged();
        }
    }

    public Color BaseColorBindable
    {
        get => target.BaseColorBindable;
        set
        {
            target.BaseColorBindable = value;
            onPropertyChanged();
        }
    }

    public Brush BaseColorBrush => Colors.BaseColor.AsBrush;
    public Brush HoverColorBrush => Colors.HoverColor.AsBrush;
    public Brush SymbolColoBrush => Colors.SymbolColor.AsBrush;

    public string AppLink
    {
        get => target.AppLink;
        set
        {
            target.AppLink = value;
            onPropertyChanged();
        }
    }

    public UniqueId<Account> Identifier => target.Identifier;

    public static AccountEditor Start(IEnumerable<Tag> tagsList)
    {
        return Start(new AccountImpl(), tagsList);
    }

    public static AccountEditor Start(Account account, IEnumerable<Tag> tagsList)
    {
        return new AccountEditor(account, tagsList);
    }


    public event Action<Account?> OnFinished;

    public void Finish(bool successful)
    {
        OnFinished.Invoke(successful ? target : null);
    }

    public Account CloneRef()
    {
        return target.CloneRef();
    }

    public void Restore(Account state)
    {
        target.Restore(state);
    }
}