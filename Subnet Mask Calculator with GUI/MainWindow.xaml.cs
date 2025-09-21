using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Subnet_Mask_Calculator_with_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Clear_All();
        }

        private void Clear_All()
        {
            txtIpAddresss.Clear();
            txtPrefixLength.Clear();
            lblMask.Content = string.Empty;
            lblNetwork.Content = string.Empty;
            lblFirstUsable.Content = string.Empty;
            lblLastUsable.Content= string.Empty;
            lblBroadcast.Content = string.Empty;
            lblHosts.Content = string.Empty;
            txtIpAddresss.Focus();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_All();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            int prefixLength;
            if(int.TryParse(txtPrefixLength.Text, out prefixLength))
                SplitIpAddress(txtIpAddresss.Text, prefixLength);
            else
            {
                MessageBox.Show("Invalid input. Subnet prefix length must be 0-32.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear_All();
            }
        }
    
    
        private byte ip1 { get; set; }   
        private byte ip2 { get; set; }       
        private byte ip3 { get; set; }   
        public byte ip4 { get; set; }
        private int prefixLength { get; set; }
        

        private void SplitIpAddress(string ipAddress, int prefixLength)
        {
            var segments = ipAddress.Split('.');

            if (segments.Length != 4)
            {
                MessageBox.Show("Invalid IP Address format. Enter 4 numbers (0-255) separated with a dot.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear_All();
                return;
            }
            else
            {
                ip1 = byte.Parse(segments[0]);
                ip2 = byte.Parse(segments[1]);
                ip3 = byte.Parse(segments[2]);
                ip4 = byte.Parse(segments[3]);
                if (prefixLength < 0 || prefixLength > 32)
                {
                    MessageBox.Show("Invalid input. Subnet prefix length must be 0-32.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    Clear_All();
                    return;
                }
                else
                {
                    this.prefixLength = prefixLength;
                    DisplayResults();
                }
            }
        }

        private void DisplayResults()
        {
            lblMask.Content = CalculateSubnetMask(prefixLength);
            lblNetwork.Content = NetworkAddress(prefixLength);
            lblFirstUsable.Content = FirstUsableAddress(prefixLength);
            lblLastUsable.Content = LastUsableAddress(prefixLength);
            lblBroadcast.Content = BroadcastAddress(prefixLength);
            lblHosts.Content = TotalHosts(prefixLength);
        }

        private string CalculateSubnetMask(int prefixLength)
        {
            uint mask = uint.MaxValue << (32 - prefixLength); 
                // MaxValue equals 4 294 967 295 in decimal, it's 32 bits of 1s.
                // (32 - prefixLength) equals the number of 0s to add at the end,
                // so shifting left by this amount creates the subnet mask.
            return string.Join(".", 
                (mask >> 24) & 0xFF,
                // Shifts mask right 24 bits.
                // 0xFF equals 255 in decimal, it's 8 bits of 1s.
                // & is bitwise AND, it will turn all 24 leading bits of mask to zero
                // because the mask's total size is 32 bits.
                (mask >> 16) & 0xFF,
                // Shifts mask right 16 bits and turn all 24 leading bits of the mask to zero.
                (mask >> 8) & 0xFF,
                // Shifts mask right 8 bits and turn all 24 leading bits of the mask to zero.
                mask & 0xFF);
                // Just turn all 24 leading bits of mask to zero.
        }
        
        private string NetworkAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4); 
                // Combine the four 8 bit bytes into a single 32 bit uint value by shifting and ORing.
                // Shifting will preserve the leading bits of the 8 bit bytes
                // because of the explicit conversion to a 32 bit uint.
                // Bitwise OR will always return 1 if either of the combined values are 1,
                // so all of the bits of the combined bytes are preserved in the final product.
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
                // Turn the trailing bits of the ip number to zero
                // by combining with the mask by using bitwise AND.
            return string.Join(".",
                (network >> 24) & 0xFF,
                (network >> 16) & 0xFF,
                (network >> 8) & 0xFF,
                network & 0xFF);
        }
        private string FirstUsableAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint firstUsable = network + 1;
                // Get the first usable ip address by adding 1 to the network address.
            return string.Join(".",
                (firstUsable >> 24) & 0xFF,
                (firstUsable >> 16) & 0xFF,
                (firstUsable >> 8) & 0xFF,
                firstUsable & 0xFF);
        }
        private string LastUsableAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint broadcast = network | ~mask; 
                // ~ is bitwise NOT, it creates the bitwise opposite of the mask value.
                // Combining the opposite of the mask with the network address
                // will produce it's maximum value, because it will turn all of
                // the trailing bits of the network address to 1.
                // The result equals the broadcast address.
            uint lastUsable = broadcast - 1;
                // Get the last usable ip address by subtracting 1 from the broadcast address.
            return string.Join(".",
                (lastUsable >> 24) & 0xFF,
                (lastUsable >> 16) & 0xFF,
                (lastUsable >> 8) & 0xFF,
                lastUsable & 0xFF);
        }
        private string BroadcastAddress(int prefixLength)
        {
            uint ip = (uint)(ip1 << 24 | ip2 << 16 | ip3 << 8 | ip4);
            uint mask = uint.MaxValue << (32 - prefixLength);
            uint network = ip & mask;
            uint broadcast = network | ~mask;
            return string.Join(".",
                (broadcast >> 24) & 0xFF,
                (broadcast >> 16) & 0xFF,
                (broadcast >> 8) & 0xFF,
                broadcast & 0xFF);
        }
        private int TotalHosts(int prefixLength)
        {
            return (int)(Math.Pow(2, 32 - prefixLength) - 2);
                // Total usable hosts equals 2^(32 - prefixLength) - 2.
                // The subtraction of 2 accounts for the network and broadcast addresses,
                // which cannot be assigned to hosts.
        }
    }
}