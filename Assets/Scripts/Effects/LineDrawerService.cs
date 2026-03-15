namespace Effects
{
    public class LineDrawerService
    {
        private readonly EffectsSettings _effectsSettings;

        public LineDrawerService(EffectsSettings effectsSettings)
        {
            _effectsSettings = effectsSettings;
        }

        public LineDrawer Create(int maxCapacity)
        {
            var lineDrawer = new LineDrawer(maxCapacity, _effectsSettings.PrefabRenderer);
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