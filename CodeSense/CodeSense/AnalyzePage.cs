using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CodeSense
{
    public partial class AnalyzePage : Form
    {
        private string selectedLanguage;

        public AnalyzePage(string language)
        {
            InitializeComponent();
            selectedLanguage = language;
            // Set the label text to show the selected language
            lblLanguage.Text = "Selected Language: " + selectedLanguage;
            // Set font and size for lblLanguage
            lblLanguage.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            // Center lblLanguage horizontally and place it above the text box
            lblLanguage.Location = new Point((ClientSize.Width - lblLanguage.Width) / 2, textBox1.Location.Y - lblLanguage.Height - 10);
        }

        // Tokenize the input statement
        private List<string> Tokenize(string statement)
        {
            List<string> tokens = new List<string>();

            // Check if the statement contains spaces and doesn't contain commas
            if (statement.Contains(" ") && !statement.Contains(","))
            {
                tokens.Add($"\"{statement}\""); // Treat everything with spaces as a single string
                return tokens;
            }

            // Split by commas to allow separate analysis
            string[] commaSeparated = statement.Split(',');

            foreach (var part in commaSeparated)
            {
                string trimmedPart = part.Trim();

                // Regex pattern to match words, operators, numbers (including float), and symbols
                string pattern = @"\b(?:False|True|None|and|or|not|if|else|elif|return|class|def|\w+|\d+\.\d+|\d+|[+\-*/%=\(\)\[\]{}<>!&|,.;]+)\b";
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(trimmedPart);

                foreach (Match match in matches)
                {
                    tokens.Add(match.Value);
                }
            }

            return tokens;
        }

        // Analyze the tokenized statement
        private string AnalyzeStatement(string statement)
        {
            List<string> tokens = Tokenize(statement); // Tokenize the input statement
            StringBuilder result = new StringBuilder();

            foreach (var token in tokens)
            {
                // If the token is a built-in function, classify it as a built-in function
                if (Array.Exists(builtInFunctions, func => func == token))
                {
                    result.AppendLine($"{token} is a built-in function");
                }
                // If the token is wrapped in quotes, treat it as a string literal
                else if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    result.AppendLine($"{token} is a string");
                }
                // Check if it's a Python keyword
                else if (Array.Exists(pythonKeywords, keyword => keyword == token))
                {
                    result.AppendLine($"{token} is a Python keyword");
                }
                // Check if it's an operator
                else if (Array.Exists(operators, op => op == token))
                {
                    result.AppendLine($"{token} is an operator");
                }
                // Check if it's a number (integer or floating-point)
                else if (double.TryParse(token, out _))
                {
                    result.AppendLine($"{token} is a number");
                }
                // Otherwise, treat it as an identifier
                else
                {
                    result.AppendLine($"{token} is an identifier");
                }
            }
            return result.ToString();
        }

        // Detect Button click event handler
        private void DetectButton_Click(object sender, EventArgs e)
        {
            string inputStatement = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(inputStatement))
            {
                FinalResult.Text = "Please enter a statement.";
                return;
            }

            string result = AnalyzeStatement(inputStatement);
            FinalResult.Text = result;

            // Center the FinalResult label regardless of the result length
            FinalResult.Location = new Point((ClientSize.Width - FinalResult.Width) / 2, label2.Location.Y + label2.Height + 10);
        }

        // Back button click event handler
        private void AnalyzeBack_Click(object sender, EventArgs e)
        {
            // Show the LanguageSelect form
            LanguageSelect languageSelectPage = new LanguageSelect();
            languageSelectPage.Show();
            // Hide the current AnalyzePage form
            this.Hide();
        }

        // Python Keywords
        private readonly string[] pythonKeywords = new[] { "False", "True", "None", "and", "or", "not", "if", "else", "elif", "return", "class", "def" };

        // Operators
        private readonly string[] operators = new[] { "+", "-", "*", "/", "%", "==", "<", ">", "<=", ">=", "&&", "||", "!", "&", "|" };

        // Built-in Functions
        private readonly string[] builtInFunctions = new[] { "print", "len", "str", "int", "input", "range" };
    }
}