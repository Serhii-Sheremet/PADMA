namespace PADMA.Pages;

public partial class SunrisePage : ContentPage
{
	public SunrisePage()
	{
		InitializeComponent();
	}
	private async void OnCloseClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
    }
}