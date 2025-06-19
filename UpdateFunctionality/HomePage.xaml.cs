namespace UpdateFunctionality;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		UpdateLabel.Text = AppInfo.VersionString;
    }
}