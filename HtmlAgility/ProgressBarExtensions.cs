using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Tweakers
{
    public static class ProgressBarExtensions
    {
        private static TimeSpan duration = TimeSpan.FromMilliseconds(500);

        public static void SetPercent(this ProgressBar progressBar, double percentage)
        {
            DoubleAnimation animation = new DoubleAnimation(percentage, duration);
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
            duration = TimeSpan.FromMilliseconds(700);
        }
    }
}
