using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;


namespace AppForm
{
    public partial class MainForm : Form
    {
        private int contadorFacturas = 1;
        public MainForm()
        {
            InitializeComponent();
            
            contadorFacturas = ObtenerUltimoNumeroFactura() + 1;

            // Crear TabControl
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };

            // Crear pestañas
            TabPage tab1 = new TabPage("Precios");
            TabPage tab2 = new TabPage("Facturas");
            TabPage tab3 = new TabPage("Opciones");

            // Crear DataGridViews
            DataGridView dgv1 = new DataGridView { Dock = DockStyle.Fill };
            DataGridView dgv2 = new DataGridView { Dock = DockStyle.Fill };

            // Estilo bonito
            ApplyStyle(dgv1);
            ApplyStyle(dgv2);

            // Agregar tablas a pestañas
            tab1.Controls.Add(dgv1);
            tab2.Controls.Add(dgv2);

            dgv1.Rows.Add("Animado (corto variado)", 2);
            dgv1.Rows.Add("Serie", 3);
            dgv1.Rows.Add("Show latino", 3);
            dgv1.Rows.Add("Novela", 3);
            dgv1.Rows.Add("Serie dorama", 4);
            dgv1.Rows.Add("Documental", 3);
            dgv1.Rows.Add("Concurso reality", 7);
            dgv1.Rows.Add("Pelicula (AVI/MPG/VOB)", 7);
            dgv1.Rows.Add("Pelicula (HD/MP4)", 20);
            dgv1.Rows.Add("Deporte", 10);
            dgv1.Rows.Add("Musica/video", 100);
            dgv1.Rows.Add("Juego (detective)", 50);
            dgv1.Rows.Add("Juego (PC)", 100);
            dgv1.Rows.Add("Windows Pak Drivers", 100);
            dgv1.Rows.Add("Booteable", 300);
            dgv1.Rows.Add("Paquete semanal", 400);

            dgv1.AllowUserToAddRows = false;

            dgv2.AllowUserToAddRows = false;
            dgv2.ReadOnly = true;

            // Agregar columnas al DataGridView de Precios
            dgv1.Columns.Add("Producto", "Producto");
            dgv1.Columns.Add("Precio", "Precio");
            dgv1.Columns.Add("Cantidad", "Cantidad");

            dgv1.Dock = DockStyle.Fill;

            // Agregar columnas al DataGridView de Facturas
            dgv2.Columns.Add("NoFactura", "No. de Factura");
            dgv2.Columns.Add("Fecha", "Fecha");
            dgv2.Columns.Add("PagoTotal", "Pago Total");
            dgv2.Columns.Add("PagoEfectivo", "Efectivo");
            dgv2.Columns.Add("PagoTransferencia", "Transferencia");

            Button btnSeleccionar = new Button
            {
                Text = "Seleccionar",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(100, 30),
                Location = new Point(tab1.ClientSize.Width - 110, tab1.ClientSize.Height - 40)
            };
            tab1.Resize += (s, e) =>
            {
                btnSeleccionar.Location = new Point(tab1.ClientSize.Width - 110, tab1.ClientSize.Height - 40);
            };

            btnSeleccionar.Click += (s, e) =>
            {
                decimal suma = 0;
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    if (row.Cells["Precio"].Value != null &&
                        decimal.TryParse(row.Cells["Precio"].Value.ToString(), out decimal precio))
                    {
                        int cantidad = 0;
                        if (row.Cells["Cantidad"].Value != null)
                            int.TryParse(row.Cells["Cantidad"].Value.ToString(), out cantidad);

                        suma += precio * cantidad;
                    }
                }

                // Abrir ventana de pago con el total
                PagoForm pagoForm = new PagoForm(suma, dgv2, contadorFacturas);
                if (pagoForm.ShowDialog() == DialogResult.OK)
                {
                    contadorFacturas = pagoForm.GetContador();
                }
            };

            // Agregar controles a pestañas
            tab1.Controls.Add(dgv1);
            tab1.Controls.Add(btnSeleccionar);
            tab2.Controls.Add(dgv2);

            Label lbl = new Label
            {
                Text = "En desarrollo",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.DarkSlateBlue
            };
            tab3.Controls.Add(lbl);

            // Agregar pestañas al TabControl
            tabControl.TabPages.Add(tab1);
            tabControl.TabPages.Add(tab2);
            tabControl.TabPages.Add(tab3);

            // Agregar al formulario
            this.Controls.Add(tabControl);
            this.Text = "Aplicacion de Grabacion";
            this.Size = new Size(800, 600);

            try
            {
                CrearBaseDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la base de datos: " + ex.Message);
            }

            CargarFacturas(dgv2);
        }

        private void ApplyStyle(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.WhiteSmoke;
            dgv.DefaultCellStyle.BackColor = Color.AliceBlue;
            dgv.DefaultCellStyle.ForeColor = Color.DarkSlateGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
        }

        private void CrearBaseDatos()
        {
            using (var connection = new SqliteConnection("Data Source=facturas.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Facturas (
                    NoFactura INTEGER PRIMARY KEY,
                    Fecha TEXT,
                    PagoTotal REAL,
                    PagoEfectivo REAL,
                    PagoTransferencia REAL
                );
                ";
                command.ExecuteNonQuery();
            }
        }

        private void CargarFacturas(DataGridView dgv)
        {
            dgv.Rows.Clear();

            using (var connection = new SqliteConnection("Data Source=facturas.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Facturas";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dgv.Rows.Add(reader.GetInt32(0),   // NoFactura
                                    reader.GetString(1),   // Fecha
                                    reader.GetDecimal(2),  // PagoTotal
                                    reader.GetDecimal(3),  // PagoEfectivo
                                    reader.GetDecimal(4)); // PagoTransferencia
                    }
                }
            }
        }

        private int ObtenerUltimoNumeroFactura()
        {
            using (var connection = new SqliteConnection("Data Source=facturas.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT IFNULL(MAX(NoFactura), 0) FROM Facturas";

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

    }
}
