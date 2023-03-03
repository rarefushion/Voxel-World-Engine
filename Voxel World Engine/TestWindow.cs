using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Voxel_World_Engine
{
    internal class TestWindow : GameWindow
    {
        private int vertexBufferHandle;
        private int indexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        public TestWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            this.CenterWindow(new Vector2i(1280, 720));
        }

        protected override void OnLoad()
        {
            GL.ClearColor(new Color4(0f, 0f, 0f, 1f));

            float x = 384f;
            float y = 400f;
            float w = 512f;
            float h = 256f;

            float[] vertices = new float[]
            {
                x, y + h, 0f,       //TL
                x + w, y + h, 0f,   //TR
                x, y, 0f,           //BL
                x + w, y, 0f        //BR
            };

            int[] indecies = new int[]
            {
                0, 1, 3, 0, 3, 2
            };

            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indecies.Length * sizeof(float), indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            string vertexShaderCode = 
                @"
                #version 330 core

                uniform vec2 ViewportSize

                layout (location = 0) in vec3 aPosition;

                void main()
                {
                    float nx = aPosition.x / ViewportSize.x * 2f - 1f;
                    float ny = aPosition.y / ViewportSize.y * 2f - 1f;
                    gl_Position = vec4(nx, ny , 0f, 1f);
                }
                ";

            string fragmentShaderCode =
                @"
                #version 330 core

                out vec4 result;

                void main()
                {
                    result = vec4(0f, 0.8f, 1f, 1f);
                }
                ";

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexArrayHandle);
            if (vertexShaderInfo != String.Empty)
            {

            }

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

            string fragmentShaderInfo = GL.GetShaderInfoLog(fragmentShaderHandle);
            if (fragmentShaderInfo != String.Empty)
            {

            }

            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            GL.UseProgram(shaderProgramHandle);
            int viewportSizeUniformLocation = GL.GetUniformLocation(shaderProgramHandle, "ViewportSize");
            GL.Uniform2(viewportSizeUniformLocation, (float)viewport[2], (float)viewport[3]);
            GL.UseProgram(0);

            base.OnLoad();
        }

        protected override void OnResize (ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(shaderProgramHandle);
            GL.BindVertexArray(vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(indexBufferHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }
    }
}
