namespace FRGenerics.Lab.Phone.Apps
{
    abstract public class Base
    {
        public View View { get; set; }
        public string Name { get; set; }
        public AppIcon Icon { get; set; }
        public int NotificationsCounter { get; set; }

        internal Base(View view, string name, AppIcon icon)
        {
            View = view;
            Name = name;
            Icon = icon;

            NotificationsCounter = 0;
        }

        public void Update() { }
    }
}
