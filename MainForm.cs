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

            Button btnAcercaDe = new Button
            {
                Text = "Acerca de",
                Dock = DockStyle.Top,
                Height = 30
            };
            btnAcercaDe.Click += BtnAcercaDe_Click;

            btnAcercaDe.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnAcercaDe.Location = new Point(ClientSize.Width - btnAcercaDe.Width - 10, 10);

            Controls.Add(btnAcercaDe);
            Controls.SetChildIndex(btnAcercaDe, 0);

            // Crear TabControl
            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };

            Panel panelPrecios = new Panel { Dock = DockStyle.Fill };

            // Crear pestañas
            TabPage tab1 = new TabPage("Precios");
            TabPage tab2 = new TabPage("Facturas");
            TabPage tab3 = new TabPage("Opciones");

            // Crear DataGridViews
            DataGridView dgv1 = new DataGridView { Dock = DockStyle.Top, Height = 400};
            DataGridView dgv2 = new DataGridView { Dock = DockStyle.Fill };

            // Estilo bonito
            ApplyStyle(dgv1);
            ApplyStyle(dgv2);

            // Agregar tablas a pestañas
            tab1.Controls.Add(dgv1);
            tab2.Controls.Add(dgv2);

            DataGridViewButtonColumn colBoton = new DataGridViewButtonColumn();
            colBoton.Name = "Agregar";
            colBoton.HeaderText = "";
            colBoton.Text = "+";
            colBoton.UseColumnTextForButtonValue = true;
            colBoton.Width = 40;

            dgv1.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgv1.Columns[e.ColumnIndex].Name == "Agregar")
                {
                    var celdaCantidad = dgv1.Rows[e.RowIndex].Cells["Cantidad"];

                    int cantidadActual = 0;
                    if (celdaCantidad.Value != null)
                        int.TryParse(celdaCantidad.Value.ToString(), out cantidadActual);

                    celdaCantidad.Value = cantidadActual + 1;
                }
            };

            dgv1.CellPainting += (s, e) =>
            {
                if (e.ColumnIndex >= 0 && dgv1.Columns[e.ColumnIndex].Name == "Agregar" && e.RowIndex >= 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(dgv1.DefaultCellStyle.BackColor), e.CellBounds);

                    using (Brush b = new SolidBrush(Color.LightBlue))
                    {
                        int size = Math.Min(e.CellBounds.Width, e.CellBounds.Height) - 6;
                        int x = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
                        int y = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;

                        e.Graphics.FillEllipse(b, x, y, size, size);

                        using (Font f = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            TextRenderer.DrawText(e.Graphics, "+", f,
                                new Rectangle(x, y, size, size),
                                Color.White,
                                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        }
                    }

                    e.Handled = true; 
                }
            };

            
            // Agregar columnas al DataGridView de Precios
            dgv1.Columns.Add("Producto", "Producto");
            dgv1.Columns.Add("Precio", "Precio");
            dgv1.Columns.Add("Cantidad", "Cantidad");
            dgv1.Columns.Add(colBoton);

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
            dgv1.Columns["Producto"].ReadOnly = true;
            dgv1.Columns["Precio"].ReadOnly = true;
            dgv1.RowHeadersVisible = false;
            dgv1.AllowUserToResizeColumns = false;
            dgv1.AllowUserToResizeRows = false;

            dgv2.AllowUserToAddRows = false;
            dgv2.ReadOnly = true;
            dgv2.AllowUserToResizeColumns = false;
            dgv2.AllowUserToResizeRows = false;

            dgv1.Columns["Producto"].Width = 250;

            dgv1.Dock = DockStyle.Fill;

            // Agregar columnas al DataGridView de Facturas
            dgv2.Columns.Add("NoFactura", "No. de Factura");
            dgv2.Columns.Add("Fecha", "Fecha");
            dgv2.Columns.Add("PagoTotal", "Pago Total");
            dgv2.Columns.Add("PagoEfectivo", "Efectivo");
            dgv2.Columns.Add("PagoTransferencia", "Transferencia");

            Button btnLimpiarFactura = new Button
            {
                Text = "Limpiar Factura",
                Width = 120,
                Height = 30,
                Location = new Point(dgv1.Left + 10, dgv1.Bottom + 345),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnLimpiarFactura.Click += (s, e) =>
            {
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    if (!row.IsNewRow)
                        row.Cells["Cantidad"].Value = null;
                }
            };
            tab1.Controls.Add(btnLimpiarFactura);


            Button btnSeleccionar = new Button
            {
                Text = "Seleccionar",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            btnSeleccionar.Click += (s, e) =>
            {
                bool hayCantidad = false;
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var valor = row.Cells["Cantidad"].Value;
                        if (valor != null && !string.IsNullOrWhiteSpace(valor.ToString()))
                        {
                            hayCantidad = true;
                            break; 
                        }
                    }
                }

                if (!hayCantidad)
                {
                    MessageBox.Show("No hay datos que procesar en la columna Cantidad.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; 
                }
                
                decimal suma = 0;
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    if (row.Cells["Precio"].Value != null &&
                        decimal.TryParse(row.Cells["Precio"].Value.ToString(), out decimal precio))
                    {
                        int cantidad = 0;

                        if (row.Cells["Cantidad"].Value != null)
                        {
                            if (!int.TryParse(row.Cells["Cantidad"].Value.ToString(), out cantidad))
                            {
                                MessageBox.Show("Error: La columna 'Cantidad' solo admite números enteros.",
                                                "Dato inválido",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                return; 
                            }
                        }

                        suma += precio * cantidad;
                    }
                }

                // Abrir ventana de pago con el total
                PagoForm pagoForm = new PagoForm(suma, dgv2, contadorFacturas);
                if (pagoForm.ShowDialog() == DialogResult.OK)
                {
                    contadorFacturas = pagoForm.GetContador();
                }
                foreach (DataGridViewRow row in dgv1.Rows)
                {
                    row.Cells["Cantidad"].Value = null;
                }
            };

            // Agregar controles a pestañas
            tab1.Controls.Add(dgv1);
            tab1.Controls.Add(btnSeleccionar);
            tab1.Controls.Add(panelPrecios);
            tab2.Controls.Add(dgv2);


            // Agregar pestañas al TabControl
            tabControl.TabPages.Add(tab1);
            tabControl.TabPages.Add(tab2);

            // Agregar al formulario
            Controls.Add(tabControl);
            Text = "Aplicacion de Grabacion";
            Size = new Size(800, 600);

            CargarFacturas(dgv2);
        }

        private void BtnAcercaDe_Click(object sender, EventArgs e)
        {
            AcercaDeForm acercaDe = new AcercaDeForm();
            acercaDe.ShowDialog();
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
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string parentFolder = Directory.GetParent(exeFolder).FullName;
            string carpetaBD = Path.Combine(parentFolder, "Base de Datos");
            if (!Directory.Exists(carpetaBD))
            {
                Directory.CreateDirectory(carpetaBD);
            }

            string rutaDB = Path.Combine(carpetaBD, "facturas.db");
            using (var connection = new SqliteConnection($"Data Source={rutaDB}"))
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
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string parentFolder = Directory.GetParent(exeFolder).FullName;
            string carpetaBD = Path.Combine(parentFolder, "Base de Datos");
            if (!Directory.Exists(carpetaBD))
            {
                Directory.CreateDirectory(carpetaBD);
            }
            string rutaDB = Path.Combine(carpetaBD, "facturas.db");
            using (var connection = new SqliteConnection($"Data Source={rutaDB}"))
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
            try
            {
                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                string parentFolder = Directory.GetParent(exeFolder).FullName;
                string carpetaBD = Path.Combine(parentFolder, "Base de Datos");
                if (!Directory.Exists(carpetaBD))
                {
                    Directory.CreateDirectory(carpetaBD);
                }

                string rutaDB = Path.Combine(carpetaBD, "facturas.db");
                using (var connection = new SqliteConnection($"Data Source={rutaDB}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT IFNULL(MAX(NoFactura), 0) FROM Facturas";

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch
            {
                CrearBaseDatos();
                return 0;
            }        
        }

    }
}
