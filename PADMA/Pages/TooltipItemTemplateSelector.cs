using static PADMA.Pages.DayPage;

namespace PADMA.Pages;

public class TooltipItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? DividerTemplate { get; set; }
    public DataTemplate? InnerDividerTemplate { get; set; }
    public DataTemplate? SpacerTemplate { get; set; }
    public DataTemplate? RangeTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            TooltipRangeLine => RangeTemplate ?? TextTemplate!,
            TooltipInnerDivider => InnerDividerTemplate ?? DividerTemplate ?? TextTemplate!,
            TooltipDivider => DividerTemplate ?? TextTemplate!,
            TooltipSpacer => SpacerTemplate ?? TextTemplate!,
            _ => TextTemplate!
        };
    }





}
