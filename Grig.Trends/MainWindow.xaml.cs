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
                Dispatcher.Invoke(() => {
                    countButton.Content = "Loading...";
                    countButton.IsEnabled = false;
                });
                
                if (string.IsNullOrEmpty(xpathTextBox.Text))
                    throw new ArgumentNullException("XPath");
                if (string.IsNullOrEmpty(uriTextBox.Text))
                    throw new ArgumentNullException("URI");
                if (string.IsNullOrEmpty(distanceTextBox.Text))
                    throw new ArgumentNullException("Distance");
                double distance = 0;
                if (!double.TryParse(distanceTextBox.Text, out distance))
                    throw new ArgumentOutOfRangeException("Distance", "can be number only.");
                if (distance > 1 || distance < 0)
                    throw new ArgumentOutOfRangeException("Distance", "can be from 0 to 1");

                int minChars = 0;
                if (!int.TryParse(minCharsTextBox.Text, out minChars))
                    throw new ArgumentOutOfRangeException("Minimal word length", "can be integer only.");
                if (minChars < 1)
                    throw new ArgumentOutOfRangeException("Distance", "can't be more than 1.");

                FuzzySearcher fuzzySearcher = new FuzzySearcher();
                PageService pageDownloader = new PageService();

                HtmlDocument page =
                    pageDownloader.Download(uriTextBox.Text);

                IEnumerable<FuzzyGroup> rankedWords = null;

                HtmlNodeCollection nodes = page.DocumentNode.SelectNodes(xpathTextBox.Text);
                if (nodes != null)
                {
                    var headersText = string.Join(" ", page.DocumentNode.SelectNodes(xpathTextBox.Text).Select(n => n.InnerText));
                    rankedWords = fuzzySearcher.RankWords(headersText, distance).Where(fg => fg.Forms.Any(f => f.Length >= minChars)).OrderByDescending(fg => fg.Count);
                }
                else
                    MessageBox.Show("Nothing was found by this XPath.");
                resultDataGrid.ItemsSource = rankedWords;
            }
            catch (ArgumentNullException anex)
            {
                MessageBox.Show(anex.ParamName + " is empty.");
            }
            catch (ArgumentOutOfRangeException aor)
            {
                MessageBox.Show(aor.ParamName + (!string.IsNullOrEmpty(aor.Message) ? aor.Message : "is invalid."));
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    countButton.Content = "Count";
                    countButton.IsEnabled = true;
                });
            }
        }
    }
}
