using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Additional helper methods for navigating the visual tree.
    /// </summary>
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Finds the ancestor of the specified <paramref name="dependencyObject"/> with type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor <see cref="DependencyObject"/> to retrieve.</typeparam>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> to find the ancestor of.</param>
        /// <returns>The first ancestor of type <typeparamref name="T"/> when found; otherwise, <see langword="null"/>.</returns>
        public static T? FindAncestorWithType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            for (DependencyObject? current = GetParent(dependencyObject); current != null; current = GetParent(current))
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
            }
            return null;
        }
        private static DependencyObject GetParent(DependencyObject d)
        {
            if (d is Visual || d is Visual3D)
                return VisualTreeHelper.GetParent(d);
            else return LogicalTreeHelper.GetParent(d);
        }
    }
}
