﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Examen_T3
{
    public partial class Form1 : Form
    {
       // Enunciado 1:

        int p_puntero = 0;
        string[] p_array = new string[50];

        // Boton Ordenar Ingresar
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string x = (textBox1.Text);
            p_array[p_puntero] = x;
            p_puntero++;
            print();
        }

        // Generando evento para solo letras y no numeros
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
            {
                MessageBox.Show("Solo se permiten letras", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true; // solo letras y no numeros
            }
        }

        private void sube(string[] a, int n, int i)
        {
            if (n - 1 > i)
            {
                if (mayor(a[i], a[i + 1]))
                {
                    string aux = a[i];
                    a[i] = a[i + 1];
                    a[i + 1] = aux;
                }
                sube(a, n, i + 1);
            }
        }
        private void orden(string[] a, int n, int i)
        {
            if (n - 1 > i)
            {
                sube(a, n, 0);
                orden(a, n, i + 1);
            }
        }
        private bool mayor(string a, string b)
        {
            int l = a.Length;
            if (l > b.Length)
            {
                l = b.Length;
            }

            for (int i = 0; i < l; i++)
            {
                int A = (int)a[i];
                int B = (int)b[i];
                if (A > B)
                {
                    return true;
                }
                if (A < B)
                {
                    return false;
                }

            }

            return a.Length > b.Length;
        }

        // Boton Ordenar
        private void btnOrdenar_Click(object sender, EventArgs e)
        {
            orden(p_array, p_puntero, 0);
            print();
        }
        private void print()
        {
            listbox.Text = "";
            for (int i = 0; i < p_puntero; i++)
            {
                listbox.AppendText(p_array[i].ToString() + "\n");
            }
        }

        // Boton Salir
        private void BTNsalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        /////////////////////////////////////////////////////////////////////////////////////////////
        // -> Formulario(el Form1) Redondeado con Bordes / (Todo lo de Abajo)


        // Campos
        private int borderRadius = 30;
        private int borderSize = 2;
        private Color borderColor = Color.Black; // cambiar de color_1.1


        // Constructor
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderSize);
            this.panelTitulo.BackColor = borderColor; // cambiando de color_1.1
            this.BackColor = borderColor; // Eliminar el parpadeo Blanco
        }


        // Método Mover / Arrastrar Formulario
        // Arrasta el -> Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- Minimize borderless form from taskbar
                return cp;
            }
        }


        // Minimizar Formulario sin Bordes desde la Barra de tareas (Form1_MouseDown -> fue llamado desde propiedad)
        private void Form1_MouseDown(object sender, MouseEventArgs e)  // -> Para mover
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);    // -> mismo para Metros
        }

        private void panelTitulo_MouseDown(object sender, MouseEventArgs e) // -> Para mover
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);    // -> mismo para Metros
        }


        // Métodos Privados
        // Crear Ruta Redondeada
        private GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }


        // Establecer Región y Borde Redondeado – Formulario
        private void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColor, float borderSize)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                using (GraphicsPath roundPath = GetRoundedPath(form.ClientRectangle, radius))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                using (Matrix transform = new Matrix())
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    form.Region = new Region(roundPath);
                    if (borderSize >= 1)
                    {
                        Rectangle rect = form.ClientRectangle;
                        float scaleX = 1.0F - ((borderSize + 1) / rect.Width);
                        float scaleY = 1.0F - ((borderSize + 1) / rect.Height);

                        transform.Scale(scaleX, scaleY);
                        transform.Translate(borderSize / 1.6F, borderSize / 1.6F);

                        graph.Transform = transform;
                        graph.DrawPath(penBorder, roundPath);  // esto da color al Marco (Color Azul) solo descomillar y se pone en Negro
                    }
                }
            }
        }


        // Establecer Región y Borde Redondeado – Panel Contenedor
        private void ControlRegionAndBorder(Control control, float radius, Graphics graph, Color borderColor)
        {
            using (GraphicsPath roundPath = GetRoundedPath(control.ClientRectangle, radius))
            using (Pen penBorder = new Pen(borderColor, 1))
            {
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                control.Region = new Region(roundPath);
                graph.DrawPath(penBorder, roundPath);
            }
        }


        // Dibujar Rutas – Esquinas del Formulario
        private void DrawPath(Rectangle rect, Graphics graph, Color color)
        {
            using (GraphicsPath roundPath = GetRoundedPath(rect,borderRadius))
            using (Pen penBorder = new Pen(color, 3))
            {
                graph.DrawPath(penBorder,roundPath);
            }
        }


        // Obtener Colores de los Limites Exteriores del Formulario
        private struct FormBoundsColors
        {
            public Color TopLeftColor;
            public Color TopRightColor;
            public Color BottomLeftColor;
            public Color BottomRightColor;
        }
        private FormBoundsColors GetFormBoundsColors()
        {
            var fbColor = new FormBoundsColors();
            using (var bmp = new Bitmap(1, 1))
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle rectBmp = new Rectangle(0, 0, 1, 1);

                //Top Left
                rectBmp.X = this.Bounds.X - 1;
                rectBmp.Y = this.Bounds.Y;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.TopLeftColor = bmp.GetPixel(0, 0);

                //Top Right
                rectBmp.X = this.Bounds.Right;
                rectBmp.Y = this.Bounds.Y;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.TopRightColor = bmp.GetPixel(0, 0);

                //Bottom Left
                rectBmp.X = this.Bounds.X;
                rectBmp.Y = this.Bounds.Bottom;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.BottomLeftColor = bmp.GetPixel(0, 0);

                //Bottom Right
                rectBmp.X = this.Bounds.Right;
                rectBmp.Y = this.Bounds.Bottom;
                graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
                fbColor.BottomRightColor = bmp.GetPixel(0, 0);
            }
            return fbColor;
        }

        // Métodos de evento: Establecer Región Redondeado y Dibujar Borde + Suavizado (Formulario)
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // borde exterior liso
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rectForm = this.ClientRectangle;
            int mWidht = rectForm.Width / 2;
            int mHeight = rectForm.Height / 2;
            var fbColors = GetFormBoundsColors();

            // arriba a la izquierda -> y el color
            DrawPath(rectForm,e.Graphics, fbColors.TopLeftColor);

            // parte superior derecha
            Rectangle rectopRight = new Rectangle(mWidht,rectForm.Y,mWidht,mHeight);
            DrawPath(rectopRight,e.Graphics, fbColors.TopRightColor);

            // Abajo a la izquierda
            Rectangle rectBottomLeft = new Rectangle(rectForm.X, rectForm.X + mHeight, mWidht, mHeight);
            DrawPath(rectBottomLeft, e.Graphics,fbColors.BottomLeftColor);

            // Bottom Right
            Rectangle rectBottomRight = new Rectangle(mWidht, rectForm.Y + mHeight, mWidht, mHeight);
            DrawPath(rectBottomRight, e.Graphics, fbColors.BottomRightColor);
        }

        // llamar desde Propiedad
        private void Form1_Paint(object sender, PaintEventArgs e)    //Métodos de evento
        {
            FormRegionAndBorder(this, borderRadius, e.Graphics, borderColor, borderSize); // -> el formulario
        }
        // Establecer Región Redondeado y Dibujar Borde Fino (Panel Contenedor)
        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {
            ControlRegionAndBorder(panelContainer,borderRadius-(borderSize/2), e.Graphics, borderColor);
        }


        // Actualizar Región y Borde
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
        }


        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

        }




        /////////////////////////////////////////////////////////////////////////////////////////////
    }
}
