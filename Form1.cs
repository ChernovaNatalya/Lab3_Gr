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
using System.Threading;
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
        private bool updatePos = false;
        private bool updateDir = false;
        private OpenTK.Vector3 camPos = new OpenTK.Vector3(0.0f, 0.0f, -12.0f);
        private OpenTK.Vector2 mouseDir = new OpenTK.Vector2(0.0f, 0.0f);

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
            view.Setup(camPos, mouseDir, RetraceDepth);
            GL.UseProgram(view.BasicProgramID);
            GL.Viewport(new Size(glControl1.Width, glControl1.Height));
            view.Setup(camPos, mouseDir, RetraceDepth);

            GL.UseProgram(view.BasicProgramID);
            GL.Viewport(new Size(glControl1.Width, glControl1.Height));

            int k = GL.GetUniformLocation(view.BasicProgramID, "iResolution");
            GL.Uniform2(k, new OpenTK.Vector2() { X = glControl1.Width, Y = glControl1.Height });


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
                if (updatePos)
                {
                    int k = GL.GetUniformLocation(view.BasicProgramID, "iCamPos");
                    GL.Uniform3(k, camPos);
                    updatePos = false;
                }
                if (updateDir)
                {
                    int k = GL.GetUniformLocation(view.BasicProgramID, "iMouseDir");
                    GL.Uniform2(k, mouseDir);
                    updateDir = false;
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

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {

            Func<float, OpenTK.Matrix2> rotate = (angle) =>
            {
                angle *= ((float)Math.PI / 180);
                return new OpenTK.Matrix2(
                    (float)Math.Cos(angle), (float)-Math.Sin(angle),
                    (float)Math.Sin(angle), (float)Math.Cos(angle)
                );
            };

            
                label1.Text = e.KeyCode.ToString();
                float degX = 180f / glControl1.Width;
                float degY = 180f / glControl1.Height;


                if (e.KeyCode.ToString() == "Oemplus")
                {
                    RetraceDepth++;
                    updateDepth = true;
                }
                if (e.KeyCode.ToString() == "OemMinus")
                {
                    if (RetraceDepth > 0)
                    {
                        RetraceDepth--;
                        updateDepth = true;
                    }
                }

                if (e.KeyCode == Keys.R)
                {
                    mouseDir.X -= degY;
                    updateDir = true;
                }
                else if (e.KeyCode == Keys.F)
                {
                    mouseDir.X += degY;
                    updateDir = true;
                }


                OpenTK.Vector3 dir = new OpenTK.Vector3(0.0f, 0.0f, 0.0f);

                if (e.KeyCode == Keys.A)
                {
                    dir += new OpenTK.Vector3(-1.0f, 0.0f, 0.0f);
                    updatePos = true;
                }
                else if (e.KeyCode == Keys.D)
                {
                    dir += new OpenTK.Vector3(1.0f, 0.0f, 0.0f);
                    updatePos = true;
                }

                if (e.KeyCode == Keys.E)
                {
                    dir += new OpenTK.Vector3(0.0f, -1.0f, 0.0f);
                    updatePos = true;
                }
                else if (e.KeyCode == Keys.Q)
                {
                    dir += new OpenTK.Vector3(0.0f, 1.0f, 0.0f);
                    updatePos = true;
                }

                if (e.KeyCode == Keys.W)
                {
                    dir += new OpenTK.Vector3(0.0f, 0.0f, 1.0f);
                    updatePos = true;
                }
                else if (e.KeyCode == Keys.S)
                {
                    dir += new OpenTK.Vector3(0.0f, 0.0f, -1.0f);
                    updatePos = true;
                }

                var MatX = rotate(-mouseDir.X);
                var MatY = rotate(-mouseDir.Y);

                dir.X = MatY.M11 * dir.X + MatY.M21 * dir.Z;
                dir.Z = MatY.M12 * dir.X + MatY.M22 * dir.Z;

                dir.Y = MatX.M11 * dir.Y + MatX.M21 * dir.Z;
                dir.Z = MatX.M12 * dir.Y + MatX.M22 * dir.Z;

                camPos += dir * 1;

            
        }
    } 
}
