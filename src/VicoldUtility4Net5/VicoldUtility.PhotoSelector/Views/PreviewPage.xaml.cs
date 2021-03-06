﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.PhotoSelector.Entities;

namespace VicoldUtility.PhotoSelector.Views
{
    /// <summary>
    /// PreviewPage.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPage : Page
    {
        private bool mouseDown;
        private Point mouseXY;

        public PreviewPage()
        {
            InitializeComponent();
        }

        public void Import(ImageItemEtt imageItemEtt)
        {
            if (imageItemEtt.FileCanShowingFullPath != null)
            {
                IMG1.Source = new BitmapImage(new Uri(imageItemEtt.FileCanShowingFullPath));
            }
        }

        #region 图片缩放
        private void IMG1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);
        }

        /// <summary>
        /// 鼠标按下时的事件，启用捕获鼠标位置并把坐标赋值给mouseXY.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;
        }

        /// <summary>
        /// 鼠标松开时的事件，停止捕获鼠标位置。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseMove(object sender, MouseEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }

        /// <summary>
        ///  鼠标移动时的事件，当鼠标按下并移动时发生
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Domousemove(ContentControl img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }

        /// <summary>
        /// group.Children中的第二个是移动的函数
        /// 它根据X.Y的值来移动。并把当前鼠标位置赋值给mouseXY.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;// 359 553   183 276    176    7
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            var ac = img.ActualWidth / img.ActualHeight;
            var ds = img.DesiredSize.Width / img.DesiredSize.Height;
            var design = ac / ds;
            //var point = new Point(img.ActualWidth / 2, img.ActualHeight / 2);
            //var point = new Point(img.DesiredSize.Width / 2, img.DesiredSize.Height / 2);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            var ws = img.DesiredSize.Width / img.ActualWidth;
            var hs = img.DesiredSize.Height / img.ActualHeight;
            DowheelZoom(group, point, delta, ws, hs);
        }

        /// <summary>
        /// 鼠标滑轮事件，得到坐标，放缩函数和滑轮指数，由于滑轮值变化较大所以*0.001.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="point"></param> 
        /// <param name="delta"></param>
        private void DowheelZoom(TransformGroup group, Point point, double delta, double ws, double hs)
        {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if ((delta < 0 && transform.ScaleX + delta < 0.1) || (delta > 0 && transform.ScaleX + delta > 20))
            {
                return;
            }
            transform.ScaleX += delta * transform.ScaleX;
            transform.ScaleY += delta * transform.ScaleY;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = point.X - (pointToContent.X * transform.ScaleX );
            transform1.Y = point.Y - (pointToContent.Y * transform.ScaleY );
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.Space)
            {
                var group = IMG.FindResource("Imageview") as TransformGroup;
                var transform = group.Children[0] as ScaleTransform;
                transform.ScaleX = 1;
                transform.ScaleY = 1;
                var transform1 = group.Children[1] as TranslateTransform;
                transform1.X = 0;
                transform1.Y = 0;
            }
        }

        #endregion
    }

    public enum AnchorPointType
    {
        /// <summary>
        /// 上下
        /// </summary>
        NS,
        /// <summary>
        /// 左右
        /// </summary>
        WE,
        /// <summary>
        /// 右上
        /// </summary>
        NE,
        /// <summary>
        /// 左下
        /// </summary>
        SW,
        /// <summary>
        /// 右下
        /// </summary>
        NW,
        /// <summary>
        /// 左上
        /// </summary>
        SE
    }
}

