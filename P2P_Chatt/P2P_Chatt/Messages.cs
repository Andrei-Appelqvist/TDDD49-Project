using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;


namespace P2P_Chatt
{
    public class Messages
    {
        public ObservableCollection<Message> messages { get; } = new System.Collections.ObjectModel.ObservableCollection<Message>();
        public string _myusername = null;
        public string _friendusername = null;
        string date;
        public List<string> jMsg = new List<string>();

        private void InitializeCollection()
        {
            messages.Add(new Tmessage("server", "Welcome to the chat"));
        }
        public void AddMessage(Message m, string flag)
        {
            //Tmessage m = new Tmessage(name, text);
            messages.Add(m);
            if (flag == "R" && _friendusername == null)
            {
                _friendusername = m.name;
                _friendusername = _friendusername.Replace(" ","");
            }
            if (flag == "S" && _myusername == null)
            {
                _myusername = m.name;
                _myusername =_myusername.Replace(" ", "");
            }
            if(date == null)
            {
                date = m.timeDate.ToString().Replace(" ", ".").Replace(":", ".");
            }

            if(flag != "A")
            {
                CreateConvo(m);
            }


        }
        public void CreateConvo(Message oB)
        {
            string ob = JsonConvert.SerializeObject(oB, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            jMsg.Add(ob);
            string projPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string folderName = Path.Combine(projPath+"\\Conversations", _friendusername);
            System.IO.Directory.CreateDirectory(folderName);
            string nJson = Path.Combine(folderName, date + ".json");
            string json = JsonConvert.SerializeObject(jMsg);
            File.WriteAllText(nJson, json);
        }

        public void reset()
        {
            messages.Clear();
            _myusername = null;
            _friendusername = null;
            date = null;
            jMsg.Clear();
        }

        public ObservableCollection<Message> get_list()
        {
            return messages;
        }

    }
    public abstract class Message
    {
        public string name { get; set; }
        public string text { get; set; }
        public string stringMessage {get;set;}

        public DateTime timeDate;

        public string img { get; set; }
        public byte[] imageinbytes { get; set; }



        public Message(string name, string text, byte[]imagebytes = null)
        {
            this.name = name;
            this.text = text;
            stringMessage = name + " :" + text;
            timeDate = DateTime.Now;

            Console.WriteLine(imageinbytes);
            if (imagebytes != null)
            {
                imageinbytes = imagebytes;
            }
            if (imagebytes != null && imageinbytes != null)
            {
                MemoryStream ms = new MemoryStream(imageinbytes, 0, imageinbytes.Length);
                ms.Write(imageinbytes, 0, imageinbytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                Console.WriteLine(timeDate);
                img = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Conversations\\"+timeDate.ToString().Replace(" ",".").Replace(":",".")+".png";

                image.Save(img, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
       

    }

    public class Tmessage : Message
    {
        public Tmessage(string name, string text) : base(name, text)
        {

        }
    }

    public class Imessage : Message
    {

        public Imessage(string name, string text, byte[] imagebytes) :base(name,text,imagebytes)
        {


        }
        /*
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        */
    }

    
}
