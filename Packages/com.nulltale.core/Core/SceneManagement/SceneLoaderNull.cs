using System;

namespace CoreLib.SceneManagement
{
    [Serializable]
    public class SceneLoaderNull : SceneLoader<SceneArgsNull>
    {
        protected override SceneArgsNull _provideArgs(SceneArgsNull args)
        {
            return null;
        }
    }
}