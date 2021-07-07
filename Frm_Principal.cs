using CopyCat___Forms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyCat___Forms
{
    public partial class Frm_Principal : Form
    {
        public Frm_Principal()
        {
            InitializeComponent();
        }

        public void CarregaDadosPasta(string diretorio)
        {
            DirectoryInfo diretorioInfo = new DirectoryInfo(diretorio);
            pgbar1.Maximum = Directory.GetFiles(diretorio, ".", SearchOption.AllDirectories).Length + Directory.GetDirectories(diretorio, "**", SearchOption.AllDirectories).Length;
            tvDados.Nodes.Clear();
            TreeNode tds = tvDados.Nodes.Add(diretorioInfo.Name);
            tds.Tag = diretorioInfo.FullName;
            tds.StateImageIndex = 0;
            CarregaArquivos(diretorio, tds);
            CarregaSubDiretorios(diretorio, tds);
        }
        public void CarregaArquivos(string diretorio, TreeNode tnd)
        {
            string[] arquivos = Directory.GetFiles(diretorio, ".");

            // Percorre os arquivos
            foreach (string arq in arquivos)
            {
                FileInfo arquivo = new FileInfo(arq);
                TreeNode tds = tnd.Nodes.Add(arquivo.Name);
                tds.Tag = arquivo.FullName;
                tds.StateImageIndex = 1;
                AtualizaProgressBar();
            }
        }

        public List<string> CarregaArquivosReturn(string diretorio)
        {
            string name = "";
            string local = "";
            try
            {
                if (Directory.Exists(diretorio))
                {
                    string[] arquivos = Directory.GetFiles(diretorio, ".");  
                    foreach (string arq in arquivos)
                    {
                        FileInfo arquivo = new FileInfo(arq);
                        name = arquivo.FullName;
                        local = arquivo.DirectoryName;
                        break;
                    }
                }                
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error: " + Ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return new List<string>() { name, local };
        }

        public void CarregaSubDiretorios(string dir, TreeNode td)
        {
            string[] subdiretorioEntradas = Directory.GetDirectories(dir);
            foreach (string subdiretorio in subdiretorioEntradas)
            {
                DirectoryInfo diretorio = new DirectoryInfo(subdiretorio);
                TreeNode tds = td.Nodes.Add(diretorio.Name);
                tds.StateImageIndex = 0;
                tds.Tag = diretorio.FullName;
                CarregaSubDiretorios2(subdiretorio, tds);
                AtualizaProgressBar();
            }
        }

        public void CarregaSubDiretorios2(string dir, TreeNode td)
        {
            string[] subdiretorioEntradas = Directory.GetDirectories(dir);
            foreach (string subdiretorio in subdiretorioEntradas)
            {
                DirectoryInfo diretorio = new DirectoryInfo(subdiretorio);
                TreeNode tds = td.Nodes.Add(diretorio.Name);
                tds.StateImageIndex = 0;
                tds.Tag = diretorio.FullName;
                CarregaSubDiretorios2(subdiretorio, tds);
                AtualizaProgressBar();
            }
        }

        public void AtualizaProgressBar()
        {
            if (pgbar1.Value < pgbar1.Maximum)
            {
                pgbar1.Value++;
                int percent = (int)(((double)pgbar1.Value / (double)pgbar1.Maximum) * 100);
                pgbar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(pgbar1.Width / 2 - 10, pgbar1.Height / 2 - 7));
                Application.DoEvents();
            }
        }

        public void tvDados_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode theNode = this.tvDados.GetNodeAt(e.X, e.Y);
            if (theNode != null && theNode.Tag != null)
            {
                if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.tvDados))
                    this.toolTip1.SetToolTip(this.tvDados, theNode.Tag.ToString());
            }
            else 
            {
                this.toolTip1.SetToolTip(this.tvDados, "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Fbd = new FolderBrowserDialog
            {
                Description = "Selecione o Caminho das imagens em PDF"
            };
            try
            {
                if (Fbd.ShowDialog() == DialogResult.OK)
                {
                    FolderPath = Fbd.SelectedPath.ToString();
                }
                CarregaDadosPasta(FolderPath);
            }
            catch (SecurityException Ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {Ex.Message}\n\n" +
                $"Details:\n\n{Ex.StackTrace}");
            }
        }

        public Image image2 { get; set; }
        public string FolderPath { get; set; }
        public string FilePath { get; set; }

        private void tvDados_KeyDown(object sender, KeyEventArgs e)
        {
            bool ver = false;
            string path = "";
            try
            {
                if (e.KeyCode == Keys.Up)
                {
                    path = FolderPath + @"\" + tvDados.SelectedNode.PrevNode.Text.ToString();
                    ver = true;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    path = FolderPath + @"\" + tvDados.SelectedNode.NextNode.Text.ToString();
                    ver = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Não é possivel visualizar a pasta raiz!");
            }
            
            if (ver)
            {
                List<string> result = CarregaArquivosReturn(path);
                string image_path = result[0];
                FilePath = result[1];

                try
                {
                    if (!string.IsNullOrEmpty(image_path))
                    {
                        image2 = Image.FromFile(image_path);
                        IMGLoader.Image = image2;
                        IMGLoader.SizeMode = PictureBoxSizeMode.StretchImage;
                        IMGLoader.Refresh();
                        Application.DoEvents();
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error: " + Ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var dir = new DirectoryInfo(FilePath);
                string destino = @"C:\CARREFOUR\" + dir.Name.ToString();
                image2.Dispose();
                Directory.Move(FilePath, destino);
            }
            catch (Exception Ex) 
            {
                MessageBox.Show("Error: " + Ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }                        
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
