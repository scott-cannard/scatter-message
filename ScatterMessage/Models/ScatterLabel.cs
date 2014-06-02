using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScatterMessage.Models
{
    public class ScatterLabel
    {
        public static double MessageWidth = 0;
        private static Random rand = new Random((int)DateTime.Now.Ticks);
        
        public Label BoxedLabel { get; private set; }
        public double xOffset { get; set; }
        public double yOffset { get; set; }
        private double speed;
        private double angle;
        private uint quadrant;



        public ScatterLabel(Label labelObj)
        {
            this.BoxedLabel = labelObj;
        }



        public void SetOff(double xOff, double yOff)
        {
            xOffset = xOff;
            yOffset = yOff;
        }


        public void ApplyForce(double force)
        {
            speed = force;
            angle = ScatterLabel.rand.NextDouble() * 2 * Math.PI;
            quadrant = 1 + (uint)(angle / (Math.PI * 0.5));
        }


        public bool UpdatePosition(int friction, double bottomEdge, double rightEdge)
        {
            xOffset += speed * Math.Cos(angle);
            yOffset += speed * Math.Sin(angle);

            //Bounce?
            //if (angle > Math.PI * 0.5 && newXPosition > rightEdge - BoxedLabel.Width)
            //{
            //    newXPosition = 2 * rightEdge - BoxedLabel.Margin.Left - horzVelocity - 2 * BoxedLabel.Width;
            //    horzVelocity = -horzVelocity;
            //    angle = (angle < Math.PI * 0.5) ? (Math.PI - angle) : (3 * Math.PI - angle);
            //}
            //else if (horzVelocity < 0 && newXPosition < 0)
            //{
            //    newXPosition = -newXPosition;
            //    horzVelocity = -horzVelocity;
            //    angle = (angle < Math.PI) ? (Math.PI - angle) : (3 * Math.PI - angle);
            //}

            //if (vertVelocity > 0 && newYPosition > bottomEdge - BoxedLabel.Height)
            //{
            //    newYPosition = 2 * bottomEdge - BoxedLabel.Margin.Top - vertVelocity - 2 * BoxedLabel.Height;
            //    vertVelocity = -vertVelocity;
            //    angle = (angle < Math.PI * 1.5) ? (angle - Math.PI) : ();
            //}
            //else if (vertVelocity < 0 && newYPosition < 0)
            //{
            //    newYPosition = 0;
            //    vertVelocity = -vertVelocity;
            //    angle = 0;
            //}

            //Adjust position for velocity
            //BoxedLabel.Margin = new Thickness(newXPosition, newYPosition, 0, 0);

            //Adjust velocity for friction
            if (Math.Abs(speed - friction) <= Double.Epsilon)
            {
                speed = 0;
                return false;
            }
            //else
            speed -= friction;
            return true;
        }


        public void TransformLayout()
        {
            BoxedLabel.RenderTransform = new TranslateTransform(xOffset, yOffset);
        }


        public void Reset()
        {
            speed = 0.0;
            xOffset = 0.0;
            yOffset = 0.0;
        }
    }
}
