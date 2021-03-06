﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace Sharpnado.Shades
{
    public class Shadows : ContentView
    {
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(int),
            typeof(Shadows),
            DefaultCornerRadius);

        public static readonly BindableProperty ShadesProperty = BindableProperty.Create(
            nameof(Shades),
            typeof(IEnumerable<Shade>),
            typeof(Shadows),
            defaultValueCreator: (bo) => new ObservableCollection<Shade> { new Shade { Parent = (Shadows)bo } },
            validateValue: (bo, v) => v is IEnumerable<Shade>,
            propertyChanged: ShadesPropertyChanged);

        private const int DefaultCornerRadius = 0;

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public IEnumerable<Shade> Shades
        {
            get => (IEnumerable<Shade>)GetValue(ShadesProperty);
            set => SetValue(ShadesProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach (var shade in Shades)
            {
                SetInheritedBindingContext(shade, BindingContext);
            }
        }

        private static void ShadesPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var shadows = (Shadows)bindable;

            if (oldvalue != null)
            {
                if (oldvalue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= shadows.OnShadeCollectionChanged;
                }

                foreach (var shade in (IEnumerable<Shade>)oldvalue)
                {
                    shade.Parent = null;
                    shade.BindingContext = null;
                }
            }

            foreach (var shade in (IEnumerable<Shade>)newvalue)
            {
                shade.Parent = shadows;
                SetInheritedBindingContext(shade, shadows.BindingContext);
            }

            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += shadows.OnShadeCollectionChanged;
            }
        }

        private void OnShadeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Shade newShade in e.NewItems)
                    {
                        newShade.Parent = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (Shade newShade in e.OldItems ?? new Shade[0])
                    {
                        newShade.Parent = null;
                    }

                    break;
            }
        }
    }
}
