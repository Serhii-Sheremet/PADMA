namespace PADMA.Pages;

public partial class NodesPage : ContentPage
{
	public NodesPage()
	{
		InitializeComponent();
	}

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}