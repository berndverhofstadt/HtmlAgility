using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using HtmlAgilityPack;

/* Bernd Verhofstadt
 * Artesis Plantijn Hogeschool Antwerpen
 * PBA-EA
 */


namespace Tweakers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    /* 
     * TODO
     * Delete from html: id="menu"
     * Delete from html: class = "tnetUrl"
     * Delete from html: id="footer"
     */

    public partial class MainWindow
    {
        private int _l;
        private readonly string[] _linkPage = new string[100]; 
        private readonly string[] _titlePage = new string[100];
        readonly DispatcherTimer _progressWebsite;
        private double _inProcess;
        
        
        public MainWindow()
        {
            InitializeComponent();
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://tweakers.mobi/"));
            DateTime time = DateTime.Now;
            DatumToday.Content = time.ToString("D");
            _progressWebsite = new DispatcherTimer(DispatcherPriority.Render);
            InitializeTimer();
            _inProcess = 0;
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) return;
            var scrape = new HtmlDocument();
            scrape.LoadHtml(e.Result);
            try
            {
                    
                HtmlNode docnode = scrape.DocumentNode;
                HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                foreach (HtmlNode t in divnodes)
                {
                    if (t.Attributes["class"] != null)
                    {
                        if (t.Attributes["class"].Value == "content")
                        {
                            HtmlNode[] anodes = t.Descendants("a").ToArray();
                            foreach (var htmlNode in anodes)
                            {
                                _linkPage[_l] = Convert.ToString(htmlNode.Attributes["href"].Value);
                                _titlePage[_l] = htmlNode.InnerText;

                                if (_titlePage[_l].Contains("Nieuwsberichten van") || _titlePage[_l].Contains("nieuwsberichten van"))
                                {
                                    listviewresult.Items.Add(new Run(_titlePage[_l]));
                                }
                                else
                                {
                                    listviewresult.Items.Add(_titlePage[_l]);
                                }
                                    
                                _l++;
                            }
                        }
                    }
                }

                try
                {
                    ProgressBar1.Value = ProgressBar1.Minimum;
                    _progressWebsite.Start();
                    BrowserPage.Navigate(_linkPage[0]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't load page...");
                    throw;
                }                    
            }
            catch (Exception)
            {
                MessageBox.Show("Controleer je netwerkverbindeng","Netwerkprobleem",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void InitializeTimer()
        {
            _progressWebsite.Tick +=ProgressWebsite_Tick;
            _progressWebsite.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _progressWebsite.IsEnabled = true;
        }

        private void ProgressWebsite_Tick(object sender, EventArgs e)
        {
            _inProcess += 5;
            ProgressBar1.SetPercent(_inProcess);
            //if (ProgressBar1.Value == ProgressBar1.Maximum)
            if (_inProcess == ProgressBar1.Maximum)
                _progressWebsite.Stop();
        }

        private void listviewresult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PageStart();
                int selectedIndex = listviewresult.SelectedIndex;
                //BrowserPage.Navigate("http://verhofstadt-creations.eu");
                BrowserPage.Navigate(_linkPage[selectedIndex]);
            }
            catch (Exception)
            {
                MessageBox.Show("Controleer je netwerkverbindeng", "Netwerkprobleem", MessageBoxButton.OK, MessageBoxImage.Warning);
                throw;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://ap.be");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PageStart();
                BrowserPage.GoBack();
            }
            catch (Exception) 
            {
                PageSucces();
            }
        }

        private void BrowserPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            PageSucces();
            
        }
        private void PageSucces()
        {
            _progressWebsite.Stop();
            ProgressBar1.SetPercent(100);
            ProgressBar1.Value = ProgressBar1.Minimum;
        }
        private void PageStart()
        {
            _inProcess = 0;
            ProgressBar1.SetPercent(0);
            ProgressBar1.Value = ProgressBar1.Minimum;
            _progressWebsite.Start();
        }
    }
}
