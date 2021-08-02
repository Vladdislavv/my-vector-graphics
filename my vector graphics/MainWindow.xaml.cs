using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Export_window;
using Get_Code__Of_Color;

namespace my_paint
{
    public partial class MainWindow : Window
    {
        Point startCursorPoint;
        List<CheckBox> drawingType;
        List<Shape> FiguresOnCanvas = new();
        Shape SelectedObject;
        int NameForObjects = 0;//используется для назначения имени обьекта, при каждом появлении нового обьекта его имя это значение этой переменной
        public MainWindow()
        {
            InitializeComponent();
        }
        Point CursorPosition { get { return Mouse.GetPosition(cnvs); } }
        private void Canvas_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            startCursorPoint = CursorPosition;
            i = 0;
            if (e.Source is Shape sh && sh.Name != "VisualEditFrame" && Mouse.OverrideCursor == null)
            {
                SelectedObject = (Shape)e.Source;
                AlignmentForEditingSelectedObject();
                SetEditFrame();
            }
            else if (Mouse.OverrideCursor == null)
            {
                EditFrame.Visibility = Visibility.Hidden;
            }
        }

        Point p1;
        Point p2;
        int i = 0;
        private void cnvs_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditFrame.Visibility != Visibility.Visible)
            {
                if (chooseEllipse.IsChecked == true)
                {
                    DrawNotEquilateralFigure("Ellipse");
                }
                else if (chooseStraightLine.IsChecked == true)
                {
                    DrawStraightLine();
                }
                else if (chooseRectangle.IsChecked == true)
                {
                    DrawNotEquilateralFigure("Rectangle");
                }
                else if (chooseCircle.IsChecked == true)
                {
                    DrawEquilateralFigure("Circle");
                }
                else if (chooseCube.IsChecked == true)
                {
                    DrawEquilateralFigure("Cube");
                }
                else
                {
                    DrawCurveLine();
                }

            }
            else
            {
                EditingTheSelectedObject(e.Source);
            }
        }
        void SetEditFrame()
        {
            EditFrame.Visibility = Visibility.Visible;
            EditFrame.Width = SelectedObject.Width;
            EditFrame.Height = SelectedObject.Height;
            Canvas.SetLeft(EditFrame, Canvas.GetLeft(SelectedObject));
            Canvas.SetTop(EditFrame, Canvas.GetTop(SelectedObject));
            Canvas.SetZIndex(EditFrame, 1);
        }
        void AlignmentForEditingSelectedObject()
        {
            double angle = ((RotateTransform)SelectedObject.RenderTransform).Angle;
            double height = SelectedObject.Height;
            double width = SelectedObject.Width;

            switch (angle)
            {
                case 90:
                    Canvas.SetLeft(SelectedObject, Canvas.GetLeft(SelectedObject) - SelectedObject.Height);
                    break;

                case 180:
                    Canvas.SetLeft(SelectedObject, Canvas.GetLeft(SelectedObject) - SelectedObject.Width);
                    Canvas.SetTop(SelectedObject, Canvas.GetTop(SelectedObject) - SelectedObject.Height);
                    break;

                case 270:
                    Canvas.SetTop(SelectedObject, Canvas.GetTop(SelectedObject) - SelectedObject.Width);
                    break;
            }

            if(angle == 90 || angle ==  270)
            {
                SelectedObject.Height = width;
                SelectedObject.Width = height;
            }

            SelectedObject.RenderTransform = new RotateTransform(0);
        }
        void EditingTheSelectedObject(object source)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Mouse.OverrideCursor == Cursors.SizeWE )
                {
                    switch (((Shape)source).Name)
                    {
                        case "RightEditPointer":
                            SelectedObject.Width = CursorPosition.X - Canvas.GetLeft(SelectedObject);
                            break;

                        case "LeftEditPointer":
                            if (Canvas.GetLeft(SelectedObject) > 0)
                            {
                                SelectedObject.Width += Canvas.GetLeft(SelectedObject) - CursorPosition.X;
                                Canvas.SetLeft(SelectedObject, CursorPosition.X);
                            }
                            break;
                    }
                }
                else if(Mouse.OverrideCursor == Cursors.SizeNS )
                {
                    switch (((Shape)source).Name)
                    {
                        case "DownEditPointer":
                            SelectedObject.Height = CursorPosition.Y - Canvas.GetTop(SelectedObject) ;
                            break;

                        case "UpEditPointer" :
                            if (Canvas.GetTop(SelectedObject) > 0)
                            {
                                SelectedObject.Height += Canvas.GetTop(SelectedObject) - CursorPosition.Y;
                                Canvas.SetTop(SelectedObject, CursorPosition.Y);
                            }
                            break;
                    }
                }
                else if(Mouse.OverrideCursor == Cursors.SizeAll)
                {
                    DragSelectedFigure();
                }
            }
            SetEditFrame();
        }
        void DragSelectedFigure()
        {
            Canvas.SetLeft(SelectedObject,Canvas.GetLeft(SelectedObject) +  CursorPosition.X -startCursorPoint.X);
            Canvas.SetTop(SelectedObject, Canvas.GetTop(SelectedObject) +  CursorPosition.Y - startCursorPoint.Y);
            if(Canvas.GetLeft(SelectedObject) <= 0)
            {
                Canvas.SetLeft(SelectedObject, 0);
            }
            if(Canvas.GetTop(SelectedObject) <= 0)
            {
                Canvas.SetTop(SelectedObject, 0);
            }
            startCursorPoint = CursorPosition;
        }
        void DrawCurveLine()
        {
            Point CursorPosBuf;
            BezierSegment line = new BezierSegment();
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (CursorPosition != CursorPosBuf)
                {
                    if (i == 0)
                    {
                        line = new BezierSegment();
                        p1 = CursorPosition;
                    }
                    else if (i == 1)
                    {
                        p2 = CursorPosition;
                    }
                    else if (i == 2)
                    {
                        Path pt = new();
                        pt.StrokeThickness = Convert.ToSingle(lineThickness.Text);
                        pt.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lineColor.Text));
                        PathGeometry pg = new();
                        line.Point2 = p1;
                        line.Point1 = p2;
                        line.Point3 = CursorPosition;

                        PathFigure p = new PathFigure();
                        p.StartPoint = startCursorPoint;
                        p.Segments.Add(line);
                        pg.Figures.Add(p);
                        pt.Data = pg;
                        cnvs.Children.Add(pt);
                        startCursorPoint = CursorPosition;
                    }
                    i++;
                    if (i > 2) { i = 0; }
                }
                CursorPosBuf = CursorPosition;
            }
        }
        int a = 0;
        Line StraightLine;
        void DrawStraightLine()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (a == 0)
                {
                    startCursorPoint = CursorPosition;
                    StraightLine = new();
                    StraightLine.StrokeThickness = Convert.ToSingle(lineThickness.Text);
                    cnvs.Children.Add(StraightLine);
                    StraightLine.X1 = startCursorPoint.X;
                    StraightLine.Y1 = startCursorPoint.Y;
                    StraightLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lineColor.Text));
                    NameForObjects++;
                    StraightLine.Name = Convert.ToString("r" + NameForObjects);
                }
                else
                {
                    StraightLine.X2 = CursorPosition.X;
                    StraightLine.Y2 = CursorPosition.Y;
                }
                a++;
            }
            else
            {
                a = 0;
            }
        }
        Shape Figure;

        void DrawNotEquilateralFigure(string Figure)//если в string написано Rectangle то рисуется прямоугольник,если Ellipse то элипс
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (a == 0)
                {
                    startCursorPoint = CursorPosition;
                    if (Figure == "Rectangle")
                        this.Figure = new Rectangle();
                    else if (Figure == "Ellipse")
                        this.Figure = new Ellipse();

                    cnvs.Children.Add(this.Figure);
                    Canvas.SetTop(this.Figure, CursorPosition.Y);
                    Canvas.SetLeft(this.Figure, CursorPosition.X);
                    NameForObjects++;
                    this.Figure.Name = Convert.ToString("r" + NameForObjects);
                    this.Figure.StrokeThickness = Convert.ToSingle(strokeThicknessFigure.Text);
                    this.Figure.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(outlineColor.Text));
                    this.Figure.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(figureColor.Text));
                }
                else
                {
                    float drawingFigureHeight = (float)(CursorPosition.Y - startCursorPoint.Y);
                    float drawingFigureWidth = (float)(CursorPosition.X - startCursorPoint.X);
                    RotateTransform rotateTransform = new RotateTransform(0);
                    if (drawingFigureWidth < 0 && drawingFigureHeight > 0)
                    {
                        rotateTransform = new RotateTransform(90);
                        this.Figure.RenderTransform = rotateTransform;
                        this.Figure.Width = Math.Abs(drawingFigureHeight);
                        this.Figure.Height = Math.Abs(drawingFigureWidth);
                    }
                    else if (drawingFigureHeight < 0 && drawingFigureWidth > 0)
                    {
                        rotateTransform = new RotateTransform(270);
                        this.Figure.RenderTransform = rotateTransform;
                        this.Figure.Width = Math.Abs(drawingFigureHeight);
                        this.Figure.Height = Math.Abs(drawingFigureWidth);
                    }
                    else if (drawingFigureHeight < 0 && drawingFigureWidth < 0)
                    {
                        rotateTransform = new RotateTransform(180);
                        this.Figure.RenderTransform = rotateTransform;
                        this.Figure.Height = Math.Abs(drawingFigureHeight);
                        this.Figure.Width = Math.Abs(drawingFigureWidth);
                    }
                    else
                    {
                        rotateTransform = new RotateTransform(0);
                        this.Figure.RenderTransform = rotateTransform;
                        this.Figure.Height = Math.Abs(drawingFigureHeight);
                        this.Figure.Width = Math.Abs(drawingFigureWidth);
                    }
                }
                a++;
            }
            else
            {
                a = 0;
            }
        }

        void DrawEquilateralFigure(string Figure)//если в string написано Cube то рисуется квадрат,если Circle то круг
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (a == 0)
                {
                    startCursorPoint = CursorPosition;

                    if (Figure == "Cube")
                        this.Figure = new Rectangle();
                    else if (Figure == "Circle")
                        this.Figure = new Ellipse();

                    cnvs.Children.Add(this.Figure);
                    Canvas.SetTop(this.Figure, CursorPosition.Y);
                    Canvas.SetLeft(this.Figure, CursorPosition.X);
                    this.Figure.StrokeThickness = Convert.ToSingle(strokeThicknessFigure.Text);
                    this.Figure.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(outlineColor.Text));
                    this.Figure.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(figureColor.Text));
                    NameForObjects++;
                    this.Figure.Name = Convert.ToString("r" + NameForObjects);
                }
                else
                {
                    float drawingFigureHeight = (float)(CursorPosition.Y - startCursorPoint.Y);
                    float drawingFigureWidth = (float)(CursorPosition.X - startCursorPoint.X);
                    RotateTransform rotateTransform = new RotateTransform(0);

                    if (drawingFigureWidth < 0 && drawingFigureHeight > 0)
                    {
                        rotateTransform = new RotateTransform(90);
                        this.Figure.RenderTransform = rotateTransform;
                    }
                    else if (drawingFigureHeight < 0 && drawingFigureWidth > 0)
                    {
                        rotateTransform = new RotateTransform(270);
                        this.Figure.RenderTransform = rotateTransform;
                    }
                    else if (drawingFigureHeight < 0 && drawingFigureWidth < 0)
                    {
                        rotateTransform = new RotateTransform(180);
                        this.Figure.RenderTransform = rotateTransform;
                    }
                    else
                    {
                        rotateTransform = new RotateTransform(0);
                        this.Figure.RenderTransform = rotateTransform;
                    }


                    if (Math.Abs(drawingFigureWidth) < Math.Abs(drawingFigureHeight) && (Canvas.GetLeft(this.Figure) - Math.Abs(drawingFigureHeight)) > 0)
                    {
                        this.Figure.Width = Math.Abs(drawingFigureHeight);
                        this.Figure.Height = Math.Abs(drawingFigureHeight);
                    }
                    else if ((Canvas.GetTop(this.Figure) - Math.Abs(drawingFigureWidth)) > 0 && Math.Abs(drawingFigureWidth) > Math.Abs(drawingFigureHeight))
                    {
                        this.Figure.Width = Math.Abs(drawingFigureWidth);
                        this.Figure.Height = Math.Abs(drawingFigureWidth);
                    }

                }
                a++;
            }
            else
            {
                a = 0;
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportDiologWindow ex = new ExportDiologWindow();
            if (ex.ShowDialog() == true)
            {
                string path = ex.PathOfFile + "/" + ex.NameOfFile + ".png";
                Export(path);
            }
        }

        void Export(string path)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)cnvs.RenderSize.Width, (int)cnvs.RenderSize.Height, 96d, 96d, PixelFormats.Default);
            rtb.Render(cnvs);

            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)cnvs.RenderSize.Width, (int)cnvs.RenderSize.Height));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = System.IO.File.OpenWrite(path))
            {
                pngEncoder.Save(fs);
            }
        }
        void GetCheckBoxesValidadion(List<CheckBox> ListObjects, Object sender)
        {
            for (int d = 0; d < ListObjects.Count(); d++)
            {
                if (ListObjects[d].Name != ((CheckBox)sender).Name)
                {
                    ListObjects[d].IsChecked = false;
                }
            }
        }

        bool ValidationFloatBox(char symbol)
        {
            if (symbol == ',' || Char.IsDigit(symbol))
                return true;
            else
                return false;
        }

        private void chooseCurveLine_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }

        private void chooseStraightLine_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }

        private void chooseRectangle_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }

        private void chooseCube_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }
        private void chooseCircle_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }

        private void chooseEllipse_Click(object sender, RoutedEventArgs e)
        {
            GetCheckBoxesValidadion(drawingType, sender);
        }

        private void lineThickness_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidationFloatBox(Convert.ToChar(e.Text));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cnvs.Background = Brushes.White;
            drawingType = new() { chooseCurveLine, chooseStraightLine, chooseCube, chooseRectangle, chooseEllipse, chooseCircle };
            Mouse.OverrideCursor = null;
        }
        private void exitTheCursorFromThePointer(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void HorizontalPointer_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeWE;
        }

        private void VertcalPointer_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNS;
        }

        private void VisualEditFrame_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void GetCodeOfColor_Button_Click(object sender, RoutedEventArgs e)
        {
            GetCodeOfColor gcoc = new();
            gcoc.ShowDialog();
        }
    }
}
