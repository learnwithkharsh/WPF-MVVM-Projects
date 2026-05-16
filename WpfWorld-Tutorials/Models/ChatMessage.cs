using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp.Models
{
    public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty]
        private string content;

        public bool IsUser { get; set; }

        public bool IsCode { get; set; }
    }


    /*public partial class ChatMessage : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ChatSegment> segments = new();


        [ObservableProperty]
        private bool isUser;
    }

    public class ChatSegment
    {
        public string Text { get; set; }
        public bool IsCode { get; set; }
        public string Language { get; set; }
    }
    */
}
