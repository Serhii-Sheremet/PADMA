namespace PADMA.Pages;

public partial class MuhurtasPage : ContentPage
{
	public MuhurtasPage()
	{
		InitializeComponent();
	}

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}