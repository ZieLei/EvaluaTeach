namespace EvaluaTeach
{
    internal static class Program
    {
        private static NavigationContext? navigationContext;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            navigationContext = new NavigationContext(new LandingPage());
            Application.Run(navigationContext);
        }

        internal static void NavigateTo(Form nextForm)
        {
            navigationContext?.NavigateTo(nextForm);
        }

        private sealed class NavigationContext : ApplicationContext
        {
            private Form currentForm;

            public NavigationContext(Form startupForm)
            {
                currentForm = startupForm;
                MainForm = startupForm;
                startupForm.FormClosed += ActiveFormClosed;
                startupForm.Show();
            }

            public void NavigateTo(Form nextForm)
            {
                Form previousForm = currentForm;

                previousForm.FormClosed -= ActiveFormClosed;
                previousForm.Hide();

                currentForm = nextForm;
                MainForm = nextForm;
                nextForm.FormClosed += ActiveFormClosed;
                nextForm.Show();

                previousForm.Close();
                previousForm.Dispose();
            }

            private void ActiveFormClosed(object? sender, FormClosedEventArgs e)
            {
                ExitThread();
            }
        }
    }
}