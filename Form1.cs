using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lab3
{
    public partial class Form1 : Form
    {
        private View view;
        
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        int frameCount;

        private int RetraceDepth = 8;
        private bool updateDepth = false;

        private OpenTK.Vector3 vectColor;

        public Form1()
        {
            InitializeComponent();
            glControl1.Height = this.Height;
            glControl1.Width = this.Width;
            view = new View();

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            view.InitShaders();
            view.Setup(RetraceDepth);
            GL.UseProgram(view.BasicProgramID);
            GL.Viewport(new Size(glControl1.Width, glControl1.Height));
        
            


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;

        }

        private void Application_Idle(object sender, EventArgs e)
        {

            while (glControl1.IsIdle)
            {
                GL.UseProgram(view.BasicProgramID);
                glControl1.Invalidate();
                if (updateDepth)
                {
                    int k = GL.GetUniformLocation(view.BasicProgramID, "RetraceDepth");
                    GL.Uniform1(k, RetraceDepth);

                    if (RetraceDepth != 8)
                    {
                        raytrace.Text = RetraceDepth.ToString();
                        raytrace.Visible = true;
                    }
                    else
                    {
                        raytrace.Visible = false;
                    }
                }
                vectColor = new OpenTK.Vector3((float)0.5, (float)0.5, (float)0.5);
                SetMaterialProperties(vectColor, (float)0.5, (float)0.5);
                displayFPS();
                glControl1.Invalidate();
            }

        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
                view.Update();
                GL.UseProgram(view.BasicProgramID);
                glControl1.SwapBuffers();
        }
        private void glControl1_KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {
           
            if ((e.KeyChar == '+' || e.KeyChar == '='))
            {
                RetraceDepth++;
                updateDepth = true;
            }
            if (e.KeyChar == '-')
            {
                if (RetraceDepth > 0)
                {
                    RetraceDepth--;
                    updateDepth = true;
                }
            }
        }

        private void SetMaterialProperties(OpenTK.Vector3 color, float transparency, float reflectivity)
        {
            GL.UseProgram(view.BasicProgramID);
            GL.Uniform3(GL.GetUniformLocation(view.BasicProgramID, "MaterialColor"), color);
            GL.Uniform1(GL.GetUniformLocation(view.BasicProgramID, "MaterialTransparency"), transparency);
            GL.Uniform1(GL.GetUniformLocation(view.BasicProgramID, "MaterialReflectivity"), reflectivity);
        }

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("Lab3 (FPS={0})", frameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }
    }
}
