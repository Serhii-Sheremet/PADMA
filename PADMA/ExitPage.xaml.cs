namespace PADMA;

public partial class ExitPage : ContentPage
{
	public ExitPage()
	{
		InitializeComponent();
        Application.Current.Quit();
    }
}