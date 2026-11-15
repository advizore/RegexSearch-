using System.Windows;
using RegexSearch.Core.Services;
using RegexSearch.Core.Models;

namespace RegexSearch.WPF
{
    public partial class MainWindow : Window
    {
        private readonly SearchService _searchService = new SearchService();

        public MainWindow()
        {
            InitializeComponent();

            // Пример текста для демонстрации
            InputTextBox.Text =
                "Примеры СНИЛС в тексте:\n" +
                "Корректные: 123-456-789 01 и 987-654-321 09\n" +
                "Ещё один: 111-222-333 44\n" +
                "Неправильные: 12-456-789 01, 123-456-78901, 123-456-789A 01\n" +
                "И ещё: 555-666-777 12";
        }

        private void BtnRegex_Click(object sender, RoutedEventArgs e)
        {
            var results = _searchService.SearchWithRegex(InputTextBox.Text);
            ResultsGrid.ItemsSource = results;
            StatusText.Text = $"Regex: найдено {results.Count} совпадений";
            StatusText.Foreground = System.Windows.Media.Brushes.DarkGreen;
        }

        private void BtnAutomaton_Click(object sender, RoutedEventArgs e)
        {
            var results = _searchService.SearchWithAutomaton(InputTextBox.Text);
            ResultsGrid.ItemsSource = results;
            StatusText.Text = $"Автомат: найдено {results.Count} совпадений";
            StatusText.Foreground = System.Windows.Media.Brushes.DarkBlue;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            ResultsGrid.ItemsSource = null;
            StatusText.Text = "Очищено. Готово к работе.";
            StatusText.Foreground = System.Windows.Media.Brushes.Gray;
        }
    }
}
