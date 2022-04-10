using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace p1eXu5.Wpf.CustomControls
{
    [ TemplatePart( Name="PART_Container", Type=typeof(Grid) )]
    public class CircleDiagramControl : ItemsControl
    {
        public const double ONE_DEGREE_RAD = 0.01745329251994329576923690768489;
        private const double RADIUS = 50.0;
        private const double CENTER = 50.0;

        private Grid _grid;
        private readonly Queue< SolidColorBrush > _brushes;
        private Path[] _paths;

        static CircleDiagramControl ()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( CircleDiagramControl ), new FrameworkPropertyMetadata( typeof( CircleDiagramControl ) ) );
        }


        public CircleDiagramControl ()
        {
            _brushes = new Queue< SolidColorBrush >( new [] {
                new SolidColorBrush( new Color(){ R = 0x26, G = 0x89, B = 0xe5, A = 0xFF } ), // 2689e5
                new SolidColorBrush( new Color(){ R = 0xe5, G = 0x92, B = 0x26, A = 0xFF } ), // e59226
                new SolidColorBrush( new Color(){ R = 0x41, G = 0xe5, B = 0x26, A = 0xFF } ), // 41e526
                new SolidColorBrush( new Color(){ R = 0xda, G = 0x26, B = 0xe5, A = 0xFF } ), // da26e5
                new SolidColorBrush( new Color(){ R = 0x26, G = 0xe3, B = 0xe5, A = 0xFF } ), // 26e3e5
                new SolidColorBrush( new Color(){ R = 0xe5, G = 0xde, B = 0x26, A = 0xFF } ), // e5de26
                new SolidColorBrush( new Color(){ R = 0xe5, G = 0x26, B = 0x7b, A = 0xFF } ), // e5267b
                new SolidColorBrush( new Color(){ R = 0xad, G = 0xe5, B = 0x26, A = 0xFF } ), // ade526
                new SolidColorBrush( new Color(){ R = 0x49, G = 0x86, B = 0xbf, A = 0xFF } ), // 4986bf
                new SolidColorBrush( new Color(){ R = 0xbb, G = 0x8e, B = 0x53, A = 0xFF } ), // bb8e53
                new SolidColorBrush( new Color(){ R = 0x72, G = 0xcf, B = 0x63, A = 0xFF } ), // 72cf63
                new SolidColorBrush( new Color(){ R = 0xb0, G = 0x56, B = 0x56, A = 0xFF } ), // b056b6
                new SolidColorBrush( new Color(){ R = 0x59, G = 0xb7, B = 0xb8, A = 0xFF } ), // 59b7b8
                new SolidColorBrush( new Color(){ R = 0xc3, G = 0xbf, B = 0x4a, A = 0xFF } ), // c3bf4a
                new SolidColorBrush( new Color(){ R = 0xcd, G = 0x44, B = 0x81, A = 0xFF } ), // cd4481
            });

            foreach ( var brush in _brushes ) {
                brush.Freeze();
            }

            _paths = new Path[0];
        }

        public string Annotation
        {
            get => (string)GetValue(AnnotationProperty);
            set => SetValue(AnnotationProperty, value);
        }

        // Using a DependencyProperty as the backing store for Annotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationProperty =
            DependencyProperty.Register("Annotation", typeof( string ), typeof(CircleDiagramControl), new PropertyMetadata(""));



        public double Indicator
        {
            get { return (double)GetValue(IndicatorProperty); }
            set { SetValue(IndicatorProperty, value ); }
        }

        // Using a DependencyProperty as the backing store for Indicator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorProperty =
            DependencyProperty.Register("Indicator", typeof(double), typeof(CircleDiagramControl), new PropertyMetadata(0.0));



        public string IndicatorTip
        {
            get { return (string)GetValue(IndicatorTipProperty); }
            set { SetValue(IndicatorTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorTipProperty =
            DependencyProperty.Register("IndicatorTip", typeof(string), typeof(CircleDiagramControl), new PropertyMetadata(""));




        protected override void OnItemsSourceChanged ( IEnumerable oldValue, IEnumerable newValue )
        {
            if ( oldValue != null ) {
                foreach ( var path in _paths ) {

                    _grid.Children.Remove( path );
                }
            }


            if ( newValue != null ) {

                var nums = newValue.Cast< (double val, string ann) >().ToArray();
                if ( nums.Length == 0 ) return;


                var sum = nums.Aggregate( 0.0d, ( s, t ) => s + t.val );
                var oneDegree = 360 / sum;
                double startDegree = 0;

                var notZeroCount = nums.Count( n => n.val > 0.0 );
                if ( notZeroCount == 0 ) { return; }
                _paths = new Path[ notZeroCount ];
                var pathIndex = 0;

                for ( int i = 0; i < nums.Length; ++i ) {

                    if ( nums[ i ].val.Equals( 0.0 ) ) {
                        GetBrush();
                        continue;
                    }

                    var endDegree = startDegree + nums[ i ].val * oneDegree;
                    _paths[ pathIndex ] = GetPath( nums, pathIndex, startDegree, endDegree );
                    _grid.Children.Add( _paths[ pathIndex ] );
                    ++pathIndex;

                    startDegree = endDegree;
                }
            }

        }

        public override void OnApplyTemplate ()
        {
            base.OnApplyTemplate();

            if ( _grid == null ) {
                _grid = ( Grid )GetTemplateChild( "PART_Container" ) ?? new Grid();
            }
        }

        private Path GetPath ( (double val,string ann)[] arr, int ind, double startDegree, double endDegree )
        {
            Point startPoint;
            Point endPoint;

            if ( arr.Length == 1 ) {
                startPoint = new Point( CENTER, 0 );
                endPoint = new Point( CENTER - 0.01d, 0 );
            }
            else {
                startPoint = GetCirclePoint( startDegree );
                endPoint = GetCirclePoint( endDegree );
            }

            var isLargeArc = (endDegree - startDegree) > 180;

            var path = new Path {

                Name = $"Path{ind}",

                Fill = GetBrush(),
                Stroke = null,
                Width = 100,
                Height = 100,
                StrokeThickness = 1,
                ToolTip = arr[ind].ann,

                Clip = new GeometryGroup {
                    Children = new GeometryCollection( new Geometry[] {
                        new RectangleGeometry( new Rect(-0.5, -0.5, 101, 101) ),
                        new EllipseGeometry( new Point(50, 50), 34.5, 34.5 ), 
                    } )
                },

                Data = new PathGeometry( new [] {
                    new PathFigure(
                        start: startPoint,
                        segments: new PathSegment[] {
                            new ArcSegment(
                                point: endPoint,
                                size: new Size(50, 50),
                                isLargeArc: isLargeArc,
                                sweepDirection: SweepDirection.Clockwise,
                                rotationAngle: 0,
                                isStroked: false
                            ),
                            new LineSegment( new Point(50, 50), isStroked: false ),
                        },
                        closed: true
                    ), 
                })
            };

            Grid.SetRow( path, 0 );
            Grid.SetRowSpan( path, 3 );

            return path;
        }

        protected virtual Point GetCirclePoint ( double degree )
        {
            return new Point( 
                        x: Math.Sin( degree * ONE_DEGREE_RAD ) * RADIUS + CENTER,
                        y: CENTER - Math.Cos( degree * ONE_DEGREE_RAD ) * RADIUS
                    );
        }

        private SolidColorBrush GetBrush ()
        {
            var brush = _brushes.Dequeue();
            _brushes.Enqueue( brush );

            return brush;
        }
    }
}
