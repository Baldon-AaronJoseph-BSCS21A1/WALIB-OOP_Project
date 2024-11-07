using System;
using System.Security.Policy;
using System.Windows.Forms;

namespace CodeSense
{
    public partial class HomePage : Form
    {
        public HomePage() // Make sure the class name matches HomePage, not Form1
        {
            InitializeComponent();
            StartButton.Click += new EventHandler(StartButton_Click); // Attach click event directly
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            // Create an instance of the LanguageSelect form
            LanguageSelect languageSelectForm = new LanguageSelect();
            // Show the LanguageSelect form
            languageSelectForm.Show();
            // Hide the current form (HomePage)
            this.Hide();
        }
    }
}
