using UnityEngine;

namespace Effects
{
    public class LineDrawerService
    {
        private readonly EffectsSettings _effectsSettings;

        public LineDrawerService(EffectsSettings effectsSettings)
        {
            _effectsSettings = effectsSettings;
        }

        public LineDrawer Create(Color color)
        {
            var lineDrawer = new LineDrawer(_effectsSettings.PrefabRenderer, color);
            return lineDrawer;
        }
        public void Return(LineDrawer lineDrawer)
        {
            lineDrawer.Destroy();
        }
        public void Return(LineDrawer lineDrawer, float delay)
        {
            lineDrawer.DestroyAsync(delay);
        }
    }
}