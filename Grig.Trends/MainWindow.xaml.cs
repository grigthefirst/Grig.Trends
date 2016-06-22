using Grig.Trends.Services;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grig.Trends
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(xpathTextBox.Text))
                    throw new ArgumentNullException("XPath");
                if (string.IsNullOrEmpty(uriTextBox.Text))
                    throw new ArgumentNullException("URI");
                if (string.IsNullOrEmpty(distanceTextBox.Text))
                    throw new ArgumentNullException("Растояние");
                double distance = 0;
                if (!double.TryParse(distanceTextBox.Text, out distance))
                    throw new ArgumentOutOfRangeException("Растояние", "может быть только числом");
                if (distance > 1 || distance < 0)
                    throw new ArgumentOutOfRangeException("Растояние", "может быть от 0 до 1");

                int minChars = 0;
                if (!int.TryParse(minCharsTextBox.Text, out minChars))
                    throw new ArgumentOutOfRangeException("Минимальная длина слова", "может быть только целым числом");
                if (minChars < 1)
                    throw new ArgumentOutOfRangeException("Растояние", "не может быть меньше 1");

                FuzzySearcher fuzzySearcher = new FuzzySearcher();
                PageService pageDownloader = new PageService();

                HtmlDocument page =
                    pageDownloader.Download(uriTextBox.Text);

                var headersText = string.Join(" ", page.DocumentNode.SelectNodes(xpathTextBox.Text).Select(n => n.InnerText));

                IEnumerable<FuzzyGroup> rankedWords = fuzzySearcher.RankWords(headersText, distance).Where(fg => fg.Forms.Any(f => f.Length >= minChars)).OrderByDescending(fg => fg.Count);

                resultDataGrid.ItemsSource = rankedWords;
            }
            catch (ArgumentNullException anex)
            {
                MessageBox.Show(anex.ParamName + " не заполнен.");
            }
            catch(ArgumentOutOfRangeException aor)
            {
                MessageBox.Show(aor.ParamName + (!string.IsNullOrEmpty(aor.Message) ? aor.Message : "имеет недопустимое значение."));
            }
        }
    }
}
