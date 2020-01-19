using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2P_Chatt
{
    class OwnException:Exception
    {
        public OwnException()
        {

        }
        public OwnException(string message) : base(message)
        {
            string messageBoxText = message;
            string caption = "Error";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }
        public OwnException(string message, Exception inner):base(message, inner)
        {
            string messageBoxText = message;
            string caption = inner.ToString();
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }
    }
}
