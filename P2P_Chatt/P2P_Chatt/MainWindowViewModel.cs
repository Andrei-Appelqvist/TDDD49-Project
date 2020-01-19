using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace P2P_Chatt
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _friendport = 5000;
        private int _myport = 5000;
        private string _myusername = "Ghenghis";
        private string _friendip = "127.0.0.1";
        private string _msg = "S";
        MainTcp t = null;
        Messages msgs = new Messages();
        public event PropertyChangedEventHandler PropertyChanged;
        ConvHist his = new ConvHist();
        private string search_string = "Ghe";




        public ICommand ConnectCommand { get; set; }
        public ICommand ListenCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand CloseListen { get; set; }

        public ICommand SearchUser { get; set; }

        public ICommand sendbuzz { get; set; }

        public MainWindowViewModel()
        {
            ConnectCommand = new TestCommand(p=>StartClientThread(), p => true);
            ListenCommand = new TestCommand(p => StartListenThread(_myport), p => true);
            SendCommand = new TestCommand(p => SendMsg( _msg), p => true);
            CloseListen = new TestCommand(p => CloseTcp(), p => true);
            SearchUser = new TestCommand(p => SearchU(search_string), p => true);
            sendbuzz = new TestCommand(p => send_buzz(), p => true);
            

            t = new MainTcp(msgs);
            his.putDir();

            //msgs.AddMessage("asd", "sadasdasdsa");
        }
        public void CloseTcp()
        {
           t.CloseTcp();
        }

        public void DoAFlip(string name)
        {
            his.putCon(name);
        }

        public void send_buzz()
        {
            t.SendMessage("/buzz");
        }

     

        public void SearchU(string name)
        {
            his.filter_search(name);
        }

        public void fetch_mess(string name)
        {
            his.getMes(name);
        }


        public void StartListenThread(int prt)
        {
            if (t.isListening())
            {
                msgs.AddMessage(new Tmessage("Server", "You are already listening for a connection"),"A");
            }
            else
            {
                t.CreateListener(prt, _myusername);
                var task = new Task(() => t.Listen());
                task.Start();
            }
        }

        public void StartClientThread()
        {
            var task = new Task(() => t.CreateClient(_myusername, _friendport, _friendip));
            task.Start();

        }

        public void NotifyPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }


        public void SendMsg(string message)
        {
            if (t.isconnected() == true)
            {
                t.SendMessage(message);
                msgs.AddMessage(new Tmessage(_myusername, message), "S");
            }
            else
            {
              msgs.AddMessage(new Tmessage("Server", "You are currently not connected to anyone"),"A");
            }

            _msg = "";
            this.NotifyPropertyChanged("Msg");
        }

        public void SendImg(byte[] pic)
        {
            if (t.isconnected() == true)
            {
                t.SendPic(pic);
                msgs.AddMessage(new Imessage(_myusername, "Picture", pic), "S");
            }
        }

        public int FriendPort
        {
            get => _friendport; set
            {
                Debug.WriteLine(value);
                _friendport = value;
            }
        }

        public string Search_s
        {
            get => search_string; set
            {
                Debug.WriteLine(value);
                search_string = value;
            }
        }

        public ObservableCollection<Message> GetM
        {
            get => msgs.messages; set
            {
                Debug.WriteLine(value);
            }
        }

        public ObservableCollection<User> GetU
        {
            get => his.users; set
            {
                Debug.WriteLine(value);
            }
        }

        public ObservableCollection<string> GetC
        {
            get => his.convs; set
            {
                Debug.WriteLine(value);
            }
        }

        public ObservableCollection<Message> GetMe
        {
            get => his.hisMsg; set
            {
                Debug.WriteLine(value);
            }
        }

        public int MyPort
        {
            get => _myport; set
            {
                Debug.WriteLine(value);
                _myport = value;
            }
        }
        public string MyUsername
        {
            get => _myusername; set
            {
                Debug.WriteLine(value);
                _myusername = value;
            }
        }
        public string FriendIp
        {
            get => _friendip; set
            {
                Debug.WriteLine(value);
                _friendip = value;
            }
        }

        public string Msg
        {
            get => _msg; set
            {
                Debug.WriteLine(value);
                _msg = value;
            }
        }

    }

    public class TestCommand : ICommand
    {
        public delegate void ICommandOnExecute(object parameter);
        public delegate bool ICommandOnCanExecute(object parameter);

        private ICommandOnExecute _execute;
        private ICommandOnCanExecute _canExecute;

        public TestCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
        {
            _execute = onExecuteMethod;
            _canExecute = onCanExecuteMethod;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }

        #endregion
    }
}
