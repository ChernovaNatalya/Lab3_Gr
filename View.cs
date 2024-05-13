using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Lab3
{
    internal class View
    {
        public int BasicProgramID;
        private int BasicVertexShader;
        private int BasicFragmentShader;
        private Vector3[] vertdata;
        private int vbo_position;
        private int attribute_vpos;
        private int uniform_pos;
        private Vector3 campos;
        private int uniform_aspect;
        private double aspect;

        public void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (System.IO.StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

       public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram(); // создание объекта программы
            loadShader("..\\..\\..\\Lab3\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\..\\Lab3\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);
            GL.ValidateProgram(BasicProgramID);
            // Проверяем успех компоновки
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
        
            vertdata = new Vector3[] {
            new Vector3(-1f, -1f, 0f),
            new Vector3( 1f, -1f, 0f),
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f) };
            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length *
            Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.Uniform3(uniform_pos, campos);
            GL.Uniform1(uniform_aspect, aspect);
            GL.UseProgram(BasicProgramID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }



        public void Update()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        public void Setup(/*OpenTK.Vector3 pos, OpenTK.Vector2 dir,*/ int depth)
        {
            /*
            int k = GL.GetUniformLocation(BasicProgramID, "iCamPos");
            GL.Uniform3(k, pos);
            k = GL.GetUniformLocation(BasicProgramID, "iMouseDir");
            GL.Uniform2(k, dir);
            */
           int  k = GL.GetUniformLocation(BasicProgramID, "RetraceDepth");
            GL.Uniform1(k, depth);
            GL.Uniform3(GL.GetUniformLocation(BasicProgramID, "MaterialColor"), new OpenTK.Vector3(0.5f));
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "MaterialTransparency"), 0.5f);
            GL.Uniform1(GL.GetUniformLocation(BasicProgramID, "MaterialReflectivity"), 0.5f);
        }

    }
}
