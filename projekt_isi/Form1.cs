using Newtonsoft.Json;
using SpotifyAPI.Web;
using System.Data;
using System.Diagnostics.Metrics;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace projekt_isi {
    public partial class Form1 : Form{
        public List<SpotifyTrack> trackList = new List<SpotifyTrack>();
        public Show ShowList = new Show();
        public string countryTag;
        public Form1(){
            InitializeComponent();
        }

        //pobieranie danych z API
        private async void GetData_Click(object sender, EventArgs e){
            List.Clear();
            //OAuth2.0
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator("a8860854819d4e7d8e178b4529e75124", "7b322add2a9049e0a34e1e6f94fdbd7a"));
            //RESTAPI
            var spotify = new SpotifyClient(config);
            try{
                var playlist = await spotify.Playlists.Get(countryTag);
                int i = 1;
                if (trackList.Count > 0){
                    trackList.Clear();
                }
                foreach (PlaylistTrack<IPlayableItem> item in playlist.Tracks.Items){
                    if (item.Track is FullTrack track){
                        var Artist = new List<string>();
                        foreach (var Art in track.Artists){
                            Artist.Add(Art.Name.ToString());
                        }
                        trackList.Add(new SpotifyTrack{
                            Place = i,
                            Name = track.Name.ToString(),
                            Albumname = track.Album.Name.ToString(),
                            Artistname = string.Join(", ", Artist)
                        });
                    }
                    i++;
                }
                ShowList.View(trackList, List);
                List.View = View.Details;
            }
            catch(Exception ex){
                MessageBox.Show("Pick the Area before Getting Data");
            }

        }
        //refresh/wy�wietlanie danch
        private void ViewData_Click(object sender, EventArgs e) {
            ShowList.View(trackList, List);
            List.View = View.Details;
        }
        //import listy z bazy danych
        private void ImportDB_Click(object sender, EventArgs e) {
            db data = new db();
            data.Import(trackList);
        }
        //export listy do bazy danych
        private void ExportDB_Click(object sender, EventArgs e){
            if (trackList.Count > 0) {
                db data = new db();
                data.Export(trackList);
            }
            else {
                MessageBox.Show("There is no data to export");
            }
        }

        //import pliku XML do listy utwor�w
        private void ImportXML_Click(object sender, EventArgs e) {
            trackList.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<SpotifyTrack>));
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Choose file";
            openFileDialog1.Filter = "All files (.)|*.*|XML File (.xml)|*.xml";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "") {
                using (Stream reader = new FileStream(openFileDialog1.FileName, FileMode.Open)) {
                    var tracks = (List<SpotifyTrack>)serializer.Deserialize(reader);
                    foreach (var track in tracks) {
                        trackList.Add(new SpotifyTrack {
                            Place = track.Place,
                            Name = track.Name,
                            Albumname = track.Albumname,
                            Artistname = track.Artistname
                        });
                    }
                }
            }
            MessageBox.Show("XML successfuly imported!");
            ShowList.View(trackList, List);
        }

        //export pliku XML do listy utwor�w
        private void ExportXML_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file";
            sfd.Filter = "XML File (.xml)|*.xml";
            sfd.ShowDialog();
            if (sfd.FileName != "") {
                XmlSerializer serializer = new XmlSerializer(typeof(List<SpotifyTrack>));
                using (TextWriter writer = new StreamWriter(sfd.FileName)) {
                    serializer.Serialize(writer, trackList);
                }
                MessageBox.Show("XML successfuly exported!");
            }

        }

        //import pliku JSON do listy utwor�w
        private void ImportJSON_Click(object sender, EventArgs e) {
            trackList.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Choose file";
            openFileDialog1.Filter = "All files (.)|*.*|Json files (.json) |*.json";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "") {
                using (StreamReader reader = new StreamReader(openFileDialog1.FileName)) {
                    string json = reader.ReadToEnd();
                    trackList = JsonConvert.DeserializeObject<List<SpotifyTrack>>(json);
                }
            }
            MessageBox.Show("JSON successfuly imported!");
            ShowList.View(trackList, List);
        }

        //export pliku JSON do listy utwor�w
        private void ExportJSON_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file";
            sfd.Filter = "JSON File (.json)|*.json";
            sfd.ShowDialog();
            if (sfd.FileName != "") {
                var temp = JsonConvert.SerializeObject(trackList);
                File.WriteAllText(sfd.FileName, temp);
                MessageBox.Show("JSON successfuly exported!");
            }
        }

        //filtry dla danych obaszar�w
        //og�lno�wiatowy
        private void worldFiltr_CheckedChanged(object sender, EventArgs e) {
            this.countryTag = "37i9dQZEVXbNG2KDcFcKOF";
        }
        //polska
        private void PolandFiltr_CheckedChanged(object sender, EventArgs e) {
            this.countryTag = "37i9dQZEVXbMZ5PAcNTDXd";

        }
        //usa
        private void USAFilter_CheckedChanged(object sender, EventArgs e) {
            this.countryTag = "37i9dQZEVXbLp5XoPON0wI";

        }
        //wielka brytania
        private void UKFilter_CheckedChanged(object sender, EventArgs e) {
            this.countryTag = "37i9dQZEVXbMwmF30ppw50";

        }
        //niemcy
        private void GerFilter_CheckedChanged(object sender, EventArgs e) {
            this.countryTag = "37i9dQZEVXbK8BKKMArIyl";

        }
    }
}

