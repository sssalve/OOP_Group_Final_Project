namespace OOP_Group_Final_Project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "OOP_Group_Final_Project" };
        }
    }
}
