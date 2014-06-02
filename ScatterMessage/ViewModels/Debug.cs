namespace ScatterMessage.ViewModels
{
    public partial class MainViewModel : ObservableItem
    {
        private string debug;
        public string Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                this.OnPropertyChanged(() => Debug);
            }
        }
    }
}
