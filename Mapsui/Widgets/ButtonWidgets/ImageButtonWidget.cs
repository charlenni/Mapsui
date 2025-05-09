﻿using Mapsui.Styles;
using Mapsui.Widgets.BoxWidgets;

namespace Mapsui.Widgets.ButtonWidgets;

/// <summary>
/// Widget that shows a button with an icon
/// </summary>
public class ImageButtonWidget : BoxWidget, IHasImage
{
    public ImageButtonWidget() : base()
    {
        BackColor = Color.Transparent;
    }

    private MRect _padding = new(0);

    /// <summary>
    /// Padding left and right for icon inside the Widget
    /// </summary>
    public MRect Padding
    {
        get => _padding;
        set
        {
            if (_padding == value)
                return;

            _padding = value;
            Invalidate();
        }
    }

    private Image? _image;

    /// <summary>
    /// The image to show as button
    /// </summary>
    public Image? Image
    {
        get => _image;
        set
        {
            if (_image == value)
                return;

            _image = value;
            Invalidate();
        }
    }

    private double _rotation;

    /// <summary>
    /// Rotation of the SVG image
    /// </summary>
    public double Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation == value)
                return;
            _rotation = value;
            Invalidate();
        }
    }
}
