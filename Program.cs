namespace AppForm;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al iniciar la aplicación:\n" + ex.Message,
                            "Error crítico",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }    
}