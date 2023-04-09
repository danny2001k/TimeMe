using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace TimeMe
{
	public partial class MainWindow : Window
	{
		private const string CsvFilePath = "words.csv";

		private List<string> commonWords;

		private Stopwatch timer = new Stopwatch();
		private string currentWord;
		private DispatcherTimer displayTimer;

		public MainWindow()
		{
			InitializeComponent();
			LoadWordsFromFile();
			newWordButton.Click += NewWordButton_Click;
			retryButton.Click += RetryButton_Click;
			inputTextBox.PreviewKeyDown += InputTextBox_PreviewKeyDown;
			inputTextBox.TextChanged += InputTextBox_TextChanged;
			saveWordsButton.Click += SaveWordsButton_Click;
			displayTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(10)
			};
			displayTimer.Tick += DisplayTimer_Tick;
			currentWord = GetRandomWord();
			StartTyping();
		}

		private void LoadWordsFromFile()
		{
			if (File.Exists(CsvFilePath))
			{
				string csvText = File.ReadAllText(CsvFilePath);
				commonWords = csvText.Split(',').Select(word => word.Trim()).ToList();
			}
			else
			{
				commonWords = new List<string>
				{
					"Salut", "Pisica", "A", "Dumnezeu", "Mihaela", "Ce", "Repede", "Strain", "Laringe", "Da", "Nu"
				};
				SaveWordsToFile();
			}
		}

		private void SaveWordsToFile()
		{
			string csvText = string.Join(",", commonWords);
			File.WriteAllText(CsvFilePath, csvText);
		}

		private void SaveWordsButton_Click(object sender, RoutedEventArgs e)
		{
			commonWords = wordListTextBox.Text.Split(',').Select(word => word.Trim()).ToList();
			SaveWordsToFile();
		}

		private void NewWordButton_Click(object sender, RoutedEventArgs e)
		{
			currentWord = GetRandomWord();
			StartTyping();
		}

		private void RetryButton_Click(object sender, RoutedEventArgs e)
		{
			StartTyping();
		}

		private void StartTyping()
		{
			wordLabel.Content = $"Type the following word: {currentWord}";
			inputTextBox.Text = "";
			inputTextBox.Focus();
			timer.Reset();
			displayTimer.Start();
			resultLabel.Content = "";
			timerLabel.Content = $"Time: 0.00 seconds";
		}

		private void InputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!timer.IsRunning && (e.Key == Key.R || e.Key == Key.N))
			{
				e.Handled = true;
				if (e.Key == Key.R)
				{
					StartTyping();
				}
				else if (e.Key == Key.N)
				{
					currentWord = GetRandomWord();
					StartTyping();
				}
			}
			else if (!timer.IsRunning && e.Key != Key.R && e.Key != Key.N)
			{
				timer.Start();
			}
		}

		private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int correctChars = 0;
			for (int i = 0; i < inputTextBox.Text.Length && i < currentWord.Length; i++)
			{
				if (inputTextBox.Text[i] == currentWord[i])
				{
					correctChars++;
				}
				else
				{
					break;
				}
			}

			inputTextBox.Foreground = correctChars == inputTextBox.Text.Length ? Brushes.Green : Brushes.Red;

			if (inputTextBox.Text == currentWord)
			{
				timer.Stop();
				displayTimer.Stop();
				resultLabel.Content = $"You typed the word correctly in {timer.Elapsed.TotalSeconds:0.00} seconds.";
				timerLabel.Content = "";
			}
		}

		private void DisplayTimer_Tick(object sender, EventArgs e)
		{
			if (timer.IsRunning)
			{
				timerLabel.Content = $"Time: {timer.Elapsed.TotalSeconds:0.00} seconds";
			}
		}

		private string GetRandomWord()
		{
			Random random = new Random();
			int index = random.Next(commonWords.Count);
			return commonWords[index];
		}
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				if (configurationTab.IsSelected)
				{
					wordListTextBox.Text = string.Join(", ", commonWords);
				}
			}
		}
	}
}

