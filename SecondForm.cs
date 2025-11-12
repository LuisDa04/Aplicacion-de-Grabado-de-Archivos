using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace AppForm
{
    public class PagoForm : Form
    {
        private decimal total;
        private DataGridView dgvFacturas;
        private int contadorFacturas;
        private TextBox txtPagoCliente;
        private Label lblDiferencia;
        private RadioButton rbEfectivo;
        private RadioButton rbTransferencia;

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
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            Label lblPago = new Label
            {
                Text = "Pago del cliente:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtPagoCliente = new TextBox
            {
                Location = new Point(150, 60),
                Width = 100
            };

            rbEfectivo = new RadioButton
            {
                Text = "Pago en efectivo",
                Location = new Point(20, 100),
                AutoSize = true
            };

            rbTransferencia = new RadioButton
            {
                Text = "Pago en transferencia",
                Location = new Point(20, 130),
                AutoSize = true
            };

            Button btnSoloCalcular = new Button
            {
                Text = "Calcular diferencia",
                Location = new Point(200, 170),
                Width = 150
            };
            btnSoloCalcular.Click += BtnSoloCalcular_Click;

            Button btnRegistrar = new Button
            {
                Text = "Registrar factura",
                Location = new Point(20, 170),
                Width = 150
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            lblDiferencia = new Label
            {
                Text = "Diferencia: ",
                Location = new Point(20, 210),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };

            this.Controls.Add(lblTotal);
            this.Controls.Add(lblPago);
            this.Controls.Add(txtPagoCliente);
            this.Controls.Add(rbEfectivo);
            this.Controls.Add(rbTransferencia);
            this.Controls.Add(btnRegistrar);
            this.Controls.Add(btnSoloCalcular);
            this.Controls.Add(lblDiferencia);
        }

        private void BtnSoloCalcular_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtPagoCliente.Text, out decimal pagoCliente))
            {
                decimal diferencia = pagoCliente - total;
                lblDiferencia.Text = $"Diferencia: {diferencia:C}";
            }
            else
            {
                MessageBox.Show("Ingrese un valor numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            if (!rbEfectivo.Checked && !rbTransferencia.Checked)
            {
                MessageBox.Show("Seleccione un método de pago.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (decimal.TryParse(txtPagoCliente.Text, out decimal pagoCliente))
            {
                decimal diferencia = pagoCliente - total;
                string metodo = rbEfectivo.Checked ? "Efectivo" : "Transferencia";
                lblDiferencia.Text = $"Método: {metodo} | Diferencia: {diferencia:C}";

                GuardarFacturaEnBD(contadorFacturas, total, metodo);
                //  Guardar en la tabla de Facturas
                dgvFacturas.Rows.Add(contadorFacturas, DateTime.Now.ToShortDateString(), total,
                                    metodo == "Efectivo" ? total : 0,
                                    metodo == "Transferencia" ? total : 0);

                contadorFacturas++; //  ahora sí se incrementa en MainForm
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ingrese un valor numérico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarFacturaEnBD(int noFactura, decimal total, string metodo)
        {
            using (var connection = new SqliteConnection("Data Source=facturas.db"))
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
                command.Parameters.AddWithValue("$pagoEfectivo", metodo == "Efectivo" ? total : 0);
                command.Parameters.AddWithValue("$pagoTransferencia", metodo == "Transferencia" ? total : 0);

                command.ExecuteNonQuery();
            }
        }

        public int GetContador() => contadorFacturas;
    }
}
