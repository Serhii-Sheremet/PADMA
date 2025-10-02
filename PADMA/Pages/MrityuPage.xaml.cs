namespace PADMA.Pages;

public partial class MrityuPage : ContentPage
{
	public MrityuPage()
	{
		InitializeComponent();
	}

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }


}