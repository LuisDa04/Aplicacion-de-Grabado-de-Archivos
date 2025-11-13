using System;
using System.Drawing;
using System.Windows.Forms;

public partial class AcercaDeForm : Form
{
    public AcercaDeForm()
    {
        Text = "Acerca De";
        Size = new Size(450, 300);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        BackColor = Color.WhiteSmoke;

        Label lblTitulo = new Label
        {
            Text = "Información del Desarrollador",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.DarkBlue,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Label lblInfo = new Label
        {
            Text =
        @"Desarrollador: Luis David Gil Quintana
        Contacto: +53 59760838
        Correo: luisdavidgilquintana@gmail.com
        GitHub: https://github.com/LuisDa04",
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            ForeColor = Color.Black,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20)
        };

        Button btnCerrar = new Button
        {
            Text = "Cerrar",
            Dock = DockStyle.Bottom,
            Height = 35
        };
        btnCerrar.Click += (s, e) => Close();

        Controls.Add(lblInfo);
        Controls.Add(lblTitulo);
        Controls.Add(btnCerrar);


    }
}