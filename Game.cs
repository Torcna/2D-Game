using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static imba_of_patch.Game;
using OpenTK.Audio.OpenAL;
using StbImageSharp;
using System.Xml.Linq;
using System.Reflection.Metadata;

namespace imba_of_patch
{
    internal class Game : GameWindow
    {
        int screenWidth;
        int screenHeight;
        

        int factor = 0;
        class models
        {
            float[] vert;
            public uint[] indices;
            public float[] texture_coord;

            public int make_ebo(int el_buff, uint[] indices)
            {
                el_buff= GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, el_buff);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint)*indices.Length, indices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                return el_buff;
            }
            public int attach_texture(string path,int texture_id)
            {
                texture_id=GL.GenTexture();

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture_id);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);


                ImageResult obj_texture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, obj_texture.Width, obj_texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, obj_texture.Data);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                return texture_id;
            }
        }
        
        class player: models
        {
            public int vao_player;
            public int vbo_player_vertices;
            public int el_buff;
            public int vbo_texture_player;

            public int curr_pl;
            int[] texture_array = new int[6];

            public string path_pl1 = "../../../textures/player1.png";
            public string path_pl2 = "../../../textures/player1,5.png";
            public string path_pl3 = "../../../textures/player2.png";
            public string path_pl4 = "../../../textures/player3.png";
            public string path_pl5 = "../../../textures/player4.png";
            public string path_pl6 = "../../../textures/player5.png";

            static public float rotate_angle =0f;
            public float[] vert =
            {
                -0.2f, 0.2f, 0f, // top left vertex - 0
                0.2f, 0.2f, 0f, // top right vertex - 1
                0.2f, -0.2f, 0f, // bottom right - 2
                -0.2f, -0.2f, 0f // bottom left - 3

            };
            public uint[] indices =
            {
                // top triangle
                0, 1, 2,
                // bottom triangle
                2, 3, 0
            };
            float[] texture_coord =
            {
                0f, 1f,
                1f, 1f,
                1f, 0f,
                0f, 0f
            };
            

            public void load()
            {
                vao_player=GL.GenVertexArray();
                GL.BindVertexArray(vao_player);


                vbo_player_vertices=GL.GenBuffer();
                
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_player_vertices);
                GL.BufferData(BufferTarget.ArrayBuffer, vert.Length*sizeof(float), vert, BufferUsageHint.StaticDraw);
                
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_player, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                vbo_texture_player = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_texture_player);
                GL.BufferData(BufferTarget.ArrayBuffer, texture_coord.Length*sizeof(float), texture_coord, BufferUsageHint.StaticRead);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_player, 1);
                    
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindVertexArray(0);

                // buff elem = order of drawing
                el_buff= make_ebo(el_buff, indices);

                texture_array[0]=attach_texture(path_pl1, vbo_texture_player);
                texture_array[1]=attach_texture(path_pl2, vbo_texture_player);
                texture_array[2]=attach_texture(path_pl3, vbo_texture_player);
                texture_array[3]=attach_texture(path_pl4, vbo_texture_player);
                texture_array[4]=attach_texture(path_pl5, vbo_texture_player);
                texture_array[5]=attach_texture(path_pl6, vbo_texture_player);
                curr_pl =0;

            }
            public void draw(int location)
            {
                

                GL.BindVertexArray(vao_player);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, el_buff);

                

                GL.BindTexture(TextureTarget.Texture2D, texture_array[curr_pl]);

                var tm = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotate_angle));
                tm= tm * Matrix4.CreateTranslation(-0.8f, -0.8f, 0f);
                GL.UniformMatrix4(location, true, ref tm);
                

                GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            public void change_image(int num)
            {
                curr_pl=num;
            }

            public void delete()
            {
                GL.DeleteVertexArray(vao_player);
                GL.DeleteBuffer(vbo_player_vertices);
                GL.DeleteTexture(texture_array[0]);
                GL.DeleteTexture(texture_array[1]);
                GL.DeleteTexture(texture_array[2]);
                GL.DeleteTexture(texture_array[3]);
                GL.DeleteTexture(texture_array[4]);
                GL.DeleteTexture(texture_array[5]);
                GL.DeleteBuffer(vbo_texture_player);
            }
        }
        /// <summary>
        ///                        BACKGROUND
        /// </summary>
        class bg:models
        {
            public int vao_bg;
            public int vbo_bg_vert;
            public int el_buff;
            
            public int curr_bg;

            int[] texture_array=new int[3];
            public int vbo_bg_texture;
            public string path_bg1= "../../../textures/bg1.png";
            public string path_bg2= "../../../textures/bg2.png";
            public string path_bg3= "../../../textures/bg3.png";


            public float[] vert =
            {
                -1f, 1f, 0f, // top left vertex - 0
                1f, 1f, 0f, // top right vertex - 1
                1f, -1f, 0f, // bottom right - 2
                -1f, -1f, 0f // bottom left - 3
            };
            public uint[] indices =
            {
                // top triangle
                0, 1, 2,
                // bottom triangle
                2, 3, 0
            };
            float[] texture_coord =
            {
                0f, 1f,
                1f, 1f,
                1f, 0f,
                0f, 0f
            };


            public void load()
            {
                vao_bg= GL.GenVertexArray();
                GL.BindVertexArray(vao_bg);


                vbo_bg_vert=GL.GenBuffer();


                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_bg_vert);
                GL.BufferData(BufferTarget.ArrayBuffer, vert.Length*sizeof(float), vert, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_bg, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                vbo_bg_texture = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_bg_texture);
                GL.BufferData(BufferTarget.ArrayBuffer, texture_coord.Length*sizeof(float), texture_coord, BufferUsageHint.StaticRead);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_bg, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindVertexArray(0);

                // baza

                el_buff = make_ebo(el_buff, indices);
                // TEXTURE ========
                texture_array[0]=attach_texture(path_bg1, vbo_bg_texture);
                texture_array[1]=attach_texture(path_bg2, vbo_bg_texture);
                texture_array[2]=attach_texture(path_bg3, vbo_bg_texture);
                curr_bg =0;

            }
            public void draw()
            {
                
                GL.BindVertexArray(vao_bg);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, el_buff);

                GL.BindTexture(TextureTarget.Texture2D, texture_array[curr_bg]);

                GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            public void change_bg(int num)
            {
                curr_bg=num;
            }
            public void delete()
            {
                GL.DeleteVertexArray(vbo_bg_texture);
                GL.DeleteBuffer(vao_bg);
                GL.DeleteTexture(texture_array[0]);
                GL.DeleteTexture(texture_array[1]);
                GL.DeleteTexture(texture_array[2]);
                GL.DeleteBuffer(vbo_bg_vert);
            }
        }

        class enemy:models
        {
            public int vao_enemy;
            public int vbo_enemy_vert;
            public int vbo_enemy_tex;
            public int el_buff;
            public int id_texture;

            public float[] vert =
            {
                0.9f, -0.5f, 0f, // top left vertex - 0
                0.6f, -0.5f, 0f, // top right vertex - 1
                0.6f, -0.8f, 0f, // bottom right - 2
                0.9f, -0.8f, 0f
            };

            float[] texture_coord =
            {
                0f, 1f,
                1f, 1f,
                1f, 0f,
                0f, 0f
            };
            public uint[] indices =
            {
                0,1,2,
                2,3,0
            };

            public void load()
            {
                vao_enemy=GL.GenVertexArray();
                GL.BindVertexArray(vao_enemy);


                vbo_enemy_vert=GL.GenBuffer();


                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_enemy_vert);
                GL.BufferData(BufferTarget.ArrayBuffer, vert.Length*sizeof(float), vert, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_enemy, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                vbo_enemy_tex = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_enemy_tex);
                GL.BufferData(BufferTarget.ArrayBuffer, texture_coord.Length*sizeof(float), texture_coord, BufferUsageHint.StaticRead);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_enemy, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindVertexArray(0);

                // buff elem = order of drawing
                el_buff= make_ebo(el_buff, indices);

                // textura nafig

                id_texture=attach_texture("../../../textures/enemy.png", id_texture);
            }

            public void draw()
            {
                GL.BindVertexArray(vao_enemy);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, el_buff);

                GL.BindTexture(TextureTarget.Texture2D, id_texture);

                GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            public void delete()
            {
                GL.DeleteVertexArray(vao_enemy);
                GL.DeleteBuffer(vbo_enemy_tex);
                GL.DeleteTexture(vbo_enemy_vert);
            }
        }

        class fireball:models
        {
            int vao_fireball;
            int vbo_fireball_vert;
            int vbo_fireball_text;
            int el_buff;
            int id_texture;

            //phys
            float speed = 0.8f;
            public bool flag_ball = false;

           

            public bool flag_set_angle = false;
            public float angle_trajectory;
            float max_t;
            //end of phys

            public float[] vert =
            {
                -0.0f, 0.05f, 0f, // top left vertex - 0
                0.1f, 0.05f, 0f, // top right vertex - 1
                0.1f, -0.05f, 0f, // bottom right - 2
                -0.0f, -0.05f, 0f // bottom left - 3

            };
            public uint[] indices =
            {
                // top triangle
                0, 1, 2,
                // bottom triangle
                2, 3, 0
            };
            float[] texture_coord =
            {
                0f, 1f,
                1f, 1f,
                1f, 0f,
                0f, 0f
            };

            public float rotate_ball = -45f;

            float time =0f;
            public void load()
            {
                vao_fireball = GL.GenVertexArray();
                GL.BindVertexArray(vao_fireball);
                vbo_fireball_vert = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fireball_vert);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float)*vert.Length, vert, BufferUsageHint.DynamicDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_fireball, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                vbo_fireball_text = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_fireball_text);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float)*texture_coord.Length, texture_coord, BufferUsageHint.DynamicDraw);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao_fireball, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindVertexArray(0);

                //order
                el_buff=make_ebo(el_buff, indices);

                //tex


                id_texture = attach_texture("../../../textures/fireball.png", id_texture);
            }

            public void draw(int location)
            {
                max_t = (float)(speed*Math.Sin(MathHelper.DegreesToRadians(angle_trajectory))/0.3);


                GL.BindVertexArray(vao_fireball);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, el_buff);



                GL.BindTexture(TextureTarget.Texture2D, id_texture);

                var tm = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotate_ball));

                float tr_x = (float)(-0.8f+(0.2f * Math.Cos(MathHelper.DegreesToRadians(angle_trajectory))) +(speed*Math.Cos(MathHelper.DegreesToRadians(angle_trajectory))*time));
                float tr_y =(float)( -0.8f+0.2f * Math.Sin(MathHelper.DegreesToRadians(angle_trajectory)) +(speed*Math.Sin(MathHelper.DegreesToRadians(angle_trajectory)*time) - 0.3*Math.Pow(time,2)/2));

                tm= tm * Matrix4.CreateTranslation(tr_x, tr_y, 0f);
                GL.UniformMatrix4(location, true, ref tm);


                GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                time+=0.01f;
                float dx = 1;
                float dy = (float)((speed*Math.Sin(MathHelper.DegreesToRadians(angle_trajectory)) - 0.3*time));
                float dypodx = dy/dx;
                if (rotate_ball>-135)
                {
                    
                    rotate_ball-=1.5f*(float)(Math.Atan(dypodx));
                }
                
                
                if (Math.Abs(tr_x)>1 || Math.Abs(tr_y)>1)
                {
                    flag_ball=true;
                    delete();
                }



            }

            public void delete()
            {
                GL.DeleteVertexArray(vao_fireball);
                GL.DeleteBuffer(vbo_fireball_text);
                GL.DeleteTexture(vbo_fireball_vert);
            }
        }

        player character = new player();
        bg back_g = new bg();
        enemy sminem = new enemy();
        fireball ball = new fireball();
        Shader shade;

        public Game(int Width, int Height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            
            screenHeight = Height;
            screenWidth = Width;

            this.CenterWindow(new Vector2i(Width, Height));
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0,0,e.Width,e.Height);
            this.screenWidth = e.Width;
            this.screenHeight=e.Height;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            VSync=VSyncMode.On;
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            StbImage.stbi_set_flip_vertically_on_load(1);

            back_g.load();
            character.load();
            sminem.load();
            

            shade = new Shader("../../../shaders/Default.vert", "../../../shaders/Default.frag");
            shade.run_shader();



        }
        protected override void OnUnload()
        {
            shade.Dispose();
            base.OnUnload();

            character.delete();

            sminem.delete();

            back_g.delete();

            

        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(Color4.Aquamarine);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // treug





            int location = GL.GetUniformLocation(shade.Handle, "transform");
            var tr = Matrix4.Identity;
            GL.UniformMatrix4(location, true, ref tr);


            back_g.draw();
            sminem.draw();
            character.draw(location);
            if (ball.flag_ball)
            {
                if (!ball.flag_set_angle)
                {
                    ball.angle_trajectory =player.rotate_angle;
                    ball.flag_set_angle=true;
                    ball.rotate_ball+=ball.angle_trajectory;
                }
                    
                ball.draw(location);
                
            }
            
            Context.SwapBuffers();


            base.OnRenderFrame(args);

            
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            
            factor++;
            if (factor == 10)
            {
                character.change_image(0);

                back_g.change_bg(0);
                
            }
            if (factor == 20)
            {
                character.change_image(1);
            }
            if (factor == 30)
            {
                character.change_image(2);
                back_g.change_bg(1);
            }
            if (factor == 40)
            {
                character.change_image(3);
            }
            if (factor == 50)
            {
                character.change_image(4);
                back_g.change_bg(2);
            }
            if (factor == 60)
            {
                character.change_image(5);
                factor = 9;
            }
            base.OnUpdateFrame(args);

            if (IsKeyDown(Keys.A))
                player.rotate_angle+=1f;
            if (IsKeyDown(Keys.D))
                player.rotate_angle-=1f;
            if (IsKeyDown(Keys.Escape))
                Close();
            if (IsKeyDown(Keys.Space))
            {
                ball = new fireball();
                ball.load();
                ball.flag_ball = true;
            }
        }



        
        public class Shader
        {
            public int Handle;

            public Shader(string vertexPath, string fragmentPath)
            {
                string VertexShaderSource = File.ReadAllText(vertexPath);

                string FragmentShaderSource = File.ReadAllText(fragmentPath);

                int VertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexShader, VertexShaderSource);

                int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentShader, FragmentShaderSource);

                GL.CompileShader(VertexShader);
                GL.CompileShader(FragmentShader);


                Handle = GL.CreateProgram();
                GL.AttachShader(Handle, VertexShader);
                GL.AttachShader(Handle, FragmentShader);

                GL.LinkProgram(Handle);

                GL.DetachShader(Handle, VertexShader);
                GL.DetachShader(Handle, FragmentShader);
                GL.DeleteShader(FragmentShader);
                GL.DeleteShader(VertexShader);

            }

            public void run_shader()
            {
                GL.UseProgram(Handle);
                

            }
            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    GL.DeleteProgram(Handle);

                    disposedValue = true;
                }
            }

            ~Shader()
            {
                if (disposedValue == false)
                {
                    Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
                }
            }


            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
    
}