using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace AppForm
{
    public class PagoForm : Form
    {
        private TextBox txtPagoEfectivo;
        private TextBox txtPagoTransferencia;
        private Label lblDiferencia;
        private Button btnRegistrar;
        private Button btnSoloCalcular;

        private decimal total;
        private DataGridView dgvFacturas;
        private int contadorFacturas;

        public PagoForm(decimal total, DataGridView dgvFacturas, int contadorFacturas)
        {
            this.total = total;
            this.dgvFacturas = dgvFacturas;
            this.contadorFacturas = contadorFacturas;

            this.Text = "Pago del Cliente";
            this.Size = new Size(400, 300);

            Label lblTotal = new Label
            {
                Text = $"Total a pagar: {total:C}",
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblEfectivo = new Label
            {
                Text = "Pago en efectivo:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            txtPagoEfectivo = new TextBox
            {
                Location = new Point(150, 60),
                Width = 100
            };

            Label lblTransferencia = new Label
            {
                Text = "Pago en transferencia:",
                Location = new Point(20, 100),
                AutoSize = true
            };
            txtPagoTransferencia = new TextBox
            {
                Location = new Point(150, 100),
                Width = 100
            };

            btnSoloCalcular = new Button
            {
                Text = "Calcular diferencia",
                Location = new Point(20, 140),
                Width = 150
            };
            btnSoloCalcular.Click += BtnSoloCalcular_Click;

            btnRegistrar = new Button
            {
                Text = "Registrar factura",
                Location = new Point(200, 140),
                Width = 150
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            lblDiferencia = new Label
            {
                Text = "Diferencia: ",
                Location = new Point(20, 180),
                AutoSize = true
            };

            Controls.Add(lblTotal);
            Controls.Add(lblEfectivo);
            Controls.Add(txtPagoEfectivo);
            Controls.Add(lblTransferencia);
            Controls.Add(txtPagoTransferencia);
            Controls.Add(btnSoloCalcular);
            Controls.Add(btnRegistrar);
            Controls.Add(lblDiferencia);
        }

        private void BtnSoloCalcular_Click(object sender, EventArgs e)
        {
            decimal pagoEfectivo = 0, pagoTransferencia = 0;
            decimal.TryParse(txtPagoEfectivo.Text, out pagoEfectivo);
            decimal.TryParse(txtPagoTransferencia.Text, out pagoTransferencia);

            decimal pagoCliente = pagoEfectivo + pagoTransferencia;
            decimal diferencia = pagoCliente - total;

            lblDiferencia.Text = $"Total: {total:C} | Pagado: {pagoCliente:C} | Diferencia: {diferencia:C}";
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            decimal pagoEfectivo = 0, pagoTransferencia = 0;
            decimal.TryParse(txtPagoEfectivo.Text, out pagoEfectivo);
            decimal.TryParse(txtPagoTransferencia.Text, out pagoTransferencia);

            if (pagoEfectivo > total || (pagoEfectivo + pagoTransferencia) > total)
            {
                pagoEfectivo = total - pagoTransferencia;
                if (pagoEfectivo < 0) pagoEfectivo = 0; 
            }

            dgvFacturas.Rows.Add(contadorFacturas, DateTime.Now.ToShortDateString(), total,
                                pagoEfectivo, pagoTransferencia);

            GuardarFacturaEnBD(contadorFacturas, total, pagoEfectivo, pagoTransferencia);

            contadorFacturas++;
            DialogResult = DialogResult.OK;
            Close();
        }


        private void GuardarFacturaEnBD(int noFactura, decimal total, decimal pagoEfectivo, decimal pagoTransferencia)
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
                INSERT INTO Facturas (NoFactura, Fecha, PagoTotal, PagoEfectivo, PagoTransferencia)
                VALUES ($noFactura, $fecha, $pagoTotal, $pagoEfectivo, $pagoTransferencia);
                ";

                command.Parameters.AddWithValue("$noFactura", noFactura);
                command.Parameters.AddWithValue("$fecha", DateTime.Now.ToShortDateString());
                command.Parameters.AddWithValue("$pagoTotal", total);
                command.Parameters.AddWithValue("$pagoEfectivo", pagoEfectivo);
                command.Parameters.AddWithValue("$pagoTransferencia", pagoTransferencia);

                command.ExecuteNonQuery();
            }
        }


        public int GetContador() => contadorFacturas;
    }
}
