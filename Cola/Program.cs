// See https://aka.ms/new-console-template for more information

using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Cola;

public class Game : GameWindow
{
    private int vertexBufferHandle;
    private int shaderProgramHandle;
    private int vertexArrayHandle;

    public Game(int width, int height, string title) : 
        base(GameWindowSettings.Default, new NativeWindowSettings() 
        { Size = (width, height), Title = title }) { }

    public static void Main(string[] args)
    {
        using Game game = new Game(800,600,"Cola - Sandbox");
        game.Run();
        
    }

    protected override void OnLoad()
    {
        
        
        GL.ClearColor(0f, 0f, 0f, 1f);
        
        float[] vertices = {
            0f, 0.5f, 0f,
            0.5f, -0.5f, 0f,
            -0.5f, -0.5f, 0f,
        };

        vertexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        vertexArrayHandle = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayHandle);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.BindVertexArray(0);
        
        string vertexShaderCode =@"
                #version 330 core

                layout(location = 0) in vec3 aPosition;

                void main(void)
                {
                    gl_Position = vec4(aPosition, 1.0);
                }
        ";
        
        string pixelShaderCode =@"
            #version 330

             out vec4 outputColor;

             void main()
               {
                  outputColor = vec4(1.0, 1.0, 0.0, 1.0);
               }
        ";

        int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
        GL.CompileShader(vertexShaderHandle);
        var vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
        if (vertexShaderInfo != string.Empty)
        {
            Console.WriteLine($"[ERROR] Vertex shader compile error: {vertexShaderInfo}");
        }
        else
        {
            Console.WriteLine($"[INFO] Vertex shader compiled successfully.");
        }
        
        int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
        GL.CompileShader(pixelShaderHandle);
        var pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
        if (pixelShaderInfo != string.Empty)
        {
            Console.WriteLine($"[ERROR] Pixel shader compile error: {pixelShaderInfo}");
        }
        else
        {
            Console.WriteLine($"[INFO] Pixel shader compiled successfully.");
        }

        shaderProgramHandle = GL.CreateProgram();
        GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
        GL.AttachShader(shaderProgramHandle, pixelShaderHandle);
        GL.LinkProgram(shaderProgramHandle);
        
        GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
        GL.DetachShader(shaderProgramHandle, pixelShaderHandle);
        
        GL.DeleteShader(vertexShaderHandle);
        GL.DeleteShader(pixelShaderHandle);
            
        base.OnLoad();
    }

    protected override void OnUnload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferHandle);
        
        GL.UseProgram(0);
        GL.DeleteProgram(shaderProgramHandle);
        
        base.OnUnload();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        
        KeyboardState input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        GL.UseProgram(shaderProgramHandle);
        
        GL.BindVertexArray(vertexArrayHandle);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        Context.SwapBuffers();
        base.OnRenderFrame(args);
    }

    public override void Close()
    {
        base.Close();
        
        System.Environment.Exit(0);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        base.OnResize(e);
    }
}