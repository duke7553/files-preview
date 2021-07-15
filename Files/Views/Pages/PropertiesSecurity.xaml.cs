using Files.DataModels.NavigationControlItems;
using Files.Filesystem;
using Files.ViewModels.Properties;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using static Files.Views.PropertiesSecurityAdvanced;

namespace Files.Views
{
    public sealed partial class PropertiesSecurity : PropertiesTab
    {
        public RelayCommand OpenAdvancedPropertiesCommand { get; set; }

        public SecurityProperties SecurityProperties { get; set; }

        private PropertiesSecurityAdvanced propsWindow;

        public PropertiesSecurity()
        {
            this.InitializeComponent();

            OpenAdvancedPropertiesCommand = new RelayCommand(() => OpenAdvancedProperties());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var np = e.Parameter as Views.Properties.PropertyNavParam;

            if (np.navParameter is ListedItem listedItem)
            {
                SecurityProperties = new SecurityProperties(listedItem, np.AppInstanceArgument);
            }
            else if (np.navParameter is DriveItem driveitem)
            {
                SecurityProperties = new SecurityProperties(driveitem, np.AppInstanceArgument);
            }

            base.OnNavigatedTo(e);
        }

        public async override Task<bool> SaveChangesAsync(ListedItem item)
        {
            if (SecurityProperties != null)
            {
                return await SecurityProperties.SetFilePermissions();
            }
            return true;
        }

        protected override void Properties_Loaded(object sender, RoutedEventArgs e)
        {
            base.Properties_Loaded(sender, e);

            if (SecurityProperties != null)
            {
                SecurityProperties.GetFilePermissions();
            }
        }

        public override void Dispose()
        {
            if (propsWindow != null)
            {
                propsWindow.Close();
            }
        }

        private void OpenAdvancedProperties()
        {
            if (SecurityProperties == null)
            {
                return;
            }

            if (propsWindow == null)
            {
                propsWindow = new PropertiesSecurityAdvanced();
                propsWindow.displayArgs = new PropertiesPageNavigationArguments()
                {
                    Item = SecurityProperties.Item,
                    AppInstanceArgument = AppInstance
                };
                propsWindow.ExtendsContentIntoTitleBar = true;
                propsWindow.Title = string.Format("SecurityAdvancedPermissionsTitle".GetLocalized(), SecurityProperties.Item.ItemName);

                propsWindow.Activate();

            }
        }
    }
}
