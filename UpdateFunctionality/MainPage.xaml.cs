using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace UpdateFunctionality
{
    public partial class MainPage : ContentPage
    {
        private string _email = string.Empty;
        private string _password = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoginCommand = new Command(async () => await ExecuteLoginAsync(), CanExecuteLogin);
        }

        #region Properties

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        public ICommand LoginCommand { get; private set; }

        #endregion

        #region Event Handlers

        private async void OnGoogleSignInClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Info", "Google Sign-In would be implemented here using Google authentication SDK.", "OK");

            GoogleSignInButton.IsEnabled = true;
            GoogleSignInButton.Text = "Sign in with Google";
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await ExecuteLoginAsync();
        }

        private void OnPasswordChanged(object sender, TextChangedEventArgs e)
        {
            Password = e.NewTextValue;
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
        }

        private async void OnCreateAccountTapped(object sender, EventArgs e)
        {
        }

        #endregion

        #region Login Logic

        private async Task ExecuteLoginAsync()
        {
            if (!CanExecuteLogin())
                return;

            try
            {
                
                LoginButton.IsEnabled = false;
                LoginButton.Text = "Logging in...";

                
                string email = EmailEntry.Text?.Trim();
                string password = PasswordEntry.Text;
                bool rememberMe = RememberMeCheckBox.IsChecked;

                if (!ValidateInputs(email, password))
                    return;

                bool loginSuccess = await AuthenticateUserAsync(email, password);

                if (loginSuccess)
                {
                    
                    await DisplayAlert("Success", "Login successful!", "OK");

                    // Navigate to main page
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
                else
                {
                    await DisplayAlert("Error", "Invalid email or password. Please try again.", "OK");
                    PasswordEntry.Text = string.Empty; // Clear password on failed login
                    PasswordEntry.Focus();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
            finally
            {
                // Restore button state
                LoginButton.IsEnabled = true;
                LoginButton.Text = "Login";
            }
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(EmailEntry?.Text) &&
                   !string.IsNullOrWhiteSpace(PasswordEntry?.Text);
        }

        private bool ValidateInputs(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                DisplayAlert("Validation Error", "Please enter your email address.", "OK");
                EmailEntry.Focus();
                return false;
            }

            if (!IsValidEmail(email))
            {
                DisplayAlert("Validation Error", "Please enter a valid email address.", "OK");
                EmailEntry.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                DisplayAlert("Validation Error", "Please enter your password.", "OK");
                PasswordEntry.Focus();
                return false;
            }

            return true;
        }

        private async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            await Task.Delay(2000);

            // Demo credentials
            if (email.Equals("admin@admin.com", StringComparison.OrdinalIgnoreCase) &&
                password == "admin")
            {
                return true;
            }

            return false; 
        }

        #endregion

        #region Helper Methods

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        

        private async Task SaveCredentialsAsync(string email)
        {
            try
            {
                await SecureStorage.SetAsync("saved_email", email);
                await SecureStorage.SetAsync("remember_user", "true");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving credentials: {ex.Message}");
            }
        }

        private async Task ClearSavedCredentialsAsync()
        {
            try
            {
                SecureStorage.Remove("saved_email");
                SecureStorage.Remove("remember_user");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing credentials: {ex.Message}");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
