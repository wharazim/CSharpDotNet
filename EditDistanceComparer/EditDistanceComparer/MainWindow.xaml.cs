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

namespace EditDistanceComparerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string MatchPatternInput { get; set; } = "Pattern to match.";

        public string MismtachInput { get; set; } = "Enter a mismatch threshold.";

        public List<string> ListSource { get; set; }

        public List<string> ListMatches { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            ListSource = new List<string>
            {
                "Johm Smith",
                "John Smith",
                "John  A Smith",
                "John Smtih",
                "Debby Downer",
                "Sally Sadsack",
                "Smitty Smith",
                "Billy Bonehead Jr.",
                "Mr. Billy A Bonehead Jr",
                "B b. Bonehead, the 3rd",
                "Smith, Joe",
                "",
            };
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(MismtachInput, out var mt);

            try
            {
                ListMatches = GetPossibleMatches(MatchPatternInput, ListSource, mt);
                Messages.Items.Add($"{ListMatches.Count()} items matched.");

                Matches.ItemsSource = ListMatches;
            }
            catch (Exception ex)
            {
                Messages.Items.Add(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private List<string> GetPossibleMatches(string target, IEnumerable<string> sources, double mt) =>
            sources.Where(c => new EditDistanceComparer(target, mt).Matches(c)).ToList();
    }
}
