using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Frontend.Components;

/// <summary>
/// Provides a numeric input component that updates its bound value immediately as the user types, rather than only on
/// change or blur events.
/// </summary>
/// <remarks>Unlike the standard InputNumber component, ImmediateInputNumber updates the bound value on every
/// input event, enabling real-time validation and feedback as the user enters data. This component is useful in
/// scenarios where immediate response to user input is required, such as live calculations or instant
/// validation.</remarks>
/// <typeparam name="TValue">The type of value to be edited, typically a numeric type such as int, long, float, double, or decimal.</typeparam>
public class ImmediateInputNumber<TValue> : InputNumber<TValue>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "number");
        builder.AddAttribute(3, "class", CssClass);
        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<string?>(
            this, value => CurrentValueAsString = value, CurrentValueAsString));
        builder.CloseElement();
    }
}
