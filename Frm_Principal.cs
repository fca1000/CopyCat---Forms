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
            //Define o valor máximo do ProgressBar
            pgbar1.Maximum = Directory.GetFiles(diretorio, ".", SearchOption.AllDirectories).Length + Directory.GetDirectories(diretorio, "**", SearchOption.AllDirectories).Length;
            TreeNode tds = tvDados.Nodes.Add(diretorioInfo.Name);
            tds.Tag = diretorioInfo.FullName;
            tds.StateImageIndex = 0;
            //carrega os arquivos e as subpastas
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
                    // Percorre os arquivos              
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
            // Pega todos os subdiretorios
            string[] subdiretorioEntradas = Directory.GetDirectories(dir);
            // Percorre os subdiretorios a ver se existem outras subpasts
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
            // Pega todos os subdiretorios
            string[] subdiretorioEntradas = Directory.GetDirectories(dir);
            // Percorre os subdiretorios a ver se existem outras subpasts
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
            // Pega o node na posição do ponteiro do mouse
            TreeNode theNode = this.tvDados.GetNodeAt(e.X, e.Y);

            // Define uma ToolTip somente se o ponteiro do mouse estive em um node
            if (theNode != null && theNode.Tag != null)
            {
                // Altera a ToolTip somente se o ponteiro do mouse se mover para um novo Node
                if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.tvDados))
                    this.toolTip1.SetToolTip(this.tvDados, theNode.Tag.ToString());

            }
            else  // O ponteiro não esta sobre um Node então limpa a ToolTip.  
            {
                this.toolTip1.SetToolTip(this.tvDados, "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(Settings.Default.FolderPath))
            {
                Settings.Default.FolderPath = @"D:\Empresa\Clientes\CopyCat - Forms\Imagens_teste\Destino";
                Settings.Default.Save();
                var Caminhoprincipal = new DirectoryInfo(Settings.Default.FolderPath);
                FolderBrowserDialog Fbd = new FolderBrowserDialog
                {
                    Description = "Selecione o Caminho das imagens em PDF",
                    SelectedPath = Caminhoprincipal.Parent.FullName
                };
                try
                {
                    if (Fbd.ShowDialog() == DialogResult.OK)
                    {
                        Settings.Default.FolderPath = Fbd.SelectedPath.ToString();
                        Settings.Default.Save();
                    }
                    CarregaDadosPasta(Settings.Default.FolderPath.ToString());
                }
                catch (SecurityException Ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {Ex.Message}\n\n" +
                    $"Details:\n\n{Ex.StackTrace}");
                }
            }
            else
            {
                MessageBox.Show("Sem arquivo");
                Settings.Default.FolderPath = @"D:\Empresa\Clientes\CopyCat - Forms\Imagens_teste\Destino";
                Settings.Default.Save();
            }
        }

        public Image image2 { get; set; }
        private void tvDados_KeyDown(object sender, KeyEventArgs e)
        {
            string path = Settings.Default.FolderPath + @"\" + tvDados.SelectedNode.Text.ToString();
            List<string> result = CarregaArquivosReturn(path);
            string image_path = result[0];

            if (Directory.Exists(Settings.Default.FolderPath))
            {
                Settings.Default.FolderPath = result[1];
                Settings.Default.Save();
            }            

            try
            {
                if (!string.IsNullOrEmpty(image_path))
                {
                    image2 = Image.FromFile(image_path);
                    IMGLoader.Image = image2;
                    IMGLoader.SizeMode = PictureBoxSizeMode.StretchImage;
                }                              
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error: " + Ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {                
            }
            
        }
        public void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var Nomedapasta = new DirectoryInfo(Settings.Default.FolderPath);
                var caminho = Settings.Default.FolderPath;
                var destino = @"D:\Empresa\Clientes\CopyCat - Forms\Imagens_teste\Destino\Carrefour\" + Nomedapasta.Name;
                IMGLoader.Dispose();
                image2.Dispose();
                tvDados.Refresh();
                Directory.Move(Nomedapasta.FullName, destino);
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
