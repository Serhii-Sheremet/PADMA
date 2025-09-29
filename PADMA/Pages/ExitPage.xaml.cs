
namespace PADMA.Pages;

public partial class ExitPage : ContentPage
{
	public ExitPage()
	{
		InitializeComponent();
        Application.Current.Quit();
    }
}