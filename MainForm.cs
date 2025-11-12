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

            dgv2.AllowUserToAddRows = false;
            dgv2.ReadOnly = true;

            // Agregar columnas al DataGridView de Precios
            dgv1.Columns.Add("Producto", "Producto");
            dgv1.Columns.Add("Precio", "Precio");

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
                Dock = DockStyle.Bottom,
                Height = 40
            };

            btnSeleccionar.Click += (s, e) =>
            {
                decimal suma = 0;
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    if (row.Cells["Precio"].Value != null &&
                        decimal.TryParse(row.Cells["Precio"].Value.ToString(), out decimal precio))
                    {
                        suma += precio;
                    }
                }

                // Abrir ventana de pago y pasar referencia a dgv2
                PagoForm pagoForm = new PagoForm(suma, dgv2, contadorFacturas);
                if (pagoForm.ShowDialog() == DialogResult.OK)
                {
                    contadorFacturas = pagoForm.GetContador(); // recoger el nuevo valor
                }
            };

            // Ajustar posición cuando cambie el tamaño de la pestaña
            tab1.Resize += (s, e) =>
            {
                btnSeleccionar.Location = new Point(tab1.ClientSize.Width - 110, tab1.ClientSize.Height - 40);
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
