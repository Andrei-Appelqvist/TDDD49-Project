using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace P2P_Chatt
{
    class ConvHist
    {
        public ObservableCollection<User> users { get; } = new ObservableCollection<User>();

        public ObservableCollection<Message> hisMsg { get; } = new ObservableCollection<Message>();
        public ObservableCollection<string> convs { get; } = new ObservableCollection<string>();
        string projPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Conversations";
        string n_path;

        public void putDir()
        {
            users.Clear();
            var RootDirectory = new DirectoryInfo(projPath);
            var tempLst = RootDirectory.GetDirectories("*", SearchOption.AllDirectories)
                .Where(dir => !dir.GetDirectories().Any())
                .ToList();
            foreach(DirectoryInfo m in tempLst)
            {
                users.Add(new User(m.ToString()));
            }
        }

        public void putCon(string name)
        {
            //projPath + \\name
            convs.Clear();
            n_path = projPath + "\\" + name;
            DirectoryInfo di = new DirectoryInfo(n_path);
            List<string> getAllJson = di.GetFiles("*.json")
                .Where(file => file.Name.EndsWith(".json"))
                 .Select(file => file.Name).ToList();
            foreach(string m in getAllJson)
            {
                convs.Add(m);
            }

        }

        public void getMes(string j)
        {
            hisMsg.Clear();
            string j_path = n_path + "\\" + j;
            List<string> mes = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(j_path));
            Console.WriteLine(mes);
            foreach(string m in mes)
            {
                Message x = JsonConvert.DeserializeObject<Message>(m, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                hisMsg.Add(x);
            }
        }

        public void filter_search(string name)
        {
            putDir();
            List<User> l = users.Where(p => p.name.Contains(name)).ToList();
            users.Clear();
            foreach (User x in l)
            {
                users.Add(x);
            }
        }
    }


    public class User
    {
        public string name { get; set; }
        public string path { get; } = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Conversations";

        public User(string name)
        {
            this.name = name;
            this.path = path + "\\" + name;
        }
    }
}
