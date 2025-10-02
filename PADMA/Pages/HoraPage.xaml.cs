namespace PADMA.Pages;

public partial class HoraPage : ContentPage
{
	public HoraPage()
	{
		InitializeComponent();
	}

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}