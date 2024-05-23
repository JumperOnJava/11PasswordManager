using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.Util;

public class AskDialogOperation : Operation
{
    public AskDialogOperation(XamlRoot xamlRoot, string title, string primaryText, string secondaryText = null, string content = null)
    {
        new DialogBuilder(xamlRoot)
            .Title(title)
            .Content(content)
            .PrimaryButtonText(primaryText)
            .SecondaryButtonText(secondaryText)
            .DefaultButton(ContentDialogButton.Primary)
            .AddPrimaryClickAction(() => Finish(true))
            .AddSecondaryClickAction(() => Finish(false))
            .Build().ShowAsync();
    }
}