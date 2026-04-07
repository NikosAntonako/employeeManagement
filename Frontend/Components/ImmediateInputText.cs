using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Frontend.Components;

/// <summary>
/// A Blazor input component that updates its bound value immediately as the user types, rather than only on change or
/// blur events.
/// </summary>
/// <remarks>Use this component when you need to respond to user input in real time, such as for live validation
/// or instant feedback scenarios. Unlike the standard InputText, ImmediateInputText triggers value updates on every
/// input event, which may result in more frequent model updates and event callbacks.</remarks>
public class ImmediateInputText : InputText
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "class", CssClass);
        builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(4, "oninput", EventCallback.Factory.CreateBinder<string?>(
            this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.CloseElement();
    }
}
