namespace YRB.App
{
    public partial class YoutubeRunner : Form
    {
        public YoutubeRunner()
        {
            InitializeComponent();

            // ������ ��������� ���� ������ � ����
            notifyIcon1.Visible = false;
            // ��������� ����� ��� ������� �� 2�� ����� �����, 
            //������� �������  notifyIcon1_MouseDoubleClick
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);

            // ��������� ������� �� ��������� ����
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            // ��������� ���� ����, � ���� ��� ���� ��������, ������ �������        
            if (WindowState == FormWindowState.Minimized)
            {
                // ������ ���� ���� �� ������
                this.ShowInTaskbar = false;
                // ������ ���� ������ � ���� ��������
                notifyIcon1.Visible = true;
            }
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // ������ ���� ������ �������
            notifyIcon1.Visible = false;
            // ���������� ����������� ���� � ������
            this.ShowInTaskbar = true;
            //������������� ����
            WindowState = FormWindowState.Normal;
        }
    }
}
