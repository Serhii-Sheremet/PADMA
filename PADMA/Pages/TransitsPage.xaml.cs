namespace PADMA.Pages;

public partial class TransitsPage : ContentPage
{
	public TransitsPage()
	{
		InitializeComponent();
	}
	private async void OnCloseClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
    }
}