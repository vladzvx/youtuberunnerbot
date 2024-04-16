namespace YRB.App
{
    public partial class YoutubeRunner : Form
    {
        public YoutubeRunner()
        {
            InitializeComponent();

            // делаем невидимой нашу иконку в трее
            notifyIcon1.Visible = false;
            // добавляем Эвент или событие по 2му клику мышки, 
            //вызывая функцию  notifyIcon1_MouseDoubleClick
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);

            // добавляем событие на изменение окна
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;
                // делаем нашу иконку в трее активной
                notifyIcon1.Visible = true;
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели
            this.ShowInTaskbar = true;
            //разворачиваем окно
            WindowState = FormWindowState.Normal;
        }
    }
}
