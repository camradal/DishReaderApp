using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace DishReaderApp
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        public DetailsPage()
        {
            InitializeComponent();
            GlobalLoading.Instance.IsLoading = true;
            webBrowser1.LoadCompleted += new LoadCompletedEventHandler(webBrowser1_LoadCompleted);
        }

        void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                DataContext = App.ViewModel.AllFeedItems[index];
            }
        }
    }
}