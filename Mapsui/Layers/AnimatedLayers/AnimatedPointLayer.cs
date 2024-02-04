using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Features;
using Mapsui.Layers.AnimationLayers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Layers.AnimatedLayers;

public class AnimatedPointLayer : Layer
{
    private FetchInfo? _fetchInfo;
    private readonly List<AnimatedPointFeature> _features = [];

    public AnimatedPointLayer(IProvider dataSource)
    {
        DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        if (DataSource is IDataChangedProvider dataChangedProvider)
            dataChangedProvider.DataChanged += (s, e) =>
            {
                Catch.Exceptions(() =>
                {
                    UpdateData();
                    DataHasChanged();
                });
            };

        // Todo: There should be a assignable function to find the previous feature, so the user has all flexibility
        IdField = "ID";
    }

    /// <summary>
    /// When the distance between the current and the previous position is larger
    /// than the DistanceThreshold it will not be animated. 
    /// The default is Double.MaxValue
    /// </summary>
    public double DistanceThreshold { get; set; } = double.MaxValue;
    public string IdField { get; set; }

    /// <summary>
    /// The period of which the animation should move from the previous position to the new position.
    /// The default is 1000 milliseconds.
    /// </summary>
    public int AnimationDuration { get; set; } = 1000;

    /// <summary>
    /// The easing function to use for the animation. The default is Easing.CubicOut.
    /// </summary>
    public Easing Easing { get; set; } = Easing.CubicOut;


    public void UpdateData()
    {
        if (_fetchInfo is null) return;

        var features = DataSource?.GetFeatures(_fetchInfo);

        if (features == null)
            return;

        SetAnimationTarget(features.Cast<PointFeature>());
        OnDataChanged(new DataChangedEventArgs());
    }

    public void SetAnimationTarget(IEnumerable<PointFeature> targets)
    {
        foreach (var target in targets)
        {
            var animatedPointFeature = FindPrevious(_features, target, IdField);
            if (animatedPointFeature is null)
            {
                animatedPointFeature = new AnimatedPointFeature(target.Point.X, target.Point.Y);
                _features.Add(animatedPointFeature);
            }
            else
            {
                animatedPointFeature.SetAnimationTarget(target.Point);
            }
            foreach (var field in target.Fields)
                animatedPointFeature[field] = target[field];
        }
    }

    private static AnimatedPointFeature? FindPrevious(IEnumerable<AnimatedPointFeature>? features, IFeature feature,
    string idField)
    {
        // There is no guarantee the idField is set since the features are added by the user. Things do not crash
        // right now because AnimatedPointSample a feature is created with an "ID" field. This is an unresolved
        // issue.
        return features?.FirstOrDefault(f => f[idField]?.Equals(feature[idField]) ?? false);
    }

    public void RefreshData(FetchInfo fetchInfo)
    {
        _fetchInfo = fetchInfo;
    }

    public override bool UpdateAnimations()
    {
        var animating = false;
        foreach (var feature in _features)
        {
            if (feature.UpdateAnimation(AnimationDuration, Easing, DistanceThreshold))
                animating = true;
        }
        return animating;
    }
}
