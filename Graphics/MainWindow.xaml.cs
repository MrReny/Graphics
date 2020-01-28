// Программа отображения каркасно реберной модели много угольника в проекциях:
// 1) Прямоугольная изометрия
// 2) Прямоугольная диметрия
// Отображения каркасно реберной модели и реализация вращения её по оси Х в проекциях:
// 1) Горизонтальная косоугозльная изометрия
// 2) Фронтальная косоугольная диметрия
// Перспективы в фронтальной косоугольной изометрии с слайдером смещения по оси Х


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; 
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using SharpGL;
using SharpGL.WPF;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;

namespace Graphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        private readonly double[,] _figureCoords= new double[8,3] {{  0,  0,  0},
                                                                   {  0,  0,1.2},
                                                                   {1.2,  0,0.6},
                                                                   {1.2,  0,  0},
                                                                   {0.6,  1,  0},
                                                                   {  0,  1,  0},
                                                                   {  0,  1,0.6},
                                                                   {0.6,  1,0.3}
        };
        
        private readonly int[,] _paths = new int[12,2]{{0,1},{3,2},{1,2},
                                                       {0,3},{3,4},{4,5},
                                                       {5,6},{0,5},{2,7},
                                                       {6,7},{1,6},{4,7}};
        
        private readonly float[][] _colors = new float[][] {
                                    new float[]{1.0f,00,00},
                                    new float[]{00,00,1.0f},
                                    new float[]{00,1.0f,00},
                                    new float[]{1.0f,00,1.0f},
                                    new float[]{00,1.0f,1.0f},
                                    new float[]{1.0f,1.0f,00},
                                    new float[]{1.0f,0.8f,0.5f},
                                    new float[]{0.8f,0.8f,0.8f},
                                    new float[]{0.5f,0.5f,0.5f}};
        
        private float _rotatePyramid = 0;

        private double _pX = 0;
        
        private double _cX = 0;
        private double _cY = 0;
        private double _cZ = -10;
        
        private double _eY = 1;
        private double _eZ = 5;
        
        private int _mode = 1;
        private bool _rOn = false;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            var control = (OpenGLControl)sender;
        
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            //  Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // Установка цвета фона в формате RGBA
            gl.ClearColor(1,1,1,1);
            if (_mode == 7)
            {
                gl.Begin(OpenGL.GL_LINES);
                
                gl.Color(1f,0,0);
                gl.Vertex(-10,1,-0.1);
                gl.Vertex( 10,1,-0.1);
                gl.Color(0,1f,0);
                gl.Vertex(-10,-0,1.1+_pX*0.7);
                gl.Vertex( 10,-0,1.1-_pX*0.7);
            
                gl.End();
            }
            else
            {
                gl.LoadIdentity();
                //  Now render some text.
                gl.DrawText((int)control.ActualWidth/2+20, (int)control.ActualHeight-20,
                    0, 0, 0,
                    "Times New Roman", 20,"Z");
                gl.LoadIdentity();
                //  Now render some text.
                gl.DrawText((int)control.ActualWidth-20, 0,
                    0, 0, 0,
                    "Times New Roman", 20,"X");
                gl.LoadIdentity();
                //  Now render some text.
                gl.DrawText(0, 0,
                    0, 0, 0,
                    "Times New Roman", 20,"Y");
            
            
                //  загружаем нулевую матрицу сцены
                gl.LoadIdentity();
                // Включаю скглаживание
                gl.Enable(OpenGL.GL_LINE_SMOOTH);
                // Установка пределов толщины линии
                gl.GetFloat(OpenGL.GL_ALIASED_LINE_WIDTH_RANGE,new float[]{0.0f,2.0f});
                // Установка толщины линии
                gl.LineWidth(2f);
            
                gl.Begin(OpenGL.GL_LINES);
                
                gl.Color(1,1,1);
                gl.Vertex(0,0,0);
                gl.Vertex(2,0,0);
                gl.Vertex(0,0,0);
                gl.Vertex(0,2,0);
                gl.Vertex(0,0,0);
                gl.Vertex(0,0,2);
            
                gl.End(); 
            }
            
            gl.FrontFace(OpenGL.GL_CW);
            gl.LoadIdentity();
            
            //  Draw a pyramid. First, rotate the modelview matrix.
            gl.Rotate(_rotatePyramid, 1.0f, 0.0f, 0.0f);
        
            //  Start drawing lines.
            gl.Begin(OpenGL.GL_LINES);
            
            // Применяем колхоз, поскольку система координат opengl не совпадает с нашей(по просту меняем Y и Z местами)
            
            for (var i = 0; i < 12; i++)
            {
                //gl.Color(_colors[i]);
                gl.Color(0,0.5f,0.5f);
                
                gl.Vertex(_figureCoords[_paths[i,0],0],
                          _figureCoords[_paths[i,0],2],
                          _figureCoords[_paths[i,0],1]);
              
                gl.Vertex(_figureCoords[_paths[i,1],0],
                          _figureCoords[_paths[i,1],2],
                          _figureCoords[_paths[i,1],1]);
            }
        
            gl.End(); 
            
            //  Flush OpenGL.
            gl.Flush();

            if (_mode % 2 == 0 & _rOn)
            {
                _rotatePyramid += 3.0f;
            }
            else
            {
                _rotatePyramid = 0;
            }
        } 
        
        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            // Get the OpenGL instance.
            OpenGL gl = args.OpenGL;

            var control = (OpenGLControl) sender;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();

            var angle1 = -45 * Math.PI/180;
            var angle2 = 35.26 * Math.PI/180;
            // Изометрия
            gl.MultMatrix(new double[]{Math.Cos(angle1),Math.Sin(angle1)*Math.Sin(angle2),0,0,
                0,Math.Cos(angle2),0,0,
                Math.Sin(angle1),-Math.Cos(angle1)*Math.Sin(angle2),0,0,
                0,0,0,2 });

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);;

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        } 
        
        // Прямоугольная изометрия
        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = 1;
            
            // Get the OpenGL instance.
            OpenGL gl = GlControl.OpenGL;

            var control = GlControl;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();

            var angle1 = -90 * Math.PI/180;
            var angle2 = -90 * Math.PI/180;
            
            angle1 = -45 * Math.PI/180;
            angle2 = 35.26 * Math.PI/180;
            // Изометрия
            gl.MultMatrix(new double[]{Math.Cos(angle1),Math.Sin(angle1)*Math.Sin(angle2),0,0,
                                           0,Math.Cos(angle2),0,0,
                                           Math.Sin(angle1),-Math.Cos(angle1)*Math.Sin(angle2),0,0,
                                           0,0,0,2 });
            
            
            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        
        // Прямоугольная диметрия
        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = 3;
            // Get the OpenGL instance.
            OpenGL gl = GlControl.OpenGL;

            var control = GlControl;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            
            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();
            
            var angle1 = -22.5 * Math.PI/180;
            var angle2 = 20.7 * Math.PI/180;
            // Диметрия
            gl.MultMatrix(new double[]{Math.Cos(angle1),Math.Sin(angle1)*Math.Sin(angle2),0,0,
                                           0,Math.Cos(angle2),0,0,
                                           Math.Sin(angle1),-Math.Cos(angle1)*Math.Sin(angle2),0,0,
                                           0,0,0,2 });
            

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        
        // Горизонтальная косоугозльная изометрия
        private void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = 2;
            // Get the OpenGL instance.
            OpenGL gl = GlControl.OpenGL;

            var control = GlControl;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            
            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();
            
            var angle2 = 45 * Math.PI/180;
            // Диметрия
            gl.MultMatrix(new double[]{1,0,0,0,
                                          0,1,0,0,
                                          -Math.Cos(angle2),-Math.Cos(angle2),0,0,
                                          0,0,0,2 });
            

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        
        // Фронтальная косоугольная диметрия
        private void Button4_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = 4;
            // Get the OpenGL instance.
            OpenGL gl = GlControl.OpenGL;

            var control = GlControl;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            
            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();
            
            var angle2 = 45 * Math.PI/180;
            // Диметрия
            gl.MultMatrix(new double[]{1,0,0,0,
                0,1,0,0,
                -0.5*Math.Cos(angle2),-0.5*Math.Cos(angle2),0,0,
                0,0,0,2 });

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            
        }
        
        // Вращение по оси Х
        private void Button5_OnClick(object sender, RoutedEventArgs e)
        {
            if(_mode%2==0)
            _rOn = !_rOn;
        }
        
        // Перспектива
        private void Button6_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = 7;
            // Get the OpenGL instance.
            OpenGL gl = GlControl.OpenGL;

            var control = GlControl;

            //  Задаем матрицу вида 
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            
            //  загружаем нулевую матрицу сцены
            gl.LoadIdentity();
            
            gl.Perspective(45,(double)800/600,0.01,100);

            gl.LookAt(_pX,_eY,_eZ,_cX,_cY,_cZ,0,1,0);

            //  задаем матрицу вида мдели 
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OpenGL gl = GlControl.OpenGL;
            _pX = e.NewValue;
            Button6_OnClick(new object(), new RoutedEventArgs());
        }

        private void Button_Resize(object sender, EventArgs e)
        {
            Button b = (Button) sender;

            b.Width = (double) RootGrid.Parent.GetValue(WidthProperty) / 3 - 20;
            b.Height = RootGrid.RowDefinitions[1].ActualHeight / 3 - 4;
        }

        private void RootGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement_OnSizeChanged(ButtonPanel,e);
            FrameworkElement_OnSizeChanged(ButtonPanel2,e);
            FrameworkElement_OnSizeChanged(ButtonPanel3,e);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            StackPanel sp = (StackPanel) sender;
            foreach (var b in sp.Children)
            {
                if (b is Button)
                {
                    Button_Resize(b, new EventArgs());
                }
            }
        }
    }
}