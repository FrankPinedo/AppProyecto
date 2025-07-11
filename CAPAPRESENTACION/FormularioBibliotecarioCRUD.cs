using CAPADATOS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; 
using System.Security.Cryptography; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPAPRESENTACION
{
    public partial class FormularioBibliotecarioCRUD : Form
    {
      
        private readonly ArregloBibliotecario ArregloBibliotecario = new ArregloBibliotecario();
        
        private List<Bibliotecario> listaBibliotecarioCompleta;
        private Bibliotecario bibliotecarioSeleccionado = null;

        public FormularioBibliotecarioCRUD()
        {
            InitializeComponent();
            txtCodigoBib.Enabled = false;
            ConfigurarListView();
            CargarBibliotecariosInicial(); 
        }

        private void ConfigurarListView()
        {
            lvBibliotecarios.View = View.Details;
            lvBibliotecarios.FullRowSelect = true;
            lvBibliotecarios.GridLines = true;

            lvBibliotecarios.Columns.Add("Código", 80, HorizontalAlignment.Left);
            lvBibliotecarios.Columns.Add("Nombres", 150, HorizontalAlignment.Left);
            lvBibliotecarios.Columns.Add("Apellidos", 150, HorizontalAlignment.Left);
        }

    
        private void CargarBibliotecariosInicial()
        {
            listaBibliotecarioCompleta = ArregloBibliotecario.LeerBibliotecariosDesdeArchivo();
            MostrarBibliotecariosEnListView(listaBibliotecarioCompleta);
        }

        private void MostrarBibliotecariosEnListView(List<Bibliotecario> bibliotecariosToShow)
        {
            lvBibliotecarios.Items.Clear();
            foreach (Bibliotecario bib in bibliotecariosToShow)
            {
                ListViewItem item = new ListViewItem(bib.codigoBibliotecario.ToString());
                item.SubItems.Add(bib.Nombres);
                item.SubItems.Add(bib.Apellidos);
                item.Tag = bib; 
                lvBibliotecarios.Items.Add(item);
            }
        }


        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtCodigoBib.Text = "";
            txtNombre.Text = "";
            txtApellidos.Text = "";
            txtBuscar.Text = ""; 
            bibliotecarioSeleccionado = null;
            lvBibliotecarios.SelectedItems.Clear(); 
            txtNombre.Focus(); // 
            CargarBibliotecariosInicial(); 
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MessageBox.Show("Por favor, ingrese Nombre y Apellido.", "Advertencia",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (bibliotecarioSeleccionado == null) 
            {
                
                Bibliotecario nuevoBibliotecario = new Bibliotecario
                {
                    Nombres = txtNombre.Text.Trim(),
                    Apellidos = txtApellidos.Text.Trim()
                };
                ArregloBibliotecario.AgregarBibliotecario(nuevoBibliotecario);
                MessageBox.Show("Bibliotecario agregado exitosamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else 
            {
                
                bibliotecarioSeleccionado.Nombres = txtNombre.Text.Trim();
                bibliotecarioSeleccionado.Apellidos = txtApellidos.Text.Trim();
               
                int index = listaBibliotecarioCompleta.FindIndex(b => b.codigoBibliotecario == bibliotecarioSeleccionado.codigoBibliotecario);
                if (index != -1)
                {
                   
                    listaBibliotecarioCompleta[index] = bibliotecarioSeleccionado;
                 
                    ArregloBibliotecario.GuardarBibliotecariosEnArchivo(listaBibliotecarioCompleta);
                    MessageBox.Show("Bibliotecario actualizado exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontró el bibliotecario para actualizar en la lista.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            CargarBibliotecariosInicial(); 
            btnNuevo_Click(sender, e);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (bibliotecarioSeleccionado != null)
            {
                if (MessageBox.Show($"¿Está seguro de que desea eliminar el bibliotecario con código" +
                                    $" {bibliotecarioSeleccionado.codigoBibliotecario}?", "Confirmar Eliminación",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                  
                    if (ArregloBibliotecario.EliminarBibliotecarioPorCodigo(bibliotecarioSeleccionado.codigoBibliotecario))
                    {
                        MessageBox.Show("Bibliotecario eliminado exitosamente.", "Éxito",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarBibliotecariosInicial(); 
                        btnNuevo_Click(sender, e); 
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el bibliotecario.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un bibliotecario de la lista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnBusca_Click(object sender, EventArgs e)
        {
            string textoBusqueda = txtBuscar.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(textoBusqueda))
            {
                CargarBibliotecariosInicial(); 
                return;
            }

         
            List<Bibliotecario> resultadosBusqueda = listaBibliotecarioCompleta.Where(bibliotecario =>
            {
               
                if (int.TryParse(textoBusqueda, out int codigoBusqueda))
                {
               
                    return bibliotecario.CodigoBibliotecario == codigoBusqueda ||
                           bibliotecario.Nombres.ToLower().Contains(textoBusqueda) ||
                           bibliotecario.Apellidos.ToLower().Contains(textoBusqueda);
                }
                else
                {
                    return bibliotecario.Nombres.ToLower().Contains(textoBusqueda) ||
                           bibliotecario.Apellidos.ToLower().Contains(textoBusqueda);
                }
            }).ToList();


            if (resultadosBusqueda.Count > 0)
            {
                MostrarBibliotecariosEnListView(resultadosBusqueda);
            }
            else
            {
                lvBibliotecarios.Items.Clear();
                MessageBox.Show("No se encontraron bibliotecarios con los criterios de búsqueda.",
                "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

       
        private void lvBibliotecarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvBibliotecarios.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvBibliotecarios.SelectedItems[0];

                bibliotecarioSeleccionado = (Bibliotecario)selectedItem.Tag;
                txtCodigoBib.Text = bibliotecarioSeleccionado.codigoBibliotecario.ToString();
                txtNombre.Text = bibliotecarioSeleccionado.Nombres;
                txtApellidos.Text = bibliotecarioSeleccionado.Apellidos;
            }
            else
            {
              
                txtCodigoBib.Text = "";
                txtNombre.Text = "";
                txtApellidos.Text = "";
                bibliotecarioSeleccionado = null;
            }
        }

        public Bibliotecario BibliotecarioParaPrestar { get; private set; }
        private void lvBibliotecarios_DoubleClick(object sender, EventArgs e)
        {
            if (lvBibliotecarios.SelectedItems.Count > 0)
            {
                BibliotecarioParaPrestar = (Bibliotecario)lvBibliotecarios.SelectedItems[0].Tag;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}