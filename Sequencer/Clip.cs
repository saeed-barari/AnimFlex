using System;

namespace AnimFlex.Sequencer
{
    [Serializable]
    public abstract class Clip
    {
        internal event Action onEndCallback;
        protected abstract void OnStart();

        public void Play(Action onEndCallback)
        {
            this.onEndCallback = onEndCallback;
            OnStart();
        }

        public void End()
        {
            onEndCallback();
        }

        /// <summary>
        /// Editor-only
        /// </summary>
        public virtual void OnValidate() { }
    }
}