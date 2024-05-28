using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.Util;

public class AskDialogOperation : Operation
{
    public AskDialogOperation(Page page, string title, string primaryText, string secondaryText = null,
        string content = null)
    {
        new DialogBuilder(page)
            .Title(title)
            .Content(content)
            .PrimaryButtonText(primaryText)
            .SecondaryButtonText(secondaryText)
            .DefaultButton(ContentDialogButton.Primary)
            .AddPrimaryClickAction((_) => Finish(true))
            .AddSecondaryClickAction((_) => Finish(false))
            .Build()
            .ShowAsync();

    }
}