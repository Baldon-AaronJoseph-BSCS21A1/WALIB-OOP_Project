using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
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
            lblLanguage.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLanguage.Location = new Point((ClientSize.Width - lblLanguage.Width) / 2, textBox1.Location.Y - lblLanguage.Height - 10);
        }

        // Tokenize the input statement
        private List<string> Tokenize(string statement)
        {
            List<string> tokens = new List<string>();

            // Check if the statement is wrapped in quotes. If it is, treat it as a single string.
            if (statement.StartsWith("\"") && statement.EndsWith("\""))
            {
                tokens.Add(statement); // Treat everything within quotes as a single string.
                return tokens;
            }

            // Improved regex pattern to match words, floating-point numbers, integers, and operators
            string pattern = @"\b(?:False|True|None|and|or|not|if|else|elif|return|class|def|\d+\.\d+|\d+|[+\-*/%=\(\)\[\]{}<>!&|,.;]+|\w+)\b";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(statement);

            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }
            return tokens;
        }

        // Analyze the tokenized statement
        private string AnalyzeStatement(string statement)
        {
            List<string> tokens = Tokenize(statement); // Tokenize the input statement
            StringBuilder result = new StringBuilder();

            // Check if the statement is a conditional statement
            if ((tokens.Contains("if") || tokens.Contains("elif") || tokens.Contains("else")) &&
                (statement.Contains(">") || statement.Contains("<") || statement.Contains("==") || statement.Contains("!=") || statement.Contains(">=") || statement.Contains("<=")))
            {
                result.AppendLine($"{statement} is a Conditional Statement");
                return result.ToString();
            }

            foreach (var token in tokens)
            {
                // Check if the token is wrapped in quotes, treating it as a string
                if (token.StartsWith("\"") && token.EndsWith("\""))
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
                // Check if it's a floating-point number
                else if (double.TryParse(token, out double number) && token.Contains("."))
                {
                    result.AppendLine($"{token} is a floating number");
                }
                // Check if it's an integer
                else if (int.TryParse(token, out _))
                {
                    result.AppendLine($"{token} is an integer");
                }
                // Check if it's a built-in function (like "print")
                else if (Array.Exists(builtInFunctions, func => func == token))
                {
                    result.AppendLine($"{token} is a built-in function");
                }
                // Check if it's a data type
                else if (Array.Exists(dataTypes, dataType => dataType == token))
                {
                    result.AppendLine($"{token} is a data type");
                }
                // Otherwise, treat it as a variable
                else
                {
                    result.AppendLine($"{token} is a variable");
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
            FinalResult.Location = new Point((ClientSize.Width - FinalResult.Width) / 2, label2.Location.Y + label2.Height + 10);
        }

        // Back button click event handler
        private void AnalyzeBack_Click(object sender, EventArgs e)
        {
            LanguageSelect languageSelectPage = new LanguageSelect();
            languageSelectPage.Show();
            this.Hide();
        }

        // Built-in functions
        private readonly string[] builtInFunctions = new[] { "print", "len", "type", "range", "input" };

        // Python Keywords
        private readonly string[] pythonKeywords = new[] { "False", "True", "None", "and", "or", "not", "if", "else", "elif", "return", "class", "def" };

        // Operators
        private readonly string[] operators = new[] { "+", "-", "*", "/", "%", "==", "<", ">", "<=", ">=", "&&", "||", "!", "&", "|" };

        // Data Types
        private readonly string[] dataTypes = new[] { "bool", "float", "int", "str", "list", "dict", "set", "tuple" };
    }
}






